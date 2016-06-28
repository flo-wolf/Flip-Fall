using System.Collections;
using UnityEngine;

//LevelObjects can either be logic items (triggers, start, end), shapes (custom mesh, boxes, triangles, circles), or enemies(spikes, chainsaws, lasers)

namespace Impulse
{
    public class LevelObject : MonoBehaviour
    {
        public GameObject levelObject; //can we just use ".this" to get the game object or do we have to drag it into the public variable in the inspector? Fix
        public static Mesh mesh = new Mesh();
        public static Vector3[] shapeVertices;
        public static int[] shapeTriangles;
        public static int numCorners { get; set; }

        private static int prevNumCorners;

        public void Start()
        {
        }

        public void OnEnable()
        {
        }

        public void Instanciate()
        {
            Vector3 pos = new Vector3(1, 1, 1);
            Quaternion rot = new Quaternion(1, 1, 1, 1);
            Instantiate(levelObject, pos, rot);
        }

        //Draw triangle shape
        public static void createShape()
        {
            // We need at least three corners
            if (numCorners < 3)
                numCorners = 3;

            mesh.Clear();

            // Calculate vertices
            shapeVertices = new Vector3[numCorners];
            for (int i = 0; i < numCorners; ++i)

            {
                float angle = i * (360.0f / numCorners) * Mathf.Deg2Rad;
                shapeVertices[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            }

            // Calculate triangles
            int t = 0;

            shapeTriangles = new int[(numCorners - 2) * 3];
            for (int i = 1; i < numCorners - 1; ++i)
            {
                shapeTriangles[t++] = 0;
                shapeTriangles[t++] = i + 1;
                shapeTriangles[t++] = i;
            }

            mesh.vertices = shapeVertices;
            mesh.triangles = shapeTriangles;

            //mesh.RecalculateNormals();
            //mesh.RecalculateBounds();
        }

        public void AddCorner(Vector2 position)
        {
        }
    }
}

/*
--------------------------------------------------------------
        public LevelObject(Level level, TYPE type)
        {
            if (type == TYPE.Shape)
            {
                type = TYPE.Shape;
                levelMesh = new Mesh();
                parentLevel = level;
                level.Add(this);
            }
            else if (type == TYPE.Logic)
            {
                type = TYPE.Logic;
                levelMesh = new Mesh();
                parentLevel = level;
                level.Add(this);
            }
            else if (type == TYPE.Enemy)
            {
                type = TYPE.Enemy;
                levelMesh = new Mesh();
                parentLevel = level;
                level.Add(this);
            }
        }

        public void Create()
        {
        }

        public void Add()
        {
            //gameobject erstellen
            levelObject.AddComponent<MeshCollider>();
            levelObject.AddComponent<MeshFilter>();
        }

        public Level GetParentLevel()
        {
            return parentLevel;
        }

        public void SetParentLevel(Level newLevel)
        {
            newLevel.Add(this);
            parentLevel.RemoveLevelObject(this);
        }

        public void Remove()
        {
            //Only works when LevelObject is a MonoBehviour (obviously)
            //Destroy(this.gameObject);
        }
        */