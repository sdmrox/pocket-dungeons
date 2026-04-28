# Pocket Dungeons — Coding Conventions

## 1. Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Class / Struct | PascalCase | `EnemySpawner`, `HealthComponent` |
| Interface | IPascalCase | `ISaveService`, `IDamageable` |
| Method | PascalCase | `TakeDamage()`, `SpawnEnemy()` |
| Property | PascalCase | `MaxHealth`, `CurrentFloor` |
| Public field | PascalCase | `AttackDamage` |
| Private field | _camelCase | `_currentHealth`, `_spawnTimer` |
| Local variable | camelCase | `enemyCount`, `damageMultiplier` |
| Parameter | camelCase | `targetPosition`, `damageAmount` |
| Constant | PascalCase | `MaxEnemiesPerRoom`, `DefaultPoolSize` |
| Enum | PascalCase | `DamageType.Fire`, `GameState.Gameplay` |
| ECS Component | PascalCase + "Component" | `HealthComponent`, `VelocityComponent` |
| ECS System | PascalCase + "System" | `MovementSystem`, `DamageSystem` |
| ECS Tag | PascalCase + "Tag" | `EnemyTag`, `ProjectileTag` |
| ScriptableObject | PascalCase + "Data" / "Config" | `EnemyData`, `GameConfig` |
| Event (SO) | PascalCase + "Event" | `OnPlayerDeathEvent`, `OnLootCollectedEvent` |

## 2. Architecture Patterns

### Layer Separation
```
DOTS/ECS Layer (Performance-Critical)     OOP Layer (UI/Meta)
├── Components (pure data structs)         ├── MonoBehaviours (Unity lifecycle)
├── Systems (pure logic)                   ├── ViewModels (MVVM data binding)
├── Authoring (bridge to GameObjects)      ├── Services (singletons via locator)
└── Jobs (Burst-compiled)                  └── ScriptableObjects (data)
```

### When to Use ECS vs OOP
| Use ECS (DOTS) | Use OOP (MonoBehaviour) |
|----------------|----------------------|
| Combat systems | UI / Menus |
| Enemy AI movement | Save / Load |
| Projectile physics | Analytics |
| Damage calculation | Audio management |
| Spawning / pooling | Store / IAP |

### Service Locator
- All global services accessed via `Services.Get<T>()`
- Register during Boot state, never in gameplay
- Always code to interface (`ISaveService`, not `SaveService`)

### ScriptableObject Events
- Use for decoupled communication between systems
- Listener registers in `OnEnable`, unregisters in `OnDisable`
- Events carry typed payload (e.g., `GameEvent<int>` for damage)

## 3. Code Organization

### File Structure
- One class per file (exception: small data structs)
- File name matches class name exactly
- Place in folder matching namespace

### Namespaces
```csharp
PocketDungeons.Core          // State machine, services, events, pooling
PocketDungeons.ECS           // Components, systems, authoring
PocketDungeons.UI            // ViewModels, views, widgets
PocketDungeons.Gameplay      // Player, enemies, dungeon, combat, loot
PocketDungeons.Data          // ScriptableObject definitions
PocketDungeons.Utils         // Extension methods, helpers
```

## 4. Git Workflow

### Branch Naming
```
feature/[short-description]     feat: new feature
fix/[short-description]         fix: bug fix
hotfix/[short-description]      hotfix: critical production fix
chore/[short-description]       chore: maintenance, tooling
```

### Commit Messages (Conventional Commits)
```
type(scope): description

Types: feat, fix, refactor, chore, docs, test, style, perf
Scope: core, ecs, ui, gameplay, dungeon, combat, loot, audio, analytics
```

Examples:
```
feat(combat): add damage number popup system
fix(dungeon): prevent overlapping room generation
refactor(core): extract object pool into generic class
chore(ci): update Unity Cloud Build config
```

### Code Review Checklist
- [ ] No `Debug.Log` in production code (use conditional `#if UNITY_EDITOR`)
- [ ] No magic numbers (use constants or ScriptableObject configs)
- [ ] ECS components are blittable (no reference types)
- [ ] Object pool used for frequently created/destroyed objects
- [ ] Events unsubscribed in `OnDisable`
- [ ] Null checks on ScriptableObject references

## 5. Performance Rules

1. **Never allocate in Update/FixedUpdate** — cache references in Awake/Start
2. **Use object pooling** for enemies, projectiles, VFX, damage numbers
3. **String operations**: use `StringBuilder` or `string.Create`, never concatenation in loops
4. **LINQ**: avoid in hot paths (causes GC allocation)
5. **GetComponent**: cache in Awake, never call per frame
6. **Burst compile** all ECS jobs
7. **Profile before optimizing** — use Unity Profiler + Xcode Instruments
