using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;
using KodiMinimalApi.Commands;
using KodiMinimalApi.Models;
using KodiMinimalApi.Services;
using Microsoft.Extensions.Options;
using static KodiMinimalApi.Models.ErrorHelper;

namespace KodiMinimalApi.Features;

public static class TVShowEndpoints
{
    public static void MapTVShowEndpoints(this IEndpointRouteBuilder app)
    {
        var tvshowApi = app.MapGroup("/api/tvshows");

        tvshowApi.MapPost("/", async (
            CommandValue command,
            IValidator<CommandValue> validator,
            IKodiService kodi,
            IOptions<KodiOptions> options
        ) =>
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult.ToErrors());

            try
            {
                var result = command switch
                {
                    TVShowList l => await kodi.SendAsync("VideoLibrary.GetTVShows",
                        new JsonObject
                        {
                            ["limits"] = new JsonObject
                            {
                                ["start"] = l.Start ?? 0,
                                ["end"] = l.End ?? 100
                            },
                            ["properties"] = new JsonArray("title", "year", "genre", "rating", "thumbnail", "file")
                        }),

                    TVShowSearch s => await kodi.SendAsync("VideoLibrary.GetTVShows",
                        new JsonObject
                        {
                            ["filter"] = new JsonObject
                            {
                                ["field"] = "title",
                                ["operator"] = "contains",
                                ["value"] = s.Query
                            },
                            ["properties"] = new JsonArray("title", "year", "genre", "rating", "thumbnail", "file")
                        }),

                    TVShowSeasons s => await kodi.SendAsync("VideoLibrary.GetSeasons",
                        new JsonObject
                        {
                            ["tvshowid"] = s.TVShowId,
                            ["properties"] = new JsonArray("season", "thumbnail", "tvshowid")
                        }),

                    TVShowEpisodes e => await kodi.SendAsync("VideoLibrary.GetEpisodes",
                        new JsonObject
                        {
                            ["tvshowid"] = e.TVShowId,
                            ["season"] = e.Season ?? -1,
                            ["properties"] = new JsonArray("title", "season", "episode", "runtime", "thumbnail", "file")
                        }),

                    TVShowRecent r => await kodi.SendAsync("VideoLibrary.GetRecentlyAddedEpisodes",
                        new JsonObject
                        {
                            ["limits"] = new JsonObject
                            {
                                ["end"] = r.Limit ?? 10
                            },
                            ["properties"] = new JsonArray("title", "season", "episode", "tvshowid", "thumbnail", "file")
                        }),

                    TVShowPlayEpisode p => await kodi.SendAsync("Player.Open",
                        new JsonObject
                        {
                            ["item"] = new JsonObject
                            {
                                ["episodeid"] = p.EpisodeId
                            }
                        }),

                    TVShowScan _ => options.Value.TVScanDirectory is string tvDir
                        ? await ScanLibrary(kodi, tvDir)
                        : throw new KodiException("TVScanDirectory is not configured in appsettings.json"),

                    TVShowSearchDir sd => options.Value.TVScanDirectory is string tvDir
                        ? await SearchDir(kodi, tvDir, sd.Query, sd.Directory, "video")
                        : throw new KodiException("TVScanDirectory is not configured in appsettings.json"),

                    TVSearchAll sa => options.Value.TVScanDirectory is string tvDir
                        ? await SearchAll(kodi, tvDir, sa.Query)
                        : throw new KodiException("TVScanDirectory is not configured in appsettings.json"),

                    _ => throw new InvalidOperationException("Unknown command type")
                };

                return Results.Text(result, "application/json");
            }
            catch (HttpRequestException)
            {
                return Results.Json(
                    new KodiProxyError("Kodi Unreachable", "Could not connect. Ensure Kodi is running and host/port in appsettings.json are correct.", 502),
                    AppJsonSerializerContext.Default.KodiProxyError,
                    statusCode: StatusCodes.Status502BadGateway);
            }
            catch (OperationCanceledException)
            {
                return Results.Json(
                    new KodiProxyError("Kodi Timeout", "Request timed out. Ensure Kodi is running and reachable at the configured host/port.", 502),
                    AppJsonSerializerContext.Default.KodiProxyError,
                    statusCode: StatusCodes.Status502BadGateway);
            }
            catch (KodiException ex)
            {
                return Results.Json(
                    new KodiProxyError("Kodi Error", ex.Message, 502),
                    AppJsonSerializerContext.Default.KodiProxyError,
                    statusCode: StatusCodes.Status502BadGateway);
            }
        });
    }

    static Task<string> ScanLibrary(IKodiService kodi, string dir) =>
        kodi.SendAsync("VideoLibrary.Scan",
            new JsonObject { ["showdialogs"] = false, ["directory"] = dir });

    static async Task<string> SearchDir(IKodiService kodi, string baseDir, string? query, string? subDir, string media)
    {
        var targetDir = string.IsNullOrEmpty(subDir)
            ? baseDir
            : baseDir.TrimEnd('/') + "/" + subDir.TrimStart('/');

        var raw = await kodi.SendAsync("Files.GetDirectory",
            new JsonObject
            {
                ["directory"] = targetDir,
                ["media"] = media
            });

        if (string.IsNullOrEmpty(query))
            return raw;

        var doc = JsonNode.Parse(raw)!.AsObject();
        var files = doc["files"]?.AsArray();
        if (files is null)
            return raw;

        var filtered = new JsonArray(
            files.Where(f => f?["label"]?.GetValue<string>()?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                 .Select(f => f!.DeepClone())
                 .ToArray<JsonNode?>()
        );
        doc["files"] = filtered;
        return doc.ToJsonString();
    }

    static async Task<string> SearchAll(IKodiService kodi, string scanDir, string query)
    {
        var results = new List<JsonNode>();
        await CollectMatching(kodi, scanDir, query, 0, 3, results);

        return new JsonObject
        {
            ["files"] = new JsonArray(results.Select(r => r.DeepClone()).ToArray<JsonNode?>())
        }.ToJsonString();
    }

    static async Task CollectMatching(IKodiService kodi, string dir, string query, int depth, int maxDepth, List<JsonNode> results)
    {
        if (depth > maxDepth)
            return;

        string raw;
        try
        {
            raw = await kodi.SendAsync("Files.GetDirectory",
                new JsonObject { ["directory"] = dir, ["media"] = "video" });
        }
        catch (KodiException)
        {
            return;
        }

        var doc = JsonNode.Parse(raw)?.AsObject();
        var files = doc?["files"]?.AsArray();
        if (files is null)
            return;

        foreach (var file in files)
        {
            if (file is null) continue;

            var label = file["label"]?.GetValue<string>();
            var filetype = file["filetype"]?.GetValue<string>();

            if (label?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                results.Add(file);

            if (filetype == "directory" && depth < maxDepth)
            {
                var path = file["file"]?.GetValue<string>();
                if (path is not null)
                    await CollectMatching(kodi, path, query, depth + 1, maxDepth, results);
            }
        }
    }
}
