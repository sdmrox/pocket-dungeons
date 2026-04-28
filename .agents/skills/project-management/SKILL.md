# Modern Project Management for Game Development (2026)

Reference guide for managing game development projects using contemporary methodologies and tools.

---

## 1. Methodology Selection

### Agile for Game Development
Game development is inherently iterative — you can't fully design fun on paper. Modern studios use adapted Agile frameworks:

| Methodology | Best For | Cycle Length | Key Ceremony |
|-------------|----------|-------------|---------------|
| **Scrum** | Teams 5-9, structured workflow | 2-week sprints | Daily standup, sprint review, retro |
| **Kanban** | Continuous flow, ops/live-ops | Continuous | WIP limits, board reviews |
| **Shape Up** | Small teams, high autonomy | 6-week cycles + 2-week cooldown | Betting table, hill charts |
| **Scrumban** | Hybrid needs | 2-week cadence + kanban flow | Flexible ceremonies |

### Recommended for Pocket Dungeons
**Shape Up** (solo/small team) or **Scrum** (3-4 person team):

#### Shape Up for Solo/Small Team
- **6-week build cycles** — Long enough for meaningful features, short enough to avoid scope creep
- **2-week cooldown** — Bug fixes, tech debt, exploration, prototyping
- **No backlog** — Evaluate pitches fresh each cycle (prevents backlog grooming overhead)
- **Appetite-based** — "We'll spend 6 weeks on this" not "How long will this take?"
- **Hill charts** — Track uncertainty vs. execution (uphill = figuring out, downhill = executing)

#### Scrum for 3-4 Person Team
- **2-week sprints** aligned with project phases
- **Sprint Planning** — Pull from prioritized backlog
- **Daily Standup** — 15 min, async OK for remote (Slack/Discord)
- **Sprint Review** — Demo playable builds every 2 weeks
- **Retrospective** — What worked, what didn't, action items

---

## 2. Project Planning Framework

### Phase-Gate Model (for Pocket Dungeons)
```
Phase 0: Foundation (Weeks 1-2)     → Gate: Can build to device, navigate menus
Phase 1: Vertical Slice (Weeks 3-6) → Gate: "Is this fun?" playtest
Phase 2: Depth & Feel (Weeks 7-10)  → Gate: "One more run" unprompted from testers
Phase 3: Meta & Retention (Weeks 11-14) → Gate: D7 retention target hit internally
Phase 4: Polish & Monetization (Weeks 15-18) → Gate: IAP works, analytics flowing
Phase 5: Social & Live-Ops (Weeks 19-22) → Gate: Cloud save, leaderboards live
Phase 6: Launch (Weeks 23-26) → Gate: Beta metrics validated, App Store approved
```

### Priority Framework
| Priority | Definition | Example |
|----------|-----------|--------|
| **P0** | Must have for current phase milestone | Core gameplay loop, player controller |
| **P1** | Important but phase can ship without | Sound effects, haptics |
| **P2** | Nice to have, cut first if behind | Clan system, App Clips |
| **P3** | Future consideration | Android port, Mac Catalyst |

### Task Estimation
- **Shape Up**: No estimates. Fixed time (6 weeks), variable scope. Cut scope to fit.
- **Scrum**: Story points (Fibonacci: 1, 2, 3, 5, 8, 13). If > 8, break it down.
- **Solo**: Day-based estimates. If > 3 days, break into subtasks.

---

## 3. Project Management Tools (2026)

### Tool Recommendations by Team Size

| Tool | Best For | Strengths | Cost |
|------|----------|-----------|------|
| **Linear** | Solo to mid-size (1-30) | Fast, keyboard-driven, beautiful UI, GitHub integration | Free (250 issues), $8/user/mo |
| **Jira** | Large studios (50+) | Scalable, custom workflows, massive integrations | Free (10 users), $7.75+/user/mo |
| **Notion** | Solo/small team | All-in-one (docs + tasks + wiki), flexible | Free (basic), $8/user/mo |
| **GitHub Projects** | Developer-heavy teams | Native GitHub integration, free | Free |
| **HacknPlan** | Game dev specific | GDD integration, discipline tagging (art, code, design) | Free (3 users), $6/user/mo |
| **Codecks** | Indie game studios | Card-based, game dev focused, fun UI | Free (3 users), €4/user/mo |

### Recommended Stack for Pocket Dungeons
- **Project tracking**: Linear or GitHub Projects
- **Documentation**: Notion or repo-based Markdown (current approach)
- **Communication**: Discord (community + team)
- **Version control**: GitHub + Git LFS
- **CI/CD**: Unity Cloud Build + Xcode Cloud

---

## 4. Sprint/Cycle Planning

### Sprint Planning Template
```markdown
## Sprint [X] — [Date Range]
### Sprint Goal
[One sentence describing the sprint's primary objective]

### Committed Items
| Task | Priority | Estimate | Owner | Status |
|------|----------|----------|-------|--------|
| ...  | P0       | 2 days   | Name  | To Do  |

### Stretch Goals (if time permits)
| Task | Priority | Estimate |
|------|----------|----------|
| ...  | P1       | 1 day    |

### Risks & Dependencies
- [Risk 1]
- [Dependency on external resource]

### Definition of Done
- [ ] Feature implemented and tested on device
- [ ] Code reviewed (if team > 1)
- [ ] No new crashes introduced
- [ ] Performance within targets
```

### Daily Standup Format (Async)
```
🔵 Yesterday: [What I completed]
🟢 Today: [What I'm working on]
🔴 Blockers: [What's blocking me]
```

---

## 5. Risk Management

### Risk Register Template
| Risk | Likelihood | Impact | Mitigation | Owner | Status |
|------|-----------|--------|-----------|-------|--------|
| Core loop isn't fun | Medium | Critical | Prototype playtest at Week 6; pivot early | Designer | Active |
| Art quality insufficient | Medium | High | Budget for commissioned art; Asset Store fallback | Producer | Active |
| Apple rejects app | Low | High | Study guidelines; pre-review consultation | Developer | Monitoring |
| Low retention | Medium | High | A/B test onboarding, daily hooks | Designer | Active |
| Scope creep | High | Medium | Strict phase gates; cut features before delaying | Producer | Active |
| Technical debt | Medium | Medium | Refactoring sprint every 4 weeks | Developer | Active |

### Risk Response Strategies
- **Avoid**: Change plan to eliminate risk
- **Mitigate**: Reduce likelihood or impact
- **Transfer**: Outsource (e.g., commission art instead of creating)
- **Accept**: Acknowledge and prepare contingency

---

## 6. Milestone Management

### Milestone Review Checklist
```markdown
## Phase [X] Milestone Review

### Deliverables
- [ ] [Feature 1] — Complete / Partial / Not Started
- [ ] [Feature 2] — Complete / Partial / Not Started

### Quality Gates
- [ ] Builds without errors on iOS device
- [ ] No P0 bugs open
- [ ] Performance within targets (60 FPS, < 300MB RAM)
- [ ] Playtest feedback incorporated

### Metrics (if applicable)
- D1 Retention: [X]%
- Avg Session Length: [X] min
- Crash-Free Rate: [X]%

### Decision
- [ ] PASS — Proceed to next phase
- [ ] CONDITIONAL PASS — Proceed with carryover items
- [ ] FAIL — Extend phase by [X] weeks

### Learnings
- [What went well]
- [What to improve]
- [Action items for next phase]
```

---

## 7. Team Communication

### Communication Matrix
| Channel | Purpose | Frequency |
|---------|---------|----------|
| Discord #dev-chat | Daily async standup, quick questions | Daily |
| Discord #decisions | Architecture decisions, trade-offs | As needed |
| Weekly sync call | Sprint review, planning, blockers | Weekly (30 min) |
| Notion/Linear | Task tracking, documentation | Always updated |
| GitHub PRs | Code review, technical discussion | Per commit |
| Monthly retro | Process improvement | Monthly (1 hour) |

### Decision Log
Record significant decisions with context:
```markdown
## Decision: [Title]
- **Date**: YYYY-MM-DD
- **Context**: [Why this decision was needed]
- **Options Considered**: [List alternatives]
- **Decision**: [What was decided]
- **Rationale**: [Why this option]
- **Consequences**: [Trade-offs accepted]
```

---

## 8. Version Control Workflow

### Git Flow (Simplified for Small Team)
```
main          ← Production (App Store releases)
  └── develop  ← Integration branch
       ├── feature/player-controller
       ├── feature/dungeon-generator
       ├── feature/combat-system
       └── hotfix/crash-fix
```

### Branch Naming Convention
- `feature/[short-description]` — New features
- `fix/[short-description]` — Bug fixes
- `hotfix/[short-description]` — Urgent production fixes
- `chore/[short-description]` — Non-functional (CI, docs, refactor)

### Commit Message Format
```
type(scope): short description

Types: feat, fix, refactor, chore, docs, test, style, perf
Examples:
  feat(combat): add critical hit damage system
  fix(dungeon): prevent unreachable rooms in BSP generation
  perf(rendering): batch sprite draw calls per biome
```

### Git LFS
Track large binary assets with Git LFS:
```bash
git lfs track "*.png" "*.psd" "*.wav" "*.mp3" "*.ogg" "*.fbx" "*.unitypackage"
```

---

## 9. Release Management

### Release Checklist
```markdown
## Release v[X.Y.Z]

### Pre-Release
- [ ] All P0 tasks complete
- [ ] All P0 bugs resolved
- [ ] QA pass on target devices (iPhone 12, 13, 14, 15, iPad)
- [ ] Performance benchmarks within targets
- [ ] Analytics events validated
- [ ] Localization reviewed
- [ ] Accessibility tested

### Build & Submit
- [ ] Version/build number incremented
- [ ] Release notes written
- [ ] App Store screenshots updated (if needed)
- [ ] TestFlight internal build verified
- [ ] Submit to App Store Review

### Post-Release
- [ ] Monitor crash reports (first 24 hours)
- [ ] Monitor App Store reviews
- [ ] Monitor analytics dashboards
- [ ] Respond to user feedback
- [ ] Tag release in Git
```

### Semantic Versioning
```
MAJOR.MINOR.PATCH
1.0.0 — Initial App Store launch
1.1.0 — Season 1 content update
1.1.1 — Hotfix for crash
2.0.0 — Major feature update (e.g., multiplayer)
```

---

## References
- Shape Up (Basecamp, 2019 — still relevant)
- Scrum Guide (2020, Ken Schwaber & Jeff Sutherland)
- Agile Playbook for Game Production (iXie Gaming, 2026)
- 7 Best PM Tools for Game Development (2026)
- Linear vs Jira comparison (2026)
