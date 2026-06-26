# kodi-client Specification

## Purpose
TBD - created by archiving change add-kodi-proxy-domains. Update Purpose after archive.
## Requirements
### Requirement: IKodiService Interface
The system SHALL define an `IKodiService` interface as the DI contract for communicating with Kodi over JSON-RPC 2.0. All domain endpoint handlers SHALL depend on this interface.

#### Scenario: Interface contract
- **WHEN** any domain endpoint handler depends on `IKodiService`
- **THEN** it SHALL have access to a `SendAsync<TResponse>(string method, object? params, CancellationToken ct)` method
- **THEN** the method SHALL return a typed `KodiRpcResponse<TResponse>` result

### Requirement: KodiService Implementation
The system SHALL provide a `KodiService` class implementing `IKodiService`, wrapping `HttpClient` to POST JSON-RPC 2.0 payloads to the configured Kodi host.

#### Scenario: Send request to Kodi
- **WHEN** `SendAsync<TResponse>` is called with a method name and params object
- **THEN** it constructs `{"jsonrpc":"2.0","method":"...","params":...,"id":1}`
- **THEN** it POSTs to `http://{host}:{port}/jsonrpc`
- **THEN** it deserializes the response into `KodiRpcResponse<TResponse>`
- **THEN** it returns the typed response to the caller

#### Scenario: Kodi connection error
- **WHEN** the Kodi host is unreachable or returns an error status
- **THEN** the service SHALL return a `KodiRpcResponse` with the error details
- **THEN** the endpoint handler SHALL map it to an appropriate HTTP response

### Requirement: DI Registration
The system SHALL register `IKodiService` as a singleton in DI, with `KodiOptions` bound from the `"Kodi"` configuration section.

#### Scenario: Application startup
- **WHEN** the application starts
- **THEN** `builder.Services.AddSingleton<IKodiService, KodiService>()` is called
- **THEN** `KodiOptions` is bound from the `"Kodi"` config section via `builder.Services.Configure<KodiOptions>(...)`
- **THEN** `IHttpClientFactory` is used to create the underlying `HttpClient`

### Requirement: KodiRpcRequest and KodiRpcResponse Models
The system SHALL provide shared `KodiRpcRequest` and `KodiRpcResponse<T>` models that mirror the JSON-RPC 2.0 envelope.

#### Scenario: Request envelope
- **WHEN** a request is built
- **THEN** the envelope SHALL contain `jsonrpc`, `method`, `params`, and `id` fields

#### Scenario: Response envelope
- **WHEN** a response is received from Kodi
- **THEN** the envelope SHALL contain either a `result` field of type `T` or an `error` field
- **THEN** the service SHALL surface the result or error in a typed manner

