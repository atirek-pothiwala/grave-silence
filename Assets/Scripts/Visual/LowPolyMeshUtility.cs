using System.Collections.Generic;
using UnityEngine;

namespace GraveSilence.Visual
{
    /// <summary>
    /// Converts smooth meshes to faceted low-poly geometry by splitting vertices per triangle.
    /// </summary>
    public static class LowPolyMeshUtility
    {
        public static Mesh ToFlatShaded(Mesh source)
        {
            if (source == null) return null;

            var tris = source.triangles;
            var verts = source.vertices;
            var colors = source.colors;
            bool hasColors = colors != null && colors.Length == verts.Length;

            var newVerts = new Vector3[tris.Length];
            var newNormals = new Vector3[tris.Length];
            var newColors = new Color[tris.Length];
            var newTris = new int[tris.Length];

            for (int i = 0; i < tris.Length; i += 3)
            {
                Vector3 a = verts[tris[i]];
                Vector3 b = verts[tris[i + 1]];
                Vector3 c = verts[tris[i + 2]];
                Vector3 flatNormal = Vector3.Cross(b - a, c - a).normalized;

                for (int j = 0; j < 3; j++)
                {
                    int idx = i + j;
                    int srcIdx = tris[idx];
                    newVerts[idx] = verts[srcIdx];
                    newNormals[idx] = flatNormal;
                    newColors[idx] = hasColors ? colors[srcIdx] : Color.white;
                    newTris[idx] = idx;
                }
            }

            var mesh = new Mesh { name = source.name + "_LowPoly" };
            mesh.vertices = newVerts;
            mesh.normals = newNormals;
            mesh.colors = newColors;
            mesh.triangles = newTris;
            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh CreateIcoSphere(int subdivisions, float radius)
        {
            var temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var mesh = Object.Instantiate(temp.GetComponent<MeshFilter>().sharedMesh);
            Object.DestroyImmediate(temp);

            mesh = ToFlatShaded(mesh);
            var verts = mesh.vertices;
            for (int i = 0; i < verts.Length; i++)
                verts[i] = verts[i].normalized * radius;
            mesh.vertices = verts;
            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh CombinePrimitives(IReadOnlyList<(Vector3 pos, Vector3 scale, Quaternion rot)> parts)
        {
            var combines = new CombineInstance[parts.Count];
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh cubeMesh = cube.GetComponent<MeshFilter>().sharedMesh;

            for (int i = 0; i < parts.Count; i++)
            {
                var part = parts[i];
                combines[i] = new CombineInstance
                {
                    mesh = cubeMesh,
                    transform = Matrix4x4.TRS(part.pos, part.rot, part.scale)
                };
            }

            Object.DestroyImmediate(cube);
            var combined = new Mesh { name = "LowPolyCombined" };
            combined.CombineMeshes(combines, true, true);
            return ToFlatShaded(combined);
        }
    }
}
