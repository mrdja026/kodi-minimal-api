
using System.Text.Json.Serialization;

namespace KodiMinimalApi.Commands;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(VolumeUp), typeDiscriminator: "UP")]
[JsonDerivedType(typeof(VolumeDown), typeDiscriminator: "DOWN")]
[JsonDerivedType(typeof(VolumeGet), typeDiscriminator: "GET")]
[JsonDerivedType(typeof(VolumeSet), typeDiscriminator: "SET")]
[JsonDerivedType(typeof(PlayerPlay), typeDiscriminator: "PLAY")]
[JsonDerivedType(typeof(PlayerPause), typeDiscriminator: "PAUSE")]
[JsonDerivedType(typeof(PlayerStop), typeDiscriminator: "STOP")]
[JsonDerivedType(typeof(MovieList), typeDiscriminator: "LIST")]
[JsonDerivedType(typeof(MovieSearch), typeDiscriminator: "SEARCH")]
[JsonDerivedType(typeof(MovieRecent), typeDiscriminator: "RECENT")]
[JsonDerivedType(typeof(MoviePlay), typeDiscriminator: "PLAY_MOVIE")]
[JsonDerivedType(typeof(MovieScan), typeDiscriminator: "SCAN")]
[JsonDerivedType(typeof(MovieScanMovies), typeDiscriminator: "SCAN_MOVIES")]
[JsonDerivedType(typeof(MovieScanTV), typeDiscriminator: "SCAN_TV")]
[JsonDerivedType(typeof(TVShowList), typeDiscriminator: "TV_LIST")]
[JsonDerivedType(typeof(TVShowSearch), typeDiscriminator: "TV_SEARCH")]
[JsonDerivedType(typeof(TVShowSeasons), typeDiscriminator: "TV_SEASONS")]
[JsonDerivedType(typeof(TVShowEpisodes), typeDiscriminator: "TV_EPISODES")]
[JsonDerivedType(typeof(TVShowRecent), typeDiscriminator: "TV_RECENT")]
[JsonDerivedType(typeof(TVShowPlayEpisode), typeDiscriminator: "TV_PLAY_EPISODE")]

public abstract record CommandValue;

public record VolumeUp(int Level) : CommandValue;
public record VolumeDown(int Level) : CommandValue;
public record VolumeGet : CommandValue;
public record VolumeSet(int Level) : CommandValue;

public record PlayerPlay : CommandValue;
public record PlayerPause : CommandValue;
public record PlayerStop : CommandValue;

public record MovieList(int? Start, int? End) : CommandValue;
public record MovieSearch(string Query) : CommandValue;
public record MovieRecent(int? Limit) : CommandValue;
public record MoviePlay(int MovieId) : CommandValue;
public record MovieScan(string? Directory) : CommandValue;
public record MovieScanMovies : CommandValue;
public record MovieScanTV : CommandValue;

public record TVShowList(int? Start, int? End) : CommandValue;
public record TVShowSearch(string Query) : CommandValue;
public record TVShowSeasons(int TVShowId) : CommandValue;
public record TVShowEpisodes(int TVShowId, int? Season) : CommandValue;
public record TVShowRecent(int? Limit) : CommandValue;
public record TVShowPlayEpisode(int EpisodeId) : CommandValue;