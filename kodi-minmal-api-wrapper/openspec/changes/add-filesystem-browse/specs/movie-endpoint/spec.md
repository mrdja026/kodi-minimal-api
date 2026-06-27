## ADDED Requirements
### Requirement: MovieSearchDir Endpoint Handler
The proxy SHALL handle `MovieSearchDir` commands on `POST /api/movies` by calling Kodi's `Files.GetDirectory` on the configured `MovieScanDirectory` combined with the optional `Directory` subpath, optionally filtering results client-side by label.

#### Scenario: Browse movie root directory
- **WHEN** a `POST /api/movies` request is sent with `{"type":"SEARCH_DIR"}`
- **AND** `MovieScanDirectory` is configured as `"/home/mrdjan/.kodi/media/"`
- **THEN** the proxy calls `Files.GetDirectory` with `{"directory": "/home/mrdjan/.kodi/media/", "media": "video"}`
- **THEN** the proxy returns the raw JSON-RPC result as-is (no filtering)

#### Scenario: Browse movie subdirectory
- **WHEN** a `POST /api/movies` request is sent with `{"type":"SEARCH_DIR","directory":"Darkest Hour (2017)"}`
- **AND** `MovieScanDirectory` is configured as `"/home/mrdjan/.kodi/media/"`
- **THEN** the proxy calls `Files.GetDirectory` with `{"directory": "/home/mrdjan/.kodi/media/Darkest Hour (2017)", "media": "video"}`
- **THEN** the proxy returns the raw JSON-RPC result as-is (no filtering)

#### Scenario: Browse movie subdirectory with query filter
- **WHEN** a `POST /api/movies` request is sent with `{"type":"SEARCH_DIR","directory":"Darkest Hour (2017)","query":"Hour"}`
- **AND** `MovieScanDirectory` is configured as `"/home/mrdjan/.kodi/media/"`
- **THEN** the proxy calls `Files.GetDirectory` with `{"directory": "/home/mrdjan/.kodi/media/Darkest Hour (2017)", "media": "video"}`
- **THEN** the proxy client-side filters the `files` array to entries where `label` contains `"Hour"` (case-insensitive)
- **THEN** the proxy returns the filtered JSON

#### Scenario: MovieScanDirectory not configured
- **WHEN** `MovieScanDirectory` is not set in config and `{"type":"SEARCH_DIR"}` is sent
- **THEN** the proxy returns a 502 error indicating the scan directory is not configured
