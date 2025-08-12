# Game Scene Core Objects Setup

This document outlines the core objects that have been set up in the Game scene according to Step 8 requirements.

## Core Objects Overview

### 1. Main Camera (URP 2D Renderer)
- **GameObject**: Main Camera
- **Components**: 
  - Camera (Orthographic, dark gray background)
  - Universal Additional Camera Data (URP 2D Renderer)
  - Audio Listener
- **Configuration**: 
  - Background color: Dark gray (0.1, 0.1, 0.1)
  - Orthographic projection
  - Uses URP 2D renderer for optimal 2D rendering

### 2. EventSystem
- **GameObject**: EventSystem
- **Components**:
  - EventSystem
  - StandaloneInputModule
- **Purpose**: Handles UI input events and navigation

### 3. Systems GameObject
- **GameObject**: Systems
- **Components**:
  - SessionManager - Manages game session flow, timing, and game over states
  - EnemySpawner - Handles enemy spawning logic and wave progression
  - XPSystem - Manages experience points and leveling
  - CurrencyManager - Handles in-game currency operations
  - UpgradeManager - Manages upgrade selections and applications
  - UIManager - Central UI management and event handling
  - SaveSystem - Handles game data persistence
  - DontDestroyOnLoadMarker - Keeps the Systems object persistent across scenes
- **Purpose**: Central hub for all game systems, marked as DontDestroyOnLoad

### 4. Canvas Prefab (GameCanvas)
- **Prefab**: Assets/Prefabs/UI/GameCanvas.prefab
- **Components**:
  - Canvas (Screen Space - Camera mode)
  - Canvas Scaler (UI Scale Mode: Scale with Screen Size)
  - Graphic Raycaster
  - UIManager script attached
- **Configuration**:
  - Render Mode: Screen Space - Camera
  - World Camera: References Main Camera
  - Reference Resolution: 1920x1080

### 5. Lighting (URP 2D Global Light)
- **GameObject**: Global Light 2D
- **Components**:
  - Light2D (Global type)
- **Configuration**:
  - Light Type: Global
  - Intensity: 1.0
  - Color: White
  - Provides basic 2D lighting for the scene

## URP Assets Configuration

### Universal Render Pipeline Assets
- **Location**: Assets/Settings/
- **Files**:
  - `UniversalRP-HighQuality.asset` - Main URP pipeline asset
  - `Universal2DRenderer.asset` - 2D renderer data

### 2D Renderer Features
- Optimized for 2D sprites and lighting
- Supports blend styles for different lighting effects
- Configured for pixel-perfect rendering

## Scene Setup Automation

### Editor Tool
- **Script**: Assets/Editor/SceneSetupEditor.cs
- **Menu**: Tankalore > Setup Game Scene
- **Features**:
  - Automatically creates all required GameObjects
  - Configures URP settings
  - Sets up component references
  - Handles duplicate prevention
  - Assigns URP assets to project settings

### Usage Instructions
1. Open the Game scene in Unity
2. Go to **Tankalore > Setup Game Scene** in the menu bar
3. The script will automatically:
   - Set up the main camera with URP 2D renderer
   - Create EventSystem if missing
   - Create Systems GameObject with all required components
   - Instantiate the GameCanvas prefab
   - Create Global Light 2D
   - Configure URP pipeline assets

## System Integration Notes

### DontDestroyOnLoad Systems
The Systems GameObject is marked with `DontDestroyOnLoadMarker` to persist across scene loads. This ensures:
- Session data is maintained
- Currency and progression persist
- Save system remains active
- UI manager continues functioning

### Cross-System Communication
All systems communicate through:
- Direct component references (FindObjectOfType)
- Event-driven architecture using C# events
- Centralized UIManager for UI updates

### Script References
All system scripts are properly organized in the following structure:
```
Assets/Scripts/
├── Systems/
│   ├── GameFlow/SessionManager.cs
│   ├── Spawning/EnemySpawner.cs
│   ├── Upgrades/XPSystem.cs
│   ├── Currency/CurrencyManager.cs
│   ├── Upgrades/UpgradeManager.cs
│   ├── Persistence/SaveSystem.cs
│   └── DontDestroyOnLoadMarker.cs
└── UI/UIManager.cs
```

## Testing the Setup

To verify the setup is working correctly:
1. Play the Game scene
2. Check that all Systems components are active
3. Verify UI rendering appears correctly
4. Confirm lighting is applied to 2D sprites
5. Test scene transitions maintain system persistence
