# Pocket Dungeons — Monetization Strategy

## 1. Philosophy

### Core Principles
1. **No pay-to-win**: Every paid item is cosmetic or convenience-only. Free players can reach any floor, unlock any hero, and compete on leaderboards.
2. **Respect player time**: No mandatory ads. No energy gates on core gameplay. If a player wants to play 50 runs in a row, they can.
3. **Fair value exchange**: Every purchase should feel worth it. Players should think "that was a good deal" not "I had to buy that."
4. **Whales subsidize dolphins and minnows**: The top 5% of spenders fund the free experience for everyone. Design for this reality.

---

## 2. Revenue Streams

### 2.1 Battle Pass ($4.99/season — 30 days)

The **primary monetization mechanic**. Proven to drive both revenue and retention.

**Structure**:
- 30 levels (1 per day of play, with catch-up XP for missed days)
- **Free track**: Gold, XP boosters, common cosmetics
- **Premium track**: Exclusive hero skin, rare cosmetics, gems, bonus gold
- **Bonus levels (31-40)**: Extra rewards for dedicated players

**Pricing**:
- Standard: $4.99
- Bundle (Battle Pass + 500 gems): $7.99
- Season Pass (3 seasons pre-paid): $11.99

**Why it works**:
- Loss aversion: "I paid for this, I should play to get my rewards"
- Sunk cost: "I'm level 22 of 30, I can't stop now"
- FOMO: Seasonal exclusive items never return
- Daily engagement: "I need to play today to stay on track"

### 2.2 Cosmetic In-App Purchases

| Item | Price | Type |
|------|-------|------|
| Hero Skin (Rare) | $1.99 | Non-consumable |
| Hero Skin (Epic) | $2.99 | Non-consumable |
| Hero Skin (Legendary) | $4.99 | Non-consumable |
| Weapon Skin Pack | $1.99 | Non-consumable |
| Death Effect | $0.99 | Non-consumable |
| Player Title | $0.99 | Non-consumable |
| Dungeon Theme | $2.99 | Non-consumable |
| Emote Pack (5 emotes) | $1.99 | Non-consumable |

### 2.3 Premium Currency (Gems)

Gems are the **bridge currency** — earned slowly for free, purchasable for impatient players.

| Pack | Gems | Price | Bonus |
|------|------|-------|-------|
| Handful | 100 | $0.99 | — |
| Pouch | 550 | $4.99 | 10% bonus |
| Chest | 1,200 | $9.99 | 20% bonus |
| Vault | 2,500 | $19.99 | 25% bonus |
| Treasury | 6,500 | $49.99 | 30% bonus |
| Dragon Hoard | 14,000 | $99.99 | 40% bonus |

**Gem sinks** (what gems buy):
- Cosmetics not available via direct purchase
- Speed up upgrade timers (convenience, not power)
- Extra daily dungeon attempts (3 free, gems for more)
- Revive during a run (1 per run, costs 50 gems)

### 2.4 Starter Pack ($2.99 — One-Time)

Shown exactly once at player level 5 (proven conversion point).

**Contains**:
- Exclusive "Founder" hero skin
- 300 gems
- 5,000 gold
- 3-day XP booster

**Why it works**: Massive perceived value (worth $8+ if purchased separately). Low price point breaks the "I never spend money on games" barrier. Once a player makes their first purchase, they're 10x more likely to spend again.

### 2.5 Advertising (Opt-In Only)

| Ad Type | Placement | Reward | Frequency Cap |
|---------|-----------|--------|---------------|
| Rewarded Video (30s) | After death: "Double your gold?" | 2x gold for that run | 3 per day |
| Rewarded Video (30s) | Free chest cooldown: "Open now?" | Instant chest open | 2 per day |
| Rewarded Video (30s) | Daily bonus: "Triple today's login reward?" | 3x daily reward | 1 per day |
| Banner Ad | Main menu bottom (small, non-intrusive) | — | Always (removable via IAP) |

**Remove Ads**: $4.99 (non-consumable) — removes all banner/interstitial ads permanently. Rewarded ads remain available since players choose to watch them.

---

## 3. Apple App Store Compliance

### 3.1 StoreKit 2 Requirements
- All IAP processed through StoreKit 2 (Apple's latest API)
- Apple commission: 15% (Small Business Program, under $1M revenue) or 30%
- Subscription auto-renewal clearly disclosed before purchase
- "Restore Purchases" button visible in settings
- Receipt validation server-side (prevent fraud)

### 3.2 App Store Review Guidelines Compliance

| Guideline | Requirement | Our Compliance |
|-----------|-------------|---------------|
| 3.1.1 | IAP for digital goods must use Apple IAP | All digital purchases via StoreKit 2 |
| 3.1.2(a) | Subscriptions must provide ongoing value | Battle Pass delivers daily content for 30 days |
| 3.2.2 | No unacceptable business model | No real-money loot boxes; gems buy specific items |
| 5.6 | No manipulation of reviews | No review prompts during frustration moments |
| 1.3 | Kids category compliance | Not targeting kids; 12+ rating; no data collection from minors |
| 5.1.1 | Privacy nutrition labels | Accurate disclosure of analytics data collected |

### 3.3 Loot/Gacha Compliance
- **No real-money randomized purchases**: Players never spend real money on random outcomes
- Gem-purchased items are **always specific** (buy exactly what you see)
- In-game loot (dropped by enemies) is random but costs no real money
- If we add cosmetic gacha later: **display odds prominently** (required in Japan, South Korea, China, Belgium)

---

## 4. Monetization Timing & Triggers

### 4.1 When to Show Offers

| Trigger | Offer | Psychology |
|---------|-------|-----------|
| Player reaches level 5 | Starter Pack | First success → "reward yourself" |
| After a "best floor" run | Gem pack | High emotion → impulse spending |
| After dying on a boss floor | Revive (50 gems) | Loss aversion → "so close!" |
| Day 3 of play | Battle Pass preview | Invested enough to see value |
| End of Season | "Last chance" Battle Pass | FOMO urgency |
| New season launches | New Battle Pass + themed skins | Novelty excitement |
| Player has 0 gems | Small gem pack ($0.99) | Low barrier, first purchase conversion |

### 4.2 When NOT to Show Offers
- During gameplay (never interrupt a run)
- After a frustrating death (player will associate spending with frustration)
- More than once per session (no nagging)
- Before level 3 (let player enjoy the game first)

---

## 5. Revenue Projections

### 5.1 Assumptions (Year 1)
- 100,000 downloads in first 3 months (organic + modest UA spend)
- D30 retention: 8% (8,000 monthly active players)
- Conversion rate: 4% (3,200 paying players year 1)
- ARPPU: $15/month average

### 5.2 Year 1 Projection

| Revenue Stream | Monthly | Annual |
|----------------|---------|--------|
| Battle Pass | $8,000 | $96,000 |
| Cosmetic IAP | $4,000 | $48,000 |
| Gem Packs | $3,000 | $36,000 |
| Starter Packs | $2,000 | $24,000 |
| Ads (rewarded + banner) | $3,000 | $36,000 |
| **Total (before Apple cut)** | **$20,000** | **$240,000** |
| Apple commission (15%) | -$2,550 | -$30,600 |
| **Net Revenue** | **$17,450** | **$209,400** |

*Note: These are conservative estimates. Top-performing roguelikes (Archero, Survivor.io) generate $1M+/month. Actual results depend heavily on UA spend and retention optimization.*

---

## 6. Key Metrics to Track

| Metric | Definition | Target |
|--------|-----------|--------|
| **ARPU** | Revenue / Total Users | > $0.50/month |
| **ARPPU** | Revenue / Paying Users | > $15/month |
| **Conversion Rate** | Paying Users / Total Users | > 4% |
| **Battle Pass Attach Rate** | BP Purchasers / MAU | > 8% |
| **First Purchase Timing** | Average days to first purchase | < 5 days |
| **LTV (Lifetime Value)** | Total revenue per user over lifetime | > $2.00 |
| **CPI (Cost Per Install)** | UA spend per acquired user | < $1.50 |
| **LTV:CPI Ratio** | Must be > 1 for sustainable growth | > 1.5 |

---

*Monetization is a feature, not an afterthought. Design it into the game from day 1, test it rigorously, and always ask: "Would I feel good paying for this?"*
