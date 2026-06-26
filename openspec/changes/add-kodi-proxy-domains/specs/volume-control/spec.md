## ADDED Requirements

### Requirement: Volume Get
The proxy SHALL expose a `VolumeGet` command that retrieves the current volume from Kodi and returns it.

#### Scenario: Get current volume
- **WHEN** a `POST /api/volume` request is sent with `{"type":"GET"}`
- **THEN** the proxy forwards `Application.GetProperties` to Kodi with `properties: ["volume"]`
- **THEN** the proxy returns the current volume as an integer between 0 and 100

### Requirement: Volume Set
The proxy SHALL expose a `VolumeSet` command that sets the volume to an absolute value.

#### Scenario: Set volume to valid value
- **WHEN** a `POST /api/volume` request is sent with `{"type":"SET","level":50}`
- **THEN** the proxy validates level is between 0 and 100
- **THEN** the proxy forwards `Application.SetVolume` to Kodi with `volume: 50`
- **THEN** the proxy returns the new volume level

#### Scenario: Set volume out of range
- **WHEN** a `POST /api/volume` request is sent with level less than 0 or greater than 100
- **THEN** the proxy returns a 400 Bad Request with validation errors

### Requirement: Volume Up
The proxy SHALL expose a `VolumeUp` command that increments the volume by a given delta.

#### Scenario: Increase volume
- **WHEN** a `POST /api/volume` request is sent with `{"type":"UP","level":10}`
- **THEN** the proxy validates level is between 1 and 100
- **THEN** the proxy forwards `Application.SetVolume` to Kodi with the delta value
- **THEN** the proxy returns the new volume level

### Requirement: Volume Down
The proxy SHALL expose a `VolumeDown` command that decrements the volume by a given delta.

#### Scenario: Decrease volume
- **WHEN** a `POST /api/volume` request is sent with `{"type":"DOWN","level":10}`
- **THEN** the proxy validates level is between 1 and 100
- **THEN** the proxy forwards `Application.SetVolume` to Kodi with the decremented value
- **THEN** the proxy returns the new volume level

### Requirement: Polymorphic Volume Command
The `POST /api/volume` endpoint SHALL use JSON polymorphic deserialization with a `type` discriminator to route between VolumeGet, VolumeSet, VolumeUp, and VolumeDown.

#### Scenario: Unknown command type
- **WHEN** a `POST /api/volume` request is sent with an unrecognized `type` value
- **THEN** the proxy returns a 400 Bad Request
