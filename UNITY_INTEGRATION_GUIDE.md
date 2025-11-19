# 🚀 F1DirtDrift - Unity Integration Guide

## ✅ COMPLETED: All Scripts Created (16 files)

### **Core Scripts (15)**
1. CarData.cs
2. TrackData.cs
3. RaceEnums.cs
4. LapData.cs
5. GameManager.cs
6. TrackManager.cs
7. UIManager.cs
8. CarController.cs
9. SteeringWheelController.cs
10. FinishLineTrigger.cs
11. LeaderboardManager.cs
12. ResultsManager.cs
13. MenuManager.cs
14. AudioManager.cs
15. ParticleController.cs

### **Editor Tools (1)**
16. **Editor/ScriptableObjectGenerator.cs** - Auto-creates all CarData and TrackData assets

---

## 📝 Step-by-Step Unity Integration

### **STEP 1: Generate ScriptableObjects** ⚡

1. **Open Unity Editor**
2. **Wait for scripts to compile** (check bottom-right of Unity)
3. **Click menu**: `F1DirtDrift > Generate All ScriptableObjects`
4. **Verify creation**:
   - Check `Assets/Resources/CarData/` (3 assets)
   - Check `Assets/Resources/TrackData/` (3 assets)

---

### **STEP 2: Create Required Tags**

1. **Open**: `Edit > Project Settings > Tags and Layers`
2. **Add Tags**:
   - `Player`
   - `FinishLine`

---

### **STEP 3: Configure GameManager Prefab**

**Prefab Location**: `Assets/Prefabs/GameManager.prefab`

1. **Open prefab** in Inspector
2. **Add Components** (if not already present):
   - Add `GameManager` script
3. **Assign References**:
   - **Timer Text**: Drag `TimerText` prefab to slot
   - **Countdown Text**: Create or assign countdown text UI element
   - **Results Panel**: Drag `ResultsPanel` prefab to slot
   - **Start Clip**: Drag audio file from `Assets/Audio/SFX/` (engine start)
   - **Finish Clip**: Drag audio file for race finish
4. **Save prefab**

---

### **STEP 4: Configure TrackRoot Prefab**

**Prefab Location**: `Assets/Prefabs/TrackRoot.prefab`

1. **Open prefab** in Inspector
2. **Add Component**: `TrackManager` script
3. **Configure Settings**:
   - **Waypoints Parent**: Assign TrackRoot's own Transform
   - **Curve Detection Angle**: 30
   - **Lap Count**: 3
   - **Auto Generate Waypoints**: ✅ CHECKED
   - **Waypoints Per Track Piece**: 3
4. **Save prefab**

---

### **STEP 5: Setup Track_1 Scene** 🏁

**Open Scene**: `Assets/Scenes/Track_1.unity`

#### **A. Configure TrackRoot Instance**
1. **Select** `TrackRoot` GameObject in Hierarchy
2. **Verify TrackManager component** is attached
3. **Assign Cars Array** (size: 3):
   - Slot 0: Player car GameObject
   - Slot 1: AI car 1 GameObject
   - Slot 2: AI car 2 GameObject

#### **B. Setup Player Car**
1. **Create** new GameObject: "PlayerCar"
2. **Add Components**:
   - `CarController` script
   - `SpriteRenderer`
   - `Rigidbody2D`
   - `CircleCollider2D` or `BoxCollider2D`
3. **Configure CarController**:
   - **Car Data**: Drag `PlayerCar.asset` from Resources/CarData
   - **Is Player**: ✅ CHECKED
   - **Start Position**: First (or let player choose)
   - **Input Mode**: Keyboard Mouse (for testing)
4. **Set Tag**: "Player"
5. **Configure Rigidbody2D**:
   - Gravity Scale: 0
   - Linear Drag: 0.5
   - Angular Drag: 2
6. **Assign Sprite**: Drag car sprite from `Assets/Sprites/Cars/`

#### **C. Setup AI Cars (Repeat for AI_1 and AI_2)**
1. **Duplicate** PlayerCar or create new GameObject
2. **Rename**: "AI_Car_1"
3. **Configure CarController**:
   - **Car Data**: Drag `AI_Car_1.asset`
   - **Is Player**: ❌ UNCHECKED
   - **Start Position**: Second (for AI_1), Third (for AI_2)
4. **Remove "Player" tag**
5. **Change sprite color** or assign different sprite

#### **D. Setup FinishLine**
1. **Select** FinishLine prefab instance
2. **Verify** `FinishLineTrigger` script attached
3. **Configure BoxCollider2D**:
   - **Is Trigger**: ✅ CHECKED
   - **Size**: Adjust to span track width (e.g., 50 x 20)
4. **Position**: Place at start/finish line waypoint
   - TrackManager will auto-correct if misaligned

#### **E. Setup UI Canvas**
1. **Select** UICanvas in Hierarchy
2. **Add LeaderboardManager** (create child panel):
   - Create GameObject: "LeaderboardPanel"
   - Add `LeaderboardManager` script
   - Add 3 sets of TextMeshPro fields:
     - Position texts (1st, 2nd, 3rd)
     - Car name texts
     - Lap texts
     - Time texts

#### **F. Configure ResultsPanel**
1. **Select** ResultsPanel prefab instance
2. **Add** `ResultsManager` script
3. **Assign UI References**:
   - Position text
   - Player time text
   - Player best lap text
   - Opponent name/time texts (2 sets)
   - Replay/Next/Menu buttons

#### **G. Setup SteeringWheel (for mobile)**
1. **Select** SteeringWheel GameObject
2. **Verify** `SteeringWheelController` script attached
3. **Auto-assigns player car** on Start

---

### **STEP 6: Configure UIManager (MainMenu Scene)**

**Open Scene**: `Assets/Scenes/MainMenu.unity`

1. **Select** Canvas or create new GameObject: "UIManager"
2. **Add Component**: `UIManager` script
3. **Assign References**:
   - **Main Menu Panel**: Main menu UI panel
   - **Track Selection Panel**: Track selection UI
   - **Select Track Button**: Button that opens track selection
   - **Track 1/2/3 Buttons**: Individual track buttons
   - **Back Button**: Returns to main menu
4. **Add MenuManager** to handle start position:
   - Create GameObject: "MenuManager"
   - Add `MenuManager` script
   - Will show start position selection after track choice

---

### **STEP 7: Setup AudioManager**

1. **Create** new GameObject in any scene: "AudioManager"
2. **Add Component**: `AudioManager` script
3. **Assign Audio Clips**:
   - **Menu Music**: Drag from Assets/Audio/Music/
   - **Race Music**: Background race music
   - **Engine Idle Sound**: Idle engine loop
   - **Skid Sound**: Tire skid SFX
   - **Crash Sound**: Collision sound
4. **Configure** (auto-creates AudioSources on Start)

---

### **STEP 8: Apply to Track_2 and Track_3**

**Repeat STEP 5** for Track_2 and Track_3 scenes:
- Use same prefab instances
- Adjust TrackRoot waypoints parent
- Position cars at track starting point

---

## 🧪 Testing Checklist

### **Test Track_1** ✅
1. **Play scene** in Unity Editor
2. **Verify**:
   - [ ] Countdown appears (3-2-1-GO)
   - [ ] Player car starts first
   - [ ] WASD/Arrow keys control car
   - [ ] Timer counts up
   - [ ] Leaderboard shows all 3 cars
   - [ ] Crossing finish line completes lap
   - [ ] AI cars start when previous car completes lap 1
   - [ ] After 3 laps, results panel appears
   - [ ] Position/times displayed correctly

### **Test Track_3** ✅
1. **Repeat above tests**
2. **Verify waypoint generation** works on different track layout

---

## 🐛 Troubleshooting

### **Issue: Waypoints not generating**
- **Check**: TrackRoot has child GameObjects with SpriteRenderers
- **Check**: TrackManager.waypointsParent is assigned
- **Check**: autoGenerateWaypoints is enabled
- **Fix**: Run `TrackManager.GenerateWaypointsFromTrackPieces()` manually

### **Issue: Cars not starting**
- **Check**: Cars are assigned to TrackManager.cars array
- **Check**: GameManager prefab is in scene
- **Check**: TrackManager.InitializeTrack() runs on Start

### **Issue: Lap not detecting**
- **Check**: FinishLine has BoxCollider2D with isTrigger = true
- **Check**: FinishLine positioned near first waypoint
- **Check**: CarController has Rigidbody2D and Collider2D

### **Issue: UI not showing**
- **Check**: Canvas has CanvasScaler and GraphicRaycaster
- **Check**: LeaderboardManager has UI text references assigned
- **Check**: ResultsPanel is child of Canvas

---

## 🎮 Controls Reference

### **Unity Editor Testing**
- **W / ↑**: Accelerate
- **S / ↓**: Brake
- **A / ←**: Steer Left
- **D / →**: Steer Right
- **ESC**: Pause (if implemented)

### **Mobile** (via UI)
- **Accelerator Button**: Bottom right (hold to accelerate)
- **Brake Button**: Bottom left
- **Steering Wheel**: Center bottom (drag)

---

## 📊 Performance Tips

1. **Reduce waypoint count** if performance issues:
   - Lower `waypointsPerTrackPiece` to 2
2. **Disable debug Gizmos** in Game view
3. **Use object pooling** for particles (future optimization)
4. **Limit leaderboard updates** to lap completions only

---

## ✨ Next Steps (Phase 2 - Future)

- [ ] Add data persistence (PlayerPrefs)
- [ ] Implement car unlocks
- [ ] Add ghost car replays
- [ ] Create more tracks
- [ ] Add power-ups/boosts
- [ ] Multiplayer support
- [ ] Garage customization
- [ ] Achievement system

---

**Build Complete!** 🏁  
**Status**: Ready for Unity integration and testing  
**Total Scripts**: 16 files (15 core + 1 editor tool)  
**Last Updated**: November 19, 2025
