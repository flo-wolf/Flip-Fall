using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditorVerticesModifier : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] verts;
    private Vector3 vertPos;
    private GameObject[] handles;

    private void OnEnable()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        verts = mesh.vertices;
        foreach (Vector3 vert in verts)
        {
            vertPos = transform.TransformPoint(vert);
            GameObject handle = new GameObject("handle");
            handle.transform.position = vertPos;
            handle.transform.parent = transform;
            handle.tag = "handle";
            //handle.AddComponent<Gizmo_Sphere>();
            print(vertPos);
        }
    }

    private void OnDisable()
    {
        GameObject[] handles = GameObject.FindGameObjectsWithTag("handle");
        foreach (GameObject handle in handles)
        {
            DestroyImmediate(handle);
        }
    }

    private void Update()
    {
        handles = GameObject.FindGameObjectsWithTag("handle");
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = handles[i].transform.localPosition;
        }
        mesh.vertices = verts;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}