using System.Text.Json.Serialization;
using FluentValidation;
using KodiMinimalApi.Commands;
using KodiMinimalApi.Features;
using KodiMinimalApi.Models;
using KodiMinimalApi.Services;
using KodiMinimalApi.Validators;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Logging.AddJsonConsole();

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.Configure<KodiOptions>(
    builder.Configuration.GetSection("Kodi"));

builder.Services.AddHttpClient<IKodiService, KodiService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddValidatorsFromAssemblyContaining<VolumeRequestValidator>();

var app = builder.Build();

app.MapOpenApi();
app.MapVolumeEndpoints();
app.MapFileEndpoints();
app.MapSystemEndpoints();
app.MapPlayerEndpoints();
app.MapMovieEndpoints();
app.MapTVShowEndpoints();

app.Run();

[JsonSerializable(typeof(CommandValue))]
[JsonSerializable(typeof(VolumeGet))]
[JsonSerializable(typeof(VolumeSet))]
[JsonSerializable(typeof(VolumeUp))]
[JsonSerializable(typeof(VolumeDown))]
[JsonSerializable(typeof(PlayerPlay))]
[JsonSerializable(typeof(PlayerPause))]
[JsonSerializable(typeof(PlayerStop))]
[JsonSerializable(typeof(MovieList))]
[JsonSerializable(typeof(MovieSearch))]
[JsonSerializable(typeof(MovieRecent))]
[JsonSerializable(typeof(MoviePlay))]
[JsonSerializable(typeof(MovieScan))]
[JsonSerializable(typeof(MovieScanMovies))]
[JsonSerializable(typeof(MovieScanTV))]
[JsonSerializable(typeof(TVShowList))]
[JsonSerializable(typeof(TVShowSearch))]
[JsonSerializable(typeof(TVShowSeasons))]
[JsonSerializable(typeof(TVShowEpisodes))]
[JsonSerializable(typeof(TVShowRecent))]
[JsonSerializable(typeof(TVShowPlayEpisode))]
[JsonSerializable(typeof(FilesDirectoryRequest))]
[JsonSerializable(typeof(FilesSourcesRequest))]
[JsonSerializable(typeof(SystemPropertiesRequest))]
[JsonSerializable(typeof(SystemActionRequest))]
[JsonSerializable(typeof(KodiRawRequest))]
[JsonSerializable(typeof(KodiProxyError))]
[JsonSerializable(typeof(KodiActionResult))]
[JsonSerializable(typeof(ValidationError))]
[JsonSerializable(typeof(ValidationError[]))]
[JsonSerializable(typeof(int))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
