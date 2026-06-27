## Implementation Checklist

### 1. Add Command Records
- [x] 1.1 Add `MovieSearchDir(string? Query) : CommandValue` with `[JsonDerivedType]` discriminator `"SEARCH_DIR"` in `KodiCommands.cs`
- [x] 1.2 Add `TVShowSearchDir(string? Query) : CommandValue` with `[JsonDerivedType]` discriminator `"TV_SEARCH_DIR"` in `KodiCommands.cs`
- [x] 1.3 Add `Directory` property to both records

### 2. Add Validators
- [x] 2.1 Add `MovieSearchDirValidator` — if `Query` is not null, validate `NotEmpty`; if `Directory` is not null, validate `NotEmpty`
- [x] 2.2 Register it in `MovieRequestValidator`
- [x] 2.3 Add `TVShowSearchDirValidator` — if `Query` is not null, validate `NotEmpty`; if `Directory` is not null, validate `NotEmpty`
- [x] 2.4 Register it in `TVShowRequestValidator`

### 3. Add Endpoint Handlers
- [x] 3.1 Add `MovieSearchDir` arm in `MovieEndpoints.cs` — call helper with `MovieScanDirectory` + optional subdirectory, return 502 if not configured
- [x] 3.2 Add `TVShowSearchDir` arm in `TVShowEndpoints.cs` — call helper with `TVScanDirectory` + optional subdirectory, return 502 if not configured
- [x] 3.3 Add `static SearchDir(IKodiService, string, string?, string?, string)` helper to each endpoint file — combines base dir with subdirectory, calls `Files.GetDirectory`, filters by label if query provided

### 4. Registration
- [x] 4.1 Add `[JsonSerializable(typeof(MovieSearchDir))]` in `Program.cs`
- [x] 4.2 Add `[JsonSerializable(typeof(TVShowSearchDir))]` in `Program.cs`

### 5. Verification
- [x] 5.1 Run `dotnet build` and fix any compilation errors
