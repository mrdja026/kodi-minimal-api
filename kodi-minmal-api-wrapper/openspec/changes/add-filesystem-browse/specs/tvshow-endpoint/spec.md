## ADDED Requirements
### Requirement: TVShowSearchDir Endpoint Handler
The proxy SHALL handle `TVShowSearchDir` commands on `POST /api/tvshows` by calling Kodi's `Files.GetDirectory` on the configured `TVScanDirectory` combined with the optional `Directory` subpath, optionally filtering results client-side by label.

#### Scenario: Browse TV root directory
- **WHEN** a `POST /api/tvshows` request is sent with `{"type":"TV_SEARCH_DIR"}`
- **AND** `TVScanDirectory` is configured as `"/home/mrdjan/.kodi/tv/"`
- **THEN** the proxy calls `Files.GetDirectory` with `{"directory": "/home/mrdjan/.kodi/tv/", "media": "video"}`
- **THEN** the proxy returns the raw JSON-RPC result as-is (no filtering)

#### Scenario: Browse TV subdirectory
- **WHEN** a `POST /api/tvshows` request is sent with `{"type":"TV_SEARCH_DIR","directory":"Penny Dreadful"}`
- **AND** `TVScanDirectory` is configured as `"/home/mrdjan/.kodi/tv/"`
- **THEN** the proxy calls `Files.GetDirectory` with `{"directory": "/home/mrdjan/.kodi/tv/Penny Dreadful", "media": "video"}`
- **THEN** the proxy returns the raw JSON-RPC result as-is (no filtering)

#### Scenario: Browse TV subdirectory with query filter
- **WHEN** a `POST /api/tvshows` request is sent with `{"type":"TV_SEARCH_DIR","directory":"Penny Dreadful","query":"S01"}`
- **AND** `TVScanDirectory` is configured as `"/home/mrdjan/.kodi/tv/"`
- **THEN** the proxy calls `Files.GetDirectory` with `{"directory": "/home/mrdjan/.kodi/tv/Penny Dreadful", "media": "video"}`
- **THEN** the proxy client-side filters the `files` array to entries where `label` contains `"S01"` (case-insensitive)
- **THEN** the proxy returns the filtered JSON

#### Scenario: TVScanDirectory not configured
- **WHEN** `TVScanDirectory` is not set in config and `{"type":"TV_SEARCH_DIR"}` is sent
- **THEN** the proxy returns a 502 error indicating the scan directory is not configured
