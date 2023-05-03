using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Foliage
{
    public class Foliage2D_Mesh
    {
        #region Fields and Properties
        private List<Vector3> meshVerts;
        private List<int> meshIndices;
        private List<Vector2> meshUVs;
        #endregion

        #region Constructor
        public Foliage2D_Mesh()
        {
            meshVerts = new List<Vector3>();
            meshUVs = new List<Vector2>();
            meshIndices = new List<int>();
        }
        #endregion

        #region General Methods
        public void Clear()
        {
            meshVerts.Clear();
            meshIndices.Clear();
            meshUVs.Clear();
        }

        public void Build(ref Mesh mesh)
        {
            for (int i = 0; i < meshVerts.Count; i += 1)
                meshVerts[i] = new Vector3(
                         (float)System.Math.Round(meshVerts[i].x, 3),
                         (float)System.Math.Round(meshVerts[i].y, 3),
                         (float)System.Math.Round(meshVerts[i].z, 3));

            mesh.Clear();
            mesh.vertices = meshVerts.ToArray();
            mesh.uv = meshUVs.ToArray();
            mesh.triangles = meshIndices.ToArray();

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        #endregion
        #region Class Methods
        public void GenerateTriangles(int widthSegments, int heightSegments, int hVertices)
        {
            for (int y = 0; y < heightSegments; y++)
            {
                for (int x = 0; x < widthSegments; x++)
                {
                    meshIndices.Add((y * hVertices) + x);
                    meshIndices.Add(((y + 1) * hVertices) + x);
                    meshIndices.Add((y * hVertices) + x + 1);

                    meshIndices.Add(((y + 1) * hVertices) + x);
                    meshIndices.Add(((y + 1) * hVertices) + x + 1);
                    meshIndices.Add((y * hVertices) + x + 1);
                }
            }
        }

        public void AddVertex(Vector2 vertexPoss, float z, Vector2 UV)
        {
            meshVerts.Add(new Vector3(vertexPoss.x, vertexPoss.y, z));
            meshUVs.Add(UV);
        }
        #endregion
    }
}
