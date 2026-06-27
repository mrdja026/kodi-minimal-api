# Kodi Minimal API Wrapper

A lightweight .NET 9 proxy that translates HTTP REST calls into Kodi JSON-RPC 2.0 commands. Provides a simple, validated API for controlling Kodi media center without dealing with raw JSON-RPC.

## Endpoints

| Endpoint                 | Method | Status                               |
| ------------------------ | ------ | ------------------------------------ |
| `/api/volume`            | POST   | Working                              |
| `/api/player`            | POST   | Working                              |
| `/api/system/properties` | POST   | Working                              |
| `/api/system/shutdown`   | POST   | Working                              |
| `/api/system/reboot`     | POST   | Working                              |
| `/api/movies`            | POST   | Working                              |
| `/api/tvshows`           | POST   | Working                              |
| `/api/files/directory`   | POST   | Implemented, depends on Kodi sources |
| `/api/files/sources`     | POST   | Working                              |
| `/api/debug/kodi`        | POST   | Debug passthrough                    |

## Configuration

Edit `appsettings.json`:

```json
{
  "Kodi": {
    "Host": "192.168.0.100",
    "Port": 8080,
    "Username": "kodi",
    "Password": "1233"
  }
}
```

The proxy listens on `http://localhost:5149` by default (configured in `Properties/launchSettings.json`).

## Build & Run

```powershell
dotnet build
dotnet run
```

## Usage

All commands use PowerShell `Invoke-RestMethod`. Replace `localhost:5149` with your proxy's address.

### Quick Reference (Client Handover)

```powershell
# Get current volume
Invoke-RestMethod -Method Post -Body '{"type":"GET"}' -ContentType "application/json" -Uri "http://localhost:5149/api/volume"

# Volume up by 10
Invoke-RestMethod -Method Post -Body '{"type":"UP","level":10}' -ContentType "application/json" -Uri "http://localhost:5149/api/volume"

# Volume down by 10
Invoke-RestMethod -Method Post -Body '{"type":"DOWN","level":10}' -ContentType "application/json" -Uri "http://localhost:5149/api/volume"

# Play (resume playback)
Invoke-RestMethod -Method Post -Body '{"type":"PLAY"}' -ContentType "application/json" -Uri "http://localhost:5149/api/player"

# Pause
Invoke-RestMethod -Method Post -Body '{"type":"PAUSE"}' -ContentType "application/json" -Uri "http://localhost:5149/api/player"

# Stop
Invoke-RestMethod -Method Post -Body '{"type":"STOP"}' -ContentType "application/json" -Uri "http://localhost:5149/api/player"

# Scan all sources (alias for SCAN)
Invoke-RestMethod -Method Post -Body '{"type":"SCAN_MOVIES"}' -ContentType "application/json" -Uri "http://localhost:5149/api/movies"

# Scan all sources (alias for SCAN)
Invoke-RestMethod -Method Post -Body '{"type":"SCAN_TV"}' -ContentType "application/json" -Uri "http://localhost:5149/api/movies"
```

### Volume

Maps to Kodi JSON-RPC `Application.GetProperties` / `Application.SetVolume`.

```powershell
# Get current volume
Invoke-RestMethod -Method Post -Body '{"type":"GET"}' -ContentType "application/json" -Uri "http://localhost:5149/api/volume"

# Set volume to 50
Invoke-RestMethod -Method Post -Body '{"type":"SET","level":50}' -ContentType "application/json" -Uri "http://localhost:5149/api/volume"

# Volume up by 10
Invoke-RestMethod -Method Post -Body '{"type":"UP","level":10}' -ContentType "application/json" -Uri "http://localhost:5149/api/volume"

# Volume down by 10
Invoke-RestMethod -Method Post -Body '{"type":"DOWN","level":10}' -ContentType "application/json" -Uri "http://localhost:5149/api/volume"
```

### Player

Maps to Kodi JSON-RPC `Player.GetActivePlayers` + `Player.PlayPause` / `Player.Stop`. Auto-detects the first active `playerid`.

```powershell
# Play (resume playback on active player)
Invoke-RestMethod -Method Post -Body '{"type":"PLAY"}' -ContentType "application/json" -Uri "http://localhost:5149/api/player"

# Pause
Invoke-RestMethod -Method Post -Body '{"type":"PAUSE"}' -ContentType "application/json" -Uri "http://localhost:5149/api/player"

# Stop
Invoke-RestMethod -Method Post -Body '{"type":"STOP"}' -ContentType "application/json" -Uri "http://localhost:5149/api/player"
```

### Movies

Maps to Kodi JSON-RPC `VideoLibrary.GetMovies` / `VideoLibrary.GetRecentlyAddedMovies` / `Player.Open`.

Requires movies in the Kodi library (ingested via library scan).

```powershell
# List first 50 movies
Invoke-RestMethod -Method Post -Body '{"type":"LIST","start":0,"end":50}' -ContentType "application/json" -Uri "http://localhost:5149/api/movies"

# Search movies by title
Invoke-RestMethod -Method Post -Body '{"type":"SEARCH","query":"Star Wars"}' -ContentType "application/json" -Uri "http://localhost:5149/api/movies"

# Recently added movies (last 10)
Invoke-RestMethod -Method Post -Body '{"type":"RECENT","limit":10}' -ContentType "application/json" -Uri "http://localhost:5149/api/movies"

# Play a library movie by ID (get the ID from LIST or SEARCH results)
Invoke-RestMethod -Method Post -Body '{"type":"PLAY_MOVIE","movieId":42}' -ContentType "application/json" -Uri "http://localhost:5149/api/movies"

# Scan ALL video sources for new content
Invoke-RestMethod -Method Post -Body '{"type":"SCAN"}' -ContentType "application/json" -Uri "http://localhost:5149/api/movies"

# Scan movies directory (alias, same as SCAN)
Invoke-RestMethod -Method Post -Body '{"type":"SCAN_MOVIES"}' -ContentType "application/json" -Uri "http://localhost:5149/api/movies"

# Scan TV directory (alias, same as SCAN)
Invoke-RestMethod -Method Post -Body '{"type":"SCAN_TV"}' -ContentType "application/json" -Uri "http://localhost:5149/api/movies"

# Scan a specific directory
Invoke-RestMethod -Method Post -Body '{"type":"SCAN","directory":"/storage/.kodi/media/movies"}' -ContentType "application/json" -Uri "http://localhost:5149/api/movies"
```

### TV Shows

Maps to Kodi JSON-RPC `VideoLibrary.GetTVShows` / `VideoLibrary.GetSeasons` / `VideoLibrary.GetEpisodes` / `Player.Open`.

Requires TV shows in the Kodi library (ingested via library scan).

```powershell
# List first 50 TV shows
Invoke-RestMethod -Method Post -Body '{"type":"TV_LIST","start":0,"end":50}' -ContentType "application/json" -Uri "http://localhost:5149/api/tvshows"

# Search TV shows by title
Invoke-RestMethod -Method Post -Body '{"type":"TV_SEARCH","query":"Breaking Bad"}' -ContentType "application/json" -Uri "http://localhost:5149/api/tvshows"

# Get seasons for a TV show (tvshowId from LIST or SEARCH)
Invoke-RestMethod -Method Post -Body '{"type":"TV_SEASONS","tvshowId":123}' -ContentType "application/json" -Uri "http://localhost:5149/api/tvshows"

# List episodes for a TV show (season is optional, 0=all)
Invoke-RestMethod -Method Post -Body '{"type":"TV_EPISODES","tvshowId":123,"season":1}' -ContentType "application/json" -Uri "http://localhost:5149/api/tvshows"

# Recently added episodes (last 10)
Invoke-RestMethod -Method Post -Body '{"type":"TV_RECENT","limit":10}' -ContentType "application/json" -Uri "http://localhost:5149/api/tvshows"

# Play a specific episode by ID (get episodeId from EPISODES or RECENT)
Invoke-RestMethod -Method Post -Body '{"type":"TV_PLAY_EPISODE","episodeId":456}' -ContentType "application/json" -Uri "http://localhost:5149/api/tvshows"
```

### System

Maps to Kodi JSON-RPC `System.GetProperties` / `System.Shutdown` / `System.Reboot`.

Only these properties are valid for `System.GetProperties` (verified via `JSONRPC.Introspect`):

- `canshutdown`
- `cansuspend`
- `canhibernate`
- `canreboot`

**`"version"` and `"system.friendlyname"` are NOT valid** — Kodi will reject them.

```powershell
# Get system capabilities
Invoke-RestMethod -Method Post -Body '{"properties":["canshutdown","canreboot"]}' -ContentType "application/json" -Uri "http://localhost:5149/api/system/properties"

# Shutdown (requires confirm: true)
Invoke-RestMethod -Method Post -Body '{"confirm":true}' -ContentType "application/json" -Uri "http://localhost:5149/api/system/shutdown"

# Reboot (requires confirm: true)
Invoke-RestMethod -Method Post -Body '{"confirm":true}' -ContentType "application/json" -Uri "http://localhost:5149/api/system/reboot"
```

### File Browser

Maps to Kodi JSON-RPC `Files.GetDirectory`. Requires valid sources configured in Kodi. Returns 502 if Kodi has no sources or the path is invalid.

```powershell
# List directory contents
Invoke-RestMethod -Method Post -Body '{"directory":"smb://server/share","media":"files"}' -ContentType "application/json" -Uri "http://localhost:5149/api/files/directory"
```

### File Sources

Maps to Kodi JSON-RPC `Files.GetSources`. Lists configured media sources.

```powershell
# List video sources (movies, TV)
Invoke-RestMethod -Method Post -Body '{"media":"video"}' -ContentType "application/json" -Uri "http://localhost:5149/api/files/sources"
```

### Debug Passthrough

Sends a raw JSON-RPC method to Kodi and returns the full response. Useful for testing Kodi API calls directly.

```powershell
# List all TV shows
Invoke-RestMethod -Method Post -Body '{"method":"VideoLibrary.GetTVShows","params":{"properties":["title","year"]}}' -ContentType "application/json" -Uri "http://localhost:5149/api/debug/kodi"

# List all movies
Invoke-RestMethod -Method Post -Body '{"method":"VideoLibrary.GetMovies","params":{"properties":["title","year"]}}' -ContentType "application/json" -Uri "http://localhost:5149/api/debug/kodi"
```

## Architecture

- **Native AOT ready** — compiled with `PublishAot=true`, no reflection at runtime
- **Polymorphic commands** — Volume and Player endpoints use a `type` discriminator for polymorphic JSON deserialization (e.g., `{"type":"PLAY"}`)
- **Player auto-detection** — Player endpoint automatically queries `Player.GetActivePlayers` and uses the first active `playerid`
- **FluentValidation** — All requests validated with inheritance-aware validators
- **Source-generated JSON** — `AppJsonSerializerContext` provides AOT-safe serialization via `[JsonSerializable]` attributes

## Known Issues

### Library Scan (SCAN / SCAN_MOVIES / SCAN_TV) is flaky

- **Disk space**: Kodi's SQLite database (`MyVideos131.db`) fails with `SQLITE_FULL` if the Pi's storage is near capacity (<1GB free). Scan returns `"OK"` but no files are ingested. Free up space via `df -h`.
- **Scraper dependency**: Kodi requires internet access + a working scraper addon (TMDB/TVDB) to add files to the library. If the scraper fails, scan returns `"OK"` but nothing appears.
- **Kodi 21 Omega bug**: `VideoLibrary.Scan` with a specific `directory` param is known to fail for TV shows (xbmc/xbmc#26169). The proxy's SCAN omits the directory parameter to work around this.
- **Directory hashing**: Kodi uses mtime-based hashes to detect changes. If the hash doesn't update, Kodi skips the scan even for new files. Restarting Kodi can reset the hash cache.

### Volume commands are jittery

- Kodi's `Application.SetVolume` and `Application.GetProperties` use the system audio level, which can lag or report stale values.
- Rapid-fire UP/DOWN commands may be batched or dropped by Kodi's event loop.
- The volume level reported by `GET` may not match the actual system level if Kodi is still processing a previous `SET`/`UP`/`DOWN`. Add a 200-300ms delay between consecutive volume commands for more reliable results.
- Moviews scan works (90%) JSON RCP is flaky
- TV_Shows needs metadata that is out my controll and it needs to be Series Name -> Season X - > SeriesName.S01E01.ext, and even that does not guarantee
