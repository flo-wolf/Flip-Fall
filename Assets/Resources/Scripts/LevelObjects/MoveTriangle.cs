using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//create MoveArea class and inherit this from it
namespace Impulse.Levels
{
    public class MoveTriangle : MonoBehaviour
    {
        public MeshFilter _mesh;
        public PolygonCollider2D poll;

        public Vector3[] newVertices;
        public Vector2[] newUV;
        public int[] newTriangles;

        private void Start()
        {
            //modify collider points to snipping tringle
            //poll.points

            Triangulator tr = new Triangulator(poll.points);
            int[] indices = tr.Triangulate();
            Vector3[] vertices = new Vector3[indices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(vertices[i].x, vertices[i].y, 0);
            }

            // Create the mesh
            Mesh msh = _mesh.mesh;
            msh.vertices = vertices;
            msh.triangles = indices;
            msh.RecalculateNormals();
            msh.RecalculateBounds();
            this.GetComponent<MeshFilter>().mesh = msh;
            // mesh.mesh.triangles = newTriangles;
        }
    }
}