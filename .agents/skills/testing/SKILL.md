# Pocket Dungeons Testing Skill

## Overview
Pocket Dungeons is a Unity 6 C# roguelike project. The test suite runs standalone via NUnit on .NET 8.0 — no Unity Editor required.

## Prerequisites
- .NET 8.0 SDK (tested with 8.0.126)
- No Unity Editor needed for running tests
- No credentials or secrets needed

## Running Tests

### Full Suite
```bash
cd Tests/PocketDungeons.Tests
dotnet test
```

### Verbose Output (shows individual test names)
```bash
dotnet test --verbosity normal
```

### Filter by Test Name
```bash
dotnet test --filter "BSP_GeneratesAtLeastTwoRooms"
```

### Generate TRX Report
```bash
dotnet test --logger "trx;LogFileName=TestResults.trx"
```

## Test Structure

| File | Phase | Systems Covered |
|------|-------|-----------------|
| CoreTests.cs | Phase 0 | Game state machine, service locator, object pooling |
| GameplayTests.cs | Phase 1 | BSP dungeon gen, combat, player, enemy scaling, loot |
| Phase2Tests.cs | Phase 2 | Hero classes, boss phases, power-ups, slow motion |
| MetaTests.cs | Phase 3 | Town upgrades, quests, battle pass, onboarding, prestige |
| MonetizationTests.cs | Phase 4 | IAP, ads, analytics events, A/B testing |
| SocialTests.cs | Phase 5 | Leaderboards, achievements, cloud save, events, daily login |
| LaunchTests.cs | Phase 6 | Balance config, difficulty, app store, beta, crash reporter, perf monitor |
| PostLaunchTests.cs | Post-Launch + Integration | Content scheduler, revenue, retention, platform abstraction, cross-system flows |

## Expected Results
- **141 total tests** (134 unit + 7 integration)
- **40 test fixture classes**
- All tests should pass with 0 failures
- Typical duration: ~150-300ms

## Known Limitations

1. **Tests are self-contained**: They re-implement game logic inline rather than importing Unity assemblies (which can't compile outside the Editor). This means tests verify algorithms match the spec but don't directly exercise the shipped code.

2. **No runtime testing**: Rendering, physics, prefab wiring, and ScriptableObject Inspector references can only be tested inside Unity Editor.

3. **No GUI to test visually**: All testing is shell-based (`dotnet test`). No recording is needed.

## Mutation Testing Approach
To verify tests are non-trivial (not tautologies), use mutation testing:
1. Temporarily change a key value in a test (e.g., `Math.Max` → `Math.Min`)
2. Run that specific test with `--filter`
3. Verify it fails with the expected error
4. Revert the mutation
5. Confirm full suite still passes

Good mutation targets:
- `BSP_LeavesDoNotOverlap`: flip `Is.False` → `Is.True`
- `MergeConflict_TakesMaxGold`: `Math.Max` → `Math.Min`
- `Multiplier_ClampedToRange`: change max clamp value
- `WeightedSelection_RespectsWeights`: skew weights heavily
- `Phase2_TriggersAt60Percent`: change threshold value

## Troubleshooting

- **CS0523 struct cycle error**: If `BSPNode` is a `struct` with nullable self-reference, change it to `class`
- **BossPhase accessibility**: `BossBase.BossPhase` enum must be `public` (not `protected`) for test access
- **[Range] attribute errors**: Remove Unity-only `[Range]` attributes from scripts that tests reference indirectly
- **Build artifacts in git**: Ensure `Tests/**/bin/`, `Tests/**/obj/`, `Tests/**/TestResults/` are in `.gitignore`

## Devin Secrets Needed
None — all tests run without credentials.
