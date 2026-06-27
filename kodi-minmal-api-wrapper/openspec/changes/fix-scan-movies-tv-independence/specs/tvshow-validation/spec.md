## ADDED Requirements
### Requirement: TVShowScanValidator
The system SHALL provide a `TVShowScanValidator` class implementing `AbstractValidator<TVShowScan>` with no additional validation rules.

#### Scenario: Validation passes
- **WHEN** a `TV_SCAN` command is received
- **THEN** the validator returns valid (no rules to fail)
