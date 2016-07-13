using System.Collections;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public Mesh mesh = new Mesh();
    public Vector3[] shapeVertices;
    public int[] shapeTriangles;
    public int numCorners { get; set; }

    private int prevNumCorners;

    private void Start()
    {
    }

    public void SetShape()
    {
        // We need at least three corners
        if (numCorners < 3)
            numCorners = 3;

        mesh.Clear();

        // Calculate vertices
        shapeVertices = new Vector3[numCorners];
        for (int i = 0; i < numCorners; ++i)

        {
            float angle = i * (360.0f / numCorners) * Mathf.Deg2Rad;
            shapeVertices[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        }

        // Calculate triangles
        int t = 0;

        shapeTriangles = new int[(numCorners - 2) * 3];
        for (int i = 1; i < numCorners - 1; ++i)
        {
            shapeTriangles[t++] = 0;
            shapeTriangles[t++] = i + 1;
            shapeTriangles[t++] = i;
        }

        mesh.vertices = shapeVertices;
        mesh.triangles = shapeTriangles;

        //mesh.RecalculateNormals();
        //mesh.RecalculateBounds();
    }

    public void AddCorner(Vector2 position)
    {
    }
}