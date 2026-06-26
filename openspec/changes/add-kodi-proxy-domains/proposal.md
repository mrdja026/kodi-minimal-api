# Change: Complete Kodi JSON-RPC proxy with Volume, Files, and System Control

## Why
The project has a partially built Volume domain with endpoints, validators, and command models, but is not wired into Program.cs, lacks a Kodi HTTP client, and has no Files or System domains. The goal is a functional typed JSON-RPC proxy that validates input and forwards requests to a Kodi instance.

## What Changes
- **BREAKING**: Replace boilerplate Program.cs Todo code with wired-up Kodi proxy endpoints
- **ADDED**: `IKodiService` interface + `KodiService` implementation — the DI contract shared by all domain endpoints, sends JSON-RPC 2.0 POST requests and deserializes responses
- **MODIFIED**: Volume domain — add `VolumeSet` command, wire endpoint handler to call `IKodiService`, add VolumeSet/VolumeGet validators
- **ADDED**: File browser domain — `POST /api/files/directory` (wraps `Files.GetDirectory`)
- **ADDED**: System control domain — `POST /api/system/properties`, `POST /api/system/shutdown`, `POST /api/system/reboot` (wraps `System.*` methods)
- **ADDED**: `KodiOptions` configuration section in `appsettings.json` for host/port
- **ADDED**: Shared `KodiRpcRequest`/`KodiRpcResponse` models moved to a models file

## Impact
- Affected specs: `volume-control` (modified), `file-browser` (added), `system-control` (added), `kodi-client` (added)
- Affected code: `Program.cs`, `src/commands/KodiCommands.cs`, `src/domains/VolumeEndpoints.cs`, `src/validations/VolumeValidation.cs`, plus 4 new files
