# Save System Implementation

## Overview
The enhanced SaveSystem provides persistent JSON-based saving and loading for the game, ensuring that player progress (currency and upgrades) is preserved between sessions.

## Key Features

### 1. Bootstrap Initialization
- **GameBootstrap.cs**: Uses `[RuntimeInitializeOnLoadMethod]` to initialize SaveSystem before any scene loads
- Automatically loads and applies saved data at application start
- Ensures SaveSystem singleton is available throughout the application lifecycle

### 2. Persistent JSON Storage
- **JSON Format**: All data is saved to `{Application.persistentDataPath}/gamedata.json`
- **Cross-platform**: Works on all Unity-supported platforms
- **Human-readable**: JSON format allows for debugging and manual inspection

### 3. Data Structure
```json
{
  "currency": 1500,
  "metaProgression": {
    "upgradeEntries": [
      {"upgradeType": "BaseHealth", "level": 3},
      {"upgradeType": "BaseDamage", "level": 2}
    ],
    "unlockedTanks": ["BasicTank"],
    "totalGamesPlayed": 5,
    "bestSurvivalTime": 120.5
  },
  "settings": {
    "masterVolume": 0.8,
    "sfxVolume": 1.0,
    "musicVolume": 0.6,
    "fullscreen": true,
    "resolution": {"x": 1920, "y": 1080},
    "quality": 2
  },
  "statistics": {
    "totalPlayTime": 300.0,
    "totalEnemiesKilled": 150,
    "totalBossesKilled": 5,
    "totalDeaths": 10,
    "highestWave": 15,
    "highestLevel": 8
  }
}
```

### 4. Automatic Saving
The SaveSystem automatically saves data in multiple scenarios:
- **Application Quit**: `OnApplicationQuit()` 
- **Application Pause**: `OnApplicationPause(true)`
- **Application Focus Loss**: `OnApplicationFocus(false)`
- **Component Destruction**: `OnDestroy()`

### 5. Currency Integration
- **CurrencyManager**: Enhanced with `SetTotalCurrency()` method for loading saved values
- **Backwards Compatibility**: Still uses PlayerPrefs as fallback for existing save data
- **Real-time Updates**: Currency changes trigger UI updates immediately

### 6. Permanent Upgrades Integration
- **Dictionary Serialization**: Custom serialization for upgrade data using `ISerializationCallbackReceiver`
- **PlayerPrefs Bridge**: Loads from JSON but applies to PlayerPrefs for compatibility with existing upgrade system
- **Dynamic Loading**: Supports all PermanentUpgradeType enum values

## Usage Examples

### Basic Usage
```csharp
// Get SaveSystem instance (automatically initializes)
var saveSystem = SaveSystem.Instance;

// Save current game state
saveSystem.SaveGameData();

// Load and apply saved data
var loadedData = saveSystem.LoadGameData();
saveSystem.ApplyLoadedData(loadedData);
```

### Testing
Use the `SaveSystemTest` component for validation:
```csharp
// Attach to any GameObject and use context menu options:
// - "Test Save System": Comprehensive save/load test
// - "Apply Loaded Data": Test data application
// - "Delete Save Data": Clear save file
```

## File Locations

### Scripts
- `SaveSystem.cs`: Core save/load functionality
- `GameBootstrap.cs`: Runtime initialization
- `SaveSystemTest.cs`: Testing utilities

### Save Files
- **Windows**: `%USERPROFILE%\AppData\LocalLow\{CompanyName}\{ProductName}\gamedata.json`
- **macOS**: `~/Library/Application Support/{CompanyName}/{ProductName}/gamedata.json`
- **Linux**: `~/.config/unity3d/{CompanyName}/{ProductName}/gamedata.json`

## Data Flow

### On Application Start
1. `GameBootstrap.InitializeGame()` called before scene load
2. SaveSystem singleton created and initialized
3. `LoadGameData()` reads JSON file
4. `ApplyLoadedData()` applies currency, upgrades, settings, and statistics
5. UI systems receive updated values through events

### During Gameplay
1. Player actions modify currency and upgrade levels
2. Changes are saved to PlayerPrefs immediately (existing system)
3. SaveSystem periodically saves all data to JSON

### On Application Quit
1. `OnApplicationQuit()` triggered
2. Current state collected from all systems
3. Final save to JSON file
4. Application closes with data preserved

## Error Handling
- **Missing Save File**: Creates new default data structure
- **Corrupted JSON**: Falls back to default values and logs error
- **File System Errors**: Logs errors but continues execution
- **Component Missing**: Handles null references gracefully

## Backwards Compatibility
- Existing PlayerPrefs data is automatically migrated to JSON
- Current upgrade button system continues to work unchanged
- Gradual transition from PlayerPrefs to JSON-first approach

## Performance Considerations
- JSON serialization is performed on main thread but files are small
- Dictionary serialization uses efficient list conversion
- Automatic saves are triggered only on significant events
- No performance impact during normal gameplay

## Testing Checklist
- [ ] SaveSystem initializes automatically on application start
- [ ] Currency persists between application restarts
- [ ] Permanent upgrades persist between sessions
- [ ] Settings and statistics are preserved
- [ ] JSON file is created in correct location
- [ ] Error handling works for corrupted/missing files
- [ ] Performance remains smooth during save operations
- [ ] Cross-platform compatibility verified
