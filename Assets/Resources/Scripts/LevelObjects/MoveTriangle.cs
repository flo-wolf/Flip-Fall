using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//create MoveArea class and inherit this from it
namespace Impulse.Levels
{
    [ExecuteInEditMode]
    public class MoveTriangle : MonoBehaviour
    {
        public MeshFilter mf;

        public Vector3[] newVertices;
        public Vector2[] newUV;
        public int[] newTriangles;

        private void Awake()
        {
            if (mf != null)
            {
                //modify collider points to snipping tringle
                //poll.points

                Vector2[] vert2d = new Vector2[mf.mesh.vertexCount];

                for (int v = 0; v < mf.mesh.vertexCount; v++)
                {
                    vert2d[v] = new Vector2(mf.mesh.vertices[v].x, mf.mesh.vertices[v].y);
                }

                Triangulator tr = new Triangulator(vert2d);
                int[] indices = tr.Triangulate();
                Vector3[] vertices = new Vector3[indices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = new Vector3(vertices[i].x, vertices[i].y, 0);
                }

                // Create the mesh
                Mesh msh = mf.mesh;
                msh.vertices = vertices;
                msh.triangles = indices;
                msh.RecalculateNormals();
                msh.RecalculateBounds();
                this.GetComponent<MeshFilter>().mesh = msh;
                // mesh.mesh.triangles = newTriangles;
            }
        }
    }
}