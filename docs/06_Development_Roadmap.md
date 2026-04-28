# Pocket Dungeons — Development Roadmap

## Timeline Overview

```
Month 1          Month 2          Month 3          Month 4          Month 5          Month 6
|── Phase 0 ──|── Phase 1 ─────────────|── Phase 2 ─────────────|── Phase 3 ─────────────|
  Foundation     Vertical Slice          Depth & Feel              Meta & Retention
                                                                                          
Month 5          Month 6          Month 7
|── Phase 4 ─────────────|── Phase 5 ─────────────|── Phase 6 ─────|
  Polish & Monetization    Social & Live-Ops         LAUNCH
```

---

## Phase 0: Foundation (Weeks 1-2)

**Goal**: Project setup, tools, architecture — zero gameplay yet.

| Task | Priority | Days | Notes |
|------|----------|------|-------|
| Create Unity 6 project (2D URP template) | P0 | 0.5 | Set up Pixel Perfect Camera |
| Configure Git + Git LFS | P0 | 0.5 | LFS for sprites, audio, binary assets |
| Establish folder structure (per architecture doc) | P0 | 0.5 | Scripts, Art, Audio, Data, Scenes, Prefabs |
| Coding conventions document | P1 | 0.5 | Naming, architecture patterns, code review rules |
| Set up CI/CD (Unity Cloud Build → TestFlight) | P1 | 1 | Automated iOS builds on push |
| Game State Machine (Boot → Menu → Gameplay → Results) | P0 | 1 | Skeleton with empty scenes |
| ScriptableObject schemas (enemies, weapons, loot) | P0 | 1 | Data-driven from day 1 |
| Service locator / dependency injection setup | P1 | 0.5 | Audio, Haptics, Analytics, Save |
| Placeholder main menu scene | P1 | 0.5 | Basic UI: "Play" button → enters gameplay |
| Object pool system | P1 | 0.5 | Generic pool for enemies, projectiles, VFX |

**Milestone**: Can build to iOS device, navigate from menu to empty gameplay scene and back.

---

## Phase 1: Core Loop — Vertical Slice (Weeks 3-6)

**Goal**: A playable dungeon run: enter, fight, die, restart. The "fun test."

| Task | Priority | Days | Notes |
|------|----------|------|-------|
| Player controller (joystick move + auto-attack) | P0 | 2 | Virtual joystick, 8-direction |
| Player dodge mechanic (swipe + i-frames) | P0 | 1 | 2-second cooldown, visual trail |
| BSP dungeon generator (basic) | P0 | 3 | 4x3 grid, 3 room templates |
| Room population system | P0 | 1 | Place enemies, loot, entrance, exit |
| Camera follow + constraints | P0 | 0.5 | Smooth follow, room boundaries |
| Basic enemy: Slime (walk toward, melee attack) | P0 | 1 | Simple chase AI |
| Basic enemy: Skeleton Archer (ranged) | P0 | 1 | Stand still, shoot projectiles |
| Basic enemy: Bat (fast, erratic) | P0 | 1 | Random movement with bursts toward player |
| Damage system (deal/receive damage, health bars) | P0 | 1.5 | Damage numbers, HP bars on enemies |
| Death system (enemy death, player death) | P0 | 1 | Enemy → loot drop; Player → results screen |
| Loot drops (gold, health potions) | P0 | 1 | Gold auto-collects; potions require tap |
| Floor progression (stairs → next floor, harder enemies) | P0 | 1 | Increase density + stats per floor |
| Results screen (gold earned, floors cleared, "retry") | P0 | 1 | Instant restart button |
| Placeholder art (free pixel art packs) | P1 | 1 | OpenGameArt or Unity Asset Store freebies |
| Basic sound effects (hits, death, pickup) | P1 | 0.5 | Placeholder SFX |

**Milestone**: Playable 2-minute run. Enter dungeon, fight 3 enemy types, collect gold, die, see results, restart instantly. The question: "Is this fun?"

---

## Phase 2: Depth & Feel (Weeks 7-10)

**Goal**: Make it feel great. Add depth. The "one more run" test.

| Task | Priority | Days | Notes |
|------|----------|------|-------|
| Hero class: Warrior (sword, shield bash) | P0 | 1.5 | Melee arc attack, stun ability |
| Hero class: Archer (bow, rain of arrows) | P0 | 1.5 | Auto-aim ranged, AoE ability |
| Hero class: Mage (staff, meteor strike) | P0 | 1.5 | Splash damage, massive AoE ability |
| 5 more enemy types (Golem, Ghost, Bomber, Healer, Shield Knight) | P0 | 4 | Each with unique AI behavior |
| Boss system + first boss | P0 | 2 | Multi-phase, unique attack patterns |
| Power-up system (choose 1 of 3 per floor) | P0 | 2 | 15+ power-ups, synergy framework |
| Equipment system (weapon, armor, accessory slots) | P1 | 2 | Rarity tiers, stat bonuses |
| **JUICE PASS** — Screen shake | P0 | 0.5 | Proportional to damage |
| **JUICE PASS** — Hit-stop (2-frame pause) | P0 | 0.5 | On every hit |
| **JUICE PASS** — Particle effects (hit, death, loot) | P0 | 1 | Per enemy type particles |
| **JUICE PASS** — Damage number popups | P0 | 0.5 | Float up, crits are gold + larger |
| **JUICE PASS** — Core Haptics integration | P1 | 1 | Hit, crit, death, level-up, loot |
| **JUICE PASS** — Slow-motion (last enemy kill) | P1 | 0.5 | 0.3s slowdown on last enemy |
| Adaptive difficulty tuning | P1 | 1 | Scale enemy stats to player performance |
| Sound effects (complete set) | P1 | 1 | Attack, dodge, ability, enemy-specific |
| Adaptive music system | P1 | 1.5 | Layers add with combat intensity |
| 3 biome visual themes | P1 | 2 | Stone Dungeon, Cursed Catacombs, Crystal Caverns |

**Milestone**: The game feels satisfying to play. Testers say "one more run" unprompted. 3 playable heroes, 8 enemy types, 1 boss, 15+ power-ups, screen shake + particles + haptics.

---

## Phase 3: Meta & Retention (Weeks 11-14)

**Goal**: Reasons to come back tomorrow, next week, next month.

| Task | Priority | Days | Notes |
|------|----------|------|-------|
| Gold → Town upgrades (Blacksmith, Armory, Tavern, etc.) | P0 | 2 | 7 buildings, 25-50 levels each |
| Hero unlock system | P0 | 1 | Unlock conditions (floor reached, kills, etc.) |
| Hero leveling (XP per run → stat growth) | P0 | 1 | 50 levels per hero |
| Gear crafting (combine loot → equipment) | P1 | 2 | Rarity system, set bonuses |
| Monster bestiary (kill count → lore + damage bonus) | P1 | 1 | Collection progress tracking |
| Achievement system (100+ achievements) | P1 | 1.5 | With gem rewards |
| Daily dungeon (fixed seed, same for all players) | P0 | 1 | Daily leaderboard |
| Daily login rewards (7-day cycle) | P0 | 0.5 | Escalating rewards |
| Daily quests (3 per day) | P0 | 1 | "Kill 50 enemies", "Reach floor 15", etc. |
| Weekly boss raid | P1 | 1.5 | Community HP pool, exclusive rewards |
| Weekly challenge (modifier runs) | P1 | 1 | Double enemies, speed run, etc. |
| Battle Pass framework | P0 | 2 | 30 levels, free + premium tracks |
| Push notifications | P1 | 1 | Energy full, daily challenge, streak reminder |
| Tutorial / onboarding flow | P0 | 1.5 | < 30 seconds to first combat |

**Milestone**: D7 retention target in internal testing. Players return daily for quests, weekly for challenges. Battle Pass drives daily engagement.

---

## Phase 4: Polish & Monetization (Weeks 15-18)

**Goal**: Production-quality art, monetization live, analytics flowing.

| Task | Priority | Days | Notes |
|------|----------|------|-------|
| Commission/create final pixel art (all heroes) | P0 | 3 | Professional quality sprites |
| Commission/create final pixel art (all enemies) | P0 | 3 | 10+ enemy types + 1 boss |
| Commission/create final pixel art (tilesets, UI) | P0 | 3 | 3 biomes complete |
| Animation polish pass | P0 | 2 | All sprites fully animated |
| UI/UX polish pass | P0 | 2 | Menus, transitions, loading screens |
| Onboarding UX testing | P0 | 1 | First-time user experience optimization |
| StoreKit 2 integration (IAP) | P0 | 2 | Battle Pass, gems, starter pack, remove ads |
| Server-side receipt validation | P0 | 1 | Prevent IAP fraud |
| Unity Ads / ironSource integration | P1 | 1 | Rewarded video ads |
| Firebase Analytics integration | P0 | 1 | Full event taxonomy |
| Unity Analytics integration | P1 | 0.5 | Backup analytics |
| Remote Config (A/B testing) | P0 | 1 | Feature flags, value tuning |
| Localization (5 languages) | P1 | 2 | English, Arabic, Japanese, Spanish, Portuguese |
| Accessibility pass | P1 | 1.5 | VoiceOver, colorblind, scalable UI |

**Milestone**: The game looks and sounds professional. IAP works. Analytics data flowing. A/B test framework ready.

---

## Phase 5: Social & Live-Ops (Weeks 19-22)

**Goal**: Social hooks, cloud save, live-ops infrastructure.

| Task | Priority | Days | Notes |
|------|----------|------|-------|
| Game Center integration (leaderboards) | P0 | 1.5 | Deepest floor, daily dungeon, weekly score |
| Game Center achievements (50+) | P0 | 1 | Synced with in-game achievements |
| CloudKit save sync | P0 | 2 | iPhone ↔ iPad ↔ Mac |
| Friend challenge system | P1 | 2 | Send "beat my score" via Messages |
| Clan/guild system (basic) | P2 | 2 | Create/join, chat, clan perks |
| Seasonal event framework | P0 | 2 | Template for themed events |
| Content creation tools | P1 | 2 | Fast pipeline to add enemies, rooms, biomes |
| Home screen widget | P2 | 1 | "Energy full!" / "Daily challenge available" |
| App Clips integration | P2 | 1 | Instant-play demo from App Store |
| Performance optimization pass | P0 | 2 | Profiling, memory, battery, load times |

**Milestone**: Social features live. Cloud save working cross-device. Live-ops tools ready for post-launch content.

---

## Phase 6: Launch Prep (Weeks 23-26)

**Goal**: Beta test, balance, submit, launch.

| Task | Priority | Days | Notes |
|------|----------|------|-------|
| Closed beta (100 TestFlight users) | P0 | 5 | 1 week of testing |
| Analyze beta analytics | P0 | 2 | Retention, progression, difficulty curves |
| Balance tuning (based on data) | P0 | 3 | Enemy stats, gold economy, drop rates |
| Open beta (1,000+ TestFlight users) | P1 | 5 | 1 week, scale testing |
| Bug fixing marathon | P0 | 3 | Priority: crashes > gameplay bugs > visual bugs |
| Performance final pass | P0 | 1 | Target devices: iPhone 12+ |
| App Store assets | P0 | 2 | Screenshots, preview video, description, icon |
| App Store submission | P0 | 0.5 | Submit for review |
| App Store review response | P0 | 1-5 | Address any reviewer feedback |
| Launch marketing execution | P0 | 2 | Social posts, press outreach, influencer coordination |
| Launch day monitoring | P0 | 1 | Real-time crash, review, analytics monitoring |

**Milestone**: LAUNCHED on the Apple App Store.

---

## Post-Launch Cadence

| Week | Activity |
|------|----------|
| +1 | Hotfix for any critical bugs discovered at scale |
| +2 | Balance patch based on population-level data |
| +4 | Season 1 content drop (new biome + Battle Pass) |
| +6 | New hero release |
| +8 | First seasonal event |
| +10 | Season 2 content drop |
| +12 | Major feature update (Clans v2, new game mode) |
| +16 | Android port begins (Unity build target switch) |
| +20 | Android launch |
| Ongoing | 4-6 week content cadence, community engagement, A/B testing |

---

## Risk Register

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| Core loop isn't fun | Medium | Critical | Prototype test at Week 6; pivot early if needed |
| Art quality insufficient | Medium | High | Budget for commissioned art; use Asset Store as fallback |
| Apple rejects app | Low | High | Study review guidelines; pre-review consultation available |
| Low retention (< 20% D1) | Medium | High | A/B test onboarding, daily hooks; follow beta data |
| Monetization too aggressive | Low | Medium | Player surveys; compare to ethical F2P benchmarks |
| Scope creep | High | Medium | Strict phase gates; cut features before delaying launch |
| Technical debt | Medium | Medium | Refactoring sprints every 4 weeks; code review discipline |

---

*Ship fast. Measure everything. Iterate based on data, not assumptions. The game you launch is the starting point, not the finished product.*
