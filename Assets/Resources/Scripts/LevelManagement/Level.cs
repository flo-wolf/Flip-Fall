using Impulse.Cam;
using Impulse.Objects;
using Impulse.Progress;
using Impulse.Theme;
using Impulse.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Level class. When a level gets loaded all moveArea meshes will be combined into one object, the moveAreaGo.
/// Is referenced in the LevelPlacer.placedLevel.
/// </summary>

namespace Impulse.Levels
{
    [Serializable]
    public class Level : MonoBehaviour
    {
        public static LevelUpdateEvent onGameStateChange = new LevelUpdateEvent();

        public class LevelUpdateEvent : UnityEvent<Level> { }

        //id is unique, no doubles allowed!
        public int id;
        public double presetTime = -1;

        public string title;
        public string author = "FlipFall";

        // Material applied onto the merged mesh - should have depthmask shader
        private Material material;

        // mergedMeshRenderer
        private MeshRenderer mr;

        [HideInInspector]
        public GameObject moveAreaGo;

        [HideInInspector]
        public PolygonCollider2D polyCollider;

        [HideInInspector]
        public Mesh mergedMesh;
        //public List<Vector2[]> levelPolys;

        //private Ghost ghost;
        [HideInInspector]
        public Spawn spawn;

        [HideInInspector]
        public Finish finish;

        // used for collision detection in Player class
        [HideInInspector]
        public List<Bounds> moveBounds = new List<Bounds>();

        private void Awake()
        {
            mergedMesh = new Mesh();
            // levelPolys = new List<Vector2[]>();

            material = Resources.Load("Materials/Game/MoveZone", typeof(Material)) as Material;
            material.SetFloat("_PlayerDistance", 10000);

            spawn = gameObject.GetComponentInChildren<Spawn>();
            finish = gameObject.GetComponentInChildren<Finish>();

            GenerateCollisionPolygons();
        }

        public void OnEnable()
        {
            // merge all movearea meshes into one gameobject
            MergeMoveArea();

            // reset the dissolve shader to zero
            mr.material.SetColor("_Color", ThemeManager.theme.moveZoneColor);
            mr.material.SetFloat("_SliceAmount", 0F);
        }

        public void GenerateCollisionPolygons()
        {
            MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in meshRenderers)
            {
                if (mr.transform.tag == "MoveArea")
                {
                    int sortingcount = -5;
                    mr.sortingOrder = sortingcount;
                    sortingcount--;

                    mr.gameObject.layer = LayerMask.NameToLayer("LevelMask");

                    Mesh m = new Mesh();
                    m = mr.gameObject.GetComponent<MeshFilter>().mesh;

                    //// generate 2d Array with the mesh's verticies
                    //Vector2[] poly = new Vector2[m.vertexCount];
                    //for (int v = 0; v < m.vertexCount; v++)
                    //{
                    //    poly[v] = new Vector2(mr.transform.TransformPoint(m.uv[v]).x, mr.transform.TransformPoint(m.uv[v]).y);
                    //}

                    //// sort that 2d array to have a correct clockwise order

                    //Array.Sort(poly, new ClockwiseComparer(mr.transform.TransformPoint(m.bounds.center)));

                    // add the array to the list of levelPolygons
                    //levelPolys.Add(poly);
                }
            }
        }

        // Merges all movearea blocks into one, deletes old MoveAreas
        // saves runtime resources and allows for easier boundary calcultions,
        // while still containing all seperated blocks in the prefab for easy editing
        private void MergeMoveArea()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            List<Vector2[]> poly2dPaths = new List<Vector2[]>();

            // Create merged movearea gameobject
            moveAreaGo = new GameObject("Merged MoveArea");
            mr = moveAreaGo.AddComponent<MeshRenderer>();
            MeshFilter mf = moveAreaGo.AddComponent<MeshFilter>();
            moveAreaGo.tag = Constants.moveAreaTag;
            moveAreaGo.layer = LayerMask.NameToLayer("LevelMask");
            moveAreaGo.transform.parent = this.transform;
            moveAreaGo.transform.localPosition = new Vector3(moveAreaGo.transform.localPosition.x, moveAreaGo.transform.localPosition.y, 0);
            mr.material = material;

            int i = 0;
            int n = 0;
            while (i < meshFilters.Length)
            {
                if (meshFilters[i].gameObject.tag == Constants.moveAreaTag)
                {
                    MeshRenderer meshRenderer = meshFilters[i].GetComponent<MeshRenderer>();

                    // add each mesh to the polyPath array
                    Vector2[] polyPath = new Vector2[meshFilters[i].sharedMesh.uv.Length];
                    for (int f = 0; f < meshFilters[i].sharedMesh.uv.Length; f++)
                    {
                        polyPath[f] = new Vector2(meshRenderer.transform.TransformPoint(meshFilters[i].sharedMesh.vertices[f]).x, meshRenderer.transform.TransformPoint(meshFilters[i].sharedMesh.vertices[f]).y);
                    }
                    Array.Sort(polyPath, new ClockwiseComparer2D(meshRenderer.transform.TransformPoint(meshFilters[i].sharedMesh.bounds.center)));

                    poly2dPaths.Add(polyPath);

                    // add each mesh to the combine array
                    combine[n].mesh = meshFilters[i].sharedMesh;
                    combine[n].transform = meshFilters[i].transform.localToWorldMatrix;
                    Destroy(meshFilters[i].gameObject);
                    n++;
                }
                i++;
            }
            mergedMesh = new Mesh();
            mergedMesh.CombineMeshes(combine);

            //if (LevelRenderMask._instance.renderTexture != null)
            //{
            //    material.SetTexture("_MainTex", LevelRenderMask._instance.renderTexture);
            //}

            mf.sharedMesh = mergedMesh;

            // create polygon collider for collision detection
            PolygonCollider2D poly2d = moveAreaGo.AddComponent<PolygonCollider2D>();
            poly2d.isTrigger = true;
            poly2d.pathCount = poly2dPaths.Count;
            for (int p = 0; p < poly2dPaths.Count; p++)
            {
                poly2d.SetPath(p, poly2dPaths[p]);
            }

            polyCollider = poly2d;

            //moveAreaGo.GetComponent<MeshRenderer>().sharedMaterial = moveAreaMaterial;
            //moveAreaGo.GetComponent<PolygonCollider2D>(). = mergedMesh;
        }

        public void DissolveLevel()
        {
            StartCoroutine(cDissolveLevel(LevelManager._instance.DissolveLevelDuration));
        }

        private IEnumerator cDissolveLevel(float duration)
        {
            if (mr != null)
            {
                yield return new WaitForSeconds(LevelManager._instance.DissolveDelay);
                Material m = mr.material;
                float t = 0F;
                while (t < 1.0f)
                {
                    t += Time.deltaTime * (Time.timeScale / duration);
                    m.SetFloat("_SliceAmount", t);
                    yield return 0;
                }
            }
            else
                Debug.Log("Dissolving Level failed, moveAreaGo MeshRenderer not found.");
            yield break;
        }

        private void FixedUpdate()
        {
        }

        public int GetID()
        {
            return id;
        }

        public void AddObject(GameObject go)
        {
        }

        public void RemoveObject(GameObject go)
        {
        }

        public void SetTitle(String newTitle)
        {
            title = newTitle;
        }

        public string GetTitle()
        {
            return title;
        }

        //public class ClockwiseVector2Comparer : IComparer<Vector2>
        //{
        //    public int Compare(Vector2 v1, Vector2 v2)
        //    {
        //        if (v1.x >= 0)
        //        {
        //            if (v2.x < 0)
        //            {
        //                return -1;
        //            }
        //            return -Comparer<float>.Default.Compare(v1.y, v2.y);
        //        }
        //        else
        //        {
        //            if (v2.x >= 0)
        //            {
        //                return 1;
        //            }
        //            return Comparer<float>.Default.Compare(v1.y, v2.y);
        //        }
        //    }
        //}

        //public class ClockwiseVector2Comparer : IComparer<Vector2>
        //{
        //    public int Compare(Vector2 v1, Vector2 v2)
        //    {
        //        return Mathf.Atan2(v1.x, v1.y).CompareTo(Mathf.Atan2(v2.x, v2.y));
        //    }
        //}
    }

    public class ClockwiseComparer2D : IComparer<Vector2>
    {
        private Vector2 m_Origin;

        #region Properties

        /// <summary>
        ///     Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public Vector2 origin { get { return m_Origin; } set { m_Origin = value; } }

        #endregion Properties

        /// <summary>
        ///     Initializes a new instance of the ClockwiseComparer class.
        /// </summary>
        /// <param name="origin">Origin.</param>
        public ClockwiseComparer2D(Vector2 origin)
        {
            m_Origin = origin;
        }

        #region IComparer Methods

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="first">First.</param>
        /// <param name="second">Second.</param>
        public int Compare(Vector2 v1, Vector2 v2)
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
        public static int IsClockwise(Vector2 first, Vector2 second, Vector2 origin)
        {
            if (first == second)
                return 0;

            Vector2 firstOffset = first - origin;
            Vector2 secondOffset = second - origin;

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