using FlipFall.Levels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Highlights the triangles which selected verticies are part of
/// </summary>

namespace FlipFall.Editor
{
    public class TriangleHighlight : MonoBehaviour
    {
        private Camera cam;
        public bool highlightTriangles;
        private Vector3[] triangles;
        public Material highlightMaterial;
        public Color highlightColor;

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        private void CreateLineMaterial()
        {
            highlightMaterial.SetInt("_ZWrite", 1);
            highlightMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
        }

        //private Vector3[] GetTriangles()
        //{
        //    Vector3[] verts = new Vector3[]
        //    return VertHandler.selectionTriangleVerts.ToArray();
        //}

        private void OnPostRender()
        {
            if (highlightTriangles)
            {
                //triangles = GetTriangles();
                triangles = VertHandler.selectionTriangleVerts.ToArray();

                // triangle array is a multiple of three
                if (triangles.Length % 3 == 0 && triangles.Length > 0)
                {
                    CreateLineMaterial();
                    highlightMaterial.SetPass(0);

                    GL.MultMatrix(LevelPlacer.generatedLevel.moveArea.transform.localToWorldMatrix);

                    GL.Begin(GL.TRIANGLES);
                    GL.Color(highlightColor);

                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        Vector3 v1 = triangles[i + 0];
                        Vector3 v2 = triangles[i + 1];
                        Vector3 v3 = triangles[i + 2];

                        GL.Vertex3(v1.x, v1.y, v1.z);
                        GL.Vertex3(v2.x, v2.y, v2.z);
                        GL.Vertex3(v3.x, v3.y, v3.z);
                    }
                    GL.End();
                }
            }
        }
    }
}