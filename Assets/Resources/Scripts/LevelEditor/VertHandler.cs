using FlipFall;
using FlipFall.Levels;
using FlipFall.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Created handles and moves the moveArea verticies based on the handler's positions.
/// </summary>

namespace FlipFall.Editor
{
    [ExecuteInEditMode]
    public class VertHandler : MonoBehaviour
    {
        public static VertHandler _instance;

        public GameObject handlePrefab;
        public GameObject handleParent;
        public Camera editorCamera;

        public int handleSize = 50;
        public bool showHandles = true;

        private Mesh mesh;
        private Vector3[] verts;
        private Vector3 vertPos;
        private GameObject[] handles;

        private bool handleDrag = false;
        private bool handlesShown = true;

        public static List<Handle> selectedHandles = new List<Handle>();
        public static Handle quickDragHandle;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this);

            selectedHandles = new List<Handle>();

            // destroy leftover handles
            DestroyHandles();

            Main.onSceneChange.AddListener(SceneChanged);
        }

        private void Start()
        {
            if (LevelPlacer.generatedLevel != null)
            {
                mesh = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh;
                verts = mesh.vertices;
                handlesShown = true;

                // crate handles
                if (showHandles)
                {
                    foreach (Vector3 vert in verts)
                    {
                        vertPos = LevelPlacer.generatedLevel.moveArea.transform.TransformPoint(vert);
                        GameObject handle = Instantiate(handlePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                        handle.name = "handle";
                        handle.tag = "handle";
                        handle.layer = LayerMask.NameToLayer("Handles");
                        handle.transform.localScale = new Vector3(1, 1, 1);

                        if (handleParent != null)
                            handle.transform.parent = handleParent.transform;
                        else
                            handle.transform.parent = transform;

                        handle.transform.position = vertPos;

                        //print(vertPos);
                    }
                }
            }
        }

        // destory handles
        private void OnDisable()
        {
            DestroyHandles();
        }

        private void DestroyHandles()
        {
            UILevelEditor.DeleteShow(false);
            GameObject[] handles = GameObject.FindGameObjectsWithTag("handle");
            foreach (GameObject handle in handles)
            {
                DestroyImmediate(handle);
                handlesShown = false;
            }
        }

        private void SceneChanged(Main.Scene s)
        {
            showHandles = false;
        }

        public void OnClick()
        {
            handleDrag = true;
        }

        public void OnRelease()
        {
            handleDrag = false;
        }

        // update selection vertices based on handler position
        private void Update()
        {
            if (showHandles && !handlesShown)
                Start();
            else if (showHandles && LevelPlacer.generatedLevel != null && handlesShown && EditorInput.itemDragged)
            {
                handles = GameObject.FindGameObjectsWithTag("handle");
                for (int i = 0; i < verts.Length; i++)
                {
                    Vector3 localHandle = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(handles[i].transform.position);
                    if (verts[i] != localHandle)
                        LevelEditor.changesAreSaved = false;
                    verts[i] = localHandle;
                }
                mesh.vertices = verts;
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                LevelPlacer.generatedLevel.moveArea.meshFilter.mesh = mesh;
            }
            else if (handlesShown && !showHandles)
                DestroyHandles();
        }

        // add a vertex at the given position - called by EditorInput class
        public void VertexAdd(Vector3 pos)
        {
            if (showHandles && LevelPlacer.generatedLevel != null && handlesShown)
            {
                // two verticies are selected, everything ready for expanding the mesh
                if (selectedHandles.Count == 2)
                {
                    print("VertexAdd() at " + pos);
                    // get the currect verticies
                    Mesh m = new Mesh();
                    m = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh;
                    verts = m.vertices;
                    Vector3[] newVerts = new Vector3[verts.Length + 1];
                    for (int i = 0; i < verts.Length; i++)
                        newVerts[i] = verts[i];

                    // add the new position to the vertex arrays
                    pos.z = 0;
                    newVerts[verts.Length] = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(pos);

                    // get the triangles
                    int[] triangles = new int[m.triangles.Length + 3];
                    for (int s = 0; s < m.triangles.Length; s++)
                    {
                        triangles[s] = m.triangles[s];
                    }

                    // add a new triangle by referencing the two selected verticies plus the new one
                    // add selected verticies into a temporary storage
                    int[] newIndicies = new int[3];
                    newIndicies[0] = newVerts.Length - 1;
                    for (int i = 0; i < newVerts.Length; i++)
                    {
                        // the interated vertex fits to the first selected handler
                        if (LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(selectedHandles[0].transform.position) == newVerts[i])
                        {
                            newIndicies[1] = i;
                        }
                        // the interated vertex fits to the second selected handler
                        else if (LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(selectedHandles[1].transform.position) == newVerts[i])
                        {
                            newIndicies[2] = i;
                        }
                    }

                    // sort the triangle verticies in a clockwise order to ensure the triangle faces our direction and wont get rendered backwards
                    Vector3[] triangleVerts = new Vector3[3];
                    triangleVerts[0] = newVerts[newIndicies[0]];
                    triangleVerts[1] = newVerts[newIndicies[1]];
                    triangleVerts[2] = newVerts[newIndicies[2]];
                    // calculate the center of the triangle
                    Vector2 center = (triangleVerts[0] + triangleVerts[1] + triangleVerts[2]) / 3;
                    Array.Sort(triangleVerts, new ClockwiseComparer(center));
                    for (int i = 0; i < newVerts.Length; i++)
                    {
                        // the interated vertex fits to the first selected handler
                        if (triangleVerts[0] == newVerts[i])
                        {
                            newIndicies[0] = i;
                        }
                        // the interated vertex fits to the second selected handler
                        else if (triangleVerts[1] == newVerts[i])
                        {
                            newIndicies[1] = i;
                        }
                        else if (triangleVerts[2] == newVerts[i])
                        {
                            newIndicies[2] = i;
                        }
                    }

                    // add the sorted indicies to the triangles array
                    triangles[m.triangles.Length] = newIndicies[0];
                    triangles[m.triangles.Length + 1] = newIndicies[1];
                    triangles[m.triangles.Length + 2] = newIndicies[2];

                    // update the mesh
                    m.vertices = newVerts;
                    m.triangles = triangles;
                    m.RecalculateBounds();
                    m.RecalculateNormals();
                    mesh = m;
                    LevelPlacer.generatedLevel.moveArea.meshFilter.mesh = m;

                    // recalculate handles
                    selectedHandles = new List<Handle>();
                    DestroyHandles();
                    Start();
                }
            }
        }

        public void DeleteAllSelectedVerts()
        {
        }

        // add a vertex at the given position - called by EditorInput class
        private void VertexDelete(Vector3 pos)
        {
        }
    }

    // compares for clockwise vertex positioning around a point
    public class ClockwiseComparer : IComparer<Vector3>
    {
        private Vector3 m_Origin;

        #region Properties

        /// <summary>
        ///     Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public Vector3 origin { get { return m_Origin; } set { m_Origin = value; } }

        #endregion Properties

        /// <summary>
        ///     Initializes a new instance of the ClockwiseComparer class.
        /// </summary>
        /// <param name="origin">Origin.</param>
        public ClockwiseComparer(Vector3 origin)
        {
            m_Origin = origin;
        }

        #region IComparer Methods

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="first">First.</param>
        /// <param name="second">Second.</param>
        public int Compare(Vector3 v1, Vector3 v2)
        {
            return IsClockwise(v2, v1, m_Origin);
        }

        #endregion IComparer Methods

        /// <summary>
        ///     Returns 1 if first comes before second in clockwise order.
        ///     Returns -1 if second comes before first.
        ///     Returns 0 if the points are identical.
        /// </summary>
        /// <param name="first">First.</param>
        /// <param name="second">Second.</param>
        /// <param name="origin">Origin.</param>
        public static int IsClockwise(Vector3 first, Vector3 second, Vector3 origin)
        {
            if (first == second)
                return 0;

            Vector3 firstOffset = first - origin;
            Vector3 secondOffset = second - origin;

            float angle1 = Mathf.Atan2(firstOffset.x, firstOffset.y);
            float angle2 = Mathf.Atan2(secondOffset.x, secondOffset.y);

            if (angle1 < angle2)
                return 1;

            if (angle1 > angle2)
                return -1;

            // Check to see which point is closest
            return (firstOffset.sqrMagnitude < secondOffset.sqrMagnitude) ? 1 : -1;
        }
    }
}