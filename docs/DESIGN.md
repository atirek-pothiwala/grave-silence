# Grave Silence — Game Design Document

## Vision

Grave Silence is a stealth-action game that captures the feel of Aragami 2 — fluid movement, shadow-based supernatural powers, and rewarding ghost play — transplanted into a zombie apocalypse. The player is an "Umbral Walker," someone who can slip between shadows while the dead hunt by sight and sound.

## Pillars

1. **Power fantasy through stealth** — The player should feel like a predator, not a survivor with a gun.
2. **Readable enemy behavior** — Zombies telegraph awareness clearly (eye glow, icons, sound).
3. **Meaningful darkness** — Light and shadow are gameplay mechanics, not just atmosphere.
4. **Consequence for mistakes** — Alerting the horde should be recoverable but costly.

## Umbral Powers

### Umbral Step (Q)
Teleport to a shadow zone or ground point within range. Primary traversal tool for bypassing patrols and reaching rooftops.

- Cost: 20 energy | Cooldown: 3s | Range: 12m
- Requires target in shadow or on navmesh ground

### Umbral Cloak (F)
Become invisible while standing in a shadow zone. Movement breaks the cloak.

- Cost: 35 energy | Cooldown: 15s | Duration: 5s
- Only usable inside ShadowZone volumes

### Umbral Lure (R)
Place a shadow decoy that emits noise, pulling zombies to investigate.

- Cost: 25 energy | Cooldown: 10s | Duration: 8s
- Click to place at cursor position

### Umbral Strike (Right Mouse)
Instant stealth kill on an unaware zombie while in shadow or cloaked.

- Cost: 15 energy | Cooldown: 1s | Range: 2.5m
- Cannot kill Brutes; target must have low awareness

## Enemy Design

### Awareness States

```
Patrol → Investigate (heard noise) → Chase → Attack
                ↑                        ↓
                └── awareness decays ────┘
```

### Detection Inputs

1. **Sight** — Cone-based, modified by player visibility
2. **Hearing** — Reacts to NoiseSystem events (footsteps, traps, lures, screams)
3. **Alert propagation** — AlertSystem spreads awareness to nearby zombies

## Level Design Guidelines

- Every encounter should have at least 2 solutions: bypass or eliminate
- Shadow highways — connected dark routes through the level
- Verticality — rooftops, fire escapes, broken floors
- Noise traps as player-placed risk/reward (kick a bottle to distract)
- Screamers placed to punish direct routes

## Mission Structure

Each mission is a self-contained level with:

- 1 required objective (reach extraction, kill target, rescue NPC)
- 1–2 optional objectives (ghost run, collect items, no kills)
- Score screen with Aragami-style medals

## Reference Games

- **Aragami 2** — Core stealth loop, shadow powers, mission structure
- **The Last of Us** — Zombie threat, environmental storytelling
- **Dishonored** — Multiple approaches, awareness system
- **Mark of the Ninja** — 2D clarity applied to 3D feedback
