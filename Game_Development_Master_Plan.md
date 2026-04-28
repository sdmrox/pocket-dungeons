# Pocket Dungeons: Complete Build-From-Scratch Master Plan
## Addictive Roguelike Dungeon Crawler for Apple App Store

---

# PART 1: LEADING-EDGE GAME DESIGN KNOWLEDGE

## 1.1 The Psychology of Addiction in Games

### The Dopamine System
The brain's reward circuit runs on dopamine — a neurotransmitter that doesn't just signal pleasure, but **anticipation of pleasure**. The most addictive games don't reward players constantly; they create cycles of anticipation → action → variable reward → anticipation. This is called the **compulsion loop**.

### The 7 Addiction Pillars (Applied to Game Design)

| # | Psychological Principle | How It Works | Game Mechanic |
|---|------------------------|--------------|---------------|
| 1 | **Variable Ratio Reinforcement** | Unpredictable rewards release more dopamine than guaranteed ones. The brain can't predict the pattern, so it keeps trying. | Random loot drops, procedural dungeons, mystery chests |
| 2 | **Loss Aversion** (Kahneman & Tversky) | Losing feels 2x more painful than winning feels good. Players will act to *avoid loss* more than to *seek gain*. | Daily streaks, decaying bonuses, limited-time events, energy systems |
| 3 | **Zeigarnik Effect** | Unfinished tasks create cognitive tension. The brain obsesses over incomplete patterns. | Collection books (127/130), progress bars at 87%, "almost" moments |
| 4 | **Social Comparison Theory** (Festinger) | Humans evaluate themselves by comparing to others. Status within a group is a primal motivator. | Leaderboards, rare cosmetics, clan rankings, "top 5%" badges |
| 5 | **Flow State** (Csikszentmihalyi) | When challenge = skill, time disappears. Too easy = boredom, too hard = frustration. The sweet spot is addictive. | Dynamic difficulty adjustment (DDA), skill-based combat, 2-3 min runs |
| 6 | **Sunk Cost Fallacy** | The more invested, the harder to quit. "I can't stop now, I've put in 40 hours." | Deep progression trees, upgradeable gear, hero leveling, prestige systems |
| 7 | **Novelty Seeking** (Dopaminergic drive) | The brain habituates to repetition. Novel stimuli re-activate the reward circuit. | Procedural generation, seasonal content, new biomes, random events |

### The Compulsion Loop (Core of All Addictive Games)
```
    ┌─────────────┐
    │  ANTICIPATION │ ← "What will I get this run?"
    └──────┬──────┘
           ▼
    ┌─────────────┐
    │    ACTION    │ ← Swipe, tap, fight, explore
    └──────┬──────┘
           ▼
    ┌─────────────┐
    │VARIABLE REWARD│ ← Random loot, score, new floor
    └──────┬──────┘
           ▼
    ┌─────────────┐
    │  INVESTMENT  │ ← Upgrade gear, level up hero
    └──────┬──────┘
           │
           └──────→ Back to ANTICIPATION
```

### Session Design: The "Just One More" Architecture
- **Micro-sessions**: Each dungeon run = 2-3 minutes (fits in any idle moment)
- **Cliff-hanger endings**: Death screen shows "You were 2 floors from the boss!" 
- **Instant restart**: < 1 second from death to new run (zero friction to retry)
- **Progress persistence**: Even failed runs yield gold/XP (no run feels wasted)
- **End-of-session hooks**: "Your daily chest unlocks in 4 hours" (reason to return)

---

## 1.2 Modern Game Design Methodologies

### Data-Driven Design (DDD)
Leading studios (Supercell, King, miHoYo) use real-time analytics to tune every variable:
- **A/B test everything**: Difficulty curves, reward amounts, UI layouts, monetization triggers
- **Key metrics**: D1/D7/D30 retention, ARPU, ARPPU, session length, session frequency
- **Funnel analysis**: Where do players drop off? What makes them return?
- **Tools**: Unity Analytics, Firebase, GameAnalytics, Amplitude, Mixpanel

### Player-Centric UX Patterns (2024-2026 Best Practices)
- **Onboarding in < 30 seconds**: No text walls. Player is *playing* within the first tap
- **Progressive disclosure**: Reveal complexity gradually (don't show all systems on day 1)
- **Haptic feedback**: iOS Taptic Engine for hits, loot drops, level-ups (hugely underused differentiator)
- **Satisfying juice**: Screen shake, particle bursts, number popups, slow-motion kill cams
- **Accessibility**: Colorblind modes, scalable UI, one-hand play support

### Live-Ops (Games as a Service)
Modern mobile games are not "ship and forget." The top-grossing games all run:
- **Daily/weekly challenges**: Fresh content without dev work (algorithm-generated)
- **Seasonal events**: Themed content every 4-6 weeks (Halloween dungeon, winter biome)
- **Battle Pass**: Paid + free track, 30-day progression, strongest retention mechanic in mobile
- **Limited-time offers**: FOMO-driven monetization at key moments (after a near-win, after level-up)

---

# PART 2: LEADING-EDGE TECHNOLOGY & TOOLS

## 2.1 Game Engine Comparison (2026 State of the Art)

### Option A: Unity 6 (C#) — RECOMMENDED
| Aspect | Details |
|--------|---------|
| **Version** | Unity 6.4 (latest stable, April 2026) |
| **Language** | C# (mature, massive ecosystem, easy hiring) |
| **2D Support** | Industry-leading: Sprites, Tilemaps, 2D Physics, 2D Lighting (URP), Spine integration |
| **iOS Support** | First-class. Direct Xcode project export, Metal rendering, IL2CPP for native performance |
| **Performance** | DOTS/ECS for data-oriented architecture (Entity Component System) — 10-50x performance for heavy object counts |
| **Monetization** | Built-in IAP (StoreKit 2 bridge), Unity Ads, Unity Analytics, Unity Remote Config |
| **Multiplayer** | Netcode for GameObjects, Unity Relay, Unity Lobby — easy online leaderboards & async PvP |
| **Asset Store** | 100,000+ assets — sprites, shaders, tools, plugins (massive time-saver) |
| **Cost** | Free under $200K revenue; Unity Pro at $2,040/year (no runtime fee since Sept 2024 reversal) |
| **Pros** | Largest mobile game engine market share (~70% of mobile games), most tutorials, biggest community |
| **Cons** | Larger build size than native (~50MB minimum), some overhead vs raw Metal |

### Option B: Godot 4.6 (GDScript/C#)
| Aspect | Details |
|--------|---------|
| **Version** | Godot 4.6 (latest stable) |
| **Language** | GDScript (Python-like) or C# |
| **2D Support** | Excellent — Godot was originally a 2D engine. Lightweight, clean API |
| **iOS Support** | Supported but less polished than Unity. Requires manual Xcode project config |
| **Performance** | Good for 2D, lighter engine footprint (~15MB builds) |
| **Monetization** | No built-in IAP — requires third-party plugins (less mature) |
| **Cost** | 100% free, MIT license, no revenue share ever |
| **Pros** | Lightweight, fast iteration, growing community, fully open source |
| **Cons** | Smaller iOS ecosystem, fewer mobile-specific plugins, less battle-tested monetization |

### Option C: Native SpriteKit + Swift
| Aspect | Details |
|--------|---------|
| **Language** | Swift 6 |
| **2D Support** | Apple's own 2D game framework, deeply integrated with iOS |
| **iOS Support** | Perfect — it IS the native platform |
| **Performance** | Best possible on iOS (direct Metal access, zero overhead) |
| **Cost** | Free (Apple Developer Program $99/year for App Store publishing) |
| **Pros** | Smallest build size, best performance, native haptics/GameKit integration, Apple favors native apps |
| **Cons** | iOS/macOS only (no Android), smaller community than Unity, more manual work for common game systems |

### Recommendation: **Unity 6 (C#)**
**Why**: Unity dominates mobile gaming for good reason — battle-tested iOS pipeline, massive ecosystem, built-in monetization tools, and the broadest talent pool. If you later want Android, it's a checkbox. The DOTS/ECS system provides cutting-edge performance when needed. The 70% mobile market share means every problem has already been solved by someone.

---

## 2.2 Apple Platform Technologies to Leverage

| Technology | Purpose | How We Use It |
|------------|---------|---------------|
| **Metal 4** | GPU rendering | Unity renders via Metal automatically; enables MetalFX upscaling for visual quality |
| **SpriteKit** | 2D rendering (native) | Not needed if using Unity, but good for mini-games or native UI overlays |
| **GameKit / Game Center** | Social gaming | Leaderboards, achievements, friend challenges, multiplayer matchmaking |
| **StoreKit 2** | In-app purchases | Subscriptions (Battle Pass), consumables (gems), non-consumables (character unlocks) |
| **Core Haptics** | Taptic Engine feedback | Hit impacts, loot drops, level-ups — critical for "juice" and satisfaction |
| **App Intents / Widgets** | Home screen presence | "Your energy is full!" widget, daily challenge widget — drives re-engagement |
| **CloudKit** | Cloud save | Cross-device progression sync (iPhone ↔ iPad ↔ Mac) |
| **SKAdNetwork 5** | Ad attribution | Track user acquisition campaign performance (privacy-compliant) |
| **App Tracking Transparency** | Privacy | Required prompt for IDFA — must design analytics around privacy constraints |

---

## 2.3 Modern Architecture Patterns

### Entity Component System (ECS) — Data-Oriented Design
The leading-edge architecture for game performance. Instead of traditional OOP inheritance:

```
Traditional OOP:                    ECS (Data-Oriented):
┌──────────────┐                    Components (Data):
│ EnemyGoblin  │                    ├── Position { x, y }
│ ├─ position  │                    ├── Health { current, max }
│ ├─ health    │     ───────→       ├── Sprite { texture_id }
│ ├─ sprite    │                    ├── AI { behavior_type }
│ ├─ ai        │                    
│ └─ loot      │                    Systems (Logic):
└──────────────┘                    ├── MovementSystem
                                    ├── CombatSystem
                                    ├── RenderSystem
                                    └── LootSystem
```

**Why ECS matters**: Cache-friendly memory layout = 10-50x faster iteration over thousands of entities. Critical for roguelikes with hundreds of enemies, projectiles, and effects on screen.

### Procedural Content Generation (PCG)
The backbone of roguelike replayability:
- **Wave Function Collapse (WFC)**: Algorithm that generates coherent dungeon layouts from small example patterns
- **Binary Space Partitioning (BSP)**: Classic dungeon generation — recursively divide space into rooms and corridors
- **Cellular Automata**: For organic cave systems and natural-feeling environments
- **Weighted Random Tables**: For loot distribution with rarity tiers (common/rare/epic/legendary)

### State Machine Architecture
For game states, enemy AI, and UI flow:
```
GameState Machine:
  MainMenu → Loading → Gameplay → Paused → Death → Results → MainMenu
  
Enemy AI State Machine:
  Idle → Patrol → Chase → Attack → Stunned → Death
  
UI State Machine:
  HUD → Inventory → Shop → Settings
```

---

# PART 3: THE BUILD PLAN

## 3.1 Game Concept: "Pocket Dungeons"

**One-line pitch**: A bite-sized roguelike dungeon crawler where every run is different, every death teaches you something, and "just one more run" becomes your mantra.

### Core Identity
- **Genre**: Action Roguelike / Dungeon Crawler
- **Platform**: iOS (iPhone + iPad), expandable to Android later
- **Session Length**: 2-3 minutes per run
- **Target Audience**: Casual-core gamers (18-35), fans of Vampire Survivors, Hades, Archero
- **Art Style**: Stylized pixel art with modern lighting & particle effects (nostalgia + polish)
- **Monetization**: Free-to-play with Battle Pass + cosmetic IAP (ethical, no pay-to-win)

### Core Gameplay Loop (30 seconds)
1. **Enter dungeon** → procedurally generated floor
2. **Swipe to move** through rooms (intuitive touch controls)
3. **Tap to attack** → auto-aim at nearest enemy (accessible)
4. **Dodge by swiping** away from enemy attacks (skill expression)
5. **Kill enemies** → they drop random loot / gold / XP
6. **Choose 1 of 3 power-ups** at end of each floor (roguelike choice)
7. **Push deeper** → enemies get harder, loot gets better
8. **Die** → keep gold/XP, lose temporary power-ups → "One More Run"

### Meta-Game Loop (Days/Weeks)
- **Permanent upgrades**: Spend gold on hero stats, unlock new starting weapons
- **Hero collection**: 12+ heroes with unique abilities (unlock over time)
- **Monster bestiary**: Collect data on every monster type (Zeigarnik)
- **Gear crafting**: Combine loot pieces into permanent equipment sets
- **Daily dungeons**: Special themed dungeons with unique rewards (daily habit)
- **Weekly boss raids**: Extra-hard challenge for top rewards (weekly appointment)
- **Battle Pass**: 30-day progression track, free + premium tiers
- **Seasonal biomes**: New dungeon themes every 6 weeks (novelty)

---

## 3.2 Technical Architecture

### Tech Stack

| Layer | Technology | Rationale |
|-------|-----------|-----------|
| **Engine** | Unity 6 (LTS) | Industry standard for mobile, best iOS pipeline |
| **Language** | C# | Type-safe, performant with IL2CPP, massive ecosystem |
| **Architecture** | Hybrid OOP + DOTS/ECS | ECS for combat/enemies, OOP for UI/menus |
| **Rendering** | Universal Render Pipeline (URP) | Mobile-optimized, 2D lighting, post-processing |
| **Procedural Gen** | Custom BSP + WFC | Dungeon generation, loot tables |
| **State Management** | Finite State Machines | Game states, enemy AI, UI flow |
| **Data Storage** | ScriptableObjects + JSON | Game data, save files, config |
| **Cloud Save** | Unity Cloud Save + CloudKit | Cross-device sync |
| **Analytics** | Unity Analytics + Firebase | A/B testing, retention tracking, funnel analysis |
| **Monetization** | Unity IAP (StoreKit 2) | Battle Pass, cosmetics, gems |
| **Ads** | Unity Ads + ironSource | Rewarded video ads (optional, non-intrusive) |
| **Backend** | Unity Game Services (UGS) | Leaderboards, matchmaking, remote config |
| **CI/CD** | Unity Cloud Build + Xcode Cloud | Automated builds, TestFlight distribution |
| **Version Control** | Git + Git LFS | Asset versioning for large sprites/audio |

### Project Structure
```
PocketDungeons/
├── Assets/
│   ├── _Project/
│   │   ├── Scripts/
│   │   │   ├── Core/                  # Game manager, state machine, singletons
│   │   │   ├── Player/                # Player controller, input, abilities
│   │   │   ├── Enemies/               # Enemy AI, spawner, behaviors
│   │   │   ├── Dungeon/               # PCG, room templates, floor manager
│   │   │   ├── Combat/                # Damage system, projectiles, effects
│   │   │   ├── Loot/                  # Drop tables, inventory, equipment
│   │   │   ├── UI/                    # HUD, menus, popups, battle pass
│   │   │   ├── Meta/                  # Progression, upgrades, collections
│   │   │   ├── Audio/                 # Sound manager, music system
│   │   │   ├── Analytics/             # Event tracking, A/B test hooks
│   │   │   └── Utilities/             # Extensions, helpers, constants
│   │   ├── Art/
│   │   │   ├── Sprites/               # Characters, enemies, items, tiles
│   │   │   ├── Animations/            # Sprite animation controllers
│   │   │   ├── VFX/                   # Particle systems, hit effects
│   │   │   ├── UI/                    # UI sprites, icons, fonts
│   │   │   └── Shaders/               # Custom 2D shaders (glow, outline)
│   │   ├── Audio/
│   │   │   ├── Music/                 # Adaptive soundtrack
│   │   │   └── SFX/                   # Sound effects
│   │   ├── Data/
│   │   │   ├── ScriptableObjects/     # Enemy stats, weapon data, loot tables
│   │   │   ├── Localization/          # Multi-language support
│   │   │   └── Config/                # Remote config defaults
│   │   ├── Scenes/
│   │   │   ├── Boot.unity             # Initialization
│   │   │   ├── MainMenu.unity         # Title screen
│   │   │   ├── Gameplay.unity         # Dungeon gameplay
│   │   │   └── Results.unity          # Death/victory screen
│   │   └── Prefabs/
│   │       ├── Player/
│   │       ├── Enemies/
│   │       ├── Rooms/
│   │       ├── Items/
│   │       └── Effects/
│   ├── Plugins/                       # Third-party SDKs
│   └── StreamingAssets/               # Runtime-loaded data
├── Packages/                          # Unity Package Manager
├── ProjectSettings/
└── Builds/
    └── iOS/
```

### Key Systems Design

#### 1. Procedural Dungeon Generation
```
Algorithm: Binary Space Partitioning (BSP) + Room Templates

Step 1: Generate BSP tree (divide grid into rectangles)
Step 2: Place hand-crafted room templates in each partition
Step 3: Connect rooms with corridors (A* pathfinding)
Step 4: Populate rooms (enemies, loot, traps) from weighted tables
Step 5: Place entrance (player start) and exit (stairs down)
Step 6: Apply biome theme (textures, lighting, palette)

Parameters (tunable via Remote Config):
- Floor size: 5x5 to 12x12 rooms (scales with depth)
- Enemy density: 3-8 per room (scales with depth)
- Loot density: 1-3 chests per floor
- Trap frequency: 0-2 per room (introduced at floor 5+)
- Boss room: Every 10 floors
```

#### 2. Combat System
```
Design: Simple but expressive

Controls:
- Virtual joystick (left thumb) = move
- Auto-attack nearest enemy within range
- Tap ability buttons (right thumb) = special moves
- Swipe to dodge (invincibility frames)

Damage Formula:
  Damage = (BaseDamage * WeaponMultiplier * SkillBonus) - EnemyArmor
  Critical Hit = 2x damage, 10% base chance + gear bonuses
  
Combat Feel ("Juice"):
- Screen shake on hit (proportional to damage)
- Damage numbers pop up and float away
- Hit-stop (2-frame pause on impact)
- Particle burst on enemy death
- Haptic feedback on every hit (Core Haptics API)
- Slow-motion on last enemy kill in room
```

#### 3. Progression System
```
Layer 1 — Within a Run (Temporary):
  - Power-up choices every floor (pick 1 of 3)
  - Examples: "+20% attack speed", "Poison arrows", "Extra life"
  - Synergy system: certain combos create super-effects
  
Layer 2 — Between Runs (Permanent):
  - Gold → Upgrade base stats (HP, ATK, DEF, SPD)
  - XP → Level up → Unlock new heroes
  - Gear pieces → Craft equipment (set bonuses)
  - Monster data → Bestiary completion → Rewards
  
Layer 3 — Long-term (Weeks/Months):
  - Hero mastery levels (play X runs with hero Y)
  - Prestige system (reset for multiplier bonuses)
  - Seasonal rankings
  - Achievement gallery
  - Battle Pass progression
```

---

## 3.3 Monetization Strategy (Ethical F2P)

### Philosophy
- **No pay-to-win**: Paid items are cosmetic only
- **Respect player time**: No mandatory ads, no energy gates on core gameplay
- **Value exchange**: Every purchase feels worth it

### Revenue Streams

| Stream | Type | Price Range | Description |
|--------|------|-------------|-------------|
| **Battle Pass** | Subscription-like | $4.99/season | 30-day progression track, cosmetic rewards, bonus gold |
| **Cosmetic Skins** | Non-consumable IAP | $1.99-$4.99 | Hero skins, weapon skins, dungeon themes |
| **Gem Packs** | Consumable IAP | $0.99-$49.99 | Premium currency for cosmetics and convenience |
| **Starter Pack** | One-time IAP | $2.99 | Shown once at level 5 — hero + skin + gems (high conversion) |
| **Rewarded Ads** | Ad | Free to player | Watch 30s ad → double gold, extra life, bonus chest |
| **Remove Ads** | Non-consumable IAP | $4.99 | Permanently removes banner/interstitial ads |

### Apple App Store Compliance
Per Apple's [App Review Guidelines](https://developer.apple.com/app-store/review/guidelines/):
- All IAP must use StoreKit 2 (Apple takes 15-30% commission)
- No "loot boxes" with real money (use in-game currency only, disclose odds)
- Subscription auto-renewal must be clearly disclosed
- No manipulation of children (if targeting under 13, strict rules apply)
- Privacy nutrition labels must be accurate
- App Tracking Transparency (ATT) prompt required before IDFA access

---

## 3.4 Development Roadmap

### Phase 0: Foundation (Weeks 1-2)
- [ ] Set up Unity 6 project with URP 2D
- [ ] Configure Git + Git LFS
- [ ] Establish folder structure and coding conventions
- [ ] Set up CI/CD (Unity Cloud Build → TestFlight)
- [ ] Create base ScriptableObject data schemas
- [ ] Implement game state machine (Boot → Menu → Gameplay → Results)

### Phase 1: Core Loop — Vertical Slice (Weeks 3-6)
- [ ] Player controller (movement, auto-attack, dodge)
- [ ] Basic enemy AI (idle, chase, attack, death)
- [ ] BSP dungeon generator (5x5 grid, 3 room templates)
- [ ] Combat system (damage, health, death)
- [ ] Basic loot drops (gold, health potions)
- [ ] Floor progression (stairs → next floor)
- [ ] Death → Results screen → Restart
- [ ] Placeholder art (free assets or programmer art)
- **Milestone**: Playable loop — enter dungeon, fight, die, restart

### Phase 2: Depth & Feel (Weeks 7-10)
- [ ] 3 hero classes (Warrior, Archer, Mage) with unique abilities
- [ ] 8+ enemy types with distinct behaviors
- [ ] Power-up system (choose 1 of 3 per floor)
- [ ] Equipment/gear system (weapon + armor + accessory)
- [ ] "Juice" pass — screen shake, particles, haptics, hit-stop
- [ ] Adaptive difficulty tuning
- [ ] Sound effects + adaptive music system
- [ ] 3 biome themes (dungeon, cave, temple)
- **Milestone**: Fun to play — the "one more run" feeling works

### Phase 3: Meta & Retention (Weeks 11-14)
- [ ] Permanent upgrade system (gold → stat upgrades)
- [ ] Hero unlock/collection system
- [ ] Monster bestiary
- [ ] Daily dungeon challenges
- [ ] Weekly boss raids
- [ ] Achievement system
- [ ] Battle Pass framework (free + premium track)
- [ ] Push notifications (energy full, daily challenge, streak reminder)
- **Milestone**: Retention hooks — reasons to come back daily/weekly

### Phase 4: Polish & Monetization (Weeks 15-18)
- [ ] Final pixel art + animations (commission or create)
- [ ] UI/UX polish pass (menus, transitions, onboarding)
- [ ] StoreKit 2 integration (IAP, subscriptions)
- [ ] Rewarded ads integration (Unity Ads)
- [ ] Analytics integration (Firebase + Unity Analytics)
- [ ] Remote Config (A/B testing framework)
- [ ] Localization (English, Arabic, Japanese, Spanish, Portuguese)
- [ ] Accessibility pass (VoiceOver, colorblind, scalable UI)
- **Milestone**: Monetization works, data pipeline ready

### Phase 5: Social & Live-Ops (Weeks 19-22)
- [ ] Game Center integration (leaderboards, achievements)
- [ ] CloudKit save sync (iPhone ↔ iPad)
- [ ] Friend challenge system (async PvP)
- [ ] Clan/guild system (basic)
- [ ] Seasonal event framework
- [ ] Home screen widget ("Energy full!" / "Daily challenge available")
- [ ] Content pipeline (tools to add new enemies, rooms, biomes quickly)
- **Milestone**: Social hooks and live-ops ready

### Phase 6: Launch Prep (Weeks 23-26)
- [ ] Closed beta via TestFlight (100 players)
- [ ] Analyze beta data (retention, monetization, difficulty)
- [ ] Balance tuning based on data
- [ ] Performance optimization (profiling, memory, battery)
- [ ] App Store assets (screenshots, preview video, description, keywords)
- [ ] App Store submission + review
- [ ] Launch marketing (social media, press kit, influencer outreach)
- **Milestone**: LAUNCH on App Store

### Post-Launch (Ongoing)
- [ ] Monitor D1/D7/D30 retention
- [ ] A/B test monetization offers
- [ ] Release Season 2 content (new biome, heroes, enemies)
- [ ] Community engagement (Discord, Reddit)
- [ ] Android port (Unity checkbox + platform-specific testing)
- [ ] iPad optimization + Mac Catalyst support

---

## 3.5 Key Performance Indicators (KPIs)

| Metric | Target | Industry Benchmark |
|--------|--------|-------------------|
| D1 Retention | > 40% | 25-35% (casual games) |
| D7 Retention | > 20% | 10-15% |
| D30 Retention | > 10% | 4-7% |
| Avg Session Length | 8-12 min | 5-7 min |
| Sessions/Day | 3-5 | 2-3 |
| ARPU (Monthly) | > $0.50 | $0.20-$0.40 |
| Conversion Rate (F2P → Paid) | > 5% | 2-3% |
| App Store Rating | > 4.5 stars | 4.0 |
| Crash-Free Rate | > 99.5% | 99% |

---

## 3.6 Team & Skills Needed

### Solo Developer Path (Feasible)
If building alone, you need these skills:
1. **Unity C# Programming** — Core gameplay, systems, architecture
2. **2D Art / Pixel Art** — Or budget for commissioned art ($2,000-$5,000)
3. **Game Design** — Balancing, progression tuning, UX
4. **Sound Design** — Or use royalty-free SFX + commission music ($500-$1,500)
5. **Marketing** — ASO (App Store Optimization), social media

### Ideal Small Team (3-4 people)
| Role | Responsibility |
|------|---------------|
| **Game Designer / Producer** | Vision, design docs, balancing, analytics |
| **Unity Developer** | Core systems, architecture, optimization |
| **2D Artist / Animator** | Pixel art, sprites, VFX, UI design |
| **Sound Designer** (part-time) | Music, SFX, adaptive audio |

### Estimated Budget (Solo/Lean)
| Item | Cost |
|------|------|
| Apple Developer Program | $99/year |
| Unity Personal (free under $200K) | $0 |
| Art assets (commissioned or asset packs) | $2,000-$5,000 |
| Sound/Music | $500-$1,500 |
| Marketing (soft launch ads) | $1,000-$3,000 |
| TestFlight beta testing | $0 |
| **Total Minimum** | **~$3,600-$9,600** |

---

## 3.7 Learning Resources

### Unity Game Development
- [Unity Learn — Official Tutorials](https://learn.unity.com/) — Free, structured courses
- [Unity 2D Roguelike Tutorial](https://learn.unity.com/project/2d-roguelike-tutorial) — Directly relevant
- [Brackeys YouTube](https://www.youtube.com/c/Brackeys) — Best Unity tutorial channel (archived but gold)
- [Sebastian Lague](https://www.youtube.com/c/SebastianLague) — Procedural generation masterclass
- [Game Programming Patterns](https://gameprogrammingpatterns.com/) — Free book, essential reading

### Game Design
- *"The Art of Game Design"* by Jesse Schell — The bible of game design
- *"A Theory of Fun for Game Design"* by Raph Koster — Understanding player psychology
- *"Hooked"* by Nir Eyal — Habit-forming product design (directly applicable)
- [GDC Vault](https://www.gdcvault.com/) — Thousands of talks from industry experts
- [Game Maker's Toolkit (YouTube)](https://www.youtube.com/c/MarkBrownGMT) — Best design analysis channel

### Roguelike-Specific
- [Roguebasin Wiki](http://www.roguebasin.com/) — Algorithms, design patterns, PCG techniques
- *"Procedural Content Generation in Games"* (free textbook) — Academic PCG reference
- Hades, Vampire Survivors, Slay the Spire, Dead Cells — Play and study these

### Apple Platform
- [Apple Developer Documentation — Games](https://developer.apple.com/games/)
- [GameKit Documentation](https://developer.apple.com/documentation/gamekit)
- [StoreKit 2 Documentation](https://developer.apple.com/documentation/storekit)
- [Core Haptics Documentation](https://developer.apple.com/documentation/corehaptics)
- [App Store Review Guidelines](https://developer.apple.com/app-store/review/guidelines/)

### Mobile Game Business
- [Deconstructor of Fun](https://www.deconstructoroffun.com/) — Deep dives into top games
- [GameRefinery](https://www.gamerefinery.com/) — Market intelligence and feature benchmarking
- [Sensor Tower](https://sensortower.com/) — App Store analytics and market data

---

# PART 4: IMMEDIATE NEXT STEPS

## If You Want Me to Start Building:

1. **Set up a Unity project** on this machine (I can create the project structure, scripts, and initial systems)
2. **Build the core gameplay loop** as a playable prototype
3. **Implement procedural dungeon generation**
4. **Create the combat system**
5. **Build the progression/meta systems**

## If You Want to Build Yourself:

1. **Install Unity Hub** → Download Unity 6 LTS
2. **Create a 2D URP project**
3. **Follow the Unity 2D Roguelike tutorial** as a starting point
4. **Read "Game Programming Patterns"** (free online)
5. **Study Vampire Survivors and Hades** — note what makes them addictive

---

*Document compiled April 2026 | Based on research from Unity 6.4 docs, Godot 4.6 docs, Apple Developer documentation (Metal 4, SpriteKit, GameKit, StoreKit 2), Apple App Store Review Guidelines, mobile gaming market statistics (Newzoo, Sensor Tower), and game design best practices from GDC, Gamasutra/GameDeveloper, and leading mobile studios.*
