## ADDED Requirements
### Requirement: Scan Directory Configuration
The `KodiOptions` class SHALL include optional `MovieScanDirectory` and `TVScanDirectory` properties for configuring default scan paths.

#### Scenario: Configured in appsettings.json
- **WHEN** `appsettings.json` contains `"MovieScanDirectory": "/home/mrdjan/.kodi/media/"` and `"TVScanDirectory": "/home/mrdjan/.kodi/tv/"`
- **THEN** `KodiOptions.MovieScanDirectory` is bound to `"/home/mrdjan/.kodi/media/"`
- **THEN** `KodiOptions.TVScanDirectory` is bound to `"/home/mrdjan/.kodi/tv/"`

#### Scenario: Directories optional
- **WHEN** `appsettings.json` does not contain `MovieScanDirectory` or `TVScanDirectory`
- **THEN** the respective properties are `null`
