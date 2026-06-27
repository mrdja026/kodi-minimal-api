## ADDED Requirements
### Requirement: MovieSearchDir Record
The system SHALL define a `MovieSearchDir` record that extends `CommandValue` with `[JsonDerivedType(typeof(MovieSearchDir), "SEARCH_DIR")]`. The record SHALL have two optional properties: `Query` of type `string?` and `Directory` of type `string?`.

#### Scenario: Deserialize SEARCH_DIR without optional params
- **WHEN** a `POST /api/movies` request is sent with `{"type":"SEARCH_DIR"}`
- **THEN** the proxy deserializes it as `MovieSearchDir`
- **THEN** `Query` is `null` and `Directory` is `null`

#### Scenario: Deserialize SEARCH_DIR with query
- **WHEN** a `POST /api/movies` request is sent with `{"type":"SEARCH_DIR","query":"Dark"}`
- **THEN** the proxy deserializes it as `MovieSearchDir`
- **THEN** `Query` is `"Dark"`

#### Scenario: Deserialize SEARCH_DIR with directory and query
- **WHEN** a `POST /api/movies` request is sent with `{"type":"SEARCH_DIR","directory":"Darkest Hour (2017)","query":"Hour"}`
- **THEN** the proxy deserializes it as `MovieSearchDir`
- **THEN** `Directory` is `"Darkest Hour (2017)"` and `Query` is `"Hour"`

### Requirement: MovieSearchDir JSON Serialization Registration
The system SHALL register `MovieSearchDir` with `[JsonSerializable(typeof(MovieSearchDir))]` in `AppJsonSerializerContext` for Native AOT compatibility.

#### Scenario: Registered in Program.cs
- **WHEN** the application starts
- **THEN** `[JsonSerializable(typeof(MovieSearchDir))]` is present on `AppJsonSerializerContext`
- **THEN** JSON serialization of `MovieSearchDir` works without reflection
