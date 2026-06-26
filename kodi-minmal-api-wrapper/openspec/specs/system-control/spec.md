# system-control Specification

## Purpose
TBD - created by archiving change add-kodi-proxy-domains. Update Purpose after archive.
## Requirements
### Requirement: System Get Properties
The proxy SHALL expose a `POST /api/system/properties` endpoint that retrieves Kodi system properties.

#### Scenario: Get system properties
- **WHEN** a `POST /api/system/properties` request is sent with `{"properties":["version","system.friendlyname"]}`
- **THEN** the proxy validates that `properties` is a non-empty array of strings
- **THEN** the proxy forwards `System.GetProperties` to Kodi
- **THEN** the proxy returns the requested property values

#### Scenario: Empty properties array
- **WHEN** a `POST /api/system/properties` request is sent with an empty `properties` array
- **THEN** the proxy returns a 400 Bad Request with validation errors

### Requirement: System Shutdown
The proxy SHALL expose a `POST /api/system/shutdown` endpoint that shuts down the Kodi host system.

#### Scenario: Shutdown Kodi host
- **WHEN** a `POST /api/system/shutdown` request is sent
- **THEN** the proxy forwards `System.Shutdown` to Kodi
- **THEN** the proxy returns a success confirmation

### Requirement: System Reboot
The proxy SHALL expose a `POST /api/system/reboot` endpoint that reboots the Kodi host system.

#### Scenario: Reboot Kodi host
- **WHEN** a `POST /api/system/reboot` request is sent
- **THEN** the proxy forwards `System.Reboot` to Kodi
- **THEN** the proxy returns a success confirmation

### Requirement: Action Confirmation for Destructive Operations
System shutdown and reboot endpoints SHALL require an explicit confirmation field in the request body.

#### Scenario: Shutdown with confirmation
- **WHEN** a `POST /api/system/shutdown` request is sent with `{"confirm":true}`
- **THEN** the proxy forwards to Kodi and returns a success response

#### Scenario: Shutdown without confirmation
- **WHEN** a `POST /api/system/shutdown` request is sent without `confirm: true`
- **THEN** the proxy returns a 400 Bad Request with a message requiring explicit confirmation

