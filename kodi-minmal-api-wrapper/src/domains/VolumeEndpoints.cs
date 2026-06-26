using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;
using KodiMinimalApi.Commands;
using KodiMinimalApi.Models;
using KodiMinimalApi.Services;
using static KodiMinimalApi.Models.ErrorHelper;

namespace KodiMinimalApi.Features;

public static class VolumeEndpoints
{
    public static void MapVolumeEndpoints(this IEndpointRouteBuilder app)
    {
        var volumeApi = app.MapGroup("/api/volume");

        volumeApi.MapPost("/", async (
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
                    VolumeGet => await kodi.SendAsync("Application.GetProperties",
                        new JsonObject { ["properties"] = new JsonArray((JsonNode?)JsonValue.Create("volume")) }),

                    VolumeSet set => await kodi.SendAsync("Application.SetVolume",
                        new JsonObject { ["volume"] = set.Level }),

                    VolumeUp up => await kodi.SendAsync("Application.SetVolume",
                        new JsonObject { ["volume"] = new JsonObject { ["increment"] = up.Level } }),

                    VolumeDown down => await kodi.SendAsync("Application.SetVolume",
                        new JsonObject { ["volume"] = new JsonObject { ["decrement"] = down.Level } }),

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
