# Grave Silence

**Live site:** [atirek-pothiwala.github.io/grave-silence](https://atirek-pothiwala.github.io/grave-silence)

A third-person stealth action game inspired by **Aragami 2**, set in a post-apocalyptic zombie world. Move through ruined cities in the shadows, use umbral powers to eliminate the undead silently, and complete missions without raising the horde.

## Core Concept

| Aragami 2 | Grave Silence |
|-----------|---------------|
| Shadow ninja assassin | Umbral survivor in a dead city |
| Shadow teleport | **Umbral Step** — blink through darkness |
| Shadow cloak | **Umbral Cloak** — vanish in shadow zones |
| Shadow lure | **Umbral Lure** — decoy noise to pull zombies |
| Shadow kill | **Umbral Strike** — assassinate from the dark |
| Samurai guards | Zombie horde with varied types |
| Spirit vision | Noise/alert awareness HUD |

## Zombie Types

- **Shambler** — Slow patrol, low awareness. Easy to sneak past.
- **Runner** — Fast chase once alerted. Prioritize shadow routes.
- **Screamer** — Screams on detection, alerts the entire area. Kill first.
- **Brute** — Cannot be stealth-killed. Avoid or distract.

## Controls

| Action | Key |
|--------|-----|
| Move | WASD |
| Look | Mouse |
| Sprint | Left Shift |
| Crouch | Left Ctrl |
| Stealth Takedown | E |
| Spirit Vision | Tab |
| Pause | Escape |
| Umbral Step | Q |
| Umbral Cloak | F |
| Umbral Lure | R |
| Umbral Strike | Right Mouse |
| Pause | Escape |

## Project Setup

### Requirements

- Unity 6 (6000.0.34f1) or Unity 2022.3 LTS+
- Universal Render Pipeline (URP)
- Input System package
- Cinemachine (optional, for advanced camera)

### Getting Started

1. Clone this repository
2. Open the project folder in Unity Hub
3. Let Unity import packages from `Packages/manifest.json`
4. Open or create a scene and follow **Scene Setup** below
5. Bake a NavMesh for zombie pathfinding (Window → AI → Navigation)

### Quick Setup (Editor)

Use **Grave Silence → Create Systems Hierarchy** and **Grave Silence → Create Player** from the menu bar.

### Scene Setup

Create these GameObjects in your test scene:

```
Scene Hierarchy
├── --- Systems ---
│   ├── GameManager          (GameManager.cs)
│   ├── InputManager         (InputManager.cs + GraveSilence.inputactions)
│   ├── NoiseSystem          (NoiseSystem.cs)
│   ├── AlertSystem          (AlertSystem.cs)
│   ├── ObjectiveTracker     (ObjectiveTracker.cs)
│   └── MissionScore         (MissionScore.cs)
├── --- Player ---
│   └── Player               (Tag: "Player")
│       ├── ThirdPersonController
│       ├── StealthController
│       ├── UmbralAbilities
│       ├── StealthTakedown
│       ├── SpiritVision
│       ├── PlayerHealth
│       ├── PlayerInputHandler
│       └── CharacterController
├── --- Camera ---
│   └── Main Camera          (ThirdPersonCamera.cs, target = Player)
├── --- UI ---
│   └── Canvas               (StealthHUD.cs)
├── --- Environment ---
│   ├── ShadowZones          (ShadowZone.cs on trigger colliders)
│   ├── ExtractionPoint      (ExtractionPoint.cs)
│   └── NoiseTraps           (NoiseTrap.cs on bottles, debris)
└── --- Enemies ---
    ├── Zombie_Shambler      (ZombieBase.cs + NavMeshAgent)
    ├── Zombie_Runner        (ZombieRunner.cs)
    └── Zombie_Screamer      (ZombieScreamer.cs)
```

### Tags to Create

Add these tags in **Edit → Project Settings → Tags and Layers**:

- `Player`
- `ShadowZone`
- `Ground`

## Architecture

```
Assets/Scripts/
├── Core/           GameManager, missions, input
├── Player/         Movement, stealth, umbral abilities
├── Enemies/        Zombie AI and variants
├── Systems/        Noise propagation, horde alert
├── Environment/    Shadow zones, extraction, traps
├── UI/             Stealth HUD
└── Camera/         Third-person follow camera
```

## Gameplay Loop

1. **Briefing** — Mission objectives shown (rescue, retrieve, eliminate)
2. **Infiltration** — Enter the level, stay in shadows
3. **Execution** — Use umbral powers and stealth takedowns
4. **Extraction** — Reach the exit without dying or failing optional ghost objectives

## Scoring (Planned)

- **Ghost** — Complete without any zombie reaching full awareness
- **Silent** — All kills were stealth takedowns
- **Speed** — Under par time bonus

## Roadmap

- [ ] Animation controller and blend trees
- [ ] Level blockout: abandoned hospital (Mission 1)
- [ ] Co-op support (2-player umbral survivors)
- [ ] Spirit vision / enemy awareness overlay
- [ ] Procedural patrol routes
- [ ] Melee combat fallback when detected
- [ ] Audio: footstep surfaces, zombie groans, umbral VFX sounds

## License

MIT
