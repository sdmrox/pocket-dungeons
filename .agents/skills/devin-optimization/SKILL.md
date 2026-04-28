# Devin Architecture & Cost Optimization (2026)

Reference guide for understanding Devin's architecture and producing maximum-quality output at minimum cost.

---

## 1. Devin Architecture Overview

### What Devin Is
Devin is an **autonomous AI software engineer** by Cognition AI. It operates in its own sandboxed cloud workspace — shell, browser, editor, desktop — and plans, executes, debugs, and verifies across many tool calls with minimal human intervention.

### Core Architecture
```
┌──────────────────────────────────────────────────────────┐
│                    DEVIN SESSION                          │
│                                                          │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌─────────┐ │
│  │  Shell    │  │  Editor  │  │  Browser │  │ Desktop │ │
│  │  (bash)   │  │  (files) │  │  (Chrome)│  │  (GUI)  │ │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬────┘ │
│       └──────────────┴──────────────┴─────────────┘      │
│                         │                                │
│              ┌──────────┴──────────┐                     │
│              │    LLM Reasoning    │                     │
│              │  (Planning + Tools) │                     │
│              └──────────┬──────────┘                     │
│                         │                                │
│  ┌──────────────────────┴───────────────────────┐        │
│  │           Persistent Memory Layer             │        │
│  │  Knowledge | Skills | Playbooks | Secrets     │        │
│  └───────────────────────────────────────────────┘        │
└──────────────────────────────────────────────────────────┘
```

### Key Components
| Component | What It Does |
|-----------|-------------|
| **Sandboxed VM** | Isolated Linux environment with shell, file system, browser, desktop |
| **LLM Reasoning** | Plans tasks, calls tools, debugs errors, iterates until done |
| **Knowledge Base** | Persistent org/user/repo context retrieved automatically by trigger |
| **Skills (SKILL.md)** | Repo-committed procedures Devin follows step-by-step |
| **Playbooks** | Reusable prompt templates for recurring tasks |
| **MCP Integrations** | Connect to external tools (Figma, DBs, monitoring, etc.) |
| **Child Sessions** | Parallel Devin instances for distributed work |

---

## 2. Cost Model: ACUs (Agentic Computing Units)

### What Is an ACU?
ACU = Cognition's normalized measure of resources consumed: **VM time + model inference + networking**. You pay for working time, not seat licenses.

### Pricing (2026)
| Plan | ACU Cost | Concurrent Sessions | Best For |
|------|----------|-------------------|----------|
| **Core** (pay-as-go) | $2.25/ACU | Up to 10 | Solo devs, variable usage |
| **Team** | Volume pricing | More concurrent | Growing teams |
| **Enterprise** | Custom | Custom | Large orgs, SSO, compliance |

### What Drives ACU Consumption
| Factor | High Cost | Low Cost |
|--------|-----------|----------|
| **Exploration time** | Devin searches codebase blindly | You point to specific files/patterns |
| **Wrong approaches** | Devin builds from scratch, then pivots | You specify the approach upfront |
| **Debugging loops** | Devin hits errors and retries 5+ times | You provide test commands and expected output |
| **Scope ambiguity** | "Make it better" (open-ended) | "Add X to file Y using pattern Z" (precise) |
| **Missing context** | Devin reads 50 files to understand | Knowledge/Skills provide context instantly |

---

## 3. The #1 Rule: Specificity Is Cheap, Ambiguity Is Expensive

### The 42→12 ACU Case Study (from Devin Docs)
**Bad prompt** (42 ACUs):
```
Add pagination to GET /api/users. Run the tests when done.
```
**What went wrong**: Devin built pagination from scratch when `src/utils/paginate.ts` already existed. Used cursor-based pagination when tests expected offset-based.

**Good prompt** (12 ACUs):
```
Add offset-based pagination to GET /api/users in src/routes/users.ts.
Use the existing paginate() helper from src/utils/paginate.ts.
Follow the same pattern used in GET /api/products (see src/routes/products.ts).
Run `npm test -- --grep "users"` to verify. All tests should pass.
```

### Why It's 3.5x Cheaper
- **Existing pattern referenced** → No reinvention
- **Specific file paths** → No codebase exploration
- **Exact approach specified** → No wrong turns
- **Test command provided** → No guessing how to verify

---

## 4. Prompt Engineering for Minimum Cost

### The Ideal Prompt Structure
```markdown
## Task
[One sentence: what to do]

## Context
- Repo: [name]
- Key files: [list specific file paths]
- Existing patterns to follow: [reference file]

## Implementation
1. [Step 1 with specific details]
2. [Step 2 with specific details]
3. [Step 3 with specific details]

## Constraints
- Do NOT [common mistake to avoid]
- Use [specific library/tool/pattern]
- Follow [convention already in codebase]

## Verification
- Run: `[exact test command]`
- Expected: [what success looks like]

## Definition of Done
- [ ] [Specific deliverable 1]
- [ ] [Specific deliverable 2]
- [ ] PR created with [description format]
```

### Prompt Do's and Don'ts

| DO | DON'T |
|----|-------|
| Reference specific files: `src/utils/paginate.ts` | Say "find the pagination utility" |
| Specify the approach: "Use offset-based pagination" | Say "add pagination" |
| Provide test commands: `npm test -- --grep users` | Say "run the tests" |
| Link to docs: `https://sequelize.org/docs/v6/...` | Say "check the docs" |
| Define scope: "Only modify these 3 files" | Say "refactor the module" |
| Give examples: "Follow pattern in `authTemplate.rs`" | Say "use the standard pattern" |
| State non-goals: "Do NOT change the database schema" | Leave implicit boundaries |
| Specify commit format: `feat(scope): description` | Say "commit the changes" |

---

## 5. The Three Persistence Layers (Use ALL of Them)

### 5.1 Knowledge Base (Org/User-Level Memory)
**What**: Persistent instructions that Devin auto-retrieves based on trigger descriptions.
**When to use**: Codebase conventions, tool preferences, credential locations, org standards.

**Best Knowledge Items**:
```
Trigger: "When working in the pocket-dungeons repo"
Content: "This is a Unity 6 project targeting iOS. Use C# with DOTS/ECS for
combat systems and traditional OOP for UI/meta. Run `unity-build ios`
to build. Test on iPhone 12+ at 60 FPS."
```

**Tips**:
- Keep each item focused on ONE topic
- Write specific trigger descriptions (not vague "when coding")
- Include exact commands, file paths, naming conventions
- Review and deduplicate weekly (schedule a Monday cleanup session)
- Don't store secrets in knowledge — use the Secrets system

### 5.2 Skills (SKILL.md — Repo-Level Procedures)
**What**: Step-by-step procedures committed to `.agents/skills/<name>/SKILL.md`.
**When to use**: Testing checklists, deployment steps, review procedures, setup guides.

**Skills vs Knowledge vs Playbooks**:
| Feature | Skills | Knowledge | Playbooks |
|---------|--------|-----------|-----------|
| Where stored | Repo (`.agents/skills/`) | Devin platform | Devin platform |
| Scope | Per-repo | Org/user/repo | Org-wide |
| Format | Markdown file | Text + trigger | Prompt template |
| Version controlled | Yes (Git) | No | No |
| Auto-discovered | Yes | Yes (by trigger) | No (manual select) |
| Best for | Repo procedures | Conventions/context | Recurring task templates |

### 5.3 Playbooks (Reusable Prompt Templates)
**What**: Pre-written prompts with variables for recurring tasks.
**When to use**: Same task type done repeatedly across repos (migrations, refactors, reviews).

**Example Playbook**:
```markdown
# Add New Enemy Type

## Task
Add a new enemy type "{{ENEMY_NAME}}" to Pocket Dungeons.

## Files to Modify
- `Assets/Data/Enemies/` — Create new ScriptableObject
- `Assets/Scripts/ECS/Components/` — Add enemy-specific component if needed
- `Assets/Scripts/ECS/Systems/EnemyAISystem.cs` — Add behavior pattern
- `Assets/Art/Enemies/` — Add placeholder sprite

## Pattern to Follow
See existing enemy: `Assets/Data/Enemies/Slime.asset` and `SlimeAI` in EnemyAISystem.

## Verification
- Build compiles without errors
- Enemy spawns in dungeon floor 1
- Enemy has correct AI behavior per spec
```

---

## 6. Multi-Agent Architecture (Parallel Sessions)

### How It Works
Devin can spawn **child sessions** — independent Devin instances running in isolated VMs in parallel. The parent orchestrates, delegates, and aggregates results.

```
Parent Devin (Orchestrator)
    ├── Child Devin 1 — Task A (isolated VM)
    ├── Child Devin 2 — Task B (isolated VM)
    └── Child Devin 3 — Task C (isolated VM)
```

### When to Use Multi-Agent
| Use Case | Why Parallel |
|----------|-------------|
| Migrating 50 files | Each file is independent — 50 parallel sessions |
| Adding tests to 10 modules | Each module is isolated |
| Refactoring across microservices | Each service is independent |
| Batch code reviews | Each PR is independent |

### When NOT to Use Multi-Agent
- Tasks with dependencies between steps
- Single complex feature (needs context across files)
- Small tasks (overhead of spawning not worth it)

### Cost Optimization with Multi-Agent
- Each child session consumes its own ACUs
- But total wall-clock time drops dramatically
- Best for: **high-volume, low-complexity, isolated tasks**
- Formula: `Total ACUs ≈ same, Total Time = Time / N children`

---

## 7. Session Insights (Free Cost Analysis)

### What It Gives You
After every completed session, click the **lightbulb icon** → **Generate Analysis**:
- **ACU Usage** — Was it proportionate to task scope?
- **Improved Prompt** — Rewritten version with missing context filled in
- **Issues Detected** — Wrong approaches, unnecessary exploration, pivots
- **Issue Timeline** — Red marks where Devin changed direction

### Workflow: Continuous Prompt Improvement
```
1. Run task with initial prompt
2. Generate Session Insights (free)
3. Copy the "Improved Prompt"
4. Compare ACU usage
5. If prompt works well → Save as Playbook
6. If task is recurring → Create Knowledge items for context
7. Repeat: each iteration should cost fewer ACUs
```

---

## 8. Cost Optimization Strategies (Ranked by Impact)

### Tier 1: Highest Impact (Save 50-70% ACUs)

| Strategy | How | Why |
|----------|-----|-----|
| **Reference existing patterns** | "Follow the pattern in `src/routes/products.ts`" | Eliminates exploration + reinvention |
| **Specify file paths** | "Modify `src/services/auth.ts` lines 45-60" | Eliminates codebase search |
| **Provide test commands** | "Run `npm test -- --grep auth`" | Eliminates test discovery |
| **Write Skills for your repo** | `.agents/skills/testing/SKILL.md` | Context provided instantly every session |
| **Use Session Insights** | Iterate on prompts after every session | Each iteration is cheaper |

### Tier 2: Medium Impact (Save 20-40% ACUs)

| Strategy | How | Why |
|----------|-----|-----|
| **Knowledge base for conventions** | Add trigger: "When writing C# in this repo..." | No re-learning conventions |
| **Playbooks for recurring tasks** | Template with variables for common patterns | Prompt engineering done once |
| **Break large tasks into slices** | 5 small sessions > 1 huge session | Smaller = more predictable = fewer wrong turns |
| **Specify non-goals** | "Do NOT refactor the auth module" | Prevents scope creep |
| **Provide links to docs** | `https://docs.unity3d.com/...` | Devin reads instead of guessing |

### Tier 3: Good Practice (Save 10-20% ACUs)

| Strategy | How | Why |
|----------|-----|-----|
| **Commit message format** | "Use `feat(scope): description`" | No format guessing |
| **PR template reference** | "Follow PR template in `.github/`" | Consistent output |
| **Connect MCP integrations** | Figma, DB, monitoring tools | Direct access vs. manual workarounds |
| **Secrets pre-provisioned** | Store API keys, DB creds in Devin Secrets | No session blocked on credentials |
| **Weekly knowledge cleanup** | Schedule Monday dedup session | Prevents contradictory/stale knowledge |

---

## 9. Task Sizing for Maximum ROI

### The Sweet Spot
Devin is most cost-effective on tasks that are:
```
Wide & Shallow  >  Tall & Deep

✓ High volume, repetitive, isolated subtasks
✓ Junior-engineer-level complexity
✓ Independently verifiable
✓ < 90 minutes of manual engineering time equivalent

✗ Complex net-new architecture decisions
✗ Tasks requiring deep domain expertise
✗ Highly interdependent multi-file changes
✗ Subjective design work without specs
```

### Best Task Types for ROI
| Task Type | ACU Efficiency | Why |
|-----------|---------------|-----|
| **Migrations** | Excellent | Repetitive, pattern-based, isolated per file |
| **Refactors** | Excellent | Clear before/after, testable |
| **Test writing** | Very good | Isolated per module, verifiable |
| **Bug fixes** (with repro) | Very good | Clear success criteria |
| **Documentation** | Good | Low risk, high value |
| **Code reviews** | Good | Read-heavy, structured output |
| **Modernizations** | Good | Pattern-following, batch-able |
| **Net-new features** | Variable | Good if well-specified; expensive if vague |

### Slicing Large Projects
Instead of: "Build the authentication system"
Do:
```
Slice 1: Create User model and migration
Slice 2: Add POST /api/auth/register endpoint (follow pattern in products.ts)
Slice 3: Add POST /api/auth/login with JWT (use jsonwebtoken library)
Slice 4: Add auth middleware (follow middleware pattern in src/middleware/)
Slice 5: Add tests for all auth endpoints
```
Each slice = 1 Devin session = predictable ACU cost = independently mergeable PR.

---

## 10. Quality Maximization Checklist

### Before Starting a Session
- [ ] **Prompt is specific** — file paths, approach, test commands all included
- [ ] **Existing patterns referenced** — "Follow `X` as a template"
- [ ] **Success criteria defined** — "All tests pass, PR created, no lint errors"
- [ ] **Non-goals stated** — "Do NOT modify X, Y, Z"
- [ ] **Knowledge base updated** — Conventions, commands, structure documented
- [ ] **Skills committed** — Testing, deployment, setup procedures in `.agents/skills/`
- [ ] **Secrets provisioned** — API keys, DB creds, tokens pre-stored
- [ ] **Scope is right-sized** — < 90 min equivalent, independently verifiable

### After a Session
- [ ] **Generate Session Insights** (free) — Check ACU usage vs. expected
- [ ] **Copy improved prompt** if provided — Save as Playbook if recurring
- [ ] **Add new knowledge** if Devin discovered something useful
- [ ] **Update skills** if testing/deployment steps changed
- [ ] **Review for patterns** — Are similar sessions consistently expensive?

---

## 11. Devin Feature Utilization Map

| Feature | What It Does | Cost Impact | Setup Time |
|---------|-------------|-------------|------------|
| **Knowledge** | Auto-injected context by trigger | High savings (less exploration) | 5 min per item |
| **Skills** | Repo procedures followed step-by-step | High savings (consistent behavior) | 15 min per skill |
| **Playbooks** | Reusable prompt templates | Medium savings (no prompt rewriting) | 10 min per playbook |
| **Session Insights** | Post-session analysis + improved prompt | Medium savings (iterative improvement) | Free, 1 click |
| **Secrets** | Pre-stored credentials | Removes blockers (no waiting) | 2 min per secret |
| **MCP Integrations** | Direct tool access (Figma, DB, etc.) | Variable (eliminates workarounds) | 5-15 min per integration |
| **Child Sessions** | Parallel execution | Same ACUs, less wall-time | Automatic |
| **Schedules** | Recurring automated sessions | Consistent maintenance | 5 min per schedule |
| **Devin Wiki** | Auto-generated codebase documentation | Faster onboarding | Automatic |
| **Devin Review** | AI-powered PR review | Faster merge cycles | Setup once |

---

## 12. Pocket Dungeons — Devin Usage Strategy

### Recommended Knowledge Items
```
1. Trigger: "When working in pocket-dungeons repo"
   Content: "Unity 6 C# project. DOTS/ECS for combat, OOP for UI/meta.
   Build: Unity Cloud Build → TestFlight. Target: 60 FPS iPhone 12+."

2. Trigger: "When writing C# code in pocket-dungeons"
   Content: "Use IComponentData for ECS components, ISystem for systems.
   ScriptableObjects for data. Service Locator for global services.
   Conventional commits: feat/fix/refactor(scope): description"

3. Trigger: "When creating enemies in pocket-dungeons"
   Content: "Follow pattern: ScriptableObject in Assets/Data/Enemies/,
   AI behavior in EnemyAISystem.cs, sprite in Assets/Art/Enemies/.
   Reference Slime as the template enemy."
```

### Recommended Playbooks
1. **"Add New Enemy Type"** — ScriptableObject + AI behavior + sprite + spawn rules
2. **"Add New Power-Up"** — ScriptableObject + effect logic + UI icon + balance values
3. **"Add New Biome"** — Tileset + color palette + enemy spawn table + ambient particles
4. **"Add StoreKit Product"** — App Store Connect + StoreKit 2 code + receipt validation
5. **"Weekly Analytics Review"** — Pull Firebase data, check retention, flag anomalies

### Task Slicing for Pocket Dungeons
Instead of "Build Phase 1", slice into:
```
Session 1: Player controller (joystick + movement)
Session 2: Dodge mechanic (swipe + i-frames)
Session 3: BSP dungeon generator
Session 4: Enemy: Slime (melee AI)
Session 5: Enemy: Skeleton Archer (ranged AI)
Session 6: Enemy: Bat (erratic AI)
Session 7: Damage system + HP bars
Session 8: Loot drops + gold collection
Session 9: Floor progression + stairs
Session 10: Results screen + retry button
```
Each session: ~8-15 ACUs, independently testable, mergeable.

---

## References
- Devin Documentation: Best Practices (docs.devin.ai/use-cases/best-practices)
- Cut a Feature Prompt from 42 to 12 ACUs (docs.devin.ai)
- Instructing Devin Effectively (docs.devin.ai)
- Devin Skills Guide (docs.devin.ai/product-guides/skills)
- Devin Knowledge Onboarding (docs.devin.ai/onboard-devin/knowledge-onboarding)
- Creating Playbooks (docs.devin.ai/product-guides/creating-playbooks)
- Session Insights (docs.devin.ai/product-guides/session-insights)
- Devin Pricing & Plans 2026 (pensero.ai)
- Devin AI Prompting Guide 2026 (sureprompts.com)
- Devin Multi-Agent Architecture (leadai.dev, 2026)
- Devin Knowledge Base Guide (Medium, Nitinmatani, 2026)
