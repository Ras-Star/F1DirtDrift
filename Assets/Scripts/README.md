# F1DirtDrift - Racing Game Scripts

## 🎮 Project Overview
F1DirtDrift is a 2D top-down time trial racing game built in Unity 6 with staggered start mechanics, AI opponents, and hybrid lap timing.

## 📁 Scripts Created (14 Files)

### **Data & Configuration**
1. **CarData.cs** - ScriptableObject for car stats (speed, acceleration, handling, sprites)
2. **TrackData.cs** - ScriptableObject for track metadata (name, scene, lap count, best times)
3. **RaceEnums.cs** - Enums for RaceState, StartPosition, InputMode
4. **LapData.cs** - Structs for lap timing and race results

### **Core Management**
5. **GameManager.cs** - Singleton race orchestrator, timer, state machine, staggered starts
6. **TrackManager.cs** - Waypoint generation (auto/manual), lap counting, car registration
7. **UIManager.cs** - Panel management, scene transitions, pause/resume

### **Vehicle & Physics**
8. **CarController.cs** - Rigidbody2D physics, dual input (keyboard+touch), AI navigation, drift mechanics
9. **SteeringWheelController.cs** - Touch-based steering wheel with snap-back
10. **FinishLineTrigger.cs** - Lap detection with auto-correction positioning

### **UI Components**
11. **LeaderboardManager.cs** - Live mini-leaderboard updates during race
12. **ResultsManager.cs** - Final race results display with medals
13. **MenuManager.cs** - Track selection, start position choice

### **Audio & Effects**
14. **AudioManager.cs** - Singleton audio system (music, SFX, engine sounds)
15. **ParticleController.cs** - Smoke particles for drift/brake effects

## 🎯 Gameplay Logic

### **Staggered Time Trial System**
- **3 cars total**: Player + 2 AI opponents
- **Start sequence**: 
  - Car 1 starts after countdown
  - Car 2 starts when Car 1 completes lap 1
  - Car 3 starts when Car 2 completes lap 1
- **Player choice**: Choose to start 1st, 2nd, or 3rd (pure preference, no advantage)
- **Winner**: Determined by lowest total race time (sum of 3 laps)

### **Hybrid Timing System**
- **Best single lap**: Fastest individual lap time recorded
- **Total race time**: Sum of all 3 laps
- **Display**: Both metrics shown on results screen
- **Leaderboard**: Real-time updates sorted by total time

### **Controls**
- **Unity Editor (Keyboard)**:
  - W/↑: Accelerate
  - S/↓: Brake
  - A/←: Steer Left
  - D/→: Steer Right
  
- **Mobile (Touch)**:
  - Accelerator button
  - Brake button
  - Steering wheel drag

## 🛠️ Technical Implementation

### **AI Navigation**
- **Waypoint System**: Hybrid auto-generation from TrackRoot + manual refinement
- **Corner Detection**: Angle change > 30° = curve
- **Speed Adjustment**: 60% speed on curves
- **Recovery**: Teleport to last waypoint if stuck >3 sec

### **Physics (Unity 6)**
- **TopSpeed**: 150 units/sec (configurable per car)
- **Acceleration**: 50 units/sec²
- **Steering**: 180° max angle
- **Drift**: Triggered at 30° velocity/forward angle difference

### **Waypoint Generation**
1. Scans TrackRoot hierarchy for track pieces (sprites)
2. Sorts pieces by proximity (nearest neighbor)
3. Generates waypoints:
   - **Straight pieces**: 1 waypoint per 50 units
   - **Corner pieces**: 3 waypoints (entry, apex, exit)
4. Detects curves via angle analysis
5. Saves as part of TrackRoot prefab

## 📊 Scene Structure

### **MainMenu Scene**
- Track selection panel (Track_1, Track_2, Track_3)
- Start position selection (1st/2nd/3rd)
- Garage/about panels
- MenuManager handles navigation

### **Track Scenes (Track_1, Track_2, Track_3)**
**Required GameObjects**:
- GameManager (prefab)
- TrackRoot with TrackManager
- UICanvas
- LeaderboardPanel (top-right)
- TimerText (top-center)
- ResultsPanel (hidden by default)
- SteeringWheel (for mobile)
- Main Camera
- FinishLine trigger
- Player car + 2 AI cars

**TrackManager Configuration**:
- Set `waypointsParent` to TrackRoot transform
- Enable `autoGenerateWaypoints`
- Set `waypointsPerTrackPiece` = 3
- Assign `cars` array (player + AI)

## 🎨 Resources Structure

### **Required ScriptableObjects** (to be created):
```
Resources/
  CarData/
    PlayerCar.asset
    AI_Car_1.asset
    AI_Car_2.asset
  TrackData/
    Track_1_Data.asset
    Track_2_Data.asset
    Track_3_Data.asset
```

## 📝 Next Steps (Remaining Tasks)

### **Task 15: Input Actions** ✅ DONE
- Input actions already configured (Move action = WASD/Arrows)

### **Task 16: Create ScriptableObjects** (Next)
- Use MCP Unity server to create CarData instances
- Use MCP Unity server to create TrackData instances
- Assign sprites and configure stats

### **Task 17: Testing** (Final)
- Validate Track_1 waypoint generation
- Test staggered start timing
- Verify lap detection
- Test AI navigation
- Validate Track_3 as secondary test
- Check leaderboard updates
- Test results display

## 🐛 Known Dependencies

### **Tags Required** (Create in Unity):
- `Player` - for player car identification
- `FinishLine` - for finish line trigger

### **Layers** (Optional):
- Default layer configuration sufficient

### **Audio Clips Needed**:
- Background music (menu + race)
- Engine sounds (idle + rev)
- Tire skid sound
- Race start sound
- Race finish sound

## 🚀 Unity 6 Features Used
- Enhanced Rigidbody2D interpolation
- New Input System (already configured)
- URP 2D Renderer
- TextMesh Pro
- ScriptableObject workflow
- Improved Physics2D collision detection

## 📖 Script Dependencies

### **GameManager depends on**:
- TrackManager
- UIManager
- LeaderboardManager
- AudioManager
- CarController
- ResultsManager

### **TrackManager depends on**:
- GameManager (for car registration)
- CarController (waypoint assignment)

### **CarController depends on**:
- GameManager (lap completion)
- CarData (stats)
- AudioManager (sounds)
- ParticleController (effects)

### **All managers are auto-discovered via FindFirstObjectByType**

## ⚠️ Important Notes

1. **TrackRoot must contain track piece sprites** for auto-generation
2. **Cars must be assigned to TrackManager.cars array**
3. **FinishLine must be positioned at lap start/end point**
4. **Player car must have "Player" tag**
5. **All managers use singleton pattern** (DontDestroyOnLoad)
6. **Staggered start position saved in PlayerPrefs**

## 🎯 Success Criteria
- ✅ All 14 scripts compile without errors
- ✅ Singleton managers initialize correctly
- ⏳ Waypoints generate from TrackRoot
- ⏳ Cars start in staggered sequence
- ⏳ Lap detection works correctly
- ⏳ Leaderboard updates in real-time
- ⏳ Results display correctly
- ⏳ Controls work (keyboard + touch)

---

**Build Status**: Core scripts complete, ready for Unity integration testing
**Last Updated**: Implementation Phase - Nov 19, 2025
