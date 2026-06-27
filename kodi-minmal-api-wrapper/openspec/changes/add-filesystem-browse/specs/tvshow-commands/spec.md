## ADDED Requirements
### Requirement: TVShowSearchDir Record
The system SHALL define a `TVShowSearchDir` record that extends `CommandValue` with `[JsonDerivedType(typeof(TVShowSearchDir), "TV_SEARCH_DIR")]`. The record SHALL have two optional properties: `Query` of type `string?` and `Directory` of type `string?`.

#### Scenario: Deserialize TV_SEARCH_DIR without optional params
- **WHEN** a `POST /api/tvshows` request is sent with `{"type":"TV_SEARCH_DIR"}`
- **THEN** the proxy deserializes it as `TVShowSearchDir`
- **THEN** `Query` is `null` and `Directory` is `null`

#### Scenario: Deserialize TV_SEARCH_DIR with query
- **WHEN** a `POST /api/tvshows` request is sent with `{"type":"TV_SEARCH_DIR","query":"Penny"}`
- **THEN** the proxy deserializes it as `TVShowSearchDir`
- **THEN** `Query` is `"Penny"`

#### Scenario: Deserialize TV_SEARCH_DIR with directory and query
- **WHEN** a `POST /api/tvshows` request is sent with `{"type":"TV_SEARCH_DIR","directory":"Penny Dreadful","query":"Penny"}`
- **THEN** the proxy deserializes it as `TVShowSearchDir`
- **THEN** `Directory` is `"Penny Dreadful"` and `Query` is `"Penny"`

### Requirement: TVShowSearchDir JSON Serialization Registration
The system SHALL register `TVShowSearchDir` with `[JsonSerializable(typeof(TVShowSearchDir))]` in `AppJsonSerializerContext` for Native AOT compatibility.

#### Scenario: Registered in Program.cs
- **WHEN** the application starts
- **THEN** `[JsonSerializable(typeof(TVShowSearchDir))]` is present on `AppJsonSerializerContext`
- **THEN** JSON serialization of `TVShowSearchDir` works without reflection
