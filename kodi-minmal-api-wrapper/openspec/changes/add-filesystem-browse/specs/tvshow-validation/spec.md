## ADDED Requirements
### Requirement: TVShowSearchDirValidator
The system SHALL provide a `TVShowSearchDirValidator` class implementing `AbstractValidator<TVShowSearchDir>`. When `Query` is not null, it SHALL validate that `Query` is not empty. When `Directory` is not null, it SHALL validate that `Directory` is not empty.

#### Scenario: Query and Directory are null — passes
- **WHEN** a `TV_SEARCH_DIR` command has `Query` = `null` and `Directory` = `null`
- **THEN** the validator returns valid

#### Scenario: Query is non-empty, Directory is null — passes
- **WHEN** a `TV_SEARCH_DIR` command has `Query` = `"Penny"` and `Directory` = `null`
- **THEN** the validator returns valid

#### Scenario: Query is empty string — fails
- **WHEN** a `TV_SEARCH_DIR` command has `Query` = `""`
- **THEN** the validator returns invalid
- **THEN** the error message indicates `Query` must not be empty

#### Scenario: Directory is empty string — fails
- **WHEN** a `TV_SEARCH_DIR` command has `Directory` = `""`
- **THEN** the validator returns invalid
- **THEN** the error message indicates `Directory` must not be empty
