# Modern iOS App Development (2026)

Reference guide for building production-quality iOS apps and games targeting the Apple ecosystem.

---

## 1. Platform Overview (2026)

| Technology | Version | Purpose |
|------------|---------|--------|
| **Xcode** | 26+ | IDE, builds, simulator, Instruments |
| **Swift** | 6.x | Primary language (strict concurrency) |
| **SwiftUI** | Latest | Declarative UI framework |
| **UIKit** | Still supported | Legacy/complex UI, interop |
| **Metal** | 4 | GPU rendering (Unity renders via Metal) |
| **GameKit** | Latest | Game Center: leaderboards, achievements, matchmaking |
| **StoreKit** | 2 | In-app purchases, subscriptions |
| **Core Haptics** | Latest | Taptic Engine feedback |
| **CloudKit** | Latest | Cross-device cloud save sync |
| **App Intents** | Latest | Widgets, Shortcuts integration |
| **iOS SDK** | 26 | Latest required for App Store submission |

---

## 2. Swift 6 Concurrency

### Key Concepts
Swift 6 enforces **strict concurrency checking** — data races are compile-time errors.

| Concept | Purpose |
|---------|--------|
| `async/await` | Structured asynchronous code |
| `Actor` | Isolated mutable state (serial execution) |
| `@MainActor` | UI-bound state and logic (main thread) |
| `Sendable` | Types safe to pass across concurrency boundaries |
| `TaskGroup` | Structured concurrent work |
| `AsyncSequence` | Async iteration (streams) |

### Rules
- `@MainActor` — UI and UI-bound state **only**. Keep decode, CPU work, disk I/O off main actor.
- `actor` — One serial executor per instance. Use for shared mutable state.
- `nonisolated` — Opt out of actor isolation for pure functions.
- All types crossing concurrency boundaries must be `Sendable`.
- Never block the main actor with synchronous heavy work.

### Pattern: Background Processing
```swift
actor DataProcessor {
    private var cache: [String: Data] = [:]
    
    func process(_ input: String) async throws -> Result {
        if let cached = cache[input] { return cached }
        let result = try await heavyComputation(input)  // Off main thread
        cache[input] = result
        return result
    }
}

// UI Layer
@MainActor
class ViewModel: ObservableObject {
    @Published var result: Result?
    private let processor = DataProcessor()
    
    func load() {
        Task {
            result = try await processor.process("input")
        }
    }
}
```

---

## 3. SwiftUI Architecture Patterns

### MVVM (Recommended for Most Apps)
```
View (SwiftUI)  ←→  ViewModel (@Observable)  ←→  Model / Services
```
- **View**: Pure UI, no business logic
- **ViewModel**: `@Observable` class (Swift 5.9+), business logic, data transformation
- **Model**: Data types, persistence, network

### The Composable Architecture (TCA)
For complex apps needing strict unidirectional data flow:
```
Action → Reducer → State → View
              ↓
          Effects (side effects)
```
- Every state change is testable
- Every side effect is controlled
- Scales well for large teams

### Coordinator Pattern
For navigation in multi-screen apps:
- Coordinators own navigation logic
- Views don't know about other views
- Deep linking becomes straightforward

### Choosing a Pattern
| Pattern | Team Size | App Complexity | Testing Needs |
|---------|-----------|---------------|---------------|
| MVVM | 1-5 | Small-Medium | Standard |
| TCA | 3-10+ | Medium-Large | Exhaustive |
| MVVM + Coordinator | 2-8 | Medium (deep navigation) | Standard |

---

## 4. Apple Platform APIs for Games

### Metal 4 (GPU Rendering)
- Unity renders via Metal automatically on iOS
- **MetalFX upscaling** — Render at lower resolution, upscale for visual quality
- GPU-driven rendering pipeline for complex scenes
- For Unity projects: Metal is the default backend; no manual Metal code needed

### GameKit / Game Center
```swift
// Authenticate player
GKLocalPlayer.local.authenticateHandler = { viewController, error in
    if let vc = viewController {
        // Present Game Center login
    }
}

// Submit score
GKLeaderboard.submitScore(score, context: 0, player: GKLocalPlayer.local,
    leaderboardIDs: ["deepest_floor", "weekly_score"]) { error in }

// Report achievement
let achievement = GKAchievement(identifier: "first_boss_kill")
achievement.percentComplete = 100
GKAchievement.report([achievement]) { error in }
```

### StoreKit 2 (In-App Purchases)
```swift
// Modern async API
let products = try await Product.products(for: ["battle_pass", "gem_pack_100"])

for product in products {
    let result = try await product.purchase()
    switch result {
    case .success(let verification):
        let transaction = try checkVerified(verification)
        // Deliver content
        await transaction.finish()
    case .userCancelled, .pending:
        break
    }
}

// Listen for transactions (app launch)
for await result in Transaction.updates {
    let transaction = try checkVerified(result)
    // Process transaction
    await transaction.finish()
}
```

**Requirements**:
- All digital goods must use StoreKit (Apple takes 15-30%)
- "Restore Purchases" button must be visible in settings
- Server-side receipt validation to prevent fraud
- Subscription auto-renewal must be clearly disclosed

### Core Haptics
```swift
import CoreHaptics

class HapticsService {
    private var engine: CHHapticEngine?
    
    func playHit(intensity: Float) {
        let event = CHHapticEvent(
            eventType: .hapticTransient,
            parameters: [
                CHHapticEventParameter(parameterID: .hapticIntensity, value: intensity),
                CHHapticEventParameter(parameterID: .hapticSharpness, value: 0.5)
            ],
            relativeTime: 0
        )
        let pattern = try? CHHapticPattern(events: [event], parameters: [])
        let player = try? engine?.makePlayer(with: pattern!)
        try? player?.start(atTime: 0)
    }
}
```

**Haptic Intensity Guide**:
| Event | Intensity | Sharpness |
|-------|-----------|-----------|
| Light tap (button press) | 0.1 | 0.3 |
| Hit enemy | 0.3 | 0.5 |
| Critical hit | 0.6 | 0.7 |
| Player damaged | 0.8 | 0.8 |
| Level up | Custom pattern (ramp) | — |
| Loot drop | 0.2 | 0.4 |

### CloudKit (Cross-Device Sync)
- **Write-local-first**, sync to cloud periodically
- Conflict resolution: Take higher value per field (max gold, max level, union of collections)
- Supports iPhone ↔ iPad ↔ Mac sync
- Debounce cloud writes (max every 30 seconds)

### App Intents & Widgets
- **"Energy full!"** widget drives re-engagement
- **"Daily challenge available"** widget
- Siri Shortcuts for quick game launch
- App Clips for instant-play demo from App Store

---

## 5. App Store Compliance (2026)

### iOS 26 SDK Requirements
- All new submissions **must** be built with iOS 26 SDK (as of April 2026)
- `UIWebView` fully removed — use `WKWebView`
- TLS 1.2 minimum for all connections
- Privacy nutrition labels must be accurate

### App Store Review Guidelines — Key Rules
| Guideline | Requirement | Compliance |
|-----------|-------------|------------|
| 3.1.1 | Digital goods must use Apple IAP | All purchases via StoreKit 2 |
| 3.1.2(a) | Subscriptions must provide ongoing value | Battle Pass delivers 30 days of content |
| 3.2.2 | No unacceptable business models | No real-money loot boxes |
| 5.1.1 | Privacy nutrition labels | Accurate analytics disclosure |
| 5.1.2 (2026) | Disclose data sharing with third parties including AI | Explicit disclosure |
| 5.6 | No review manipulation | No prompts during frustration; max 3x/year per Apple |
| 1.3 | Kids category compliance | 12+ rating; no minor data collection |

### Privacy Requirements
1. Privacy policy URL in **App Store Connect metadata** AND inside the app
2. URL must return valid HTML (no PDF, no 404, no redirect to homepage)
3. Policy must match actual data collection
4. App Tracking Transparency prompt required before IDFA access
5. Disclose if data is shared with any third-party AI systems (2026 rule)

### Loot / Gacha Compliance
- **No real-money randomized purchases** — players never spend real money on random outcomes
- Gem-purchased items are **always specific** (buy exactly what you see)
- In-game loot (dropped by enemies) is random but costs no real money
- If cosmetic gacha added later: **display odds prominently** (required in Japan, South Korea, China, Belgium)

---

## 6. Unity-to-iOS Pipeline

### Build Process
```
Unity Project → IL2CPP (C# → C++) → Xcode Project → Metal Rendering → .ipa
```

### IL2CPP Best Practices
- **AOT compilation only** — iOS prohibits JIT
- Avoid heavy reflection and dynamic generic instantiation
- Use `link.xml` to prevent code stripping of needed types
- Test on **real devices** — Editor uses Mono (JIT), IL2CPP behaves differently
- Common crash: `ExecutionEngineException: Attempting to call method for which no AOT code was generated`
  - Fix: Add `[Preserve]` attribute or explicit references in `link.xml`

### Build Size Optimization
- Enable **Managed Stripping Level: High** (but test thoroughly)
- Use **ASTC texture compression** (best quality-to-size ratio on iOS)
- Asset Bundles for non-essential content (downloadable biomes)
- On-Demand Resources for large assets
- Target: < 150 MB initial download

### Performance on iOS
- Target: **60 FPS on iPhone 12+**
- Profile with Xcode **Instruments** (not just Unity Profiler)
- Metal Frame Capture for GPU debugging
- Thermal throttling: iOS aggressively limits CPU/GPU after ~10 min of heavy use
- Test on oldest supported device (iPhone 12) with thermal soak

---

## 7. TestFlight & Beta Distribution

| Phase | Players | Duration | Goal |
|-------|---------|----------|------|
| Alpha (internal) | 10-20 | 2 weeks | Critical bugs, core loop validation |
| Closed Beta | 100-500 | 3 weeks | Retention data, balance, crashes |
| Open Beta | 1,000-5,000 | 2 weeks | Scale testing, ASO keyword testing |

### Beta Validation Metrics
| Metric | Must Hit | Action if Missed |
|--------|----------|------------------|
| D1 Retention | > 35% | Rework onboarding |
| D7 Retention | > 15% | More meta progression, daily hooks |
| Avg Session | > 5 min | Deeper core loop |
| Crash-Free | > 99% | Fix before launch |

---

## 8. App Store Optimization (ASO)

### Title & Keywords
- **Title**: `Pocket Dungeons: Roguelike RPG` (max 30 chars)
- **Subtitle**: `One More Run.` (max 30 chars)
- **Keywords**: dungeon crawler, roguelike, pixel art game, RPG roguelike, pocket dungeon

### Screenshots
- Use all 10 slots
- First 3 screenshots visible without scrolling — make them count
- Show gameplay, not menus
- Add short captions ("Endless Dungeons. One More Run.")

### App Preview Video
- 30 seconds max
- First 3 seconds: Hook (logo + action)
- Show core loop: combat → loot → power-up → boss
- End with CTA + App Store badge

---

## References
- Apple Developer Documentation (2026)
- iOS 26 SDK Migration Guide
- Swift 6 Concurrency Guide
- StoreKit 2 Documentation
- Core Haptics Documentation
- App Store Review Guidelines (April 2026 update)
- Unity IL2CPP Best Practices
