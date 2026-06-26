## 0. Kodi Service Interface
- [ ] 0.1 Define `IKodiService` interface with `SendAsync<TResponse>(string method, object? params, CancellationToken ct)` method
- [ ] 0.2 Implement `KodiService` class wrapping `IHttpClientFactory`, building the JSON-RPC 2.0 envelope and deserializing responses
- [ ] 0.3 Create `KodiOptions` class with `Host` and `Port` properties, bound from `"Kodi"` section in `appsettings.json`
- [ ] 0.4 Add `appsettings.json` Kodi configuration section
- [ ] 0.5 Create `KodiRpcRequest` and `KodiRpcResponse<T>` shared models
- [ ] 0.6 Register `IKodiService` (singleton) and `KodiOptions` in DI in `Program.cs`
- [ ] 0.7 Add all request/response types to `JsonSerializerContext` for AOT

## 1. Volume Domain (Complete)
- [ ] 1.1 Add `VolumeSet` record to `KodiCommands.cs`
- [ ] 1.2 Add `VolumeSetValidator` and `VolumeGetValidator` to `VolumeValidation.cs`
- [ ] 1.3 Rewrite `VolumeEndpoints.cs` handler to dispatch commands via `IKodiService` and return typed responses
- [ ] 1.4 Wire `MapVolumeEndpoints()` in Program.cs

## 2. Files Domain (Stub)
- [ ] 2.1 Create `FileEndpoints.cs` with `POST /api/files/directory` endpoint
- [ ] 2.2 Add request model `FilesDirectoryRequest` with `directory` (required) and `media` (optional) fields
- [ ] 2.3 Add `FilesDirectoryValidator` with non-empty string check on `directory`
- [ ] 2.4 Wire `MapFileEndpoints()` in Program.cs

## 3. System Control Domain
- [ ] 3.1 Create `SystemEndpoints.cs` with `POST /api/system/properties`, `/shutdown`, `/reboot` endpoints
- [ ] 3.2 Add request models: `SystemPropertiesRequest`, `SystemActionRequest` with `Confirm` field
- [ ] 3.3 Add validators: properties non-empty array check, confirm=true check for destructive ops
- [ ] 3.4 Wire `MapSystemEndpoints()` in Program.cs

## 4. Cleanup
- [ ] 4.1 Remove boilerplate Todo code from `Program.cs`
- [ ] 4.2 Verify `dotnet build` succeeds with no AOT warnings
- [ ] 4.3 Verify `dotnet run` starts and endpoints respond
