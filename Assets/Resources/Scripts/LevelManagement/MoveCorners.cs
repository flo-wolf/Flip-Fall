using FlipFall;
using FlipFall.Levels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves the verticies on the Outside of the MoveArea (verticies on levelbounds)
/// </summary>
public class MoveCorners : MonoBehaviour
{
    // amount of time needed for one movement pattern to complete
    public float rotationSpeed;

    // original position of all Corners
    public Vector3[] corners;

    public Mesh mergedMesh;

    // next postition
    private Vector3 nextPosition;

    private void Start()
    {
        // has to be executed after a level got placed, and LevelPlacer.playcedLevel != null
        GetCorners();
    }

    private void GetCorners()
    {
        if (LevelPlacer.placedLevel != null)
        {
            Level l = LevelManager.GetLevel();
            //MeshFilter[] meshFilters = l.GetComponentsInChildren<MeshFilter>();
            //Mesh[] meshes = new Mesh[meshFilters.Length];

            //// get all meshes of the MoveZone
            //for (int i = 0; i < meshFilters.Length; i++)
            //{
            //    if (meshFilters[i].gameObject.tag == Constants.moveAreaTag)
            //    {
            //        meshes[i] = meshFilters[i].mesh;
            //    }
            //}
        }
        else
            Debug.LogError("Can't modify level corners because there is no level placed.");
    }

    public void MoveVertices()
    {
    }

    public void GetOutsides()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}

public static class EdgeHelpers
{
    public struct Edge
    {
        public int v1;
        public int v2;
        public int triangleIndex;

        public Edge(int aV1, int aV2, int aIndex)
        {
            v1 = aV1;
            v2 = aV2;
            triangleIndex = aIndex;
        }
    }

    public static List<Edge> GetEdges(int[] aIndices)
    {
        List<Edge> result = new List<Edge>();
        for (int i = 0; i < aIndices.Length; i += 3)
        {
            int v1 = aIndices[i];
            int v2 = aIndices[i + 1];
            int v3 = aIndices[i + 2];
            result.Add(new Edge(v1, v2, i));
            result.Add(new Edge(v2, v3, i));
            result.Add(new Edge(v3, v1, i));
        }
        return result;
    }

    public static List<Edge> FindBoundary(this List<Edge> aEdges)
    {
        List<Edge> result = new List<Edge>(aEdges);
        for (int i = result.Count - 1; i > 0; i--)
        {
            for (int n = i - 1; n >= 0; n--)
            {
                if (result[i].v1 == result[n].v2 && result[i].v2 == result[n].v1)
                {
                    // shared edge so remove both
                    result.RemoveAt(i);
                    result.RemoveAt(n);
                    i--;
                    break;
                }
            }
        }
        return result;
    }

    public static List<Edge> SortEdges(this List<Edge> aEdges)
    {
        List<Edge> result = new List<Edge>(aEdges);
        for (int i = 0; i < result.Count - 2; i++)
        {
            Edge E = result[i];
            for (int n = i + 1; n < result.Count; n++)
            {
                Edge a = result[n];
                if (E.v2 == a.v1)
                {
                    // in this case they are already in order so just continoue with the next one
                    if (n == i + 1)
                        break;
                    // if we found a match, swap them with the next one after "i"
                    result[n] = result[i + 1];
                    result[i + 1] = a;
                    break;
                }
            }
        }
        return result;
    }
}