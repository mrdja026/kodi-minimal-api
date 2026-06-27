using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;
using KodiMinimalApi.Commands;
using KodiMinimalApi.Models;
using KodiMinimalApi.Services;
using Microsoft.Extensions.Options;
using static KodiMinimalApi.Models.ErrorHelper;

namespace KodiMinimalApi.Features;

public static class MovieEndpoints
{
    public static void MapMovieEndpoints(this IEndpointRouteBuilder app)
    {
        var movieApi = app.MapGroup("/api/movies");

        movieApi.MapPost("/", async (
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
                    MovieList m => await kodi.SendAsync("VideoLibrary.GetMovies",
                        new JsonObject
                        {
                            ["limits"] = new JsonObject
                            {
                                ["start"] = m.Start ?? 0,
                                ["end"] = m.End ?? 100
                            },
                            ["properties"] = new JsonArray("title", "year", "genre", "runtime", "thumbnail", "file")
                        }),

                    MovieSearch s => await kodi.SendAsync("VideoLibrary.GetMovies",
                        new JsonObject
                        {
                            ["filter"] = new JsonObject
                            {
                                ["field"] = "title",
                                ["operator"] = "contains",
                                ["value"] = s.Query
                            },
                            ["properties"] = new JsonArray("title", "year", "genre", "runtime", "thumbnail", "file")
                        }),

                    MovieRecent r => await kodi.SendAsync("VideoLibrary.GetRecentlyAddedMovies",
                        new JsonObject
                        {
                            ["limits"] = new JsonObject
                            {
                                ["end"] = r.Limit ?? 10
                            },
                            ["properties"] = new JsonArray("title", "year", "genre", "runtime", "thumbnail", "file")
                        }),

                    MoviePlay p => await kodi.SendAsync("Player.Open",
                        new JsonObject
                        {
                            ["item"] = new JsonObject
                            {
                                ["movieid"] = p.MovieId
                            }
                        }),

                    MovieScan s => await ScanLibrary(kodi, s.Directory),

                    MovieScanMovies _ => options.Value.MovieScanDirectory is string movieDir
                        ? await ScanLibrary(kodi, movieDir)
                        : throw new KodiException("MovieScanDirectory is not configured in appsettings.json"),

                    MovieScanTV _ => options.Value.TVScanDirectory is string tvDir
                        ? await ScanLibrary(kodi, tvDir)
                        : throw new KodiException("TVScanDirectory is not configured in appsettings.json"),

                    MovieSearchDir sd => options.Value.MovieScanDirectory is string movieDir
                        ? await SearchDir(kodi, movieDir, sd.Query, sd.Directory, "video")
                        : throw new KodiException("MovieScanDirectory is not configured in appsettings.json"),

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

    static Task<string> ScanLibrary(IKodiService kodi, string? dir) =>
        kodi.SendAsync("VideoLibrary.Scan",
            dir is not null
                ? new JsonObject { ["showdialogs"] = false, ["directory"] = dir }
                : new JsonObject { ["showdialogs"] = false });

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
}
