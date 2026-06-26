
using System.Text.Json.Serialization;

namespace KodiMinimalApi.Commands;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(VolumeUp), typeDiscriminator: "UP")]
[JsonDerivedType(typeof(VolumeDown), typeDiscriminator: "DOWN")]
[JsonDerivedType(typeof(VolumeGet), typeDiscriminator: "GET")]
[JsonDerivedType(typeof(VolumeSet), typeDiscriminator: "SET")]

public abstract record CommandValue;

public record VolumeUp(int Level) : CommandValue;
public record VolumeDown(int Level) : CommandValue;
public record VolumeGet : CommandValue;
public record VolumeSet(int Level) : CommandValue;