namespace KodiMinimalApi.Models;

public record KodiProxyError(string Title, string Detail, int StatusCode);

public record KodiActionResult(string Message);

public record ValidationError(string PropertyName, string Message);
