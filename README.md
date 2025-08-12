# Tankalore - Rogue-like Tank Arena Game

A Unity 2022 LTS 2D URP rogue-like tank arena game with modular architecture designed for easy expansion.

## Project Structure

```
Assets/
├── Art/                    # Visual assets
│   ├── Sprites/           # Tank, enemy, and effect sprites
│   ├── UI/                # UI graphics and icons
│   └── Effects/           # Visual effect assets
├── Audio/                 # Audio assets
│   ├── SFX/              # Sound effects
│   └── Music/            # Background music
├── Prefabs/              # Game object prefabs
│   ├── Player/           # Player tank prefabs
│   ├── Enemies/          # Enemy prefabs
│   ├── Bosses/           # Boss prefabs
│   └── UI/               # UI prefabs
├── Scenes/               # Unity scenes
├── Scripts/              # All C# scripts
│   ├── Player/           # Player-related scripts
│   ├── Enemies/          # Enemy AI and behavior
│   ├── Bosses/           # Boss-specific scripts
│   ├── Systems/          # Core game systems
│   └── UI/               # User interface scripts
├── ScriptableObjects/    # Data-driven configurations
│   ├── Tanks/           # Tank stat configurations
│   ├── Enemies/         # Enemy configurations
│   ├── Upgrades/        # Upgrade definitions
│   └── GameModes/       # Game mode settings
├── Settings/            # Project settings and configurations
└── Tests/               # Unit and integration tests
```

## Core Features

### Player System
- **PlayerController**: Handles movement, aiming, and shooting
- **TankStats**: ScriptableObject-based tank configuration
- Modular upgrade system affecting tank stats

### Enemy System
- **EnemyController**: AI-driven enemy behavior with multiple states
- **BossController**: Multi-phase boss encounters
- **EnemyData**: ScriptableObject configurations for easy enemy creation

### Progression Systems
- **XPSystem**: Experience tracking and level progression
- **UpgradeManager**: Random upgrade selection with rarity weighting
- **CurrencyManager**: Session and persistent currency management

### Game Flow
- **SessionManager**: 15-minute timer and game session management
- **EnemySpawner**: Wave-based enemy spawning with scaling difficulty
- **SaveSystem**: JSON-based persistent data storage

### UI System
- **UIManager**: Centralized UI management
- **UpgradeButton**: Interactive upgrade selection interface

## Setup Instructions

### 1. Unity Project Setup
1. Create a new Unity 2022 LTS project
2. Set up 2D URP (Universal Render Pipeline)
3. Copy the Assets folder structure and scripts to your project

### 2. Required Unity Packages
Install these packages via Package Manager:
- Universal RP (2D renderer)
- TextMeshPro
- Input System (optional, for enhanced input)

### 3. Project Settings
- **Player Settings**: Set appropriate resolution and platform settings
- **Physics2D**: Configure collision layers for Player, Enemy, Bullet, and Wall
- **Tags**: Create tags for "Player", "Enemy", "Wall", "Obstacle"
- **Layers**: Set up layers for collision detection

### 4. Scene Setup
- Create three scenes: MainMenu, Game, and UpgradeShop
- Set up Game scene with:
  - Main Camera with URP 2D renderer
  - Player tank prefab
  - UI Canvas with UIManager
  - Game systems (SessionManager, XPSystem, etc.)

### 5. Asset Integration
- Import Kenney.nl asset packs for placeholder sprites
- Create bullet prefabs with Bullet script
- Set up enemy and boss prefabs with respective controllers

### 6. ScriptableObject Creation
Create the following ScriptableObjects in their respective folders:
- TankStats for different tank types
- EnemyData for various enemy types
- UpgradeData for all available upgrades

## Development Notes

### Architecture Principles
- **Modular Design**: Each system is self-contained and communicates via events
- **Data-Driven**: Game content configured through ScriptableObjects
- **Scalable**: Easy to add new tanks, enemies, upgrades, and features

### Key Design Patterns
- **Observer Pattern**: Extensive use of C# events for system communication
- **Strategy Pattern**: Different enemy behaviors and upgrade effects
- **Singleton Pattern**: SaveSystem for persistent data management

### Performance Considerations
- Object pooling recommended for bullets and enemies in production
- Event subscriptions properly managed to prevent memory leaks
- ScriptableObjects used to reduce runtime allocations

## Future Expansion Ideas

### Content
- Additional tank types with unique abilities
- More enemy varieties and boss types
- Environmental hazards and interactive elements
- Power-ups and temporary abilities

### Systems
- Meta-progression with permanent upgrades
- Multiple game modes (survival, waves, boss rush)
- Leaderboards and statistics tracking
- Achievement system

### Polish
- Particle effects and screen shake
- Advanced audio mixing
- Localization support
- Mobile platform optimization

## Asset Credits
- Placeholder graphics: Kenney.nl (CC0 License)
- Built with Unity 2022 LTS
- Uses Universal Render Pipeline (URP)

