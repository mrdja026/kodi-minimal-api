## Context

A .NET 9 AOT-ready minimal API that proxies typed requests to a Kodi instance over JSON-RPC 2.0 (HTTP POST). The existing Volume domain is partially written but disconnected; three domains (Volume, Files, System) must be completed with a shared transport layer.

## Goals / Non-Goals

- Goals:
  - Centralized Kodi HTTP transport via `IKodiService` / `KodiService`
  - Typed commands with FluentValidation per domain
  - Configuration-driven Kodi target (host/port)
  - AOT-safe JSON serialization
- Non-Goals:
  - No generic passthrough endpoint
  - No authentication/authorisation on the proxy itself
  - No response caching or retry logic

## Decisions

- **Decision**: `IKodiService` interface with `KodiService` implementation, registered as singleton in DI
  - **Rationale**: An interface-based contract allows all domain endpoints (Volume, Files, System) to depend on the same `IKodiService` while keeping the transport layer swappable for testing. `KodiService` wraps `IHttpClientFactory` to create `HttpClient` instances.
  - **Alternatives**: Concrete `KodiClient` class — rejected because it prevents unit testing domain handlers; inline `HttpClient` per endpoint — rejected due to duplication.

- **Decision**: Domain endpoints use a shared `KodiRpcRequest` / `KodiRpcResponse<T>` model pair
  - **Rationale**: Every Kodi JSON-RPC call follows the same envelope (`{"jsonrpc":"2.0","method":"...","params":...,"id":1}`). A generic response model with `T Result` allows typed deserialization.
  - **Alternatives**: Raw `JsonElement` passthrough — rejected because typed endpoints should return typed responses.

- **Decision**: `KodiOptions` bound from `appsettings.json` section `"Kodi"`
  - **Rationale**: Standard .NET options pattern; works with AOT trimming. Environment variable override via `Kodi__Host` is available for free.
  - **Format**: `{ "Kodi": { "Host": "localhost", "Port": 8080 } }`
  - **URL built as**: `http://{Host}:{Port}/jsonrpc`

- **Decision**: Each domain uses its own endpoint group with a shared prefix `/api`
  - Volume: `POST /api/volume` (command-based polymorphic body)
  - Files: `POST /api/files/directory` (typed body)
  - System: `POST /api/system/properties`, `POST /api/system/shutdown`, `POST /api/system/reboot`

## Risks / Trade-offs

- **AOT compatibility**: All JSON-serialized types must be in `JsonSerializerContext`. The polymorphic `CommandValue` hierarchy already uses `[JsonDerivedType]`. New models must follow the same pattern.
- **Volume command routing**: A single `POST /api/volume` endpoint uses polymorphic deserialization (`type` discriminator) to dispatch VolumeGet/VolumeSet/VolumeUp/VolumeDown. This is elegant but makes the OpenAPI schema more complex — acceptable for this scope.

## Open Questions

- None — all clarifying questions have been answered by the user.
