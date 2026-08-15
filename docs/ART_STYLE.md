# Grave Silence — Low-Poly Art Style

Grave Silence uses a **minimal low-poly** look: flat-shaded geometry, solid colors, no textures. Think *Aragami* silhouettes meets *Lara Croft GO* simplicity — readable at a glance, fast to iterate, and perfect for stealth readability.

## Principles

1. **Flat faces only** — Every mesh is faceted. No smooth normals, no normal maps.
2. **Solid colors** — No albedo textures. Optional emissive for umbral glow and zombie eyes.
3. **Low vertex count** — Primitives and simple extrusions. Target &lt;500 tris per character.
4. **Strong silhouettes** — Blocky limbs and oversized heads for zombies; hooded shape for player.
5. **Stealth readability** — Shadow zones are visibly darker purple volumes; extraction is green.

## Technical Setup

| Asset | Path |
|-------|------|
| Flat-shaded lit shader | `Assets/Shaders/LowPolyLit.shader` |
| Unlit variant (props) | `Assets/Shaders/LowPolyUnlit.shader` |
| Color palette | `Assets/Art/LowPolyPalette.asset` |
| Base material | `Assets/Art/Materials/LowPolyLit.mat` |

### Unity Menu

- **Grave Silence → Low Poly → Create Palette & Material** — One-time setup
- **Grave Silence → Low Poly → Generate Placeholder Scene** — Buildings, ground, zombies, zones
- **Grave Silence → Low Poly → Generate Player Model** — Blocky umbral survivor

## Color Palette

| Role | Color | Hex |
|------|-------|-----|
| Player robe | Dark umbral purple | `#2E2447` |
| Zombie flesh | Muted green-gray | `#6B8561` |
| Brute flesh | Dark gray-purple | `#595261` |
| Ground | Asphalt gray | `#38383D` |
| Building | Concrete | `#615C57` |
| Shadow zone | Deep violet-black | `#140F24` |
| Extraction | Toxic green | `#33BF73` |
| Umbral glow | Purple highlight | `#7347BF` |

## Mesh Guidelines

- **Player** — 6–8 cubes: hooded torso, block head, stub arms/legs
- **Zombie** — Same rig, greener palette, glowing eye cube
- **Brute** — 1.4× scale, darker colors, wider torso
- **Buildings** — Stacked boxes with one accent stripe (brick band)
- **Debris** — Random-scaled cubes, no rotation smoothing

## Lighting

- Single directional moonlight (cool blue, ~0.4 intensity)
- Low ambient (`#1a1a2e`) — shadows stay readable
- No real-time reflections or SSAO
- Umbral abilities: additive purple particles (flat quads, no soft blobs)

## What to Avoid

- PBR materials, roughness/metallic maps
- High-poly sculpts or subdivision surfaces
- Photorealistic post-processing (bloom OK sparingly for umbral FX only)
- Detailed foliage — use 2–3 flat green wedges max per cluster

## Upgrading Later

When swapping placeholders for authored art, keep:
- Flat shading (`LowPolyMesh` component or `_Flatness = 1`)
- Palette colors from `LowPolyPalette`
- Triangle budget under 2k per scene chunk
