using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;
using KodiMinimalApi.Models;
using KodiMinimalApi.Services;
using static KodiMinimalApi.Models.ErrorHelper;

namespace KodiMinimalApi.Features;

public record FilesDirectoryRequest(string Directory, string? Media);
public record FilesSourcesRequest(string Media);

public static class FileEndpoints
{
    public static void MapFileEndpoints(this IEndpointRouteBuilder app)
    {
        var fileApi = app.MapGroup("/api/files");

        fileApi.MapPost("/directory", async (
            FilesDirectoryRequest request,
            IValidator<FilesDirectoryRequest> validator,
            IKodiService kodi
        ) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult.ToErrors());

            try
            {
                var result = await kodi.SendAsync("Files.GetDirectory",
                    new JsonObject
                    {
                        ["directory"] = request.Directory,
                        ["media"] = request.Media ?? "files"
                    });

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

        fileApi.MapPost("/sources", async (
            FilesSourcesRequest request,
            IValidator<FilesSourcesRequest> validator,
            IKodiService kodi
        ) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult.ToErrors());

            try
            {
                var result = await kodi.SendAsync("Files.GetSources",
                    new JsonObject
                    {
                        ["media"] = request.Media
                    });

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
