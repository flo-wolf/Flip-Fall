using Impulse.Audio;
using Impulse.Progress;
using Impulse.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Impulse.Background
{
    public class WaveGenerator : MonoBehaviour
    {
        public static WaveGenerator _instance;
        public static WaveMeshUpdateEvent onMeshUpdate = new WaveMeshUpdateEvent();

        public class WaveMeshUpdateEvent : UnityEvent<Mesh> { }

        // toggle audio wave gathering
        public float wavePeak = 0.2F;
        public float lPeak = 0.1F;
        public float uPeak = 0.2F;
        private float amplitude;
        private float randAmplitude;

        // should waves get generated? Can be turned off to save performance
        public static bool generateWaves = true;

        // amplitude of each wave generated through sound or by random
        public static float bgAmplitude = 1F;

        // size of each horizonpart
        public static int bgSize = 32;

        public static float waveStopDuration = 1F;

        private float newestAudioValue = 0F;
        private float lerpedAudioValue = 0F;

        // final mesh, applied to all WaveSetters added to HorizonPart-GameObjects
        public static Mesh waveMesh;

        // The wave points along the top of the mesh
        private Vector3[] points = null;

        // The wave points along the top of the mesh of the last update call
        private Vector3[] lastPoints = null;

        // Mutable lists for all the vertices and triangles of the mesh
        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();

        // original height of the mesh around which the waves will allocate
        public float height = 1F;

        private bool started = false;

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

        private void Start()
        {
            if (started == false)
            {
                waveMesh = new Mesh();
                started = true;
            }

            bgAmplitude = ProgressManager.GetProgress().settings.backgroundSpeed;
            UISettingsManager.onHorizonSpeedChange.AddListener(HorizonSpeedChanged);

            onMeshUpdate.Invoke(Prewarm());
        }

        // EventListener, UISettingsManager.onHorizonSpeedChange
        private void HorizonSpeedChanged(float f)
        {
            bgAmplitude = f;
            if (f == 5)
            {
                StopAllCoroutines();
                StartCoroutine(cStopLerp(waveStopDuration));
            }
        }

        private IEnumerator cStopLerp(float duration)
        {
            float t = 0;
            while (t < 1.0f)
            {
                t += Time.deltaTime * (Time.timeScale / duration);

                bgAmplitude = Mathf.SmoothStep(bgAmplitude, 0, t);
                yield return 0;
            }

            generateWaves = false;

            yield break;
        }

        private Mesh Prewarm()
        {
            waveMesh.Clear();

            // Generate 64 random points for the top (i.e. the actual wave)
            points = new Vector3[bgSize];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Vector3(0.5f * i, height, 0f);
            }

            lastPoints = points;

            //add buttom curves and fill verticie and triangle arrays
            int resolution = 29;
            for (int i = 0; i < resolution; i++)
            {
                float t = (float)i / (float)(resolution - 1);
                // Get the point on our curve using the 4 points generated above
                Vector3 p = CalculateBezierPoint(t, points[i], points[i + 1], points[i + 2], points[i + 3]);
                AddWavePoint(p);
            }

            waveMesh.vertices = vertices.ToArray();
            waveMesh.triangles = triangles.ToArray();

            vertices.Clear();
            triangles.Clear();

            return waveMesh;
        }

        private void FixedUpdate()
        {
            onMeshUpdate.Invoke(UpdateMesh());
        }

        private Mesh UpdateMesh()
        {
            if (waveMesh != null && generateWaves)
            {
                waveMesh.Clear();

                // Generate 64 random points for the top (i.e. the actual wave)
                points = new Vector3[bgSize];
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

                            amplitude = Mathf.Lerp(lastPoints[i].y, height + newestAudioValue, Time.deltaTime * bgAmplitude);
                            //if (lerpedAudioValue > hWave)
                            //    hWave = lerpedAudioValue;
                            //amplitude = Mathf.Lerp(lPeak, uPeak, Mathf.InverseLerp(0, hWave, lerpedAudioValue));

                            //there is audio
                            if (amplitude < 1 && newestAudioValue > 0.05)
                            {
                                points[i] = new Vector3(0.5f * (float)i, Mathf.Abs(Mathf.Lerp(lastPoints[i].y, amplitude, Time.time)), 0f);
                            }

                            //there is no audio, switch to random input
                            else
                            {
                                randAmplitude = Mathf.Lerp(lastPoints[i].y, height + Mathf.Abs(Random.Range(lPeak, uPeak)), Time.deltaTime * bgAmplitude);
                                points[i] = new Vector3(0.5f * (float)i, Mathf.Abs(Mathf.Lerp(lastPoints[i].y, randAmplitude, Time.time)), 0f);
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

                // Number of points to draw, how smooth the curve is. has to be smaller than points.Legth+3.
                int resolution = 29;
                for (int i = 0; i < resolution; i++)
                {
                    float t = (float)i / (float)(resolution - 1);
                    // Get the point on our curve using the 4 points generated above
                    Vector3 p = CalculateBezierPoint(t, points[i], points[i + 1], points[i + 2], points[i + 3]);
                    AddWavePoint(p);
                }

                // Assign the vertices and triangles to the mesh
                waveMesh.vertices = vertices.ToArray();
                waveMesh.triangles = triangles.ToArray();

                vertices.Clear();
                triangles.Clear();
            }

            return waveMesh;
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
    }
}