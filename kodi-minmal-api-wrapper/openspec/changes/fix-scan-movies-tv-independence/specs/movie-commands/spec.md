## REMOVED Requirements
### Requirement: MovieScanTV Record
**Reason**: TV scan functionality moved to `/api/tvshows` as `TVShowScan`. The `SCAN_TV` discriminator no longer belongs under movie commands.
**Migration**: Use `{"type":"TV_SCAN"}` on `POST /api/tvshows` instead of `{"type":"SCAN_TV"}` on `POST /api/movies`.
