using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;
using KodiMinimalApi.Commands;
using KodiMinimalApi.Models;
using KodiMinimalApi.Services;
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

                    MovieScanMovies _ => await ScanLibrary(kodi, null),
                    MovieScanTV _ => await ScanLibrary(kodi, null),

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

    static Task<string> ScanLibrary(IKodiService kodi, string? dir) =>
        kodi.SendAsync("VideoLibrary.Scan",
            dir is not null
                ? new JsonObject { ["showdialogs"] = false, ["directory"] = dir }
                : new JsonObject { ["showdialogs"] = false });
}
