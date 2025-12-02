# Urge To Merge

![Urge To Merge Banner](docs/images/urge_to_merge_banner.png)

A Unity 2D merge puzzle game featuring strategic number combination mechanics with mystery effects and bonus systems.

[![Play on itch.io](https://img.shields.io/badge/itch.io-Play%20Now-FA5C5C?style=for-the-badge&logo=itch.io&logoColor=white)](https://adielrozenfeld.itch.io/urge-to-merge)

![Gameplay Demo](docs/images/gameplay_gif.gif)

## 🎮 Game Overview

Urge To Merge is a merge puzzle game where players combine numbered tiles using different mathematical operations to reach target goals.

### How to Play

**Objective**: Merge tiles strategically to create specific target numbers and complete goals. Each time you reach a goal, a new, harder target is set for continuous progression.

**Gameplay Mechanics**:
- **Merge Tiles**: Drag and drop numbered tiles onto each other to combine them
- **Mathematical Operations**: Choose between addition or subtraction merge strategies
- **Progressive Goals**: Reach target numbers to complete goals - difficulty increases with each success
- **Energy Management**: Actions consume energy that regenerates over time
- **Mystery Tiles**: Special tiles appear that trigger random effects when activated
- **Status Effects**: Tiles can become frozen (locked), burning (countdown to deletion), or carry other effects

**Strategic Elements**:
- Plan your merges carefully - order matters in subtraction
- Use mystery tiles tactically to change the board state
- Manage energy efficiently for optimal moves
- Work around frozen or burning tiles
- Activate bonus power-ups at the right moment

The game features a dynamic board system, fortune wheel bonuses, and various tile status effects that add strategic depth to the gameplay.

![Fortune Wheel](docs/images/fortune-wheel.gif)

## ✨ Key Features

- **Strategic Merge System**: Combine tiles using addition and subtraction strategies
- **Mystery Tile Effects**: Special tiles that trigger 7 unique gameplay effects when activated:
  - Random tile deletion
  - Spawn new tiles
  - Mystery/transform tiles
  - Fortune wheel triggers
  - Tile rerolling
  - Freeze effects
  - Burn effects
- **Fortune Wheel**: Spin-based reward system with configurable reward data
- **Bonus System**: Collect and activate power-ups including elemental chaos effects
- **Status Effects**: Tiles can be frozen, burning, or have other active effects
- **Energy System**: Resource management for actions
- **Save System**: Complete progress persistence using JSON + PlayerPrefs
- **Sound Manager**: Comprehensive audio system for effects and music

![Mystery Effects](docs/images/mystery.gif)

## 🏗️ Architecture

### Design Patterns

- **Singleton Pattern**: Consistent `SingletonInstance<T>` base class for all managers
- **Strategy Pattern**: Pluggable merge strategies (Addition, Subtraction)
- **Observer Pattern**: Event-driven communication via `GameEvents`
- **Object Pool**: Efficient tile instance management

### Core Systems

#### Board System
- `BoardInit`: Initializes game board and tile slots
- `NumberSlotGenerator`: Manages slot creation and tile spawning
- `SlotTile`: Represents individual board positions

#### Gameplay
- `NumberManager`: Central game logic coordinator
- `GoalManager`: Tracks and validates win conditions
- **Merge Strategies**: `IMergeStrategy` interface with Addition/Subtraction implementations

#### Mystery Tile Effects System
- `MysteryEffectRunner`: Centralized effect execution manager
- `IMysteryEffect`: Interface for all 7 mystery tile effects
- Type-safe effect identification using `MysteryEffectType` enum

#### Tile Architecture
Component-based design:
- `NumberTile`: Core tile data and logic
- `TileAnimator`: Handles all tile animations
- `TileDragDetector`: Drag-and-drop interaction
- `TileMysteryEffect`: Mystery tile behavior
- `TileStatusEffects`: Manages frozen, burning, and other states
- `SortingOrder`: Visual layering control

#### Systems
- **Energy**: `EnergySystem` for action resource management
- **Bonus**: `BonusSystem` for power-up inventory and activation
- **Save**: `SaveSystem` with comprehensive state persistence
- **Sound**: `SoundManager` for audio playback

## 📁 Project Structure

```
Assets/Scripts/
├── Board/              # Board initialization and slot management
├── Core/               # Shared utilities (events, pooling, singletons)
├── Effects/            # Visual effect controllers
├── Gameplay/           # Core game logic and merge strategies
│   └── Merge/          # Strategy pattern implementations
├── Mystery/            # 7 mystery tile effect implementations + runner
├── Systems/            # Game systems (energy, fortune, bonus, save, sound)
│   ├── Energy/
│   ├── Fortune Wheel/
│   ├── Rewards/
│   ├── Save/
│   └── Sound/
├── Tiles/              # Tile components and behaviors
└── UI/                 # UI managers and menus
```

---

For detailed documentation on specific components, see:
- [Mystery Tile Effects System](docs/MYSTERY_EFFECTS.md) *(coming soon)*
- [Tile Architecture](docs/TILE_SYSTEM.md) *(coming soon)*
- [Save System](docs/SAVE_SYSTEM.md) *(coming soon)*
- [Merge Strategies](docs/MERGE_STRATEGIES.md) *(coming soon)*
