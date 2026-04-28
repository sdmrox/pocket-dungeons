# Modern Game Development Architecture (2026)

Reference guide for building scalable, performant game systems using contemporary architecture patterns.

---

## 1. Hybrid Architecture: ECS + OOP

Modern Unity games use a **hybrid architecture** — not pure OOP or pure ECS, but each where it excels:

| Layer | Pattern | Why |
|-------|---------|-----|
| **Combat / Simulation** | ECS (DOTS) | Cache-friendly, parallelizable, handles thousands of entities |
| **UI / Menus** | OOP + MVVM | Unity UI Toolkit data binding, familiar patterns |
| **Meta / Progression** | OOP + Services | ScriptableObjects, save data, straightforward logic |
| **Audio** | OOP + Service Locator | Event-driven, global access needed |
| **Analytics** | OOP + Event Bus | Decoupled event tracking |

---

## 2. Entity Component System (Unity DOTS)

### Core Concepts
```
Entities    = Lightweight IDs (just integers, no data or logic)
Components  = Pure data structs (IComponentData) — Position, Health, Velocity
Systems     = Pure logic — iterate over entities with specific component combos
```

### Why ECS
- **Cache-friendly memory layout** — Components stored contiguously in memory (SoA)
- **10-100x performance** over OOP for large entity counts
- **Automatic parallelization** via Unity Job System
- **Burst Compiler** — Compiles C# to highly optimized native code

### DOTS Stack
| Component | Purpose |
|-----------|---------|
| **Entities** | Data-oriented entity management |
| **Jobs System** | Safe multi-threading |
| **Burst Compiler** | LLVM-based native code generation |
| **Collections** | Native containers (NativeArray, NativeHashMap) |
| **Mathematics** | SIMD-optimized math library |

### ECS Code Pattern
```csharp
// COMPONENTS (Pure Data)
public struct PositionComponent : IComponentData { public float2 Value; }
public struct HealthComponent : IComponentData { public int Current; public int Max; }
public struct VelocityComponent : IComponentData { public float2 Value; }
public struct EnemyTag : IComponentData { }

// SYSTEMS (Pure Logic)
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (pos, vel) in 
            SystemAPI.Query<RefRW<PositionComponent>, RefRO<VelocityComponent>>())
        {
            pos.ValueRW.Value += vel.ValueRO.Value * SystemAPI.Time.DeltaTime;
        }
    }
}
```

### When to Use ECS vs Traditional
| Use ECS For | Use Traditional OOP For |
|-------------|------------------------|
| Enemies (hundreds on screen) | Main menu UI |
| Projectiles / bullets | Save/load system |
| Particles & VFX logic | IAP / StoreKit integration |
| Damage calculations | Settings screens |
| Loot drop physics | Analytics events |
| AI pathfinding (mass) | Tutorial/onboarding flow |

---

## 3. Game State Machine

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
```

### State Machine Principles
- **Single source of truth** for current game state
- **Explicit transitions** — No hidden state changes
- **Entry/Exit actions** — Each state defines setup and teardown
- **Guard conditions** — Transitions only fire when preconditions met
- **Observable** — UI and systems subscribe to state changes

---

## 4. Service Architecture

### Service Locator Pattern
```csharp
public static class Services
{
    public static ISaveService Save { get; set; }
    public static IAnalyticsService Analytics { get; set; }
    public static IAudioService Audio { get; set; }
    public static IHapticsService Haptics { get; set; }
    public static IRemoteConfigService Config { get; set; }
}
```

### When to Use Each Pattern
| Pattern | Use When |
|---------|----------|
| **Service Locator** | Global services (audio, analytics, save) — simple, small teams |
| **Dependency Injection** | Large codebase, need testability, team > 5 devs |
| **Singleton** | Avoid in production — use only for true one-instance needs (e.g., GameManager bootstrap) |
| **ScriptableObject Events** | Decoupled communication between systems |

### Dependency Injection (Manual Container)
```csharp
public class GameBootstrap : MonoBehaviour
{
    void Awake()
    {
        var saveService = new JsonSaveService();
        var audioService = new UnityAudioService();
        var analyticsService = new FirebaseAnalyticsService();
        
        // Register services
        Services.Save = saveService;
        Services.Audio = audioService;
        Services.Analytics = analyticsService;
    }
}
```

---

## 5. MVVM for UI (Unity 6 UI Toolkit)

Unity 6 introduced **runtime data binding** in UI Toolkit, enabling true MVVM:

```
Model (Data)  ←→  ViewModel (Logic + Binding)  ←→  View (UXML + USS)
```

### Structure
- **Model**: ScriptableObject or plain C# class holding game data
- **ViewModel**: Implements `INotifyPropertyChanged`, exposes bindable properties
- **View**: UXML layout + USS styles, binds to ViewModel properties

### Benefits
- **Automatic UI updates** when data changes (no manual `SetText()` calls)
- **Separation of concerns** — UI artists edit UXML/USS, programmers edit ViewModels
- **Testable** — ViewModels can be unit tested without UI

---

## 6. Event-Driven Architecture

### ScriptableObject Event System
```csharp
[CreateAssetMenu(menuName = "Events/Game Event")]
public class GameEvent : ScriptableObject
{
    private List<IGameEventListener> listeners = new();
    
    public void Raise() => listeners.ForEach(l => l.OnEventRaised());
    public void Register(IGameEventListener listener) => listeners.Add(listener);
    public void Unregister(IGameEventListener listener) => listeners.Remove(listener);
}
```

### Event Categories
| Category | Events |
|----------|--------|
| Combat | `OnEnemyKilled`, `OnPlayerDamaged`, `OnCriticalHit`, `OnBossDefeated` |
| Progression | `OnLevelUp`, `OnFloorCleared`, `OnPowerUpChosen`, `OnGoldCollected` |
| Meta | `OnTownUpgraded`, `OnHeroUnlocked`, `OnGearEquipped` |
| UI | `OnMenuOpened`, `OnPopupDismissed`, `OnTabSwitched` |
| System | `OnGameStateChanged`, `OnSaveCompleted`, `OnRemoteConfigLoaded` |

---

## 7. Procedural Content Generation

### Dungeon Generation Pipeline
```
Input: floor_depth, biome_type, seed
         ↓
Step 1: SEED RANDOM → Deterministic generation (replayable daily dungeons)
         ↓
Step 2: BSP PARTITION → Recursively split grid into sub-rectangles (min 6x6 tiles)
         ↓
Step 3: ROOM PLACEMENT → Fit hand-crafted templates into BSP partitions
         ↓
Step 4: CORRIDOR GENERATION → A* pathfinding between room centers (2-3 tile width)
         ↓
Step 5: ROOM POPULATION → Weighted random: enemies, loot, traps (by depth + biome)
         ↓
Step 6: THEMING → Apply tileset, lighting, ambient particles per biome
         ↓
Step 7: VALIDATION → Verify path start→exit, all rooms reachable, difficulty bounds
         ↓
Output: Playable floor
```

### Algorithm Choices
| Algorithm | Use Case | Strengths |
|-----------|----------|----------|
| **BSP (Binary Space Partitioning)** | Room-based dungeons | Guaranteed non-overlapping rooms, clean corridors |
| **Wave Function Collapse (WFC)** | Tile-based organic layouts | Coherent patterns from small examples, stylistic variety |
| **Cellular Automata** | Cave systems, organic terrain | Natural-looking irregular shapes |
| **RL-Enhanced WFC** | Controllable generation (2025+) | Global structure control, higher success rates |

### Loot Table System
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
}

// Rarity base weights (modified by depth + luck):
// Common:    60% → decreases with depth
// Uncommon:  25% → stable
// Rare:      10% → increases with depth
// Epic:       4% → increases with depth
// Legendary:  1% → increases with depth + luck
```

---

## 8. Performance Architecture

### Object Pooling
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
```

**Pool these**: Enemies, Projectiles, Damage Numbers, Particles, Loot Icons, VFX

### Performance Targets
| Metric | Target | Strategy |
|--------|--------|----------|
| Frame Rate | 60 FPS on iPhone 12+ | ECS for combat, object pooling, LOD |
| Load Time | < 2s cold start to menu | Addressables, async loading, splash optimization |
| Memory | < 300 MB RAM | Texture atlasing, sprite compression, unload unused |
| Battery | < 5% per 30 min | Target 60fps not 120, minimize GPU overdraw |
| Build Size | < 150 MB initial | Asset bundles, on-demand resources |
| Crash-Free | > 99.5% | Crashlytics, defensive coding |

### Addressables & Async Loading
- Use Unity Addressables for all non-essential assets
- **Preload** next biome's assets while player is on current floor
- **Unload** previous biome assets after floor transition
- **Sprite Atlases** for batching draw calls (one atlas per biome)
- **Texture compression**: ASTC on iOS for 75% memory savings

---

## 9. Data Architecture

### ScriptableObject-Driven Design
All game data lives in ScriptableObjects — not hardcoded:
- Enemy stats, weapon data, loot tables, power-up definitions
- Biome configurations, room templates
- Tunable via Unity Inspector without recompiling
- Overridable via Remote Config for live-ops

### Save System
```
On Save:
  1. Write to local JSON (encrypted with AES-256)
  2. Queue cloud sync (debounced, max every 30 seconds)
  3. Background upload to CloudKit / Unity Cloud Save

On Load:
  1. Read local save
  2. Fetch cloud save (if available)
  3. Conflict resolution: take higher-value per field
  4. Merge and write back to both local + cloud
```

---

## 10. CI/CD Pipeline

```
Developer Push → GitHub
         ↓
    Unity Cloud Build
         ↓
    Build iOS + Run Tests
         ↓
    Pass? → Deploy to TestFlight (internal)
    Fail? → Notify team (Slack/Discord)
         ↓
    Manual QA on TestFlight
         ↓
    Promote to External TestFlight (beta)
         ↓
    App Store Review → Phased Rollout (1% → 10% → 50% → 100%)
```

---

## 11. Security Architecture

| Threat | Mitigation |
|--------|------------|
| Save file tampering | AES-256 encryption + server-side validation for leaderboards |
| Memory editing | Server-authoritative scoring for competitive features |
| Jailbreak exploits | Detect and flag, don't block (avoid false positives) |
| IAP receipt fraud | Server-side receipt validation with Apple's `verifyReceipt` API |
| Replay attacks | Nonce + timestamp on all server requests |

---

## 12. Clean Architecture Layers

```
┌──────────────────────────────────────────┐
│           Presentation Layer             │
│  (UI Views, UXML, USS, Animations)       │
├──────────────────────────────────────────┤
│           Application Layer              │
│  (ViewModels, Use Cases, State Machine)  │
├──────────────────────────────────────────┤
│            Domain Layer                  │
│  (Entities, Components, Game Rules,      │
│   Interfaces, Value Objects)             │
├──────────────────────────────────────────┤
│         Infrastructure Layer             │
│  (Save System, CloudKit, Firebase,       │
│   StoreKit, Platform Services)           │
└──────────────────────────────────────────┘
```

### Dependency Rule
Dependencies flow **inward** — outer layers depend on inner layers, never the reverse. Domain layer has **zero** external dependencies.

---

## References
- Unity DOTS & ECS Guide (2026)
- Unity 6 Design Patterns — MVVM Tutorial
- Clean Architecture for Unity (SnappGames, 2026)
- Game Programming Patterns (Robert Nystrom)
- ECS Complete Tutorial (Generalist Programmer, 2025)
