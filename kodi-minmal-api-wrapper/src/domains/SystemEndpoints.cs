using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;
using KodiMinimalApi.Models;
using KodiMinimalApi.Services;
using static KodiMinimalApi.Models.ErrorHelper;

namespace KodiMinimalApi.Features;

public record SystemPropertiesRequest(string[] Properties);
public record SystemActionRequest(bool Confirm);
public record KodiRawRequest(string Method, JsonElement? Params);

public static class SystemEndpoints
{
    public static void MapSystemEndpoints(this IEndpointRouteBuilder app)
    {
        var systemApi = app.MapGroup("/api/system");

        systemApi.MapPost("/properties", async (
            SystemPropertiesRequest request,
            IValidator<SystemPropertiesRequest> validator,
            IKodiService kodi
        ) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult.ToErrors());

            try
            {
                var props = new JsonArray(
                    request.Properties.Select(p => (JsonNode?)JsonValue.Create(p)).ToArray());

                var result = await kodi.SendAsync("System.GetProperties",
                    new JsonObject { ["properties"] = props });

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

        systemApi.MapPost("/shutdown", async (
            SystemActionRequest request,
            IValidator<SystemActionRequest> validator,
            IKodiService kodi
        ) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult.ToErrors());

            try
            {
                await kodi.SendAsync("System.Shutdown", null);
                return Results.Ok(new KodiActionResult("Shutdown command sent"));
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

        systemApi.MapPost("/reboot", async (
            SystemActionRequest request,
            IValidator<SystemActionRequest> validator,
            IKodiService kodi
        ) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult.ToErrors());

            try
            {
                await kodi.SendAsync("System.Reboot", null);
                return Results.Ok(new KodiActionResult("Reboot command sent"));
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

        var debugApi = app.MapGroup("/api/debug");

        debugApi.MapPost("/kodi", async (
            KodiRawRequest request,
            IKodiService kodi
        ) =>
        {
            try
            {
                var jsonParams = request.Params is null
                    ? null
                    : JsonNode.Parse(request.Params?.GetRawText() ?? "null");
                var result = await kodi.SendAsync(request.Method, jsonParams);
                return Results.Text(result, "application/json");
            }
            catch (HttpRequestException)
            {
                return Results.Json(
                    new KodiProxyError("Kodi Unreachable", "Could not connect to Kodi.", 502),
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
