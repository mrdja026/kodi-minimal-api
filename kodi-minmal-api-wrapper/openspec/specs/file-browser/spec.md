# file-browser Specification

## Purpose
TBD - created by archiving change add-kodi-proxy-domains. Update Purpose after archive.
## Requirements
### Requirement: Files.GetDirectory
The proxy SHALL expose a `POST /api/files/directory` endpoint that lists the contents of a given directory on the Kodi host.

#### Scenario: List directory contents
- **WHEN** a `POST /api/files/directory` request is sent with `{"directory":"/path/to/share","media":"files"}`
- **THEN** the proxy validates that `directory` is a non-empty string
- **THEN** the proxy forwards `Files.GetDirectory` to Kodi with the provided parameters
- **THEN** the proxy returns the directory listing including files, directories, and limits

#### Scenario: Missing directory parameter
- **WHEN** a `POST /api/files/directory` request is sent without a `directory` field
- **THEN** the proxy returns a 400 Bad Request with validation errors

### Requirement: Files Directory Request Validation
The `directory` parameter SHALL be a non-empty string. The `media` parameter SHALL be optional and default to `"files"` if not provided.

#### Scenario: Default media type
- **WHEN** a `POST /api/files/directory` request is sent with only `directory`
- **THEN** the proxy SHALL use `"media": "files"` as the default

