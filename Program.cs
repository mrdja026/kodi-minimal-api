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

app.Run();

[JsonSerializable(typeof(CommandValue))]
[JsonSerializable(typeof(VolumeGet))]
[JsonSerializable(typeof(VolumeSet))]
[JsonSerializable(typeof(VolumeUp))]
[JsonSerializable(typeof(VolumeDown))]
[JsonSerializable(typeof(FilesDirectoryRequest))]
[JsonSerializable(typeof(SystemPropertiesRequest))]
[JsonSerializable(typeof(SystemActionRequest))]
[JsonSerializable(typeof(KodiProxyError))]
[JsonSerializable(typeof(KodiActionResult))]
[JsonSerializable(typeof(ValidationError))]
[JsonSerializable(typeof(ValidationError[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
