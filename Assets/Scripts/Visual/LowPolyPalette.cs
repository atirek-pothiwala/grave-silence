using UnityEngine;

namespace GraveSilence.Visual
{
    /// <summary>
    /// Shared low-poly color palette. Use flat colors, no textures.
    /// </summary>
    [CreateAssetMenu(fileName = "LowPolyPalette", menuName = "Grave Silence/Low Poly Palette")]
    public class LowPolyPalette : ScriptableObject
    {
        [Header("Player & Umbral")]
        public Color playerRobe = new(0.18f, 0.14f, 0.28f);
        public Color playerSkin = new(0.72f, 0.58f, 0.48f);
        public Color umbralGlow = new(0.45f, 0.28f, 0.75f);

        [Header("Zombies")]
        public Color zombieFlesh = new(0.42f, 0.52f, 0.38f);
        public Color zombieRot = new(0.28f, 0.34f, 0.24f);
        public Color zombieEyesCalm = new(0.2f, 0.85f, 0.3f);
        public Color zombieEyesAlert = new(0.95f, 0.2f, 0.15f);
        public Color bruteFlesh = new(0.35f, 0.32f, 0.38f);

        [Header("Environment")]
        public Color groundAsphalt = new(0.22f, 0.22f, 0.24f);
        public Color buildingConcrete = new(0.38f, 0.36f, 0.34f);
        public Color buildingBrick = new(0.45f, 0.28f, 0.22f);
        public Color shadowZone = new(0.08f, 0.06f, 0.14f);
        public Color extractionZone = new(0.2f, 0.75f, 0.45f);
        public Color debris = new(0.32f, 0.3f, 0.28f);

        [Header("Props")]
        public Color barrel = new(0.5f, 0.35f, 0.18f);
        public Color fence = new(0.25f, 0.25f, 0.28f);
        public Color foliage = new(0.25f, 0.42f, 0.22f);
    }
}
