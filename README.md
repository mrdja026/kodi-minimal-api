# Kodi Minimal API Wrapper

A lightweight .NET 9 proxy that translates HTTP REST calls into Kodi JSON-RPC 2.0 commands. Provides a simple, validated API for controlling Kodi media center without dealing with raw JSON-RPC.

## Endpoints

| Endpoint | Method | Status |
|----------|--------|--------|
| `/api/volume` | POST | Working |
| `/api/player` | POST | Working |
| `/api/system/properties` | POST | Working |
| `/api/system/shutdown` | POST | Working |
| `/api/system/reboot` | POST | Working |
| `/api/files/directory` | POST | Implemented, depends on Kodi sources |

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

## Architecture

- **Native AOT ready** — compiled with `PublishAot=true`, no reflection at runtime
- **Polymorphic commands** — Volume and Player endpoints use a `type` discriminator for polymorphic JSON deserialization (e.g., `{"type":"PLAY"}`)
- **Player auto-detection** — Player endpoint automatically queries `Player.GetActivePlayers` and uses the first active `playerid`
- **FluentValidation** — All requests validated with inheritance-aware validators
- **Source-generated JSON** — `AppJsonSerializerContext` provides AOT-safe serialization via `[JsonSerializable]` attributes
