using FlipFall;
using FlipFall.LevelObjects;
using FlipFall.Levels;
using FlipFall.Theme;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Parses LevelData and creates the fitting gameobjects and instantiates them as children of this object.
/// Instanciated Level's spawns will be centered on this transform.position
/// </summary>

namespace FlipFall.Levels
{
    public class LevelPlacer : MonoBehaviour
    {
        public static LevelPlacer _instance;

        public static Transform placingParent;

        public static LevelPlaceEvent onLevelPlace = new LevelPlaceEvent();

        public class LevelPlaceEvent : UnityEvent<Level> { }

        // obsolete
        public static Level placedLevel;

        public static LevelDataMono generatedLevel;

        //
        public static float moveAreaZ = 0;
        public static float levelObjectZ = 0;

        [Header("LevelObject Prefabs")]
        public Level level;
        public MoveArea moveAreaPrefab;
        public Attractor attractorPrefab;
        public Finish finishPrefab;
        public Spawn spawnPrefab;
        public Turret turretPrefab;
        public Portal portalPrefab;
        public SpeedStrip speedStripPrefab;

        private void OnEnable()
        {
            placingParent = this.transform;
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
        }

        public static Level Place(Level level)
        {
            Level t = null;
            if (level != null && !IsPlaced(level.id))
            {
                DestroyChildren(placingParent);
                t = (Level)Instantiate(level, new Vector3(-0f, -2.0f, 7.8f), Quaternion.identity);
                //t = (Level)PrefabUtility.InstantiatePrefab(level);

                t.gameObject.transform.parent = placingParent;

                Vector3 spawnPosition = t.GetComponentInChildren<Spawn>().transform.position;
                Vector3 levelPosition = t.gameObject.transform.transform.position;
                Vector3 levelToSpawn = (spawnPosition - levelPosition);
                //t.gameObject.transform.transform.position = placingParent.transform.position + levelToSpawn;
                t.gameObject.transform.transform.position = placingParent.transform.position;

                Debug.Log("[LevelPlacer]: Place(): Level " + level.id + " placed.");

                placedLevel = t;
            }
            else
            {
                Debug.Log("[LevelPlacer]: Place(): Cant place Level " + level.id + ", it already exists in the scene!");
            }
            return t;
        }

        // generate a level based on the input leveldata and assign it as placedLevel, ready for referencing
        public bool PlaceCustom(LevelData levelData)
        {
            if (levelData != null)
            {
                // create the base level gameobject, parent of all levelObjects
                GameObject generatedLevelGO;
                generatedLevelGO = new GameObject("Generated Level " + levelData.id);

                // add leveldatamono
                generatedLevel = generatedLevelGO.AddComponent<LevelDataMono>();
                generatedLevel.levelData = levelData;
                generatedLevel.gameObject.tag = "Level";
                generatedLevel.gameObject.layer = LayerMask.NameToLayer("LevelMask");
                generatedLevel.transform.parent = transform;
                generatedLevel.transform.position = transform.position;

                // add spawn
                Spawn spawn = (Spawn)Instantiate(spawnPrefab, Vector3.zero, Quaternion.identity);
                spawn.transform.parent = generatedLevel.transform;
                Vector2 spawnPos = new Vector3(levelData.spawnPosition.x, levelData.spawnPosition.y, levelObjectZ);
                print("spawnpos loading: " + spawnPos);
                spawn.transform.localPosition = spawnPos;
                generatedLevel.spawn = spawn;

                // add finish
                Finish finish = (Finish)Instantiate(finishPrefab, Vector3.zero, Quaternion.identity);
                finish.transform.parent = generatedLevel.transform;
                Vector2 finishPos = new Vector3(levelData.finishPosition.x, levelData.finishPosition.y, levelObjectZ);
                finish.transform.localPosition = finishPos;
                generatedLevel.finish = finish;

                // create movearea
                MoveArea moveArea = (MoveArea)Instantiate(moveAreaPrefab, Vector3.zero, Quaternion.identity);
                moveArea.transform.parent = generatedLevel.transform;
                moveArea.transform.localPosition = Vector3.zero;
                int length = levelData.moveVerticies.Length;
                generatedLevel.moveArea = moveArea;

                // if there are verticies listed, generate a mesh and place it, otherwise leave the default quad mesh
                if (length > 0)
                {
                    MeshFilter mr = moveArea.GetComponent<MeshFilter>();

                    Vector3[] verts = new Vector3[length];
                    for (int i = 0; i < length; i++)
                    {
                        verts[i] = new Vector3(levelData.moveVerticies[i].x, levelData.moveVerticies[i].y, moveAreaZ);
                    }
                    Mesh m = CreateMoveAreaMesh(verts, levelData.moveTriangles);
                    mr.sharedMesh = m;
                }

                // add turrets
                foreach (TurretData turretData in levelData.turretDatas)
                {
                    // create the turret and add it as a child to the level
                    Turret t = (Turret)Instantiate(turretPrefab, Vector3.zero, Quaternion.identity);
                    t.transform.parent = generatedLevel.transform;
                    Vector2 pos = new Vector3(turretData.position.x, turretData.position.y, levelObjectZ);
                    t.transform.localPosition = pos;

                    // assign values
                    t.shotDelay = turretData.shotDelay;
                    t.startupDelay = turretData.startupDelay;
                    t.constantFire = turretData.constantFire;
                    t.shotSpeed = turretData.shotSpeed;
                }
                return true;
            }
            else
            {
                print("[LevelLoader] level already placed or the LevelData trying to be generated is null");
            }
            return false;
        }

        public Mesh CreateMoveAreaMesh(Vector3[] verts, int[] triangles)
        {
            Mesh m = new Mesh();
            m.vertices = verts;
            m.triangles = triangles;
            m.RecalculateBounds();
            m.RecalculateNormals();
            return m;
        }

        private static bool IsPlaced(int _id)
        {
            int childCount = placingParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (placingParent.GetChild(0).GetComponent<Level>().id == _id)
                    return true;
            }
            return false;
        }

        public static void DestroyChildren(Transform root)
        {
            int childCount = root.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject.Destroy(root.GetChild(0).gameObject);
            }
        }
    }
}