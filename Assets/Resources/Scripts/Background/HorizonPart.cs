using Impulse.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizonPart : MonoBehaviour
{
    public int ID = 1;
    public float timeBetweenVertAdd = 0.5F;
    public MeshFilter mf;
    public bool updating = true;
    public float vertMovementSpeed = 10F;
    public float hoMovementSpeed = 10F;
    public float leftBorder = -2000F;
    public float rightBorder = 100F;
    public int nbrOfButtomVerts = 10;

    //private Vector3[] vertSafe;
    //private Vector3[] normSafe;

    //backup from last modification
    private Vector3 normDefault;

    private Mesh originalmesh;
    private Vector3[] originalVert;
    private Vector3[] originalNorm;
    private Vector2[] originalUv;
    private int[] originalTri;
    private int verticesAmount = 10;

    //the curve the lined up verticies should display
    private Vector2[] curve;
    private float currentAmplitude = 0;
    private float newAmplitude = 0;
    private float bottom;

    //index which indicates the point in the verticies array from which new vertices were added
    private int firstAddedVerticesIndex;

    private void Awake()
    {
        if (mf == null)
            mf = GetComponent<MeshFilter>();
        originalmesh = mf.mesh;
        bottom = -(mf.mesh.bounds.size.x / 2);
        normDefault = mf.mesh.normals[0];
        originalVert = mf.mesh.vertices;
        originalNorm = mf.mesh.normals;
        originalUv = mf.mesh.uv;
        originalTri = mf.mesh.triangles;
    }

    private void Start()
    {
        //this.GetComponent<MeshRenderer>().sortingOrder = 50;
        StartCoroutine(CalcAmplitude());
        DontDestroyOnLoad(this);
    }

    private void FixedUpdate()
    {
        currentAmplitude = Mathf.Lerp(currentAmplitude, newAmplitude, Time.deltaTime);
        //MoveVerticies();
        //Debug.Log("NEW --- " + newAmplitude);
        //Debug.Log("CUR --- " + currentAmplitude);
        //UpdateVertices();
    }

    public void MoveVerticies()
    {
        Vector3[] newVerts = originalVert;

        int id = 0;

        float t = Mathf.Sin(Time.time);
        //get top verts
        for (int i = 0; i < originalVert.Length; i++)
        {
            //its on the bottom line, modify the vertex

            if (originalVert[i].z == bottom)
            {
                Debug.Log("MOD" + Time.time);
                //float newX = Mathf.Lerp(newVerts[i].x - 1, newVerts[i].x + 1, Mathf.Sin(Time.deltaTime));
                //Debug.Log(newX);
                newVerts[i] = new Vector3(newVerts[i].x, newVerts[i].y, newVerts[i].z - Time.time);
                id++;
            }
        }

        Mesh msh = mf.mesh;
        msh.Clear();
        msh.vertices = newVerts;
        msh.uv = originalUv;
        msh.triangles = originalTri;
        msh.RecalculateBounds();
        msh.RecalculateNormals();
        mf.sharedMesh = msh;
        //Debug.Log(mf.sharedMesh.vertices[120]);
        //mf.sharedMesh = null;
        //mf.sharedMesh = m;
    }

    //Adds vertices to the butttom edge of the input quad for later modification
    private Mesh AddBottomVertices(Mesh msh)
    {
        Vector3 buttomLeft = new Vector3(-0.5F, -0.5F, transform.position.z);
        Vector3 buttomRight = new Vector3(0.5F, -0.5F, transform.position.z);

        Vector3[] vertices = msh.vertices;
        Vector3[] normals = msh.normals;

        firstAddedVerticesIndex = vertices.Length;

        Vector3[] newVerts = new Vector3[vertices.Length + nbrOfButtomVerts];
        Vector3[] newNorms = new Vector3[vertices.Length + nbrOfButtomVerts];

        //add the existing vertices into the new array
        for (int n = 0; n < firstAddedVerticesIndex; n++)
        {
            newVerts[n] = vertices[n];
            newNorms[n] = normals[0];
            Debug.Log(newVerts[n]);
        }

        Debug.Log("fistIndex: " + firstAddedVerticesIndex + " verticies.Length: " + newVerts.Length);

        //add the new vertices into the new array
        int p = 1;
        for (int i = firstAddedVerticesIndex; i < newVerts.Length - 1; i++)
        {
            //float distance = 1F;
            newVerts[i] = new Vector3(-0.5F + ((1F / nbrOfButtomVerts) * p), buttomLeft.y, buttomLeft.z);
            newNorms[i] = normals[0];
            Debug.Log(newVerts[i] + "---" + ((1F / nbrOfButtomVerts) * p));
            // Debug.Log(distance + " - " + buttomLeft.x + " - " + buttomRight.x + " - " + ((buttomRight.x / nbrOfButtomVerts) * p));
            p++;
        }

        //set the new verticies to the old mesh and return it
        Mesh m = new Mesh();
        m.vertices = newVerts;
        m.normals = newNorms;
        //m.RecalculateNormals();
        //m.RecalculateBounds();
        mf.mesh = m;
        return m;
    }

    private IEnumerator CalcAmplitude()
    {
        while (updating)
        {
            if (AudioInterpreter.currentValue < 1F && AudioInterpreter.currentValue > 0)
                newAmplitude = AudioInterpreter.currentValue * 100;
            else
                newAmplitude = 0;
            Debug.Log("NEW --- " + newAmplitude);
            MoveVerticies();
            yield return new WaitForSeconds(timeBetweenVertAdd);
        }
        yield break;
    }

    //public void UpdateVertices()
    //{
    //    //Debug.Log(f);

    //    //cycle through all filters here

    //    Vector3[] newVerts = mf.mesh.vertices;
    //    Vector3[] newNorms = mf.mesh.normals;

    //    ////Copy the current vertices
    //    //for (int n = 0; n < newVerts.Length; n++)
    //    //{
    //    //    newVerts[n] = mesh.vertices[n];
    //    //    newNorms[n] = mesh.normals[n];
    //    //}
    //    for (int i = 0; i < newVerts.Length; i++)
    //    {
    //        //vertices[i] += normals[i] * Mathf.Sin(Time.time);
    //        //vertices[i] = mesh.vertices[i] * Mathf.Lerp(0.9F, 1.1F, Mathf.InverseLerp(0, 10, f));

    //        //Movement to the right

    //        //newVerts[i] = new Vector3(vertices[i].x, vertices[i].y, vertices[i].z);
    //        if (newVerts[i].x > rightBorder)
    //        {
    //            Debug.Log(newVerts[i].x);
    //            newVerts[i] = vertSafe[i];
    //        }
    //    }

    //    Mesh m = new Mesh();
    //    m.vertices = newVerts;
    //    m.normals = newNorms;
    //    m.RecalculateNormals();
    //    m.RecalculateBounds();
    //    mf.mesh = m;
    //    Debug.Log(msh.vertices != vertices);
    //    msh.vertices = vertices;
    //}

    //public float CalcAmplitude(float value)
    //{
    //    float f;
    //    value = Mathf.Lerp(transform.localScale.x, value, Time.deltaTime);
    //    float scaleFactor = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, 1, value));
    //    scaleFactor = 0.9F;
    //    Debug.Log(value + " --- " + scaleFactor);
    //    transform.localScale = new Vector3(value * scaleFactor, value * scaleFactor, 0);

    //    return f;
    //}
}