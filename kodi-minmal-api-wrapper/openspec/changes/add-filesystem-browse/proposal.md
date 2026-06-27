# Change: Add Filesystem Browse Endpoints

## Why
Clients can currently search the Kodi video library by title, but have no way to browse the actual filesystem to discover what media folders exist under the configured scan directories. This makes it impossible for clients to know what shows/movies are available for scanning or to present a folder-browsing UX.

## What Changes
- **ADDED**: `MovieSearchDir` command record ("SEARCH_DIR") with optional `Query` string on `/api/movies`
- **ADDED**: `TVShowSearchDir` command record ("TV_SEARCH_DIR") with optional `Query` string on `/api/tvshows`
- **ADDED**: Endpoint handlers calling `Files.GetDirectory` on configured `MovieScanDirectory`/`TVScanDirectory`, client-side filtered by `label` when `Query` is provided
- **ADDED**: `MovieSearchDirValidator` / `TVShowSearchDirValidator` — validates `Query` is non-empty when present
- **ADDED**: `[JsonSerializable]` registrations for both new types

## Impact
- Affected specs: `movie-commands`, `tvshow-commands`, `movie-endpoint`, `tvshow-endpoint`, `movie-validation`, `tvshow-validation`
- Affected code: `KodiCommands.cs`, `MovieEndpoints.cs`, `TVShowEndpoints.cs`, `MovieValidation.cs`, `TVShowValidation.cs`, `Program.cs`
