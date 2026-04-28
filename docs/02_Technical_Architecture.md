# Pocket Dungeons — Technical Architecture

## 1. System Overview

```
┌─────────────────────────────────────────────────────────────┐
│                        UNITY 6 (C#)                         │
│                  Universal Render Pipeline                   │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │  Game     │  │  Dungeon  │  │  Combat  │  │  UI       │  │
│  │  State    │  │  Generator│  │  System  │  │  System   │  │
│  │  Machine  │  │  (PCG)    │  │  (ECS)   │  │  (MVVM)   │  │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘  │
│       │              │              │              │         │
│  ┌────┴──────────────┴──────────────┴──────────────┴─────┐  │
│  │                   DATA LAYER                          │  │
│  │  ScriptableObjects | JSON Save | Remote Config        │  │
│  └───────────────────────────────────────────────────────┘  │
│                                                             │
│  ┌───────────────────────────────────────────────────────┐  │
│  │                 PLATFORM SERVICES                     │  │
│  │  GameKit | StoreKit 2 | CloudKit | Core Haptics      │  │
│  │  Firebase | Unity Analytics | Unity Ads               │  │
│  └───────────────────────────────────────────────────────┘  │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│              iOS (Metal 4) | Android (Vulkan)               │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. Architecture Patterns

### 2.1 Game State Machine

```csharp
public enum GameState
{
    Boot,           // Initialize services, load config
    MainMenu,       // Title screen, hero select
    Loading,        // Generate dungeon, load assets
    Gameplay,       // Active dungeon run
    Paused,         // Pause menu overlay
    PowerUpSelect,  // Choose 1 of 3 power-ups
    BossIntro,      // Boss cutscene/animation
    Death,          // Death animation + results
    Results,        // Score screen, rewards
    Shop,           // IAP store
    Settings        // Options menu
}

// Transitions:
// Boot → MainMenu → Loading → Gameplay ⟷ Paused
// Gameplay → PowerUpSelect → Gameplay
// Gameplay → BossIntro → Gameplay
// Gameplay → Death → Results → MainMenu
```

### 2.2 Entity Component System (DOTS) — Combat Layer

Using Unity's DOTS for performance-critical combat with hundreds of entities:

```csharp
// COMPONENTS (Pure Data)
public struct PositionComponent : IComponentData { public float2 Value; }
public struct HealthComponent : IComponentData { public int Current; public int Max; }
public struct DamageComponent : IComponentData { public int Value; public DamageType Type; }
public struct VelocityComponent : IComponentData { public float2 Value; }
public struct EnemyTag : IComponentData { }
public struct ProjectileTag : IComponentData { }
public struct LootDropComponent : IComponentData { public LootTableId TableId; }

// SYSTEMS (Pure Logic)
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (pos, vel) in SystemAPI.Query<RefRW<PositionComponent>, RefRO<VelocityComponent>>())
        {
            pos.ValueRW.Value += vel.ValueRO.Value * SystemAPI.Time.DeltaTime;
        }
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct DamageSystem : ISystem { /* Process damage events */ }

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EnemyAISystem : ISystem { /* State machine per enemy */ }

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct RenderSyncSystem : ISystem { /* Sync ECS → GameObjects for rendering */ }
```

### 2.3 Traditional OOP — Meta/UI Layer

UI, menus, progression, and save data use standard Unity patterns:

```csharp
// Service Locator for global services
public static class Services
{
    public static ISaveService Save { get; set; }
    public static IAnalyticsService Analytics { get; set; }
    public static IAudioService Audio { get; set; }
    public static IHapticsService Haptics { get; set; }
    public static IRemoteConfigService Config { get; set; }
}

// MVVM for UI
public class HeroSelectViewModel : INotifyPropertyChanged
{
    public ObservableCollection<HeroData> Heroes { get; }
    public HeroData SelectedHero { get; set; }
    public ICommand SelectHeroCommand { get; }
    public ICommand StartRunCommand { get; }
}
```

---

## 3. Procedural Content Generation

### 3.1 Dungeon Generation Pipeline

```
Input: floor_depth, biome_type, seed

Step 1: SEED RANDOM (deterministic for daily dungeons)
         ↓
Step 2: BSP PARTITION
         - Start with full grid (e.g., 40x40 tiles)
         - Recursively split into sub-rectangles
         - Min room size: 6x6 tiles
         - Max depth: 4-6 splits
         ↓
Step 3: ROOM PLACEMENT
         - Select room template from pool (hand-designed layouts)
         - Templates tagged: combat, treasure, shop, challenge, rest
         - Fit template into BSP partition
         ↓
Step 4: CORRIDOR GENERATION
         - A* pathfinding between room centers
         - Corridor width: 2-3 tiles
         - Add occasional widening for visual variety
         ↓
Step 5: ROOM POPULATION
         - Enemy spawner: weighted by depth + biome
         - Loot placement: weighted by depth + room type
         - Trap placement: introduced at depth 5+
         - Environmental hazards: biome-specific
         ↓
Step 6: THEMING
         - Apply tileset (biome-specific)
         - Place decorative objects
         - Set lighting (per-biome color palette)
         - Add ambient particles
         ↓
Step 7: VALIDATION
         - Verify path exists from start → exit
         - Verify all rooms reachable
         - Verify difficulty within bounds
         ↓
Output: Playable floor
```

### 3.2 Loot Table System

```csharp
[CreateAssetMenu(menuName = "Pocket Dungeons/Loot Table")]
public class LootTable : ScriptableObject
{
    public LootEntry[] Entries;
    
    [System.Serializable]
    public struct LootEntry
    {
        public ItemData Item;
        public float Weight;        // Relative probability
        public int MinFloor;        // Don't drop before this floor
        public Rarity MinRarity;    // Minimum rarity at this entry
    }
    
    public ItemData Roll(int floor, float luckModifier = 1f)
    {
        // Filter entries valid for this floor
        // Apply luck modifier to rarity weights
        // Weighted random selection
        // Return item with rolled rarity
    }
}

// Rarity weights (base, modified by depth + luck):
// Common:    60% → decreases with depth
// Uncommon:  25% → stable
// Rare:      10% → increases with depth
// Epic:       4% → increases with depth
// Legendary:  1% → increases with depth + luck
```

---

## 4. Save System

### 4.1 Local Save (Primary)

```csharp
[System.Serializable]
public class PlayerSaveData
{
    // Profile
    public string PlayerId;
    public int PlayerLevel;
    public int TotalXP;
    
    // Currency
    public int Gold;
    public int Gems;
    
    // Heroes
    public List<HeroSaveData> Heroes;
    
    // Town Upgrades
    public Dictionary<string, int> TownUpgradeLevels;
    
    // Gear
    public List<GearSaveData> Inventory;
    public Dictionary<string, string> EquippedGear; // heroId → gearId
    
    // Collections
    public HashSet<string> BestiaryUnlocked;
    public HashSet<string> AchievementsCompleted;
    public HashSet<string> ItemsDiscovered;
    
    // Streaks & Daily
    public int DailyLoginStreak;
    public DateTime LastLoginDate;
    public DateTime LastFreeChestTime;
    
    // Battle Pass
    public int BattlePassLevel;
    public bool BattlePassPremium;
    
    // Stats
    public int TotalRuns;
    public int DeepestFloor;
    public int TotalEnemiesKilled;
    public TimeSpan TotalPlayTime;
}
```

### 4.2 Cloud Save (CloudKit + Unity Cloud Save)

```
Strategy: Write-local-first, sync-to-cloud periodically

On Save:
  1. Write to local JSON (encrypted with AES-256)
  2. Queue cloud sync (debounced, max every 30 seconds)
  3. Background upload to CloudKit / Unity Cloud Save

On Load:
  1. Read local save
  2. Fetch cloud save (if available)
  3. Conflict resolution: take higher-value for each field
     (e.g., max gold, max level, union of collections)
  4. Merge and write back to both local + cloud

Cross-device:
  iPhone ↔ iPad ↔ Mac (via CloudKit)
  iOS ↔ Android (via Unity Cloud Save) [future]
```

---

## 5. Performance Targets

| Metric | Target | Strategy |
|--------|--------|----------|
| **Frame Rate** | 60 FPS constant on iPhone 12+ | ECS for combat, object pooling, LOD |
| **Load Time** | < 2 seconds (cold start to menu) | Addressables, async loading, splash optimization |
| **Memory** | < 300 MB RAM | Texture atlasing, sprite compression, unload unused |
| **Battery** | < 5% per 30 min session | Target 60 FPS not 120, minimize GPU overdraw |
| **Build Size** | < 150 MB (initial download) | Asset bundles for additional biomes, on-demand resources |
| **Crash-Free Rate** | > 99.5% | Crashlytics monitoring, defensive coding, null checks |

### 5.1 Object Pooling

```csharp
public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> pool;
    private T prefab;
    private Transform parent;
    
    public T Get()
    {
        if (pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        return Object.Instantiate(prefab, parent);
    }
    
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}

// Pools for: Enemies, Projectiles, Damage Numbers, Particles, Loot Icons
```

---

## 6. Analytics Architecture

### 6.1 Event Taxonomy

```
// Session Events
session_start { hero_id, town_level, equipped_gear[] }
session_end { floors_cleared, gold_earned, death_cause, duration_sec }

// Combat Events
enemy_killed { enemy_type, floor, damage_dealt, time_alive }
player_death { floor, enemy_type, hero_hp_at_death, power_ups[] }
boss_defeated { boss_id, time_to_kill, hero_id, power_ups[] }

// Progression Events
level_up { new_level, hero_id }
town_upgrade { building, new_level, gold_spent }
gear_equipped { gear_id, rarity, hero_id }
power_up_chosen { power_up_id, alternatives[], floor }

// Monetization Events
iap_view { product_id, context }
iap_purchase { product_id, price, currency }
ad_watched { ad_type, context, reward }
battle_pass_level { new_level, is_premium }

// Engagement Events
daily_login { streak_count, day_of_week }
daily_quest_complete { quest_id }
chest_opened { chest_type, items_received[] }
```

### 6.2 A/B Testing Framework

```csharp
public class ABTestManager
{
    // Managed via Unity Remote Config or Firebase Remote Config
    
    public T GetVariant<T>(string experimentId, T defaultValue)
    {
        // 1. Check if user is in experiment
        // 2. Return assigned variant value
        // 3. Log exposure event for analysis
    }
}

// Example experiments:
// - "starting_gold": 100 vs 200 vs 500
// - "power_up_choices": 2 vs 3 vs 4
// - "daily_reward_multiplier": 1x vs 2x vs 3x
// - "first_iap_price": $0.99 vs $1.99 vs $2.99
```

---

## 7. Security

| Threat | Mitigation |
|--------|-----------|
| Save file tampering | AES-256 encryption + server-side validation for leaderboards |
| Memory editing (Cheat Engine) | Server-authoritative scoring for competitive features |
| Jailbreak exploits | Detect and flag, don't block (avoid false positives) |
| IAP receipt fraud | Server-side receipt validation with Apple's verifyReceipt API |
| Replay attacks | Nonce + timestamp on all server requests |

---

## 8. CI/CD Pipeline

```
Developer Push → Git (GitHub)
                    ↓
              Unity Cloud Build
                    ↓
              ┌─────┴─────┐
              │ Build iOS  │
              │ Build Tests│
              └─────┬─────┘
                    ↓
              Run Unit Tests
              Run Integration Tests
                    ↓
              ┌─────┴─────┐
              │  Pass?     │
              │  Yes → Deploy to TestFlight (internal)
              │  No  → Notify team (Slack/Discord)
              └────────────┘
                    ↓
              Manual QA on TestFlight
                    ↓
              Promote to External TestFlight (beta)
                    ↓
              Analyze beta metrics
                    ↓
              Submit to App Store Review
                    ↓
              Release (phased rollout: 1% → 10% → 50% → 100%)
```

---

*This architecture is designed to scale from MVP to millions of players. Start simple, optimize based on data.*
