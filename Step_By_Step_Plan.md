# Pocket Dungeons — Step-by-Step Plan: Develop → Deploy → Monetize

A complete actionable plan from zero code to live revenue, pulling from all 5 skill guides and existing project documentation.

---

## OVERVIEW

```
Weeks 1-2:   PHASE 0 — Foundation & Setup
Weeks 3-6:   PHASE 1 — Core Loop (Vertical Slice)
Weeks 7-10:  PHASE 2 — Depth & Feel (Juice Pass)
Weeks 11-14: PHASE 3 — Meta & Retention Systems
Weeks 15-18: PHASE 4 — Polish & Monetization Integration
Weeks 19-22: PHASE 5 — Social, Cloud & Live-Ops
Weeks 23-26: PHASE 6 — Beta, Launch & Go Live
Week 27+:    POST-LAUNCH — Content Cadence & Growth
```

**North Star Metric**: Daily Runs Completed per Active Player
*(from Business Analytics SKILL — captures engagement + depth + satisfaction)*

---

## PHASE 0: Foundation & Setup (Weeks 1-2)

### Step 1: Project & Tool Setup
| Action | Skill Reference | Time |
|--------|----------------|------|
| Create Unity 6 project (2D URP template) with Pixel Perfect Camera | Architecture SKILL §1 | 0.5 day |
| Configure Git + Git LFS (`*.png *.psd *.wav *.mp3 *.ogg *.fbx *.unitypackage`) | PM SKILL §8 | 0.5 day |
| Set up branch strategy: `main` → `develop` → `feature/*` branches | PM SKILL §8 | 0.5 day |
| Establish folder structure: `Scripts/`, `Art/`, `Audio/`, `Data/`, `Scenes/`, `Prefabs/` | Architecture SKILL §9 | 0.5 day |
| Choose project tracker: **Linear** (solo) or **GitHub Projects** (free) | PM SKILL §3 | 0.5 day |
| Set up Discord server (channels: `#dev-chat`, `#decisions`, `#beta-feedback`) | PM SKILL §7 | 0.5 day |

### Step 2: Architecture Foundation
| Action | Skill Reference | Time |
|--------|----------------|------|
| Implement Game State Machine (Boot → MainMenu → Loading → Gameplay → Death → Results) | Architecture SKILL §3 | 1 day |
| Set up Service Locator pattern (Audio, Haptics, Analytics, Save services) | Architecture SKILL §4 | 0.5 day |
| Create ScriptableObject schemas (enemies, weapons, loot tables, power-ups) | Architecture SKILL §9 | 1 day |
| Build generic Object Pool system (enemies, projectiles, VFX, damage numbers) | Architecture SKILL §8 | 0.5 day |
| Create ScriptableObject Event System for decoupled communication | Architecture SKILL §6 | 0.5 day |
| Placeholder main menu scene: "Play" button → enters gameplay scene | UI/UX SKILL §5 | 0.5 day |

### Step 3: CI/CD Pipeline
| Action | Skill Reference | Time |
|--------|----------------|------|
| Configure Unity Cloud Build → iOS builds on push | Architecture SKILL §10 | 0.5 day |
| Set up TestFlight (internal) for automated distribution | iOS SKILL §7 | 0.5 day |
| Verify: Build to iOS device, navigate menu → empty gameplay → back | — | Test |

### Phase 0 Gate ✓
> **Can build to iOS device, navigate from menu to empty gameplay scene and back.**

---

## PHASE 1: Core Loop — Vertical Slice (Weeks 3-6)

### Step 4: Player Controller
| Action | Skill Reference | Time |
|--------|----------------|------|
| Virtual joystick (bottom-left thumb zone, 8-direction movement) | UI/UX SKILL §3, §4 | 2 days |
| Auto-attack nearest enemy in range | GDD §2.2 | 0.5 day |
| Dodge mechanic (swipe, i-frames, 2s cooldown, ghost trail) | UI/UX SKILL §6 | 1 day |
| Camera follow with room boundary constraints | GDD §2.3 | 0.5 day |

### Step 5: Dungeon Generation
| Action | Skill Reference | Time |
|--------|----------------|------|
| BSP dungeon generator (40x40 grid, recursive split, min 6x6 rooms) | Architecture SKILL §7 | 3 days |
| 3 hand-crafted room templates fitting BSP partitions | Architecture SKILL §7 | included |
| A* corridor generation between room centers (2-3 tile width) | Architecture SKILL §7 | included |
| Room population system (place enemies, loot, entrance, exit) | Architecture SKILL §7 | 1 day |
| Validation: Verify path start→exit, all rooms reachable | Architecture SKILL §7 | included |

### Step 6: Combat System (ECS/DOTS)
| Action | Skill Reference | Time |
|--------|----------------|------|
| Set up DOTS stack: Entities + Jobs + Burst + Collections + Mathematics | Architecture SKILL §2 | 0.5 day |
| ECS Components: Position, Health, Velocity, Damage, EnemyTag, ProjectileTag | Architecture SKILL §2 | 0.5 day |
| ECS Systems: Movement, Damage, EnemyAI, RenderSync | Architecture SKILL §2 | 1 day |
| 3 enemy types: Slime (melee), Skeleton Archer (ranged), Bat (erratic) | GDD | 3 days |
| Damage system: deal/receive damage, HP bars, damage number popups | Architecture SKILL §2 | 1.5 days |
| Death system: enemy → loot drop; player → results screen | — | 1 day |

### Step 7: Loot & Progression
| Action | Skill Reference | Time |
|--------|----------------|------|
| Loot drops: gold (auto-collect), health potions (tap to pickup) | Architecture SKILL §7 | 1 day |
| Floor progression: stairs → next floor, enemies scale per floor | — | 1 day |
| Results screen: gold earned, floors cleared, instant "Retry" button | UI/UX SKILL §5 | 1 day |

### Step 8: HUD Implementation
| Action | Skill Reference | Time |
|--------|----------------|------|
| Top bar: HP bar (color gradient), floor number, gold count | UI/UX SKILL §4 | 0.5 day |
| Bottom controls: joystick left, ability buttons right, dodge button | UI/UX SKILL §4 | 0.5 day |
| HUD < 15% screen area, semi-transparent (40-60% opacity idle) | UI/UX SKILL §4 | included |
| Safe areas: Respect Dynamic Island + home indicator | UI/UX SKILL §9 | included |
| Placeholder art (free pixel art packs from OpenGameArt/Asset Store) | — | 1 day |
| Basic SFX (hits, death, pickup) | — | 0.5 day |

### Phase 1 Gate ✓
> **Playable 2-minute run. Enter dungeon, fight 3 enemy types, collect gold, die, see results, restart. Ask: "Is this fun?"**

---

## PHASE 2: Depth & Feel (Weeks 7-10)

### Step 9: Hero Classes
| Action | Skill Reference | Time |
|--------|----------------|------|
| Warrior: sword, shield bash (melee arc + stun) | GDD §2 | 1.5 days |
| Archer: bow, rain of arrows (auto-aim + AoE) | GDD §2 | 1.5 days |
| Mage: staff, meteor strike (splash damage + massive AoE) | GDD §2 | 1.5 days |
| Hero select UI: swipeable carousel, rarity-colored borders | UI/UX SKILL §5 | 1 day |

### Step 10: Enemy Depth
| Action | Skill Reference | Time |
|--------|----------------|------|
| 5 new enemy types: Golem, Ghost, Bomber, Healer, Shield Knight | GDD | 4 days |
| Boss system + first boss (multi-phase, unique attack patterns) | GDD | 2 days |
| Power-up system: choose 1 of 3 per floor (15+ power-ups, synergies) | Architecture SKILL §7 | 2 days |
| Equipment system: weapon, armor, accessory slots, rarity tiers | — | 2 days |

### Step 11: JUICE PASS (Critical for Retention)
| Action | Skill Reference | Time |
|--------|----------------|------|
| Screen shake (proportional to damage, 0.1-0.3s, exponential decay) | UI/UX SKILL §6 | 0.5 day |
| Hit-stop (2-frame pause at 60fps = ~33ms per hit; 4 frames for crits) | UI/UX SKILL §6 | 0.5 day |
| Particle effects: hit flash, death explosion, coin scatter, loot glow | UI/UX SKILL §6 | 1 day |
| Damage number popups: float up + fade, crits = gold + larger | UI/UX SKILL §6 | 0.5 day |
| Core Haptics integration (see intensity table in UI/UX SKILL §6) | iOS SKILL §4 | 1 day |
| Slow-motion on last enemy kill (0.3s at 0.3x speed) | UI/UX SKILL §6 | 0.5 day |
| Multi-sensory feedback: every action = visual + audio + haptic | UI/UX SKILL §6 | validation |

### Step 12: Audio & Visuals
| Action | Skill Reference | Time |
|--------|----------------|------|
| Complete SFX set: attack, dodge, ability, enemy-specific | — | 1 day |
| Adaptive music system (layers add with combat intensity) | — | 1.5 days |
| 3 biome visual themes: Stone Dungeon, Cursed Catacombs, Crystal Caverns | — | 2 days |
| Adaptive difficulty tuning (enemy stats scale to player performance) | — | 1 day |

### Phase 2 Gate ✓
> **Testers say "one more run" unprompted. 3 heroes, 8 enemies, 1 boss, 15+ power-ups, juice feels great.**

---

## PHASE 3: Meta & Retention (Weeks 11-14)

### Step 13: Town & Progression
| Action | Skill Reference | Time |
|--------|----------------|------|
| Town upgrades: Blacksmith, Armory, Tavern, etc. (7 buildings, 25-50 levels) | GDD | 2 days |
| Hero unlock system (conditions: floor reached, kills, etc.) | GDD | 1 day |
| Hero leveling (XP per run → stat growth, 50 levels per hero) | GDD | 1 day |
| Gear crafting (combine loot → equipment, rarity system, set bonuses) | — | 2 days |
| Monster bestiary (kill count → lore + damage bonus) | — | 1 day |
| Achievement system (100+ achievements with gem rewards) | — | 1.5 days |

### Step 14: Daily Engagement Systems
| Action | Skill Reference | Time |
|--------|----------------|------|
| Daily dungeon (fixed seed, same for all players, daily leaderboard) | Architecture SKILL §7 | 1 day |
| Daily login rewards (7-day escalating cycle) | — | 0.5 day |
| Daily quests (3/day: "Kill 50 enemies", "Reach floor 15") | — | 1 day |
| Weekly boss raid (community HP pool, exclusive rewards) | — | 1.5 days |
| Weekly challenge (modifier runs: double enemies, speed run) | — | 1 day |
| Push notifications (energy full, daily challenge, streak reminder) | — | 1 day |

### Step 15: Battle Pass Framework
| Action | Skill Reference | Time |
|--------|----------------|------|
| 30-level Battle Pass: free track + premium track ($4.99/season) | Monetization Doc §2.1 | 2 days |
| BP XP earned through daily play (1 level/day, catch-up XP) | Analytics SKILL §3 | included |
| Premium track: exclusive skin, rare cosmetics, gems, bonus gold | Monetization Doc §2.1 | included |
| Bonus levels 31-40 for dedicated players | — | included |

### Step 16: Onboarding & Tutorial
| Action | Skill Reference | Time |
|--------|----------------|------|
| Tutorial dungeon: tap to start → immediately playing (< 30 seconds) | UI/UX SKILL §7 | 1.5 days |
| Contextual prompts: "Swipe to move" → "Tap to attack" → "Swipe to dodge" | UI/UX SKILL §7 | included |
| Progressive disclosure schedule (see UI/UX SKILL §7 table) | UI/UX SKILL §7 | included |
| No text walls, no forced tutorials, dismiss on any tap | UI/UX SKILL §7 | validation |

### Phase 3 Gate ✓
> **D7 retention target hit in internal testing. Players return daily for quests and weekly for challenges.**

---

## PHASE 4: Polish & Monetization (Weeks 15-18)

### Step 17: Art & Animation Polish
| Action | Skill Reference | Time |
|--------|----------------|------|
| Final pixel art: all heroes (16x24 characters) | Art Guide | 3 days |
| Final pixel art: all enemies + 1 boss | Art Guide | 3 days |
| Final pixel art: tilesets + UI (3 biomes) | Art Guide | 3 days |
| Animation polish pass (all sprites fully animated) | — | 2 days |

### Step 18: UI/UX Polish
| Action | Skill Reference | Time |
|--------|----------------|------|
| Menu slide transitions (200ms, ease-in-out curves) | UI/UX SKILL §5, §10 | 1 day |
| Button press animations (scale 95% + darken) | UI/UX SKILL §5 | included |
| Card UI: rarity borders with glow (Epic/Legendary) | UI/UX SKILL §5 | 0.5 day |
| Loading screen optimization (no loading for menu transitions) | UI/UX SKILL §5 | 0.5 day |

### Step 19: StoreKit 2 Integration (MONETIZATION GOES LIVE)
| Action | Skill Reference | Time |
|--------|----------------|------|
| Register products in App Store Connect: Battle Pass, gem packs, starter pack, remove ads | iOS SKILL §4 | 0.5 day |
| Implement StoreKit 2 async purchase flow (see iOS SKILL §4 code) | iOS SKILL §4 | 2 days |
| Transaction listener on app launch (`Transaction.updates`) | iOS SKILL §4 | included |
| "Restore Purchases" button in Settings screen | iOS SKILL §5 | included |
| Server-side receipt validation (prevent IAP fraud) | iOS SKILL §5, Architecture SKILL §11 | 1 day |

### Step 20: Ad Integration
| Action | Skill Reference | Time |
|--------|----------------|------|
| Unity Ads / ironSource SDK: rewarded video ads | Monetization Doc §2.5 | 1 day |
| Rewarded placements: Double gold (after death), instant chest, 3x daily reward | Monetization Doc §2.5 | included |
| Frequency caps: 3 + 2 + 1 per day respectively | Monetization Doc §2.5 | included |
| Banner ad on main menu (removable via $4.99 IAP) | Monetization Doc §2.5 | included |

### Step 21: Analytics & A/B Testing Setup
| Action | Skill Reference | Time |
|--------|----------------|------|
| Firebase Analytics: implement full event taxonomy (see Analytics SKILL §5) | Analytics SKILL §5, §6 | 1 day |
| Unity Analytics: backup integration | Analytics SKILL §6 | 0.5 day |
| Firebase Crashlytics: crash reporting | Analytics SKILL §6 | included |
| Firebase Remote Config: A/B testing framework + feature flags | Analytics SKILL §7 | 1 day |
| Validate all events fire correctly in debug builds | — | 0.5 day |

### Step 22: Monetization Trigger Logic
| Action | Skill Reference | Time |
|--------|----------------|------|
| Starter Pack shown at level 5 (once only) | Monetization Doc §4.1 | 0.5 day |
| Gem pack offered after "best floor" runs | Monetization Doc §4.1 | included |
| Revive offer after boss floor death (50 gems) | Monetization Doc §4.1 | included |
| Battle Pass preview on Day 3 | Monetization Doc §4.1 | included |
| **Never interrupt gameplay, never show after frustration** | Monetization Doc §4.2 | validation |

### Step 23: Accessibility & Localization
| Action | Skill Reference | Time |
|--------|----------------|------|
| Colorblind modes (shader-based filters) | UI/UX SKILL §8 | 0.5 day |
| Scalable UI (75%-150%) | UI/UX SKILL §8 | 0.5 day |
| VoiceOver support for all menus | UI/UX SKILL §8 | 0.5 day |
| Reduced motion option (disable shake, reduce particles) | UI/UX SKILL §8 | included |
| Localization: English, Arabic, Japanese, Spanish, Portuguese | — | 2 days |

### Phase 4 Gate ✓
> **Game looks/sounds professional. IAP works end-to-end. Analytics data flowing. A/B test framework ready.**

---

## PHASE 5: Social, Cloud & Live-Ops (Weeks 19-22)

### Step 24: Game Center Integration
| Action | Skill Reference | Time |
|--------|----------------|------|
| Player authentication (see iOS SKILL §4 GameKit code) | iOS SKILL §4 | 0.5 day |
| Leaderboards: deepest floor, daily dungeon, weekly score | iOS SKILL §4 | 1 day |
| 50+ Game Center achievements synced with in-game achievements | iOS SKILL §4 | 1 day |

### Step 25: CloudKit Save Sync
| Action | Skill Reference | Time |
|--------|----------------|------|
| Write-local-first save system (AES-256 encrypted JSON) | Architecture SKILL §9 | 1 day |
| CloudKit sync (debounced, max every 30 seconds) | iOS SKILL §4 | 1 day |
| Conflict resolution: take higher value per field | iOS SKILL §4 | included |
| Test: iPhone ↔ iPad sync | — | 0.5 day |

### Step 26: Social & Live-Ops
| Action | Skill Reference | Time |
|--------|----------------|------|
| Friend challenge system ("beat my score" via Messages) | — | 2 days |
| Seasonal event framework (template for themed events) | — | 2 days |
| Content creation tools (fast pipeline for enemies, rooms, biomes) | — | 2 days |
| Optional: Home screen widget ("Energy full!", "Daily challenge") | iOS SKILL §4 | 1 day |
| Optional: App Clips (instant-play demo) | iOS SKILL §4 | 1 day |

### Step 27: Performance Optimization
| Action | Skill Reference | Time |
|--------|----------------|------|
| Profile with Xcode Instruments (not just Unity Profiler) | iOS SKILL §6 | 0.5 day |
| Verify 60 FPS on iPhone 12 (oldest supported device) | Architecture SKILL §8 | 0.5 day |
| ASTC texture compression (75% memory savings) | iOS SKILL §6 | 0.5 day |
| Build size check: < 150 MB initial download | iOS SKILL §6 | included |
| Battery test: < 5% drain per 30 min session | Architecture SKILL §8 | included |
| Thermal soak test: 10+ min continuous play on iPhone 12 | iOS SKILL §6 | 0.5 day |

### Phase 5 Gate ✓
> **Social features live. Cloud save cross-device. Live-ops tools ready. Performance within targets.**

---

## PHASE 6: Beta → Launch (Weeks 23-26)

### Step 28: Closed Beta (TestFlight)
| Action | Skill Reference | Time |
|--------|----------------|------|
| Invite 100-500 TestFlight users | iOS SKILL §7 | 0.5 day |
| Run for 1 week, collect data | — | 5 days |
| Monitor: D1 > 35%, D7 > 15%, sessions > 5 min, crash-free > 99% | Analytics SKILL §2, iOS SKILL §7 | daily |
| If D1 < 25%: rework onboarding before proceeding | Analytics SKILL §2 | decision |

### Step 29: Analyze & Balance
| Action | Skill Reference | Time |
|--------|----------------|------|
| Analyze beta analytics: retention curves, progression, difficulty | Analytics SKILL §9 | 2 days |
| Balance tuning: enemy stats, gold economy, drop rates (data-driven) | Analytics SKILL §7 | 3 days |
| A/B test: starting gold, power-up choices, tutorial length | Analytics SKILL §7 | concurrent |

### Step 30: Open Beta
| Action | Skill Reference | Time |
|--------|----------------|------|
| Expand to 1,000-5,000 TestFlight users | iOS SKILL §7 | 0.5 day |
| Scale testing + ASO keyword validation | iOS SKILL §8 | 5 days |
| Bug fixing marathon: crashes > gameplay bugs > visual bugs | — | 3 days |
| Final performance pass on target devices | Architecture SKILL §8 | 1 day |

### Step 31: App Store Submission
| Action | Skill Reference | Time |
|--------|----------------|------|
| App Store assets: 10 screenshots (gameplay, not menus), 30s preview video | iOS SKILL §8 | 2 days |
| Title: `Pocket Dungeons: Roguelike RPG` / Subtitle: `One More Run.` | iOS SKILL §8 | included |
| Keywords: dungeon crawler, roguelike, pixel art, RPG | iOS SKILL §8 | included |
| Privacy policy URL (valid HTML, in-app + metadata) | iOS SKILL §5 | 0.5 day |
| Privacy nutrition labels (accurate analytics disclosure) | iOS SKILL §5 | included |
| AI data sharing disclosure (2026 requirement) | iOS SKILL §5 | included |
| Submit for App Store Review | — | 0.5 day |
| Address reviewer feedback (1-5 days) | iOS SKILL §5 | as needed |

### Step 32: Launch Day
| Action | Skill Reference | Time |
|--------|----------------|------|
| Phased rollout: 1% → 10% → 50% → 100% | Architecture SKILL §10 | 3-5 days |
| Real-time monitoring: crashes (Crashlytics), reviews, analytics | Analytics SKILL §12 | continuous |
| Launch marketing: social posts, press outreach, influencer coordination | Marketing Doc | 2 days |
| Monitor daily dashboard: DAU, revenue, D1 retention, crash-free, top death floor | Analytics SKILL §12 | daily |

### Phase 6 Gate ✓
> **LAUNCHED on the Apple App Store.**

---

## POST-LAUNCH: Content Cadence & Revenue Growth (Week 27+)

### Step 33: First Week
| Action | Skill Reference | Time |
|--------|----------------|------|
| Hotfix any critical bugs discovered at scale | PM SKILL §9 | Week +1 |
| Respond to App Store reviews | — | daily |
| Daily dashboard monitoring (see Analytics SKILL §12) | Analytics SKILL §12 | daily |

### Step 34: First Month
| Action | Skill Reference | Time |
|--------|----------------|------|
| Balance patch based on population-level data | Analytics SKILL §7 | Week +2 |
| Season 1 content drop: new biome + Battle Pass season 1 | — | Week +4 |
| First revenue review: check actuals vs projections | Analytics SKILL §8 | Week +4 |
| New hero release | — | Week +6 |

### Step 35: Months 2-3
| Action | Skill Reference | Time |
|--------|----------------|------|
| First seasonal event | — | Week +8 |
| Season 2 content drop | — | Week +10 |
| Major feature update (Clans v2, new game mode) | — | Week +12 |
| Weekly report cadence (see Analytics SKILL §12 template) | Analytics SKILL §12 | weekly |

### Step 36: Months 4-5 — Android Expansion
| Action | Skill Reference | Time |
|--------|----------------|------|
| Android port (Unity build target switch) | — | Week +16 |
| Android launch | — | Week +20 |
| Double UA surface area, new revenue channel | Analytics SKILL §4 | — |

### Step 37: Ongoing Revenue Optimization
| Action | Skill Reference | Cadence |
|--------|----------------|---------|
| A/B test monetization variables (prices, offers, timing) | Analytics SKILL §7 | Every 2 weeks |
| Track LTV:CPI ratio — must stay > 1.5 for sustainable growth | Analytics SKILL §4 | Weekly |
| Content cadence: new biome/hero/event every 4-6 weeks | — | 4-6 weeks |
| OKR reviews (see Analytics SKILL §10 for templates) | Analytics SKILL §10 | Quarterly |
| Competitive intelligence updates | Analytics SKILL §11 | Monthly |

---

## KEY REVENUE MILESTONES

| Milestone | Target | When | How You'll Know |
|-----------|--------|------|----------------|
| First IAP purchase | 1 paying user | Week 15-18 (internal testing) | StoreKit 2 transaction completes |
| Battle Pass launch | 8% attach rate | Launch day | BP purchases ÷ MAU |
| Break-even | LTV > CPI | Month 2-3 | Analytics dashboard |
| $17,450/month net revenue | Projected steady state | Month 3-4 | App Store Connect + analytics |
| $209,400 net annual revenue | Year 1 projection | Month 12 | Cumulative App Store Connect |
| LTV:CPI > 2.0 | Scale UA aggressively | When achieved | Analytics |

---

## BUDGET SUMMARY

| Item | Cost |
|------|------|
| Apple Developer Program | $99/year |
| Unity Personal | $0 (free under $200K revenue) |
| Art assets (commission/Asset Store) | $2,000-$5,000 |
| Sound/Music | $500-$1,500 |
| Marketing (soft launch UA) | $1,000-$3,000 |
| TestFlight | $0 |
| Firebase | $0 (free tier) |
| **Total Investment** | **$3,600-$9,600** |
| **Projected Year 1 Net Revenue** | **$209,400** |
| **Projected ROI** | **~22-58x** |

---

## DAILY WORKFLOW

```
Morning:
  1. Check daily dashboard (DAU, revenue, D1 retention, crash-free)
  2. Review overnight crash reports
  3. Post async standup: 🔵 Yesterday / 🟢 Today / 🔴 Blockers

Work Session:
  4. Work on current sprint/cycle tasks
  5. Commit frequently (conventional commits: feat/fix/refactor/chore)
  6. Test on real iOS device at least once per day

Evening:
  7. Push changes, trigger CI build
  8. Review TestFlight build (if available)
  9. Update task tracker (Linear/GitHub Projects)
```

---

## DECISION CHECKPOINTS

These are moments where you STOP and decide whether to continue, pivot, or cut:

| When | Question | If YES | If NO |
|------|----------|--------|-------|
| Week 6 | "Is the core loop fun?" | Continue to Phase 2 | Rework core mechanics |
| Week 10 | "Do testers say 'one more run'?" | Continue to Phase 3 | More juice/depth |
| Week 14 | "Is D7 retention > 15% internally?" | Continue to Phase 4 | Add more daily hooks |
| Week 18 | "Does IAP work end-to-end?" | Continue to Phase 5 | Fix monetization |
| Week 22 | "Performance within targets?" | Continue to beta | Optimize first |
| Week 26 | "Beta D1 > 35%, crash-free > 99%?" | LAUNCH | Extend beta, fix issues |

---

*Ship fast. Measure everything. Iterate based on data, not assumptions. The game you launch is the starting point, not the finished product.*
