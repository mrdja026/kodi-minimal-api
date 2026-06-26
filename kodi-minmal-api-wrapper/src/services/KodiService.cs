using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using KodiMinimalApi.Models;

namespace KodiMinimalApi.Services;

public class KodiService : IKodiService
{
    private readonly HttpClient _httpClient;
    private readonly KodiOptions _options;

    public KodiService(HttpClient httpClient, IOptions<KodiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        if (!string.IsNullOrEmpty(_options.Username) && !string.IsNullOrEmpty(_options.Password))
        {
            var authBytes = Encoding.UTF8.GetBytes($"{_options.Username}:{_options.Password}");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
        }
    }

    public async Task<string> SendAsync(string method, JsonNode? requestParams, CancellationToken ct)
    {
        var envelope = new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["method"] = method,
            ["id"] = 1
        };

        if (requestParams is not null)
            envelope["params"] = requestParams;

        var json = envelope.ToJsonString();
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var url = $"http://{_options.Host}:{_options.Port}/jsonrpc";
        var response = await _httpClient.PostAsync(url, content, ct);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(ct);
        var doc = JsonDocument.Parse(responseJson);

        if (doc.RootElement.TryGetProperty("error", out var error))
            throw new KodiException(error.GetProperty("message").GetString() ?? "Unknown Kodi error");

        return doc.RootElement.GetProperty("result").GetRawText();
    }
}
