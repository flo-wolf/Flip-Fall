using Impulse.Audio;
using Impulse.Progress;
using Impulse.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGenerator : MonoBehaviour
{
    public int id;

    // toggle audio wave gathering
    public bool updating = true;
    public float updatingDelay = 0.5F;
    public float wavePeak = 0.2F;
    public float lPeak = 0.1F;
    public float uPeak = 0.2F;
    private float amplitude;
    private float randAmplitude;
    public float backgroundSpeed = 10;

    private float newestAudioValue = 0F;
    private float lerpedAudioValue = 0F;

    // Reference to the mesh we will generate
    private Mesh mesh = null;

    // The wave points along the top of the mesh
    private Vector3[] points = null;

    // The wave points along the top of the mesh of the last update call
    private Vector3[] lastPoints = null;

    // Mutable lists for all the vertices and triangles of the mesh
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    // original height of the mesh around which the waves will allocate
    private float height;

    private void Start()
    {
        GetComponent<MeshRenderer>().sortingOrder = id;
        height = GetComponent<MeshFilter>().mesh.bounds.extents.y;
        backgroundSpeed = ProgressManager.GetProgress().settings.backgroundSpeed;

        UISettingsManager.onHorizonSpeedChange.AddListener(HorizonSpeedChanged);
        //StartCoroutine(CalcAudio());
    }

    private void HorizonSpeedChanged(float f)
    {
        Debug.Log(f);
        backgroundSpeed = f;
    }

    private void FixedUpdate()
    {
        //Debug.Log(lerpedAudioValue);

        // Update the mesh and insert the newest spectrumdata as the newest vertex
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        // Get a reference to the mesh component and clear it
        MeshFilter filter = GetComponent<MeshFilter>();
        mesh = filter.mesh;
        mesh.Clear();
        float hWave = 10F;

        // Generate 4 (or more) random points for the top (i.e. the actual wave)
        points = new Vector3[64];
        for (int i = 0; i < points.Length; i++)
        {
            // if there are no lastPoints to lerp from create random points
            if (lastPoints == null)
            {
                points[i] = new Vector3(0.5f * (float)i, Random.Range(lPeak, uPeak), 0f);
            }
            else
            {
                // assign newest amplitude on the very left generated through the audio data
                if (i == 0)
                {
                    // Get the spectrumdata of the background music and lerp its updated value
                    if (AudioInterpreter.currentValue < 1F && AudioInterpreter.currentValue > 0)
                        newestAudioValue = AudioInterpreter.currentValue * 10;
                    else
                        newestAudioValue = 0;

                    amplitude = Mathf.Lerp(lastPoints[i].y, 0.5F + newestAudioValue, Time.deltaTime * backgroundSpeed);
                    //Debug.Log(amplitude);

                    //if (lerpedAudioValue > hWave)
                    //    hWave = lerpedAudioValue;

                    //amplitude = Mathf.Lerp(lPeak, uPeak, Mathf.InverseLerp(0, hWave, lerpedAudioValue));

                    //there is audio hearable
                    if (amplitude < 1 && newestAudioValue > 0.05)
                    {
                        //Debug.Log(lerpedAudioValue);
                        points[i] = new Vector3(0.5f * (float)i, Mathf.Abs(Mathf.Lerp(lastPoints[i].y, amplitude, Time.time)), 0f);
                    }
                    //there is no audio, switch to random input
                    else
                    {
                        randAmplitude = Mathf.Lerp(lastPoints[i].y, 0.5F + Mathf.Abs(Random.Range(lPeak, uPeak)), Time.deltaTime * backgroundSpeed);
                        points[i] = new Vector3(0.5f * (float)i, Mathf.Abs(Mathf.Lerp(lastPoints[i].y, randAmplitude, Time.time)), 0f);
                        //Debug.Log("2");
                        //Random.Range(lastPoints[i].y - wavePeak, lastPoints[i].y + wavePeak)
                    }
                }
                // shift each point one to the right - execute these in coroutine for customizable delays, through slider
                else
                {
                    points[i] = new Vector3(0.5f * (float)i, lastPoints[i - 1].y, 0f);
                }
            }

            //AddWavePoint(points[i]);
        }
        lastPoints = points;

        // Number of points to draw, how smooth the curve is. has to be smaller than points.Legth+4.
        int resolution = 20;
        for (int i = 0; i < resolution; i++)
        {
            float t = (float)i / (float)(resolution - 1);
            // Get the point on our curve using the 4 points generated above
            Vector3 p = CalculateBezierPoint(t, points[i], points[i + 1], points[i + 2], points[i + 3]);
            AddWavePoint(p);
        }

        // Assign the vertices and triangles to the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        vertices.Clear();
        triangles.Clear();
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    private void AddWavePoint(Vector3 point)
    {
        // Create a corresponding point along the bottom
        vertices.Add(new Vector3(point.x, 0f, 0f));
        // Then add our top point
        vertices.Add(point);
        if (vertices.Count >= 4)
        {
            // We have completed a new quad, create 2 triangles
            int start = vertices.Count - 4;
            triangles.Add(start + 0);
            triangles.Add(start + 1);
            triangles.Add(start + 2);
            triangles.Add(start + 1);
            triangles.Add(start + 3);
            triangles.Add(start + 2);
        }
    }

    private IEnumerator CalcAudio()
    {
        while (updating)
        {
            if (AudioInterpreter.currentValue < 1F && AudioInterpreter.currentValue > 0)
                newestAudioValue = AudioInterpreter.currentValue * 100;
            else
                newestAudioValue = 0;
            yield return new WaitForSeconds(updatingDelay);
        }
        yield break;
    }
}