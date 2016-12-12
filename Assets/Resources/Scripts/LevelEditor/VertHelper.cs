using FlipFall.Levels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Vertex and Mesh Editing Helper class.
/// Position Snapping, Inside-mesh-Checks
/// </summary>

namespace FlipFall.Editor
{
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

            Vector3 correctionDirection = -(currentPos - snapPos).normalized;

            Mesh m = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh;
            Vector3 localSnapPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(snapPos);
            Vector3 localOldPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(currentPos);
            bool inside = IsInsideMesh(m, currentPos, localSnapPos);
            bool crossing = !IsHandlerPositionValid(localOldPos, localSnapPos);

            // we are trying to snap a mesh's vertex to the grid, not a levelObject
            if (vertexSnapping == true)
            {
                if (crossing)
                {
                    int testCount = 0;
                    int multi = 1;
                    Vector3 testSnap = snapPos;
                    while ((inside || crossing) && multi < 3)
                    {
                        Debug.Log("not inside - testcount " + testCount + " - testSnap " + testSnap + " - oldPosition " + localOldPos);
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
                Debug.Log("!inside");
                return FindNextSnapPoint(snapPos, correctionDirection);
            }
            return snapPos;
        }

        private enum SnapPointTestType { upLeft, upRight, lowRight, lowLeft }

        // find the next surrounding possible snapposition inside a mesh, given a failed try
        private static Vector3 FindNextSnapPoint(Vector3 failedSnapPosition, Vector3 correctionDirection)
        {
            Vector3 testSnap = failedSnapPosition;
            SnapPointTestType testType = SnapPointTestType.upLeft;
            Vector3 localTestSnap;
            float snapValue = GridOverlay._instance.smallStep;
            bool inside = false;
            int testCount = 0;

            //Debug.Log("correctionDirection: " + correctionDirection);

            // get what kind of snap got passed
            if (correctionDirection.x >= 0 && correctionDirection.y >= 0)
                testType = SnapPointTestType.upRight;
            else if (correctionDirection.x < 0 && correctionDirection.y >= 0)
                testType = SnapPointTestType.upLeft;
            else if (correctionDirection.x >= 0 && correctionDirection.y < 0)
                testType = SnapPointTestType.lowRight;
            else if (correctionDirection.x < 0 && correctionDirection.y < 0)
                testType = SnapPointTestType.lowLeft;

            //Debug.Log("testType: " + testType);

            // rotate clockwise through the three possible nearby intersections and try each of them, till one is inside the mesh
            while (!inside && testCount < 3)
            {
                //Debug.Log("testCount: " + testCount);
                //testSnap = failedSnapPosition;
                switch (testCount)
                {
                    case 0:
                        switch (testType)
                        {
                            // we started at the upper left, thus now try the upper right
                            case SnapPointTestType.upLeft:
                                testSnap.x += snapValue;
                                break;
                            // we started at the upper right, thus now try the lower right
                            case SnapPointTestType.upRight:
                                testSnap.y -= snapValue;
                                break;

                            case SnapPointTestType.lowRight:
                                testSnap.x -= snapValue;
                                break;

                            case SnapPointTestType.lowLeft:
                                testSnap.y += snapValue;
                                break;
                        }
                        break;

                    case 1:
                        switch (testType)
                        {
                            // we started at the upper left, thus now try the lower right (2nd clockwise postion to check)
                            case SnapPointTestType.upLeft:
                                testSnap.y -= snapValue;
                                break;
                            // we started at the upper right, thus now try the lower left (2nd clockwise postion to check)
                            case SnapPointTestType.upRight:
                                testSnap.x -= snapValue;
                                break;

                            case SnapPointTestType.lowRight:
                                testSnap.y += snapValue;
                                break;

                            case SnapPointTestType.lowLeft:
                                testSnap.x += snapValue;
                                break;
                        }
                        break;

                    case 2:
                        switch (testType)
                        {
                            case SnapPointTestType.upLeft:
                                testSnap.y -= snapValue;
                                break;

                            case SnapPointTestType.upRight:
                                testSnap.x += snapValue;
                                break;

                            case SnapPointTestType.lowRight:
                                testSnap.x += snapValue;
                                break;

                            case SnapPointTestType.lowLeft:
                                testSnap.y -= snapValue;
                                break;
                        }
                        break;
                }

                localTestSnap = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(testSnap);
                inside = IsInsideMesh(LevelPlacer.generatedLevel.moveArea.meshFilter.mesh, Vector3.zero, localTestSnap);
                testCount++;
            }
            if (!inside)
                return Vector3.zero;
            return testSnap;
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
            p.z = 0;

            bool inside = false;

            // cycle through all triangles in the mesh and check if the point p is inside any of the triangles; if so, return true
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
                        // check if point is within the triangle - doesnt include edge cases
                        if ((pPoints[i].y > p.y) != (pPoints[j].y > p.y) &&
                             p.x < (pPoints[j].x - pPoints[i].x) * (p.y - pPoints[i].y) / (pPoints[j].y - pPoints[i].y) + pPoints[i].x)
                        {
                            inside = !inside;
                        }
                        // if the above returned true, check the edges
                        if (!inside && (IsOnLine(pPoints[0], pPoints[1], p) || (IsOnLine(pPoints[1], pPoints[2], p)) || (IsOnLine(pPoints[2], pPoints[0], p))))
                            return true;
                    }
                    if (inside)
                        return inside;
                }
            }
            return inside;
        }

        public static bool IsOnLine(Vector3 start, Vector3 end, Vector3 check)
        {
            //bool onLine = (checkPoint.y - endPoint1.y) / (endPoint2.y - endPoint1.y)
            //    == (checkPoint.x - endPoint1.x) / (endPoint2.x - endPoint1.x);

            var reference = Math.Atan2(start.y - end.y, start.x - end.x);
            var aTanTest = Math.Atan2(start.y - check.y, start.x - check.x);

            //  check from line segment end perspective
            if (reference == aTanTest)
            {
                reference = Math.Atan2(end.y - start.y, end.x - start.x);
                aTanTest = Math.Atan2(end.y - check.y, end.x - check.x);
            }

            return reference == aTanTest;
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
            //Debug.Log("getByVertex verts " + vertices.Length + " tris " + triangles.Length + " vertex " + vertex);
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