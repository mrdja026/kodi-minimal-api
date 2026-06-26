using System.Text.Json.Nodes;

namespace KodiMinimalApi.Services;

public interface IKodiService
{
    Task<string> SendAsync(string method, JsonNode? parameters, CancellationToken ct = default);
}
