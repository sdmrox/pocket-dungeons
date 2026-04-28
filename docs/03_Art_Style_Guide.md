# Pocket Dungeons — Art Style Guide

## 1. Visual Identity

### 1.1 Art Direction
**Style**: Modern pixel art — the nostalgia of classic 16-bit games with contemporary lighting, particles, and post-processing.

**References**:
- *Dead Cells* — fluid pixel art animation with dynamic lighting
- *Hyper Light Drifter* — rich color palettes and atmospheric pixel art
- *Enter the Gungeon* — clean, readable enemy designs with personality

**Principle**: Every element must be **readable at a glance** on a small phone screen. Silhouette-first design — if you can't tell what something is from its outline alone, redesign it.

### 1.2 Resolution & Scale

| Parameter | Value |
|-----------|-------|
| Base pixel size | 16x16 tiles |
| Character sprites | 16x24 pixels (2 tiles tall) |
| Boss sprites | 32x32 to 64x64 pixels |
| Target render resolution | 480x854 (scaled up to device) |
| Pixel-perfect rendering | Yes (URP Pixel Perfect Camera) |
| Aspect ratio | 16:9 portrait (primary), safe areas for notch/island |

### 1.3 Color Palette

Each biome has a **5-color primary palette** + neutrals:

**Stone Dungeon (Default)**:
```
Background:  #1a1a2e (deep navy)
Walls:       #4a4a6a (stone grey-purple)
Floor:       #2d2d44 (dark slate)
Accent:      #e07020 (torch orange)
Highlight:   #ffcc44 (gold)
```

**Cursed Catacombs**:
```
Background:  #0d0d1a (void black)
Walls:       #3d2d4a (dark purple)
Floor:       #1a1425 (deep purple)
Accent:      #8b44cc (magic purple)
Highlight:   #cc44ff (neon purple)
```

**Crystal Caverns**:
```
Background:  #0a1628 (deep blue)
Walls:       #2a4a6a (teal stone)
Floor:       #1a3040 (dark teal)
Accent:      #44ccee (crystal cyan)
Highlight:   #88eeff (bright cyan)
```

### 1.4 Rarity Colors
```
Common:     #ffffff (white)
Uncommon:   #44cc44 (green)
Rare:       #4488ff (blue)
Epic:       #cc44ff (purple)
Legendary:  #ffaa00 (gold) + glow particle effect
```

---

## 2. Character Design

### 2.1 Player Heroes

**Design Principles**:
- Large head (3:1 head-to-body ratio) — expressive, cute, recognizable
- Distinct silhouette per hero — identifiable without color
- 4-frame idle animation, 4-frame walk, 3-frame attack, 2-frame hurt, 4-frame death
- Each hero has a **signature color** for easy identification

| Hero | Signature Color | Visual Theme |
|------|----------------|-------------|
| Kael (Warrior) | Red | Heavy armor, large sword, shield on back |
| Lyra (Archer) | Green | Light leather, hooded cloak, quiver |
| Zara (Mage) | Blue | Flowing robes, pointed hat, glowing staff |
| Rex (Berserker) | Orange | Shirtless, war paint, dual axes |
| Luna (Assassin) | Purple | Dark cloth, face mask, twin daggers |
| Orin (Paladin) | Gold | Full plate, holy symbol, hammer |

### 2.2 Enemies

**Design Principles**:
- **Readable behavior**: Visual design hints at enemy behavior
  - Ranged enemies have visible weapons (bow, staff)
  - Fast enemies are small and angular
  - Tanky enemies are large and round
  - Dangerous enemies have warm colors (red, orange)
- Each enemy has an **idle animation** that hints at their attack pattern
- Death animation: enemies explode into particles matching their color

### 2.3 Bosses

- 4x to 8x larger than standard enemies
- Multi-phase: visual changes at 50% and 25% HP
- Unique idle animations that telegraph attack patterns
- Entry animation when player enters boss room (dramatic camera zoom)

---

## 3. Environment Art

### 3.1 Tilemap System

Each biome requires:
- Floor tiles (8+ variants for visual variety)
- Wall tiles (top, side, corners, inner corners = 16+ pieces)
- Door tiles (open, closed, locked)
- Decorative objects (torches, barrels, crates, bones, crystals, etc.)
- Transition tiles (between biomes for visual blending)

### 3.2 Lighting

Using URP 2D Lighting:
- **Global light**: Dim ambient (biome-specific color)
- **Point lights**: Torches, crystals, magical effects
- **Player light**: Subtle radial glow around player (always visible)
- **Enemy lights**: Glow effects for magical enemies
- **Shadow casters**: All walls cast 2D shadows

### 3.3 Particles & VFX

| Effect | Usage | Particle Count |
|--------|-------|---------------|
| Torch flame | Environmental | 10 particles |
| Footstep dust | Player movement | 3 particles |
| Hit impact | Combat | 8 particles |
| Enemy death | Combat | 15 particles |
| Loot sparkle | Loot drops | 5 particles |
| Level-up burst | Progression | 30 particles |
| Healing glow | Power-ups | 8 particles |
| Boss aura | Boss rooms | 20 particles |
| Dodge trail | Player dodge | 5 ghost images |

---

## 4. UI Design

### 4.1 HUD (In-Game)

```
┌─────────────────────────────┐
│ [HP BAR]  Floor 12  [GOLD]  │  ← Top bar (minimal)
│                             │
│                             │
│                             │
│       GAMEPLAY AREA         │
│                             │
│                             │
│                             │
│ [JOYSTICK]    [SKILL1][SKILL2]│  ← Bottom controls
│              [DODGE]         │
└─────────────────────────────┘
```

**Principles**:
- Maximum gameplay visibility (HUD covers < 15% of screen)
- Semi-transparent controls
- HP bar uses color gradient (green → yellow → red)
- Damage numbers float up and fade (max 3 visible at once)
- Mini-map in corner (togglable)

### 4.2 Menu UI

- **Font**: Pixel-style font for headings, clean sans-serif for body text
- **Buttons**: Rounded rectangles with subtle gradient and press animation
- **Transitions**: Slide animations between screens (200ms)
- **Cards**: Hero/item cards with rarity-colored borders and glow
- **Scrolling**: Smooth momentum scrolling for lists
- **Popups**: Center-screen with dimmed background overlay

### 4.3 Iconography

- 16x16 pixel icons for inventory items
- 32x32 pixel icons for hero portraits
- 24x24 pixel icons for UI buttons
- All icons have 1px outline for readability on any background

---

## 5. Animation Guidelines

### 5.1 Frame Counts

| Animation | Frames | Loop | FPS |
|-----------|--------|------|-----|
| Idle | 4 | Yes | 4 |
| Walk | 4 | Yes | 8 |
| Attack | 3 | No | 12 |
| Hurt | 2 | No | 8 |
| Death | 4 | No | 8 |
| Dodge | 3 | No | 12 |
| Skill cast | 4 | No | 10 |

### 5.2 Animation Principles
- **Anticipation**: 1 frame wind-up before attack
- **Squash & stretch**: Exaggerated on jumps and impacts
- **Follow-through**: Weapon swing continues past hit point
- **Smear frames**: Use on fast attacks for speed impression

---

## 6. Asset Production Pipeline

```
1. Concept sketch (pencil/digital)
2. Pixel art sprite (Aseprite)
3. Animation frames (Aseprite)
4. Export sprite sheet (PNG, power-of-2 dimensions)
5. Import to Unity (Sprite Atlas for batching)
6. Configure in Unity (pivot points, colliders, animation clips)
7. Integration test (in-game visual check)
8. Optimization pass (texture compression, atlas packing)
```

### 6.1 Tools
- **Aseprite**: Pixel art creation and animation
- **TexturePacker**: Sprite atlas optimization
- **Unity Sprite Editor**: Slicing, pivot, physics shape
- **Spine** (optional): For complex character animation (boss rigs)

---

*Art direction should prioritize clarity and readability above visual complexity. The game is played on small screens, often in motion — every pixel must communicate.*
