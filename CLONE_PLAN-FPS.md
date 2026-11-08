# CLONE_PLAN-FPS.md — Clean-Room Fast-Paced FPS Implementation Plan

> **Note:** this plan was created in a separate work round and is NOT the Muck-clone (fishing) track plan. The fishing-clone track plan lives in `CLONE_PLAN.md`.

**Target Reference:** Combat Master (AlfaBravoInc)  
**Project Objective:** Build an original, high-velocity competitive FPS engine and gameplay prototype completely from scratch.  
**Strict Clean-Room Constraint:** Zero code, assets, audio, models, or text copied from the target. All tunables are original baseline values calibrated within the ±10% feel band. Original class taxonomy and architectural design.

---

## 1. Compliance & Original-Only Rules

| Boundary | Rule |
| :--- | :--- |
| **Code** | 100% original C# code. No decompiled or disassembled IL2CPP functions from `GameAssembly.dll` copied or transliterated. |
| **Assets & Art** | Procedural / original primitives (ProBuilder/Blender blockouts). Zero asset extraction from `data.unity3d` or `StreamingAssets\Bundles`. |
| **Audio** | Procedural synthesizers / original CC0 effects for gunshots, reloads, and slide audio. |
| **Names & Identifiers** | All class names, component names, and UI copy are original (`KineticMotor`, `ArmoryWeapon`, etc.). |
| **Tunables** | Speeds, damage, recoil, and timers are defined independently within the ±10% competitive FPS band. |

---

## 2. Implementation Roadmap

### Phase 1: Core Kinetic Controller & Camera Dynamics [x]
- [x] **First-Person Camera Rig (`FpsCameraRig`):**
  - Mouse look pitch (-85° to +85°) and yaw smoothing.
  - Dynamic FOV transitions: Base (80°), ADS Zoom, Recoil pitch/yaw spring kick.
- [x] **Kinetic Character Motor (`FpsPlayerMotor`):**
  - Walk (5.2 m/s), Sprint (9.2 m/s), Slide (12.0 m/s), Jump Velocity (6.5 m/s).
  - Power Slide state machine with friction decay and momentum inheritance.

### Phase 2: Procedural Weaponry & Ballistics [x]
- [x] **Procedural Viewmodel Socket (`FpsWeaponView`):**
  - Spring-damper mouse sway + ADS lerp with sub-180ms response.
- [x] **Hitscan Arsenal Engine (`FpsWeapon`, `FpsShotgun`, `FpsSmg`, `FpsSniperRifle`):**
  - Assault Rifle: 25 dmg, 9 rps, 30 round mag.
  - Tactical Shotgun: 8 pellets, 15 dmg per pellet, pump recoil.
  - Rapid SMG: 18 dmg, 16 rps, fast ADS (100ms).
  - Bolt-Action Sniper: 115 dmg, 2.5x headshot, zoom FOV ADS.
- [x] **Weapon Manager & Switcher (`FpsWeaponManager`):**
  - Slot cycling (1-4, mouse wheel) with holster/draw timers and live HUD GUI.

### Phase 3: Vertical Traversal & Environmental Actors [x]
- [x] **Environmental Launch Pads (`FpsLaunchPad`):** Impulse triggers with air-control dampening.
- [x] **Spline Zipline System (`FpsCableZipline`):** Smooth cable slide traversal at 16 m/s.
- [x] **Reactive Target Dummies (`FpsTargetDummy`):** Headshot tagging, damage floating feedback, reset timer.
- [x] **3-Lane Arena Scene Builder (`FpsArenaBuilder`):** Menu item `Clone -> Scene -> Create FPS Arena Scene` generates the complete competitive arena blockout.
  - Tracer pooling system (`TracerPoolManager`) rendering fast line-renderer trails.

### Phase 3: Vertical Traversal & Environmental Actors (Week 3)
- [ ] **Environmental Launch Pads (`PropulsionJumpPad`):**
  - Trigger volume applying directional impulse ($V_y = 15.5\text{ m/s}$, $V_{xz} = 12.0\text{ m/s}$) with temporary air-control dampening.
- [ ] **Spline Zipline System (`CableZipline`):**
  - Two-point anchor spline allowing player snap-in, gliding at constant 14.0 m/s with detachment jump.
- [ ] **Moving Platform Registry (`DynamicPlatform`):**
  - Character kinematic attachment ensuring correct momentum inheritance when riding escalators and elevators.

### Phase 4: Modular Gunsmith & Attachments (Week 4)
- [ ] **Modular Weapon Assembly:**
  - Base weapon prefab with socket transforms: `MuzzleSocket`, `OpticSocket`, `MagSocket`, `BarrelSocket`.
  - Attachment ScriptableObjects modifying weapon recoil curves, ADS speed, and optics FOV.
  - Dynamic reticle rendering (Reflex red-dot shader, holographic sight frame).

### Phase 5: Arena Map Blockout & Bot Sandbox (Week 5)
- [ ] **Compact 3-Lane Arena ("Courtyard Sector"):**
  - Symmetrical competitive layout designed for fast traversal (12s spawn-to-spawn).
  - High ground jump-pad access, interior CQB corridors, and long rifle sightlines.
- [ ] **Reactive Target Dummies & Simple AI:**
  - Standard humanoid health (100 HP + 3-plate armor).
  - Hit reaction animations / ragdoll on death.
  - Basic patrol and line-of-sight tracking for combat testing.

### Phase 6: Networking Spine & Multiplayer Replication (Week 6)
- [ ] **Authoritative Client-Server Netcode:**
  - Client-side prediction for local movement and gunplay.
  - Server validation of position history and hit registration (lag compensation).
  - State replication for remote player view pitch/yaw, weapon firing bursts, and health.

---

## 3. Immediate Milestone Deliverables (Starting Phase 1)

1. **Unity Asset Hierarchy:**
   - `Assets/Scripts/Kinetic/KineticCharacterController.cs`
   - `Assets/Scripts/Kinetic/KineticCameraLook.cs`
   - `Assets/Scripts/Armory/ArmoryGunController.cs`
   - `Assets/Scripts/Armory/ProceduralWeaponSway.cs`
   - `Assets/Scripts/Environment/PropulsionJumpPad.cs`
2. **Playable Testing Scene:**
   - Flat testing greybox arena with jump pads, obstacles to slide under, ramps, and reactive hit-targets.
