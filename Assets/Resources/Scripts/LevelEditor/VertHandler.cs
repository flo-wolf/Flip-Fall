using FlipFall;
using FlipFall.Audio;
using FlipFall.Levels;
using FlipFall.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates handles and moves the moveArea verticies based on the handler's positions.
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
        public static bool showHandles;

        private Mesh mesh;
        private Vector3[] verts;
        private Vector3 vertPos;
        private GameObject[] handles;

        private bool handlesShown = true;

        // handles currently selected
        public static List<Handle> selectedHandles = new List<Handle>();

        // handles belonging to a triangle that contains a selected handle, while not selected itself
        public static List<Vector3> selectionTriangleVerts = new List<Vector3>();

        // handle that gets dragged but is not selected
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
            showHandles = true;

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
                        handle.transform.localScale = handlePrefab.transform.localScale;

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

        public static void SelectHandle(Handle h)
        {
            selectedHandles.Add(h);

            //LevelEditor.editorMode = LevelEditor.EditorMode.edit;

            Vector3[] vertices = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh.vertices;
            int[] triangles = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh.triangles;
            Vector3[] newSelectionVerts = VertHelper.GetTriangleVerticiesByVertex(vertices, triangles, h.transform.position);
            print("seöectHandle " + newSelectionVerts.Length);

            foreach (Vector3 v in newSelectionVerts)
            {
                selectionTriangleVerts.Add(v);
            }
            SoundManager.PlayLightWobble();

            // there are at lest three verticies in total left and we plan to delete a vertex that is component of only one triangle
            if (vertices.Length > 3 && selectionTriangleVerts.Count <= 3)
                UILevelEditor.DeleteShow(true);
            else
                UILevelEditor.DeleteShow(false);
        }

        public static void DeselectHandle(Handle h)
        {
            selectedHandles.Remove(h);
            SoundManager.PlayLightWobble(0.6F);

            Vector3[] vertices = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh.vertices;
            int[] triangles = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh.triangles;
            Vector3[] newDeselectionVerts = VertHelper.GetTriangleVerticiesByVertex(vertices, triangles, h.transform.position);

            foreach (Vector3 v in newDeselectionVerts)
            {
                selectionTriangleVerts.Remove(v);
            }

            if (selectedHandles.Count == 0)
            {
                selectionTriangleVerts.Clear();
                UILevelEditor.DeleteShow(false);
            }
            else if (vertices.Length > 3 && selectionTriangleVerts.Count <= 3)
                UILevelEditor.DeleteShow(true);
        }

        // destory handles
        public void OnDisable()
        {
            showHandles = false;
            handlesShown = false;
            DestroyHandles();
        }

        public void DestroyHandles()
        {
            GameObject[] handles = GameObject.FindGameObjectsWithTag("handle");
#if UNITY_EDITOR
            //if (EditorApplication.isPlaying)
            //{
            //    UILevelEditor.DeleteShow(false);
            //}
#endif
#if UNITY_ANDROID
            UILevelEditor.DeleteShow(false);
#endif

            foreach (GameObject handle in handles)
            {
                DestroyImmediate(handle);
                handlesShown = false;
            }
            selectionTriangleVerts = new List<Vector3>();
            selectedHandles = new List<Handle>();
        }

        private void SceneChanged(Main.ActiveScene s)
        {
            showHandles = false;
        }

        private bool changesMade = false;
        private bool firstChangeCheck = true;

        // update selection vertices based on handler position
        private void Update()
        {
            if (showHandles && !handlesShown)
                Start();
            else if (showHandles && LevelPlacer.generatedLevel != null && handlesShown)
            {
                //Debug.Log("EditorInput.itemDragged " + EditorInput.vertexDragged);
                if (!EditorInput.vertexDragged)
                {
                    changesMade = false;
                    firstChangeCheck = true;
                }

                handles = GameObject.FindGameObjectsWithTag("handle");
                for (int i = 0; i < verts.Length; i++)
                {
                    Vector3 localHandle = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(handles[i].transform.position);
                    if (verts[i] != localHandle)
                    {
                        EditorInput.vertexDragged = true;
                        if (changesMade == false)
                            changesMade = true;
                    }
                    verts[i] = localHandle;
                }
                mesh.vertices = verts;
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                LevelPlacer.generatedLevel.moveArea.meshFilter.mesh = mesh;

                if (changesMade && firstChangeCheck)
                {
                    //UndoManager.AddUndoPoint();
                    LevelEditor.changesAreSaved = false;
                    firstChangeCheck = false;
                }
            }
            else if (handlesShown && !showHandles)
                DestroyHandles();
        }

        // add a vertex at the given position - called by EditorInput class
        public bool VertexAdd(Vector3 pos)
        {
            Debug.Log("VertexAdd at " + pos);
            if (showHandles && LevelPlacer.generatedLevel != null && handlesShown)
            {
                // two verticies are selected, everything ready for expanding the mesh
                if (selectedHandles.Count == 2)
                {
                    // get the currect verticies
                    Mesh m = new Mesh();
                    m = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh;

                    Vector3 localPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(pos);

                    if (!VertHelper.IsInsideMesh(m, Vector3.zero, localPos))
                    {
                        verts = m.vertices;
                        Vector3[] newVerts = new Vector3[verts.Length + 1];
                        for (int i = 0; i < verts.Length; i++)
                            newVerts[i] = verts[i];

                        // snap the position
                        pos = VertHelper.Snap(pos, true);
                        pos.z = 0;
                        Vector3 localSnapPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(pos);

                        // add the new position to the vertex arrays
                        newVerts[verts.Length] = localSnapPos;

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
                        return true;
                    }
                }
            }
            return false;
        }

        public bool DeleteAllSelectedVerts()
        {
            if (showHandles && LevelPlacer.generatedLevel != null && handlesShown)
            {
                // get the mesh
                Mesh m = new Mesh();
                m = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh;
                Vector3[] vertices = m.vertices;
                int[] triangles = m.triangles;

                // two verticies are selected and there are enough verticies left
                if (selectedHandles.Count > 0 && vertices.Length > selectedHandles.Count + 2)
                {
                    // get the selected vertices about to be deleted in local space
                    int length = selectedHandles.Count;
                    Vector3[] selectedVerts = new Vector3[length];
                    List<int> selectedIndices = new List<int>();
                    for (int i = 0; i < length; i++)
                    {
                        Vector3 local = selectedHandles[i].transform.position;
                        local = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(local);
                        selectedVerts[i] = local;
                    }

                    // get indices of these selected vertices
                    for (int i = 0; i < triangles.Length; i++)
                    {
                        Vector3 p1 = vertices[triangles[i + 0]];
                        foreach (Vector3 v in selectedVerts)
                        {
                            if (v == p1)
                            {
                                selectedIndices.Add(triangles[i]);
                            }
                        }
                    }

                    // find triangles in the triangle array that contain these indicies and remove them
                    List<int> newTriangles = new List<int>();
                    for (int k = 0; k < triangles.Length; k += 3)
                    {
                        int i1 = triangles[k + 0];
                        int i2 = triangles[k + 1];
                        int i3 = triangles[k + 2];

                        // check if the current pointed triangle contains one of the indiciees we want to delete
                        foreach (int index in selectedIndices)
                        {
                            //print("index " + index + " i1: " + i1 + " i2: " + i2 + " i3: " + i3 + " selectedIndicesCount: " + selectedIndices.Count);
                            // if thats not the case, keep them, otherwise dont include them in the new triangle array
                            if (!(index == i1 || index == i2 || index == i3))
                            {
                                // compensate for the deleted verticies by making the indices point to the correct vertex again
                                if (i1 >= index)
                                {
                                    newTriangles.Add(i1 - 1);
                                    //print("remove 1 from entry " + k + " with value " + (i1 - 1));
                                }
                                else
                                {
                                    newTriangles.Add(i1);
                                    //print("entry " + (k + 1) + " with value " + (i1 - 0));
                                }
                                if (i2 >= index)
                                {
                                    newTriangles.Add(i2 - 1);
                                    //print("remove 1 from entry " + (k + 2) + " with value " + (i1 - 1));
                                }
                                else {
                                    newTriangles.Add(i2);
                                    //print("entry " + (k + 2) + " with value " + (i1 - 0));
                                }
                                if (i3 >= index)
                                {
                                    newTriangles.Add(i3 - 1);
                                    //print("remove 1 from entry " + (k + 3) + " with value " + (i1 - 1));
                                }
                                else {
                                    newTriangles.Add(i3);
                                    //print("remove 1 from entry " + (k + 3) + " with value " + (i1 - 0));
                                }
                            }
                        }
                    }

                    // remove all the selected vertices from the mesh's vertices array
                    Vector3[] newVerts = new Vector3[verts.Length - length];
                    for (int j = 0, p = 0; j < vertices.Length; j++)
                    {
                        if (!selectedVerts.Any(x => x == vertices[j]))
                        {
                            newVerts[p] = vertices[j];
                            p++;
                        }
                    }

                    // update the mesh
                    Mesh newMesh = new Mesh();
                    newMesh.vertices = newVerts;
                    newMesh.triangles = newTriangles.ToArray();
                    newMesh.RecalculateBounds();
                    newMesh.RecalculateNormals();
                    mesh = newMesh;
                    LevelPlacer.generatedLevel.moveArea.meshFilter.mesh = newMesh;

                    // recalculate handles
                    selectedHandles = new List<Handle>();
                    DestroyHandles();
                    Start();

                    return true;
                }
            }

            return false;
        }

        // add a vertex at the given position - called by EditorInput class
        private void VertexDelete(Vector3 pos)
        {
        }
    }
}