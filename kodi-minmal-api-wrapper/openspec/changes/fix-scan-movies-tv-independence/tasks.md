## Implementation Checklist

### 1. Add Config Properties
- [ ] 1.1 Add `MovieScanDirectory` and `TVScanDirectory` (nullable string) to `KodiOptions.cs`
- [ ] 1.2 Add both to `appsettings.json` under `"Kodi"` section with sensible defaults

### 2. Fix Movie Endpoint Handlers
- [ ] 2.1 Inject `IOptions<KodiOptions>` into the `/api/movies` handler delegate
- [ ] 2.2 Update `MovieScanMovies` arm — call `VideoLibrary.Scan` with `MovieScanDirectory`; return 502 if not configured
- [ ] 2.3 **REMOVE** `MovieScanTV` arm from `MovieEndpoints.cs` (moved to TV endpoint)

### 3. Add TV Show Scan Command
- [ ] 3.1 Add `TVShowScan` record with discriminator `"TV_SCAN"` in `KodiCommands.cs`
- [ ] 3.2 Add `[JsonDerivedType]` attribute on `CommandValue` for `TVShowScan`

### 4. Add TV Show Scan Endpoint
- [ ] 4.1 In `TVShowEndpoints.cs`, add `TVShowScan` handler → `VideoLibrary.Scan` with `TVScanDirectory`; return 502 if not configured

### 5. Add Validators
- [ ] 5.1 Add `TVShowScanValidator` (no rules needed — no params) in `TVShowValidation.cs`
- [ ] 5.2 Register it in the TV request validator

### 6. Registration
- [ ] 6.1 Add `[JsonSerializable]` for `TVShowScan` in `Program.cs`

### 7. Verification
- [ ] 7.1 Run `dotnet build` and fix any compilation errors
- [ ] 7.2 Verify `{"type":"SCAN_MOVIES"}` scans the configured movie directory
- [ ] 7.3 Verify `{"type":"TV_SCAN"}` scans the configured TV directory
- [ ] 7.4 Verify `{"type":"SCAN_TV"}` returns 400 (no longer exists on movie endpoint)
- [ ] 7.5 Verify `{"type":"SCAN"}` (no directory) still scans all sources
