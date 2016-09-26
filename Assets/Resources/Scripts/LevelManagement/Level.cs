using Impulse.Cam;
using Impulse.Objects;
using Impulse.Progress;
using Impulse.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

        // Material applied onto the merged mesh - should have depthmask shader
        private Material material;

        // mergedMeshRenderer
        private MeshRenderer mr;
        private Mesh mergedMesh;
        private GameObject moveAreaGo;

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
            material = Resources.Load("Materials/Game/MoveZone", typeof(Material)) as Material;
            material.SetFloat("_PlayerDistance", 10000);

            spawn = gameObject.GetComponentInChildren<Spawn>();
            finish = gameObject.GetComponentInChildren<Finish>();

            int sortingcount = -5;
            MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in meshRenderers)
            {
                if (mr.transform.tag == "MoveArea")
                {
                    // should be worldspace, dont use the meshfilter.bounds, it'll be local space
                    Bounds b = mr.gameObject.GetComponent<Renderer>().bounds;

                    mr.gameObject.layer = LayerMask.NameToLayer("LevelMask");

                    moveBounds.Add(b);
                    //Debug.Log(b + " -- id --- " + moveBounds.Count);

                    mr.sortingOrder = sortingcount;
                    sortingcount--;
                }
            }
        }

        public void Start()
        {
            MergeMoveArea();
        }

        // Merges all movearea blocks into one, delete old MoveAreas
        // saves runtime recources and allows for easier boundary calcultions,
        // while still containing all blocks in the prefab for easy editing
        private void MergeMoveArea()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            int n = 0;
            while (i < meshFilters.Length)
            {
                if (meshFilters[i].gameObject.tag == Constants.moveAreaTag)
                {
                    combine[n].mesh = meshFilters[i].sharedMesh;
                    combine[n].transform = meshFilters[i].transform.localToWorldMatrix;
                    Destroy(meshFilters[i].gameObject);
                    n++;
                }
                i++;
            }
            mergedMesh = new Mesh();
            mergedMesh.CombineMeshes(combine);

            moveAreaGo = new GameObject("Merged MoveArea");
            mr = moveAreaGo.AddComponent<MeshRenderer>();
            MeshFilter mf = moveAreaGo.AddComponent<MeshFilter>();
            PolygonCollider2D poly2d = moveAreaGo.AddComponent<PolygonCollider2D>();
            moveAreaGo.tag = Constants.moveAreaTag;
            moveAreaGo.layer = LayerMask.NameToLayer("LevelMask");
            moveAreaGo.transform.parent = this.transform;
            moveAreaGo.transform.localPosition = new Vector3(moveAreaGo.transform.localPosition.x, moveAreaGo.transform.localPosition.y, 0);
            mr.material = material;

            //if (LevelRenderMask._instance.renderTexture != null)
            //{
            //    material.SetTexture("_MainTex", LevelRenderMask._instance.renderTexture);
            //}

            mf.sharedMesh = mergedMesh;

            //moveAreaGo.GetComponent<MeshRenderer>().sharedMaterial = moveAreaMaterial;
            //moveAreaGo.GetComponent<PolygonCollider2D>(). = mergedMesh;
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
    }
}