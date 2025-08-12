# Step 12: Wire EnemySpawner & SessionManager Configuration Guide

## Overview
This guide explains how to configure the EnemySpawner and SessionManager components in the Game scene to complete Step 12 of the project.

## Changes Made

### 1. EnemySpawner Script Updates
The EnemySpawner script has been enhanced with:
- **EnemyData List**: New field to hold ScriptableObject references for enemy configurations
- **Spawn Curve**: AnimationCurve for difficulty scaling over time
- **Difficulty Scaling**: Automatic scaling based on session time
- **Event Integration**: Listens to SessionManager OnTimeChanged events

### 2. SessionManager Configuration
The SessionManager is already configured with:
- **Duration**: Set to 900 seconds (15 minutes) ✓
- **Timer Events**: OnTimeChanged event already connected to UIManager ✓
- **Game Over Handling**: Complete game flow management ✓

## Required Unity Editor Configuration

### In the Game Scene:

#### 1. Configure EnemySpawner Component
Locate the Systems GameObject in the Game scene and find the EnemySpawner component:

**EnemyData List Assignment:**
- Size: 4
- Element 0: BasicEnemy (Assets/ScriptableObjects/Enemies/BasicEnemy.asset)
- Element 1: FastEnemy (Assets/ScriptableObjects/Enemies/FastEnemy.asset)
- Element 2: HeavyEnemy (Assets/ScriptableObjects/Enemies/HeavyEnemy.asset)
- Element 3: SniperEnemy (Assets/ScriptableObjects/Enemies/SniperEnemy.asset)

**Spawn Curve Configuration:**
- The spawn curve is pre-configured as AnimationCurve.EaseInOut(0f, 1f, 1f, 3f)
- This provides gradual difficulty increase from 1x to 3x over the session duration
- Time 0s: Difficulty 1.0x
- Time 900s: Difficulty 3.0x

**Difficulty Scaling Settings:**
- Difficulty Multiplier: 1.0 (will be updated automatically)
- Max Difficulty Time: 900.0 (matches session duration)

#### 2. Verify SessionManager Component
The SessionManager should already be configured with:
- Session Duration: 900 (15 minutes)
- Enable Time Limit: True

#### 3. Verify UIManager Component
The UIManager should already be connected and will automatically display:
- Session timer (countdown or elapsed time)
- Wave information
- Health and XP bars

## Functionality Verification

### EnemySpawner Integration:
1. **Event Listening**: EnemySpawner subscribes to SessionManager.OnTimeChanged
2. **Difficulty Scaling**: Spawn rates increase over time using the spawn curve
3. **Enemy Variety**: Uses EnemyData ScriptableObjects for diverse enemy types
4. **Dynamic Adjustment**: Spawn timing reduces from 1s to 0.33s at max difficulty

### SessionManager Integration:
1. **Timer Display**: UIManager automatically shows session timer
2. **Game Over**: Properly handles time-up and player death scenarios
3. **Event Broadcasting**: Sends time updates to all listening systems

## Testing Instructions

1. **Play the Game Scene**
2. **Verify Timer Display**: Check that the session timer appears in the UI
3. **Monitor Difficulty**: Notice spawn rates increase over time
4. **Check Enemy Variety**: Different enemy types should spawn based on EnemyData
5. **Test End Conditions**: Game should end after 15 minutes or on player death

## Expected Behavior

- **Early Game (0-5 min)**: Slow, basic enemy spawns
- **Mid Game (5-10 min)**: Moderate spawn rates, mixed enemy types
- **Late Game (10-15 min)**: Fast spawn rates, challenging enemy combinations
- **Game Over**: Clean transition to game over screen with session stats

## Debug Information

The EnemySpawner now provides debug information:
- Current difficulty multiplier via `GetCurrentDifficulty()`
- Real-time spawn rate adjustments
- Wave progression with difficulty scaling

## File Changes Summary

1. **EnemySpawner.cs**: Enhanced with EnemyData support and difficulty scaling
2. **SessionManager.cs**: Already properly configured (no changes needed)
3. **UIManager.cs**: Already properly connected (no changes needed)

All code changes maintain backward compatibility and existing functionality.
