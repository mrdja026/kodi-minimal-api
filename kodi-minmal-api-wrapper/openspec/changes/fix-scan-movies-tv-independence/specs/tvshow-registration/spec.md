## ADDED Requirements
### Requirement: TVShowScan JSON Serialization Registration
The system SHALL register `TVShowScan` with `[JsonSerializable]` in `AppJsonSerializerContext` for Native AOT compatibility.

#### Scenario: Registered in Program.cs
- **WHEN** the application starts
- **THEN** `[JsonSerializable(typeof(TVShowScan))]` is present on `AppJsonSerializerContext`
- **THEN** JSON serialization of `TVShowScan` works without reflection
