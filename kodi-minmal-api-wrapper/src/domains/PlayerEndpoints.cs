using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;
using KodiMinimalApi.Commands;
using KodiMinimalApi.Models;
using KodiMinimalApi.Services;
using static KodiMinimalApi.Models.ErrorHelper;

namespace KodiMinimalApi.Features;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        var playerApi = app.MapGroup("/api/player");

        playerApi.MapPost("/", async (
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
                if (command is PlayerPlay { File: not null } playFile)
                {
                    var openResult = await kodi.SendAsync("Player.Open",
                        new JsonObject { ["item"] = new JsonObject { ["file"] = playFile.File } });
                    return Results.Text(openResult, "application/json");
                }

                var activePlayersJson = await kodi.SendAsync("Player.GetActivePlayers", null);
                var players = JsonNode.Parse(activePlayersJson)?.AsArray();

                if (players is null || players.Count == 0)
                    return Results.Json(
                        new KodiProxyError("No Active Player", "No player is currently active. Start playback first.", 502),
                        AppJsonSerializerContext.Default.KodiProxyError,
                        statusCode: StatusCodes.Status502BadGateway);

                var playerId = players[0]!["playerid"]!.GetValue<int>();

                var result = command switch
                {
                    PlayerPlay => await kodi.SendAsync("Player.PlayPause",
                        new JsonObject { ["playerid"] = playerId, ["play"] = true }),

                    PlayerPause => await kodi.SendAsync("Player.PlayPause",
                        new JsonObject { ["playerid"] = playerId, ["play"] = false }),

                    PlayerStop => await kodi.SendAsync("Player.Stop",
                        new JsonObject { ["playerid"] = playerId }),

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
}
