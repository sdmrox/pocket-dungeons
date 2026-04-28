# Pocket Dungeons

**An addictive roguelike dungeon crawler for the Apple App Store.**

Every run is different. Every death teaches you something. "Just one more run" becomes your mantra.

---

## Game Overview

| Detail | Description |
|--------|-------------|
| **Genre** | Action Roguelike / Dungeon Crawler |
| **Platform** | iOS (iPhone + iPad), expandable to Android |
| **Engine** | Unity 6 (C#) |
| **Session Length** | 2-3 minutes per run |
| **Target Audience** | Casual-core gamers (18-35) |
| **Art Style** | Stylized pixel art with modern lighting & particle effects |
| **Monetization** | Free-to-play, Battle Pass + cosmetic IAP (no pay-to-win) |

## Core Gameplay Loop

```
Enter Dungeon → Explore Rooms → Fight Enemies → Collect Loot → Choose Power-ups → Push Deeper → Die → Upgrade → One More Run
```

- **Swipe to move**, **tap to attack**, **swipe to dodge**
- Procedurally generated dungeons — no two runs are the same
- Pick 1 of 3 power-ups per floor — build unique synergies each run
- Die and keep gold/XP — every run progresses your heroes permanently

## Project Structure

```
pocket-dungeons/
├── README.md                              # This file
├── Game_Development_Master_Plan.md        # Complete build plan, tech stack, architecture, timeline
├── docs/
│   ├── 01_Game_Design_Document.md         # Full GDD with mechanics, systems, content
│   ├── 02_Technical_Architecture.md       # Architecture, ECS, PCG algorithms, state machines
│   ├── 03_Art_Style_Guide.md              # Visual direction, pixel art specs, UI guidelines
│   ├── 04_Monetization_Strategy.md        # Revenue model, IAP design, Apple compliance
│   ├── 05_Marketing_Launch_Plan.md        # ASO, social media, beta testing, launch strategy
│   └── 06_Development_Roadmap.md          # 26-week phased plan with milestones
└── .gitignore
```

## Development Roadmap

| Phase | Weeks | Milestone |
|-------|-------|-----------|
| 0 - Foundation | 1-2 | Unity project setup, CI/CD, folder structure |
| 1 - Vertical Slice | 3-6 | Playable core loop (enter, fight, die, restart) |
| 2 - Depth & Feel | 7-10 | 3 heroes, 8+ enemies, juice pass, music |
| 3 - Meta & Retention | 11-14 | Upgrades, collections, daily challenges, Battle Pass |
| 4 - Polish & Monetization | 15-18 | Final art, IAP, analytics, localization |
| 5 - Social & Live-Ops | 19-22 | Game Center, cloud save, seasonal events |
| 6 - Launch | 23-26 | Beta test, balance, App Store submission |

## Tech Stack

- **Engine**: Unity 6 LTS with Universal Render Pipeline (URP)
- **Language**: C# with DOTS/ECS for performance-critical systems
- **Procedural Generation**: Binary Space Partitioning + Wave Function Collapse
- **Apple APIs**: Metal 4, GameKit, StoreKit 2, Core Haptics, CloudKit
- **Analytics**: Firebase + Unity Analytics
- **CI/CD**: Unity Cloud Build + Xcode Cloud

## Psychology-Driven Design

Built on 7 addiction science principles:

1. **Variable Ratio Reinforcement** — Random loot, procedural dungeons
2. **Loss Aversion** — Streaks, limited-time events
3. **Zeigarnik Effect** — Collections at 94%, progress bars
4. **Social Comparison** — Leaderboards, rare cosmetics
5. **Flow State** — 2-3 min runs, adaptive difficulty
6. **Sunk Cost** — Deep progression trees
7. **Novelty Seeking** — Procedural generation, seasonal content

## License

All rights reserved. This project and its documentation are proprietary.

---

*Built with passion for creating the most addictive mobile gaming experience on the App Store.*
