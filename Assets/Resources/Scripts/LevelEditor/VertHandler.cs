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
        public bool showHandles = true;

        private Mesh mesh;
        private Vector3[] verts;
        private Vector3 vertPos;
        private GameObject[] handles;

        private bool handleDrag = false;
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

        public static void SelectHandle(Handle h)
        {
            selectedHandles.Add(h);

            Vector3[] vertices = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh.vertices;
            int[] triangles = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh.triangles;
            Vector3[] newSelectionVerts = VertHelper.GetTriangleVerticiesByVertex(vertices, triangles, h.transform.position);
            print("seöectHandle " + newSelectionVerts.Length);

            foreach (Vector3 v in newSelectionVerts)
            {
                selectionTriangleVerts.Add(v);
            }

            // there are at lest three verticies in total left and we plan to delete a vertex that is component of only one triangle
            if (vertices.Length > 3 && selectionTriangleVerts.Count <= 3)
                UILevelEditor.DeleteShow(true);
            else
                UILevelEditor.DeleteShow(false);
        }

        public static void DeselectHandle(Handle h)
        {
            selectedHandles.Remove(h);

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
        private void OnDisable()
        {
            DestroyHandles();
        }

        private void DestroyHandles()
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
            else if (showHandles && LevelPlacer.generatedLevel != null && handlesShown)
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
                        pos = VertHelper.Snap(pos, false);
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
                    }
                }
            }
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
                            print("index " + index + " i1: " + i1 + " i2: " + i2 + " i3: " + i3 + " selectedIndicesCount: " + selectedIndices.Count);
                            // if thats not the case, keep them, otherwise dont include them in the new triangle array
                            if (!(index == i1 || index == i2 || index == i3))
                            {
                                // compensate for the deleted verticies by making the indices point to the correct vertex again
                                if (i1 >= index)
                                {
                                    newTriangles.Add(i1 - 1);
                                    print("remove 1 from entry " + k + " with value " + (i1 - 1));
                                }
                                else
                                {
                                    newTriangles.Add(i1);
                                    print("entry " + (k + 1) + " with value " + (i1 - 0));
                                }
                                if (i2 >= index)
                                {
                                    newTriangles.Add(i2 - 1);
                                    print("remove 1 from entry " + (k + 2) + " with value " + (i1 - 1));
                                }
                                else {
                                    newTriangles.Add(i2);
                                    print("entry " + (k + 2) + " with value " + (i1 - 0));
                                }
                                if (i3 >= index)
                                {
                                    newTriangles.Add(i3 - 1);
                                    print("remove 1 from entry " + (k + 3) + " with value " + (i1 - 1));
                                }
                                else {
                                    newTriangles.Add(i3);
                                    print("remove 1 from entry " + (k + 3) + " with value " + (i1 - 0));
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

        //public bool DeleteAllSelectedVerts()
        //{
        //    if (showHandles && LevelPlacer.generatedLevel != null && handlesShown)
        //    {
        //        // get the mesh
        //        Mesh m = new Mesh();
        //        m = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh;
        //        Vector3[] vertices = m.vertices;
        //        int[] triangles = m.triangles;

        //        // two verticies are selected and there are enough verticies left
        //        if (selectedHandles.Count > 0 && vertices.Length > selectedHandles.Count + 2)
        //        {
        //            // get the selected vertices about to be deleted in local space
        //            int length = selectedHandles.Count;
        //            Vector3[] selectedVerts = new Vector3[length];
        //            for (int i = 0; i < length; i++)
        //            {
        //                Vector3 local = selectedHandles[i].transform.position;
        //                local = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(local);
        //                selectedVerts[i] = local;
        //            }

        //            List<int> deleteTriangles = new List<int>();
        //            List<int> keepTriangles = new List<int>();

        //            // remove all the selected vertices from the mesh's vertices array
        //            Vector3[] newVerts = new Vector3[verts.Length - length];
        //            for (int j = 0, p = 0; j < vertices.Length; j++)
        //            {
        //                if (!selectedVerts.Any(x => x == vertices[j]))
        //                {
        //                    newVerts[p] = vertices[j];
        //                    p++;
        //                }
        //            }

        //            // compensates for deleted vertices in the triangle array, will be added to the indicies value
        //            int triangleOffset = 0;

        //            // find all triangles connected to the selectedVerts...
        //            for (int i = 0; i < triangles.Length; i += 3)
        //            {
        //                Vector3 p1 = vertices[triangles[i + 0]];
        //                Vector3 p2 = vertices[triangles[i + 1]];
        //                Vector3 p3 = vertices[triangles[i + 2]];

        //                foreach (Vector3 v in selectedVerts)
        //                {
        //                    print("v " + v + " p1 " + p1 + " p2 " + p2 + " p3 " + p3);
        //                    // the currently viewed triangle doesn't contain one of the vertices about to be deleted => keep it
        //                    if ((v == p1 || v == p2 || v == p3))
        //                    {
        //                        triangleOffset += 3;
        //                        print("offset " + triangleOffset);
        //                    }
        //                    else
        //                    {
        //                        print("keepTriangles " + i);
        //                        keepTriangles.Add(i - triangleOffset);
        //                        keepTriangles.Add((i + 1) - triangleOffset);
        //                        keepTriangles.Add((i + 2) - triangleOffset);
        //                    }
        //                }
        //            }

        //            foreach (int i in keepTriangles)
        //            {
        //                print(i);
        //            }

        //            // create a new triangle array containing only triangles about to be kept
        //            int[] newTriangles = new int[keepTriangles.Count];
        //            for (int j = 0; j < newTriangles.Length; j++)
        //            {
        //                newTriangles[j] = keepTriangles[j];
        //                print("newTriangles " + j + " , " + newTriangles[j]);
        //            }
        //            print("newTriangles " + keepTriangles.Count + " verts " + newVerts.Length);

        //            // update the mesh
        //            Mesh newMesh = new Mesh();
        //            newMesh.vertices = newVerts;
        //            newMesh.triangles = newTriangles;
        //            newMesh.RecalculateBounds();
        //            newMesh.RecalculateNormals();
        //            mesh = newMesh;
        //            LevelPlacer.generatedLevel.moveArea.meshFilter.mesh = newMesh;

        //            // recalculate handles
        //            selectedHandles = new List<Handle>();
        //            DestroyHandles();
        //            Start();

        //            return true;
        //        }
        //    }
        //    return false;
        //}

        // add a vertex at the given position - called by EditorInput class
        private void VertexDelete(Vector3 pos)
        {
        }
    }

    public static class VertHelper
    {
        // snap a position to the next allowed position on the grid
        public static Vector3 Snap(Vector3 currentPos, bool vertexSnapping)
        {
            float snapValue = GridOverlay._instance.smallStep;
            Vector3 snapPos = new Vector3
            (
                snapValue * Mathf.Round(currentPos.x / snapValue),
                snapValue * Mathf.Round(currentPos.y / snapValue),
                currentPos.z
            );

            Vector3 correctionDirection = (currentPos - snapPos).normalized;

            Mesh m = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh;
            Vector3 localSnapPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(snapPos);
            Vector3 localOldPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(currentPos);
            bool inside = IsInsideMesh(m, currentPos, localSnapPos);
            bool crossing = !IsHandlerPositionValid(localOldPos, localSnapPos);

            if (vertexSnapping == true)
            {
                if (inside || crossing)
                {
                    int testCount = 0;
                    int multi = 1;
                    Vector3 testSnap = snapPos;
                    while ((inside || crossing) && multi < 3)
                    {
                        testSnap = snapPos;
                        switch (testCount)
                        {
                            case 0:
                                testSnap.x -= snapValue * multi;
                                testSnap.y -= snapValue * multi;
                                break;

                            case 1:
                                testSnap.x += snapValue * multi;
                                testSnap.y -= snapValue * multi;
                                break;

                            case 2:
                                testSnap.x -= snapValue * multi;
                                testSnap.y += snapValue * multi;
                                break;

                            case 3:
                                testSnap.x += snapValue * multi;
                                testSnap.y += snapValue * multi;
                                testCount = 0;
                                multi++;
                                break;
                        }

                        localSnapPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(testSnap);
                        inside = IsInsideMesh(m, localOldPos, localSnapPos);
                        crossing = !IsHandlerPositionValid(localOldPos, localSnapPos);
                        testCount++;
                    }
                    if (!inside)
                    {
                        snapPos = testSnap;
                    }
                }

                Debug.Log("INSIDE????? Snap sais = " + inside);

                // update the selection triangle verts to fit the snapped position
                Vector3 localnewPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(snapPos);
                // get all vector entries that are equal to this position and replace them with the newest position
                List<int> indexes = Enumerable.Range(0, VertHandler.selectionTriangleVerts.Count).Where(k => VertHandler.selectionTriangleVerts[k] == localOldPos).ToList();

                foreach (int ind in indexes)
                {
                    VertHandler.selectionTriangleVerts[ind] = localnewPos;
                }
            }
            // levelObject snapping - the suggested snap position is outside the moveArea, which is not valid -> snap inside
            else if (!inside)
            {
                // replace with correct snapping algortithm
                return Vector3.zero;
            }
            return snapPos;
        }

        // prevent verticies from crossing the line between the two opposing verticies in a triangle, which would create swapped meshes
        public static bool IsHandlerPositionValid(Vector3 oldPos, Vector3 destination)
        {
            // get the vertices and
            Vector3[] verts = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh.vertices;
            int[] triangles = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh.triangles;

            // transform the input values into local space
            //destination = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(destination);
            //oldPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(oldPos);

            // find triangles this handler is connected with
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 p1 = verts[triangles[i + 0]];
                Vector3 p2 = verts[triangles[i + 1]];
                Vector3 p3 = verts[triangles[i + 2]];

                // this triangle contains our handler's vector
                if (oldPos == p1)
                {
                    // the destination position is on the other side of the line
                    if (IsLeft(p2, p3, p1) != IsLeft(p2, p3, destination))
                    {
                        return false;
                    }
                }
                else if (oldPos == p2)
                {
                    // the destination position is on the other side of the line
                    if (IsLeft(p1, p3, p2) != IsLeft(p1, p3, destination))
                        return false;
                }
                else if (oldPos == p3)
                {
                    // the destination position is on the other side of the line
                    if (IsLeft(p1, p2, p3) != IsLeft(p1, p2, destination))
                        return false;
                }
            }
            return true;
        }

        // is the next point p inside the movearea mesh?
        static public bool IsInsideMesh(Mesh m, Vector3 oldPos, Vector3 p)
        {
            Vector3[] vertices = m.vertices;
            int[] triangles = m.triangles;
            Vector2 point = oldPos;

            bool inside = false;

            for (int c = 0; c < triangles.Length; c += 3)
            {
                Vector2[] pPoints = new Vector2[3];
                pPoints[0] = vertices[triangles[c]];
                pPoints[1] = vertices[triangles[c + 1]];
                pPoints[2] = vertices[triangles[c + 2]];

                if (!(pPoints[0] == point || pPoints[1] == point || pPoints[2] == point))
                {
                    //Debug.Log("inside: p " + p + " p0 " + pPoints[0] + " p1 " + pPoints[1] + " p2 " + pPoints[2]);

                    inside = false;
                    for (int i = 0, j = pPoints.Length - 1; i < pPoints.Length; j = i++)
                    {
                        if ((pPoints[i].y > p.y) != (pPoints[j].y > p.y) &&
                             p.x < (pPoints[j].x - pPoints[i].x) * (p.y - pPoints[i].y) / (pPoints[j].y - pPoints[i].y) + pPoints[i].x)
                        {
                            inside = !inside;
                        }
                    }
                    if (inside)
                        return inside;
                }
            }
            return inside;
        }

        // check if point c is on the left of a line drawn between a and b
        public static bool IsLeft(Vector3 a, Vector3 b, Vector3 c)
        {
            return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
        }

        // returns all vertices that are part of a triangle from an input vertex.
        // return length will always be a multiple of 3.
        public static Vector3[] GetTriangleVerticiesByVertex(Vector3[] vertices, int[] triangles, Vector3 vertex)
        {
            vertex = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(vertex);
            Debug.Log("getByVertex verts " + vertices.Length + " tris " + triangles.Length + " vertex " + vertex);
            List<Vector3> triangleVerts = new List<Vector3>();

            // find triangles this handler is connected with
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 p1 = vertices[triangles[i + 0]];
                Vector3 p2 = vertices[triangles[i + 1]];
                Vector3 p3 = vertices[triangles[i + 2]];

                // this triangle contains our handler's vector
                if (vertex == p1 || vertex == p2 || vertex == p3)
                {
                    triangleVerts.Add(p1);
                    triangleVerts.Add(p2);
                    triangleVerts.Add(p3);
                }
            }
            return triangleVerts.ToArray();
        }

        public static int[] GetTriangleIndicesByVertex(Vector3[] vertices, int[] triangles, Vector3 vertex)
        {
            vertex = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(vertex);
            List<int> triangleIndices = new List<int>();

            // find triangles this handler is connected with
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 p1 = vertices[triangles[i + 0]];
                Vector3 p2 = vertices[triangles[i + 1]];
                Vector3 p3 = vertices[triangles[i + 2]];

                // this triangle contains our handler's vector
                if (vertex == p1 || vertex == p2 || vertex == p3)
                {
                    triangleIndices.Add(i + 0);
                    triangleIndices.Add(i + 1);
                    triangleIndices.Add(i + 2);
                }
            }
            return triangleIndices.ToArray();
        }

        public static Vector3[] GetTriangleVerticesByIndex(Vector3[] vertices, int[] triangles, int index)
        {
            List<Vector3> triangleVertices = new List<Vector3>();
            Vector3 vertex = vertices[index];

            // find triangles this handler is connected with
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 p1 = vertices[triangles[i + 0]];
                Vector3 p2 = vertices[triangles[i + 1]];
                Vector3 p3 = vertices[triangles[i + 2]];

                // this triangle contains our handler's vector
                if (vertex == p1 || vertex == p2 || vertex == p3)
                {
                    triangleVertices.Add(p1);
                    triangleVertices.Add(p2);
                    triangleVertices.Add(p3);
                }
            }
            return triangleVertices.ToArray();
        }

        public static int[] GetTriangleIndicesByIndex(Vector3[] vertices, int[] triangles, int index)
        {
            List<int> triangleIndices = new List<int>();
            Vector3 vertex = vertices[index];

            // find triangles this handler is connected with
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 p1 = vertices[triangles[i + 0]];
                Vector3 p2 = vertices[triangles[i + 1]];
                Vector3 p3 = vertices[triangles[i + 2]];

                // this triangle contains our handler's vector
                if (vertex == p1 || vertex == p2 || vertex == p3)
                {
                    triangleIndices.Add(i + 0);
                    triangleIndices.Add(i + 1);
                    triangleIndices.Add(i + 2);
                }
            }
            return triangleIndices.ToArray();
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