using UnityEngine;

namespace GraveSilence.Visual
{
    /// <summary>
    /// Applies low-poly flat shading and palette color to a mesh at runtime or in editor.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class LowPolyMesh : MonoBehaviour
    {
        [SerializeField] private bool flattenOnAwake = true;
        [SerializeField] private Color vertexColor = Color.white;
        [SerializeField] private Material lowPolyMaterial;

        private void Awake()
        {
            if (flattenOnAwake)
                ApplyFlatShading();
        }

        public void ApplyFlatShading()
        {
            var filter = GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null) return;

            filter.sharedMesh = LowPolyMeshUtility.ToFlatShaded(filter.sharedMesh);
            ApplyVertexColor(vertexColor);

            if (lowPolyMaterial != null)
                GetComponent<MeshRenderer>().sharedMaterial = lowPolyMaterial;
        }

        public void SetColor(Color color)
        {
            vertexColor = color;
            ApplyVertexColor(color);
        }

        private void ApplyVertexColor(Color color)
        {
            var filter = GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null) return;

            var mesh = filter.sharedMesh;
            var colors = new Color[mesh.vertexCount];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;
            mesh.colors = colors;
        }
    }
}
