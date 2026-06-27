# Change: Fix Scan Movies / Scan TV Independence

## Why
`MovieScanMovies` and `MovieScanTV` both call `VideoLibrary.Scan` with no directory (scan everything), ignoring their configured directories entirely. There's no way to scan just movies or just TV shows independently. The `MovieScanDirectory`/`TVScanDirectory` config options were spec'd in the previous change but never implemented — `KodiOptions.cs`, `appsettings.json`, and the endpoint handlers were all left incomplete.

## What Changes
- **FIXED**: `KodiOptions.cs` — add `MovieScanDirectory` and `TVScanDirectory` properties
- **FIXED**: `appsettings.json` — add both config options with defaults
- **FIXED**: `MovieEndpoints.cs` — inject `IOptions<KodiOptions>`, use configured directories in `MovieScanMovies` and `MovieScanTV` handlers, return 502 if directory not configured
- **FIXED**: `MovieScanTV` handler — use `TVScanDirectory` instead of `null`
- **FIXED**: `MovieScanMovies` handler — use `MovieScanDirectory` instead of `null`
- **REMOVED**: `MovieScanTV` from `/api/movies` endpoint (doesn't belong there)
- **ADDED**: `TVShowScan` command (`"TV_SCAN"`) under `/api/tvshows` endpoint
- **ADDED**: `TVShowScan` endpoint handler in `TVShowEndpoints.cs`
- **ADDED**: `TVShowScanValidator` in `TVShowValidation.cs`
- **ADDED**: `[JsonSerializable]` and `[JsonDerivedType]` registrations for `TVShowScan`

## Impact
- Affected specs: `kodi-client`, `tvshow-commands`, `tvshow-endpoint`, `tvshow-validation`, `tvshow-registration`
- Affected code: `KodiOptions.cs`, `appsettings.json`, `KodiCommands.cs`, `MovieEndpoints.cs`, `MovieValidation.cs`, `TVShowEndpoints.cs`, `TVShowValidation.cs`, `Program.cs`
