# Pocket Dungeons — Game Design Document (GDD)

## 1. Game Identity

### 1.1 One-Line Pitch
A bite-sized roguelike dungeon crawler where every run is different, every death teaches you something, and "just one more run" becomes your mantra.

### 1.2 Elevator Pitch
Pocket Dungeons is a free-to-play mobile roguelike that distills the best of Hades, Vampire Survivors, and Slay the Spire into 2-3 minute runs you can play anywhere. Procedurally generated dungeons, a growing collection of heroes and gear, and psychology-driven retention loops make it impossible to put down.

### 1.3 Core Pillars
1. **Accessible Depth** — Easy to learn, endlessly deep
2. **Every Run Matters** — Even failed runs yield progress
3. **One More Run** — The session design makes stopping feel wrong
4. **Expressive Builds** — Players craft unique strategies via power-up combos

---

## 2. Gameplay

### 2.1 Core Loop (Per Run: 2-3 minutes)

```
1. TAP "Enter Dungeon"
2. Procedurally generated floor appears
3. SWIPE to move through rooms
4. AUTO-ATTACK nearest enemy in range
5. SWIPE to dodge enemy attacks (i-frames)
6. KILL enemies → they DROP loot (gold, potions, gear)
7. CLEAR FLOOR → Choose 1 of 3 POWER-UPS
8. DESCEND to next floor (enemies harder, loot better)
9. DIE (or reach the boss on floor 10/20/30...)
10. RESULTS SCREEN → Gold earned, XP gained, best floor
11. SPEND gold/XP on PERMANENT UPGRADES
12. "ONE MORE RUN" button (zero friction to restart)
```

### 2.2 Controls

| Input | Action |
|-------|--------|
| Left thumb — Virtual joystick | Move in 8 directions |
| Auto-aim | Attack nearest enemy within weapon range |
| Right thumb — Ability buttons (up to 2) | Activate special abilities |
| Swipe anywhere | Dodge roll (short invincibility, 2s cooldown) |
| Tap loot | Pick up items |

Design principle: **One-hand playable** (portrait mode with simplified controls option).

### 2.3 Camera
- Top-down, slightly angled (45-degree isometric feel)
- Follows player with smooth dampening
- Zooms out slightly when many enemies are present
- Shakes proportionally to damage dealt/received

---

## 3. Dungeon System

### 3.1 Floor Structure
Each floor is a grid of interconnected rooms:

```
Early Floors (1-10):    Mid Floors (11-20):    Late Floors (21+):
┌───┬───┐               ┌───┬───┬───┐          ┌───┬───┬───┬───┐
│ S │   │               │ S │   │   │          │ S │   │   │   │
├───┼───┤               ├───┼───┼───┤          ├───┼───┼───┼───┤
│   │ E │               │   │   │   │          │   │   │   │   │
└───┴───┘               ├───┼───┼───┤          ├───┼───┼───┼───┤
3x2 rooms               │   │   │ E │          │   │   │   │   │
                        └───┴───┴───┘          ├───┼───┼───┼───┤
                        4x3 rooms              │   │   │   │ E │
                                               └───┴───┴───┴───┘
                                               5x4 rooms

S = Start, E = Exit (stairs down)
```

### 3.2 Room Types

| Room Type | Frequency | Contents |
|-----------|-----------|----------|
| **Combat Room** | 60% | 3-8 enemies, clear to unlock doors |
| **Treasure Room** | 15% | 1-3 chests, possible trap |
| **Shop Room** | 5% | NPC vendor, buy items with gold |
| **Challenge Room** | 10% | Timed/wave challenge, bonus rewards |
| **Rest Room** | 5% | Heal 25% HP, choose to upgrade a power-up |
| **Boss Room** | Every 10th floor | Boss encounter, guaranteed rare+ loot |
| **Secret Room** | 5% (hidden) | Hidden entrance, best loot |

### 3.3 Biomes (Seasonal Rotation)

| Biome | Visual Theme | Unique Mechanic | Season |
|-------|-------------|-----------------|--------|
| **Stone Dungeon** | Classic grey stone, torches | None (tutorial biome) | Permanent |
| **Cursed Catacombs** | Dark purple, bone walls | Curse debuffs on player | Permanent |
| **Crystal Caverns** | Blue crystals, bioluminescent | Reflecting projectiles | Permanent |
| **Inferno Depths** | Lava, fire, magma floors | Floor damage zones | Summer |
| **Frozen Tomb** | Ice, snow, frozen enemies | Slippery movement, freeze effects | Winter |
| **Haunted Manor** | Victorian, ghosts, chandeliers | Visibility fog, ghost enemies | Halloween |
| **Jade Temple** | East Asian, bamboo, jade | Puzzle traps, monk enemies | Spring |

### 3.4 Procedural Generation Algorithm

```
GENERATE_FLOOR(depth):
  1. grid_size = BASE_SIZE + floor(depth / 5)     // Floors get bigger
  2. bsp_tree = BSP_PARTITION(grid_size)           // Split into rooms
  3. rooms = PLACE_ROOM_TEMPLATES(bsp_tree)        // Hand-crafted templates
  4. corridors = CONNECT_ROOMS(rooms, A_STAR)      // Pathfind corridors
  5. start_room = RANDOM(rooms)                    // Player spawn
  6. exit_room = FARTHEST_FROM(start_room)         // Stairs down
  7. POPULATE_ROOMS(rooms, depth):
       - enemies = WEIGHTED_RANDOM(enemy_table, depth)
       - loot = WEIGHTED_RANDOM(loot_table, depth)
       - traps = WEIGHTED_RANDOM(trap_table, depth)
  8. APPLY_BIOME_THEME(rooms, current_biome)
  9. VALIDATE_FLOOR(rooms, corridors)              // Ensure solvable
  RETURN floor
```

---

## 4. Combat System

### 4.1 Damage Formula

```
Base Damage = WeaponDamage * (1 + HeroATK/100)
Armor Reduction = EnemyArmor / (EnemyArmor + 100)
Final Damage = Base Damage * (1 - Armor Reduction) * CritMultiplier * ElementalBonus

Critical Hit:
  - Base chance: 10%
  - Bonus from gear/power-ups
  - Critical multiplier: 2.0x (upgradeable)

Elemental System:
  Fire > Nature > Ice > Fire (rock-paper-scissors)
  Matching element: +25% damage
  Weak element: -25% damage
```

### 4.2 Combat Feel ("Juice")

Every interaction must feel **satisfying**:

| Event | Visual Effect | Audio Effect | Haptic |
|-------|--------------|-------------|--------|
| Player hits enemy | Hit flash (white), damage number popup, blood particles | Sword slash / arrow thud / magic zap | Light tap |
| Critical hit | Larger flash, screen shake (small), bigger number (gold color) | Enhanced hit + "crit" sting | Medium tap |
| Enemy dies | Explosion particles, coins scatter, slow-mo (last enemy) | Death sound + coin jingle | Medium tap |
| Player hit | Red screen flash, knockback, damage number (red) | Pain grunt + impact | Heavy tap |
| Player dodge | Ghost trail, i-frame flash | Whoosh sound | Light tap |
| Level up | Full-screen golden flash, particle fountain | Triumphant jingle | Success pattern |
| Loot drop | Glow effect, bounce animation | Sparkle sound | Light tap |

### 4.3 Enemy Design Philosophy

Each enemy teaches a **lesson** through gameplay:

| Enemy | Behavior | What Player Learns |
|-------|----------|-------------------|
| **Slime** | Walks toward player, slow | Basic combat (tutorial enemy) |
| **Skeleton Archer** | Stands still, shoots arrows | Dodge projectiles |
| **Bat** | Fast, erratic movement | Track fast targets |
| **Golem** | Slow, heavy hits, armored | Kite and dodge |
| **Ghost** | Phases through walls | Spatial awareness |
| **Bomber** | Explodes on death | Positioning matters |
| **Healer** | Heals nearby enemies | Target priority |
| **Shield Knight** | Blocks frontal attacks | Flank enemies |
| **Mimic** | Disguised as chest | Awareness/caution |
| **Necromancer** | Summons dead enemies | Kill priority, crowd control |

---

## 5. Hero System

### 5.1 Starting Heroes

| Hero | Class | Weapon | Special Ability | Unlock |
|------|-------|--------|----------------|--------|
| **Kael** | Warrior | Sword (melee, 360 arc) | Shield Bash (stun + damage) | Default |
| **Lyra** | Archer | Bow (ranged, auto-aim) | Rain of Arrows (AoE) | Reach Floor 10 |
| **Zara** | Mage | Staff (ranged, AoE splash) | Meteor Strike (massive AoE) | Reach Floor 20 |

### 5.2 Unlockable Heroes (Post-Launch Content)

| Hero | Class | Unlock Condition |
|------|-------|-----------------|
| **Rex** | Berserker | Complete 50 runs |
| **Luna** | Assassin | Kill 1000 enemies |
| **Orin** | Paladin | Beat the final boss |
| **Hex** | Warlock | Collect 100 unique items |
| **Kai** | Monk | Complete 10 daily challenges |
| **Nova** | Elementalist | Master all 3 starting heroes |
| **Drake** | Dragon Knight | Season 1 Battle Pass reward |
| **Aria** | Bard | Season 2 Battle Pass reward |
| **Grim** | Reaper | Halloween event exclusive |

### 5.3 Hero Progression

```
Hero Level (1-50):
  - XP earned per run (scales with floors cleared)
  - Each level: +1% base stats
  - Milestone levels (10, 20, 30, 40, 50): Unlock hero-specific cosmetic

Hero Mastery (Bronze → Silver → Gold → Diamond):
  - Play X runs with the hero
  - Each tier: Unique mastery skin + small stat bonus
  - Diamond mastery: Animated portrait + special death effect
```

---

## 6. Progression Systems

### 6.1 Within-Run Progression (Temporary)

**Power-Up Selection** — After each floor, choose 1 of 3:

| Category | Examples |
|----------|---------|
| **Offense** | +20% attack speed, Poison arrows, Chain lightning, Piercing shots |
| **Defense** | +50 HP, Thorns (reflect damage), Heal on kill, Second wind (revive once) |
| **Utility** | Magnet (auto-collect loot), Speed boost, Extra gold, Larger dodge window |
| **Synergy** | "Fire Starter" + "Oil Slick" = Enemies burn when hit by AoE |

Power-up pool grows as player unlocks more heroes and reaches deeper floors.

### 6.2 Between-Run Progression (Permanent)

**Gold → Town Upgrades:**

| Building | Effect | Max Level |
|----------|--------|-----------|
| **Blacksmith** | +ATK for all heroes | 50 |
| **Armory** | +DEF for all heroes | 50 |
| **Tavern** | +Max HP for all heroes | 50 |
| **Dojo** | +Crit Chance for all heroes | 25 |
| **Library** | +1 power-up choice (4 instead of 3) | 1 |
| **Stables** | +Movement speed for all heroes | 25 |
| **Vault** | +Gold earned per run | 25 |

**Gear System:**

```
Gear Slots: Weapon, Armor, Accessory
Rarity: Common (white) → Uncommon (green) → Rare (blue) → Epic (purple) → Legendary (gold)
Sets: Collect 3 matching pieces for set bonus
  Example: "Shadow Set" = +30% crit damage, attacks have 10% chance to go invisible
```

### 6.3 Long-Term Progression (Weeks/Months)

| System | Description | Retention Hook |
|--------|-------------|---------------|
| **Monster Bestiary** | Kill 10 of each monster to unlock lore + bonus damage vs. that type | Zeigarnik (127/130 monsters) |
| **Achievement Gallery** | 100+ achievements with gem rewards | Completionism |
| **Prestige System** | Reset town upgrades for permanent multiplier (1.1x, 1.2x, ...) | Sunk cost + fresh start |
| **Seasonal Rankings** | Monthly leaderboard based on deepest floor | Social comparison |
| **Collection Log** | Track every item, power-up, hero skin found | Zeigarnik + completionism |

---

## 7. Daily & Weekly Content

### 7.1 Daily Content

| Feature | Description | Reward |
|---------|-------------|--------|
| **Daily Dungeon** | Fixed-seed dungeon (same for all players) | 2x gold + daily leaderboard |
| **Daily Login** | Increasing rewards for consecutive days (7-day cycle) | Gold, gems, gear pieces |
| **Daily Quest** | "Kill 50 enemies" / "Reach floor 15" / "Use 5 power-ups" | Gold + XP |
| **Free Chest** | Opens every 4 hours (max 2 stacked) | Random loot |

### 7.2 Weekly Content

| Feature | Description | Reward |
|---------|-------------|--------|
| **Weekly Boss Raid** | Extra-hard boss, community HP pool | Exclusive gear piece |
| **Weekly Challenge** | Modifier run (double enemies, no healing, speed run) | Gems + rare cosmetic |
| **Clan War** | Clan vs. clan total floors cleared | Clan XP + cosmetic banner |

---

## 8. Social Systems

### 8.1 Game Center Integration
- **Leaderboards**: Deepest floor (all-time), weekly score, daily dungeon time
- **Achievements**: 50+ achievements synced to Game Center
- **Friend Challenges**: Send a "beat my score" challenge to Game Center friends

### 8.2 Clan System
- Create/join a clan (max 30 members)
- Clan chat
- Clan perks (bonus gold, bonus XP) based on clan level
- Clan wars (weekly competitive event)

### 8.3 Async Multiplayer
- **Ghost Runs**: Race against friends' recorded runs
- **Revenge System**: When a friend dies, get their dungeon seed — try to beat it

---

## 9. Audio Design

### 9.1 Music
- **Adaptive soundtrack**: Layers add/remove based on combat intensity
- **Per-biome themes**: Each biome has a unique musical identity
- **Boss music**: Intense, unique track per boss
- **Menu music**: Calm, inviting, with progression (gets richer as player levels up)

### 9.2 Sound Effects
- Every action has a unique sound (attack, dodge, pickup, death, level-up)
- **Spatial audio**: Enemies behind walls are muffled
- **UI sounds**: Satisfying clicks, whooshes, and chimes for every interaction
- **Priority system**: Important sounds (damage, death) always play; ambient sounds duck

---

## 10. Accessibility

| Feature | Description |
|---------|-------------|
| **Colorblind modes** | Deuteranopia, Protanopia, Tritanopia filters |
| **Scalable UI** | All UI elements scale 75%-150% |
| **One-hand mode** | Simplified controls for portrait play |
| **Screen reader** | VoiceOver support for menus and results |
| **Reduced motion** | Disable screen shake, reduce particle effects |
| **Auto-play assist** | Auto-dodge option for less experienced players |
| **Subtitles** | All dialogue/lore text displayed on screen |

---

*This is a living document. Update as design decisions are validated through playtesting and analytics.*
