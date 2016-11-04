using FlipFall.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the animated colored background
/// </summary>
public class AudioToCircle : MonoBehaviour
{
    public static AudioToCircle _instance;
    private AudioInterpreter audioToWave;
    public MeshFilter mf;
    private Vector3 defaultScale;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    private void Start()
    {
        audioToWave = AudioInterpreter._instance;
        BeginMovement();
        this.GetComponent<MeshRenderer>().sortingOrder = 25;
        defaultScale = transform.localScale;
    }

    public void BeginMovement()
    {
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        int v = 10;
        if (AudioInterpreter.currentValue < 1F && AudioInterpreter.currentValue > 0)
            v = (int)(AudioInterpreter.currentValue * 10000);

        //verticies amount forwarded
        MakeCircle(50);
        ResizeCircle(v);
    }

    public void ResizeCircle(float value)
    {
        value = Mathf.Lerp(transform.localScale.x, value, Time.deltaTime);
        float scaleFactor = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, 1, value));
        scaleFactor = 0.9F;
        //Debug.Log(value + " --- " + scaleFactor);
        transform.localScale = new Vector3(value * scaleFactor, value * scaleFactor, 0);
    }

    public void MakeCircle(int numOfPoints)
    {
        //Debug.Log(numOfPoints + " --- " + AudioToWave.currentValue);
        float angleStep = 360.0f / (float)numOfPoints;
        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();
        Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, angleStep);
        // Make first triangle.
        vertexList.Add(new Vector3(0.0f, 0.0f, 10f));  // 1. Circle center.
        vertexList.Add(new Vector3(0.0f, 0.5f, 10f));  // 2. First vertex on circle outline (radius = 0.5f)
        vertexList.Add(quaternion * vertexList[1]);     // 3. First vertex on circle outline rotated by angle)
                                                        // Add triangle indices.
        triangleList.Add(0);
        triangleList.Add(1);
        triangleList.Add(2);
        for (int i = 0; i < numOfPoints - 1; i++)
        {
            triangleList.Add(0);                      // Index of circle center.
            triangleList.Add(vertexList.Count - 1);
            triangleList.Add(vertexList.Count);
            vertexList.Add(quaternion * vertexList[vertexList.Count - 1]);
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertexList.ToArray();
        mesh.triangles = triangleList.ToArray();
        mf.mesh = mesh;
    }
}