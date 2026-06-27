using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;
using KodiMinimalApi.Commands;
using KodiMinimalApi.Models;
using KodiMinimalApi.Services;
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
            IKodiService kodi
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
            catch (KodiException ex)
            {
                return Results.Json(
                    new KodiProxyError("Kodi Error", ex.Message, 502),
                    AppJsonSerializerContext.Default.KodiProxyError,
                    statusCode: StatusCodes.Status502BadGateway);
            }
        });
    }
}
