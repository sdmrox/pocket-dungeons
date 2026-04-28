# Business Analytics for Mobile Game Development (2026)

Reference guide for data-driven decision making, KPI tracking, and financial modeling for F2P mobile games.

---

## 1. The Four Pillars of Mobile Game Health

Every metric falls into one of four pillars. Optimizing one while neglecting others is the most dangerous mistake:

| Pillar | Question | Key Metrics |
|--------|----------|-------------|
| **Engagement** | Are players coming back? | Retention (D1/D7/D30), DAU/MAU, session length, session frequency |
| **Monetization** | Are they spending? | ARPU, ARPPU, conversion rate, LTV |
| **Acquisition** | Are you growing efficiently? | CPI, CAC, ROAS, organic vs paid ratio |
| **Technical** | Is the experience stable? | Crash-free rate, load time, FPS, ANR rate |

---

## 2. Engagement KPIs

### Retention (The Most Important Metric)

| Metric | Definition | Target (Roguelike) | Industry Median |
|--------|-----------|-------------------|----------------|
| **D1 Retention** | % returning Day 1 | > 35% | 22% |
| **D7 Retention** | % returning Day 7 | > 15% | 8% |
| **D30 Retention** | % returning Day 30 | > 8% | 3% |

**Interpretation**:
- D1 < 25%: Core loop or onboarding problem → iterate before scaling
- D7 < 10%: Missing meta progression or daily hooks
- D30 < 5%: No long-term engagement systems (Battle Pass, seasons, social)

### Session Metrics
| Metric | Target | Why It Matters |
|--------|--------|---------------|
| Avg Session Length | 8-12 min (3-4 runs) | Too short = not engaging; too long = burnout risk |
| Sessions per Day | 3-5 | Multiple short sessions = healthy habit formation |
| DAU/MAU Ratio | > 20% | "Stickiness" — how often monthly users play daily |

### North Star Metric
For Pocket Dungeons, the **North Star Metric** should be:
> **Daily Runs Completed per Active Player**

This captures engagement (they're playing), depth (completing full runs), and satisfaction (choosing to run again). It's correlated with retention and monetization but isn't either directly.

---

## 3. Monetization KPIs

| Metric | Definition | Target | Formula |
|--------|-----------|--------|--------|
| **ARPU** | Average Revenue Per User | > $0.50/month | Total Revenue ÷ Total Users |
| **ARPPU** | Average Revenue Per Paying User | > $15/month | Total Revenue ÷ Paying Users |
| **Conversion Rate** | Free → Paid | > 4% | Paying Users ÷ Total Users × 100 |
| **LTV** | Lifetime Value | > $2.00 | Revenue per user over their entire lifetime |
| **Battle Pass Attach Rate** | BP purchases ÷ MAU | > 8% | BP Purchasers ÷ MAU × 100 |
| **First Purchase Timing** | Days to first spend | < 5 days | Avg days from install to first IAP |

### LTV Calculation Methods

#### Simple LTV
```
LTV = ARPDAU × Avg Lifetime Days
```

#### Cohort-Based LTV (More Accurate)
```
LTV(D30) = Σ(Revenue per user on day d) for d = 0 to 30
LTV(Projected) = LTV(D30) × Extrapolation Factor

Extrapolation factors by genre (roguelike):
  D30 → D60:  1.4x
  D30 → D90:  1.7x
  D30 → D180: 2.1x
  D30 → D365: 2.5x
```

### Revenue Stream Analysis
| Stream | Expected % of Revenue | Health Indicator |
|--------|----------------------|------------------|
| Battle Pass | 35-40% | Primary driver; if low, season content needs improvement |
| Cosmetic IAP | 20-25% | Healthy if growing with content releases |
| Gem Packs | 15-20% | Monitor for whale concentration |
| Starter Packs | 10-15% | One-time; front-loaded revenue |
| Ads (rewarded) | 10-15% | Should supplement, not dominate |

---

## 4. Acquisition KPIs

### Cost Metrics (2026 Benchmarks)
| Metric | iOS Benchmark | Android Benchmark |
|--------|--------------|-------------------|
| **CPI** (Cost Per Install) | $4.22 avg | $1.54 avg |
| **CPI** (Roguelike/Mid-Core) | $2.50-$5.00 | $1.00-$2.50 |
| **CAC** (Cost to Acquire) | CPI + attribution cost | CPI + attribution cost |

### The Profitability Equation
```
LTV > CPI × (1 + Apple Commission Rate)

For profitability: LTV:CPI ratio must be > 1.5
  - 1.0 = Break even (unsustainable)
  - 1.5 = Minimum viable
  - 2.0+ = Healthy, scalable
  - 3.0+ = Excellent, increase UA spend
```

### ROAS (Return on Ad Spend)
| Timeframe | Target | Interpretation |
|-----------|--------|---------------|
| D7 ROAS | > 15% | Early signal — use to decide whether to scale |
| D30 ROAS | > 40% | Validation — campaign is on track |
| D90 ROAS | > 80% | Maturity — approaching payback |
| D180 ROAS | > 100% | Profitability — campaign has paid for itself |

### UA Channel Strategy
| Channel | Budget/Month | CPI Target | Notes |
|---------|-------------|------------|-------|
| Apple Search Ads | $1,000 | < $1.50 | Highest intent, best conversion |
| TikTok Ads | $500 | < $2.00 | Best for viral gameplay clips |
| Instagram/FB | $500 | < $2.50 | Broad reach, awareness |
| Cross-promotion | $0 | $0 | Partner with indie devs |

---

## 5. Analytics Event Taxonomy

### Event Schema
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

---

## 6. Analytics Tools

| Tool | Purpose | Best For | Cost |
|------|---------|----------|------|
| **Firebase Analytics** | Event tracking, funnels, audiences | Primary analytics (free tier generous) | Free |
| **Unity Analytics** | In-engine analytics | Backup + Unity-specific events | Free with Unity |
| **GameAnalytics** | Game-specific dashboards | Retention curves, progression | Free (indie) |
| **Amplitude** | Behavioral analytics + A/B testing | Deep funnel analysis, cohorts | Free (10M events/mo) |
| **Mixpanel** | Event analytics + user profiles | Detailed user journeys | Free (20M events/mo) |
| **Sensor Tower** | Market intelligence | Competitor analysis, ASO | Paid |

### Recommended Stack for Pocket Dungeons
- **Primary**: Firebase Analytics (free, robust, integrates with Remote Config)
- **Secondary**: Unity Analytics (built-in, zero setup)
- **A/B Testing**: Firebase Remote Config (or Unity Remote Config)
- **Crash Reporting**: Firebase Crashlytics

---

## 7. A/B Testing Framework

### What to Test
| Variable | Variants | Success Metric |
|----------|----------|---------------|
| Starting gold | 100 / 200 / 500 | D7 retention |
| Power-up choices | 2 / 3 / 4 | Session length, run depth |
| Daily reward multiplier | 1x / 2x / 3x | D1 return rate |
| Starter pack price | $0.99 / $1.99 / $2.99 | Conversion rate |
| Tutorial length | 1 run / 2 runs / 3 runs | D1 retention, session 2 rate |
| Battle Pass price | $3.99 / $4.99 / $6.99 | Attach rate, revenue |

### A/B Test Process
1. **Hypothesis**: "Increasing starting gold to 200 will improve D7 retention by 5%"
2. **Sample size**: Min 1,000 users per variant (use power calculator)
3. **Duration**: Min 7 days (capture weekly patterns)
4. **Analysis**: Statistical significance > 95% before acting
5. **Decision**: Ship winner, document learnings

### Guardrail Metrics
Always monitor these alongside your test metric to catch negative side effects:
- Crash-free rate (don't ship if it drops)
- Session length (don't accidentally reduce engagement)
- Revenue per user (don't cannibalize monetization)

---

## 8. Financial Modeling

### Unit Economics
```
Revenue per User = LTV
Cost per User = CPI + (Server Cost / Users) + (Support Cost / Users)
Profit per User = LTV - Total Cost per User

For sustainability: Profit per User must be > 0
```

### Year 1 Revenue Projection Template
| Assumption | Value |
|-----------|-------|
| Downloads (first 3 months) | 100,000 |
| D30 retention | 8% (8,000 MAU) |
| Conversion rate | 4% (3,200 paying) |
| ARPPU | $15/month |

| Revenue Stream | Monthly | Annual |
|----------------|---------|--------|
| Battle Pass | $8,000 | $96,000 |
| Cosmetic IAP | $4,000 | $48,000 |
| Gem Packs | $3,000 | $36,000 |
| Starter Packs | $2,000 | $24,000 |
| Ads | $3,000 | $36,000 |
| **Gross Revenue** | **$20,000** | **$240,000** |
| Apple commission (15%) | -$2,550 | -$30,600 |
| **Net Revenue** | **$17,450** | **$209,400** |

### Budget Tracking
| Item | Estimated Cost |
|------|---------------|
| Apple Developer Program | $99/year |
| Unity Personal | $0 (free under $200K) |
| Art assets | $2,000-$5,000 |
| Sound/Music | $500-$1,500 |
| Marketing (soft launch) | $1,000-$3,000 |
| TestFlight | $0 |
| **Total** | **$3,600-$9,600** |

---

## 9. Funnel Analysis

### Player Journey Funnel
```
Install
  └── First Launch ─────────── 100%
       └── Complete Tutorial ── 85% (target)
            └── Complete Run 1 ── 70%
                 └── Start Run 2 ── 55%
                      └── Return Day 2 ── 35% (D1 retention)
                           └── First Purchase ── 4% (conversion)
                                └── Return Day 7 ── 15% (D7)
                                     └── Return Day 30 ── 8% (D30)
```

### Identifying Drop-Off Points
- **Install → Tutorial**: ASO/ad mismatch (players expected different game)
- **Tutorial → Run 1**: Onboarding too long or confusing
- **Run 1 → Run 2**: Core loop not fun enough
- **Run 2 → D2 Return**: No compelling reason to come back
- **D2 → D7**: Missing meta progression or daily hooks
- **D7 → D30**: Content exhaustion or monetization friction

---

## 10. OKR Framework for Game Studios

### What Are OKRs?
- **Objective**: Qualitative goal (what you want to achieve)
- **Key Results**: Quantitative measures (how you know you achieved it)
- Typically set quarterly, reviewed monthly

### Example OKRs for Pocket Dungeons

#### Q1: Pre-Launch
**Objective**: Validate that the core gameplay loop is fun and retainable.
| Key Result | Target |
|-----------|--------|
| Internal playtest "fun rating" | > 4/5 |
| Closed beta D1 retention | > 35% |
| Avg session length in beta | > 5 min |
| Crash-free rate in beta | > 99% |

#### Q2: Launch
**Objective**: Successfully launch on App Store and achieve product-market fit.
| Key Result | Target |
|-----------|--------|
| App Store rating | > 4.5 stars |
| D7 retention (first 10K users) | > 15% |
| IAP conversion rate | > 3% |
| 0 P0 bugs in first week | 0 |

#### Q3: Growth
**Objective**: Build sustainable user acquisition and monetization.
| Key Result | Target |
|-----------|--------|
| LTV:CPI ratio | > 1.5 |
| Monthly net revenue | > $15,000 |
| Battle Pass attach rate | > 8% |
| D30 retention | > 8% |

---

## 11. Competitive Intelligence

### Competitor Analysis Framework
| Dimension | Your Game | Competitor 1 | Competitor 2 |
|-----------|----------|-------------|-------------|
| Core loop | | | |
| Session length | | | |
| Monetization model | | | |
| Art style | | | |
| App Store rating | | | |
| Estimated downloads | | | |
| Update frequency | | | |
| Social features | | | |

### Key Competitors to Study
- **Archero** — Pioneered mobile roguelike action
- **Survivor.io / Vampire Survivors** — Mass-market roguelite
- **Slay the Spire** — Roguelike deckbuilder (deep strategy)
- **Dead Cells** — Premium roguelike (combat feel reference)
- **Soul Knight** — Multiplayer dungeon crawler

### Tools for Competitive Research
- **Sensor Tower**: Download estimates, revenue estimates, keyword rankings
- **GameRefinery**: Feature benchmarking, market intelligence
- **AppFollow**: Review analysis, ASO tracking
- **SteamDB / AppMagic**: Broader market data

---

## 12. Dashboards & Reporting

### Daily Dashboard
| Metric | Source |
|--------|--------|
| DAU | Firebase |
| Revenue (today) | App Store Connect |
| D1 retention (yesterday's cohort) | Firebase |
| Crash-free rate | Crashlytics |
| Top death floor | Custom event |

### Weekly Report Template
```markdown
## Week of [Date]

### Headline Metrics
| Metric | This Week | Last Week | Δ |
|--------|-----------|-----------|---|
| DAU | | | |
| Revenue | | | |
| New installs | | | |
| D1 Retention | | | |
| Crash-free | | | |

### Highlights
- [What went well]

### Concerns
- [What needs attention]

### Actions for Next Week
- [Specific action items]
```

---

## References
- The 20 Mobile Game KPIs That Actually Matter (Game Growth Advisor, 2026)
- Unit Economics for F2P Games (Tenjin, 2026)
- Mobile Game UA KPIs 2026 (Ramiz Trtovac)
- Mobile Game CPI Benchmarks 2026 (Game Growth Advisor)
- Retention and LTV Analysis (Playio, 2025)
- OKR vs North Star Metric (IdeaPlan, 2026)
- 10 Best Game Analytics Tools (Bizzware, 2026)
- 9 Best Mobile A/B Testing Tools (Amplitude, 2026)
