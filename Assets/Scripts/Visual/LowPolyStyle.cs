using UnityEngine;

namespace GraveSilence.Visual
{
    /// <summary>
    /// Applies low-poly palette colors to renderers and keeps materials untextured.
    /// </summary>
    public class LowPolyStyle : MonoBehaviour
    {
        [SerializeField] private LowPolyPalette palette;
        [SerializeField] private LowPolyStyleType styleType;
        [SerializeField] private Material litMaterialTemplate;

        public enum LowPolyStyleType
        {
            Player,
            Zombie,
            ZombieBrute,
            Ground,
            Building,
            ShadowZone,
            Extraction,
            Debris,
            Foliage
        }

        private void Awake()
        {
            ApplyStyle();
        }

        public void ApplyStyle()
        {
            if (palette == null) return;

            Color color = styleType switch
            {
                LowPolyStyleType.Player => palette.playerRobe,
                LowPolyStyleType.Zombie => palette.zombieFlesh,
                LowPolyStyleType.ZombieBrute => palette.bruteFlesh,
                LowPolyStyleType.Ground => palette.groundAsphalt,
                LowPolyStyleType.Building => palette.buildingConcrete,
                LowPolyStyleType.ShadowZone => palette.shadowZone,
                LowPolyStyleType.Extraction => palette.extractionZone,
                LowPolyStyleType.Debris => palette.debris,
                LowPolyStyleType.Foliage => palette.foliage,
                _ => Color.gray
            };

            var lowPoly = GetComponent<LowPolyMesh>();
            if (lowPoly != null)
            {
                lowPoly.SetColor(color);
                return;
            }

            var renderer = GetComponent<Renderer>();
            if (renderer != null && litMaterialTemplate != null)
            {
                var mat = new Material(litMaterialTemplate);
                mat.SetColor("_BaseColor", color);
                renderer.sharedMaterial = mat;
            }
        }
    }
}
