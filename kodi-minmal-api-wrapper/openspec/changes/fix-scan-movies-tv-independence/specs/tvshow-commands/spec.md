## ADDED Requirements
### Requirement: TVShowScan Record
The system SHALL define a `TVShowScan` record that extends `CommandValue` with `[JsonDerivedType(typeof(TVShowScan), "TV_SCAN")]`. This record SHALL have no properties.

#### Scenario: Deserialize TV_SCAN
- **WHEN** a `POST /api/tvshows` request is sent with `{"type":"TV_SCAN"}`
- **THEN** the proxy deserializes it as `TVShowScan`

### Requirement: TVShowScan Endpoint Handler
The proxy SHALL expose a `TVShowScan` command under `/api/tvshows` that scans the configured TV directory in the Kodi video library.

#### Scenario: Scan TV directory
- **WHEN** a `POST /api/tvshows` request is sent with `{"type":"TV_SCAN"}`
- **THEN** the proxy calls `VideoLibrary.Scan` with `{showdialogs: false, directory: <TVScanDirectory from config>}`
- **THEN** the proxy returns the raw JSON-RPC result

#### Scenario: TVScanDirectory not configured
- **WHEN** `TVScanDirectory` is not set in config and `{"type":"TV_SCAN"}` is sent
- **THEN** the proxy returns a 502 error indicating the TV scan directory is not configured
