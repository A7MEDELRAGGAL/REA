# AGENT.md — Developer & AI Agent Context for REA

Welcome to the **REA (Reverse Engineering Architecture)** repository.
This document provides immediate context, conventions, and operational workflows for any AI agent or developer running on this machine (local or VPS).

---

## 1. High-Level Project Structure

This repository is a **strict clean-room game reconstruction** workspace containing three active tracks:

| Path | Track | Engine / Tech | Status |
|---|---|---|---|
| `WebGame/` | **Tidebreak** | Procedural 3D Web Game (Three.js + HTML5 Canvas + Socket.io) | ✅ Complete & playable offline/online; tests pass 54/54. |
| `WebGame/server/` | **Tidebreak Multiplayer** | Node.js + Express + Socket.io | ✅ Real-time player movement and state replication relay. |
| `UnityProject/` | **Muck Survival Clone** | Unity 6000.0.32f1 + FishNet | ✅ Core survival loop (movement, stamina, combat, inventory, crafting, shared chest, currency, save/load, procedural boss). |
| `UnityProject/Assets/Scripts/Fps/` | **Kinetic FPS Engine** | Unity C# (Combat Master velocity feel) | ✅ Complete kinetic motor, 4 weapon systems (AR, Shotgun, SMG, Sniper), WeaponManager, 3-lane Arena Builder. |

---

## 2. Essential Commands

### 2.1 WebGame Verification & Build
Every change to `WebGame/js/*` or `WebGame/css/*` **must pass verification**:
```bash
cd WebGame
npm run verify
```
This runs:
1. `tools/apicheck.js` — validates all 50+ API contract functions and prototype methods.
2. `build.js` — bundles everything into a single zero-dependency file at `dist/tidebreak.html`.
3. `tools/verify-dist.js` — boots a headless environment, runs 300 real frames, verifies rendering, physics, and security (no exposed API keys).

### 2.2 Running the Multiplayer Server on VPS
```bash
cd WebGame/server
npm install
node index.js
# Or with PM2 for background daemon:
# pm2 start index.js --name tidebreak-server
```

### 2.3 Serving the WebGame
```bash
# Serve static dist directory (e.g. on port 8080):
npx serve WebGame/dist -p 8080
```

---

## 3. Strict Clean-Room Guidelines
- **Zero Asset / Code Piracy**: Never copy decompiled code, bytecode, meshes, or textures from reference targets (`artifacts/` is strictly for high-level behavioural observation).
- All code in `WebGame/js/` and `UnityProject/Assets/Scripts/` is 100% original.
- Do not add external CDNs or unverified third-party libraries into `WebGame/dist/tidebreak.html` (the verification gate will fail).

---

## 4. Immediate Development Roadmap

1. **WebGame Multiplayer Expansion**:
   - Add in-game text chat overlay (`Tidebreak Chat`).
   - Add entity interpolation / smooth lerp for remote players on high-ping VPS connections.
   - Add remote player weapon swing/cast animations.
2. **Unity Headless Server Setup**:
   - Create a dedicated headless Linux server build script for FishNet on VPS.
   - Set up automatic game state backup script.
3. **Audio & Polish**:
   - Procedural WebAudio synthesis for water splashes, rod casting, and hit feedback.
