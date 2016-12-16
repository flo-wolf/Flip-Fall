using FlipFall;
using FlipFall.Editor;
using FlipFall.LevelObjects;
using FlipFall.Levels;
using FlipFall.Theme;
using System;
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

        // obsolete, replaced by generatedLevel
        public static Level placedLevel;

        public static LevelDataMono generatedLevel;

        // Z-position defaults
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
        public Bouncer bouncerPrefab;

        [Header("Bouncer Materials")]
        public PhysicsMaterial2D bounce05;
        public PhysicsMaterial2D bounce1;
        public PhysicsMaterial2D bounce15;
        public PhysicsMaterial2D bounce2;
        public PhysicsMaterial2D bounce25;
        public PhysicsMaterial2D bounce3;

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

        // generate a level based on the input leveldata and assign it as placedLevel, ready for referencing
        public bool Place(LevelData levelData)
        {
            if (levelData != null)
            {
                if (generatedLevel != null)
                {
                    bool vertsDifferen = levelData.moveVerticies != generatedLevel.levelData.moveVerticies;
                    Debug.Log("Destroy Old generated Level, verticies are different: " + vertsDifferen);
                    DestroyImmediate(generatedLevel.gameObject);
                    generatedLevel = null;
                }

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

                // create lists
                generatedLevel.turrets = new List<Turret>();
                generatedLevel.portals = new List<Portal>();
                generatedLevel.speedStrips = new List<SpeedStrip>();
                generatedLevel.attractors = new List<Attractor>();
                generatedLevel.bouncers = new List<Bouncer>();

                // add spawn
                Spawn spawn = (Spawn)Instantiate(spawnPrefab, Vector3.zero, Quaternion.identity);
                spawn.transform.parent = generatedLevel.transform;
                Vector2 spawnPos = new Vector3(levelData.objectData.spawnPosition.x, levelData.objectData.spawnPosition.y, levelObjectZ);
                print("spawnpos loading: " + spawnPos);
                spawn.transform.localPosition = spawnPos;
                generatedLevel.spawn = spawn;

                // add finish
                Finish finish = (Finish)Instantiate(finishPrefab, Vector3.zero, Quaternion.identity);
                finish.transform.parent = generatedLevel.transform;
                Vector2 finishPos = new Vector3(levelData.objectData.finishPosition.x, levelData.objectData.finishPosition.y, levelObjectZ);
                finish.transform.localPosition = finishPos;
                generatedLevel.finish = finish;

                // add turrets
                foreach (TurretData td in levelData.objectData.turretData)
                {
                    // create gameobject
                    Turret turret = (Turret)Instantiate(turretPrefab, Vector3.zero, Quaternion.identity);
                    turret.transform.parent = generatedLevel.transform;
                    Vector2 turretPos = new Vector3(td.position.x, td.position.y, levelObjectZ);
                    turret.transform.localPosition = turretPos;

                    turret.transform.rotation = Quaternion.Euler(td.rotation.x, td.rotation.y, td.rotation.z);

                    // assign values
                    turret.shotDelay = td.shotDelay;
                    turret.startupDelay = td.startupDelay;
                    turret.constantFire = td.constantFire;
                    turret.shotSpeed = td.shotSpeed;

                    generatedLevel.turrets.Add(turret);
                }

                // add attractors
                foreach (AttractorData ad in levelData.objectData.attractorData)
                {
                    // create gameobject
                    Attractor attractor = (Attractor)Instantiate(attractorPrefab, Vector3.zero, Quaternion.identity);
                    attractor.transform.parent = generatedLevel.transform;
                    Vector2 attractorPos = new Vector3(ad.position.x, ad.position.y, levelObjectZ);
                    attractor.transform.localPosition = attractorPos;

                    // assign values
                    attractor.pullRadius = ad.radius;
                    attractor.maxPullForce = ad.pullStrength;
                    attractor.SetScale();

                    // add to the reference list for later saving access
                    generatedLevel.attractors.Add(attractor);
                }

                // add speedstrips
                foreach (SpeedStripData sd in levelData.objectData.speedStripData)
                {
                    // create gameobject
                    SpeedStrip speedStrip = (SpeedStrip)Instantiate(speedStripPrefab, Vector3.zero, Quaternion.identity);
                    speedStrip.transform.parent = generatedLevel.transform;
                    Vector2 speedStripPos = new Vector3(sd.position.x, sd.position.y, levelObjectZ);
                    speedStrip.transform.localPosition = speedStripPos;
                    speedStrip.transform.rotation = Quaternion.Euler(sd.rotation.x, sd.rotation.y, sd.rotation.z);
                    speedStrip.accelSpeed = sd.pushStrength;
                    generatedLevel.speedStrips.Add(speedStrip);
                }

                // add bouncers
                foreach (BouncerData bd in levelData.objectData.bouncerData)
                {
                    Bouncer bouncer = (Bouncer)Instantiate(bouncerPrefab, Vector3.zero, Quaternion.identity);
                    bouncer.transform.parent = generatedLevel.transform;
                    Vector2 bouncerPos = new Vector3(bd.position.x, bd.position.y, levelObjectZ);
                    bouncer.transform.localPosition = bouncerPos;
                    bouncer.transform.rotation = Quaternion.Euler(bd.rotation.x, bd.rotation.y, bd.rotation.z);
                    bouncer.bounciness = bd.bounciness;
                    bouncer.width = bd.width;
                    generatedLevel.bouncers.Add(bouncer);
                }

                // add portals...
                foreach (PortalData pd in levelData.objectData.portalData)
                {
                    // create gameobject
                    Portal portal = (Portal)Instantiate(portalPrefab, Vector3.zero, Quaternion.identity);
                    portal.transform.parent = generatedLevel.transform;
                    Vector2 portalPos = new Vector3(pd.position.x, pd.position.y, levelObjectZ);
                    portal.transform.localPosition = portalPos;

                    // assign values
                    portal.portalID = pd.portalID;
                    portal.linkedPortalID = pd.linkedPortalID;
                    portal.active = pd.active;

                    generatedLevel.portals.Add(portal);
                }

                // ... and link them with one another - if there is no fitting counterpart, delete them
                foreach (Portal p in generatedLevel.portals)
                {
                    // get the portal that matches the linked id
                    Portal linkPortal = generatedLevel.portals.Find(x => x.portalID == p.linkedPortalID);
                    p.Link(linkPortal);

                    //// link both portals
                    //if (linkPortal != null)
                    //{
                    //    p.linkedPortal = linkPortal;
                    //    linkPortal.linkedPortal = p;
                    //}
                    //// no matching portal was found, linking not possible. Set this portal to inactive.
                    //else
                    //{
                    //    p.active = false;
                    //}
                }

                // create movearea
                MoveArea moveArea = (MoveArea)Instantiate(moveAreaPrefab, Vector3.zero, Quaternion.identity);
                moveArea.transform.parent = generatedLevel.transform;
                moveArea.transform.localPosition = Vector3.zero;
                int length = levelData.moveVerticies.Length;
                generatedLevel.moveArea = moveArea;

                // if the leveldata has verticies listed, generate a mesh from it and place it
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
                // ... otherwise create a default level mesh and place it
                else
                {
                    MeshFilter mr = moveArea.GetComponent<MeshFilter>();

                    Vector3[] verts = new Vector3[4];
                    int[] tri = new int[6];

                    // upper left
                    verts[0] = Round(generatedLevel.moveArea.transform.InverseTransformPoint(new Vector3(3990f, 1110f, moveAreaZ)));
                    // upper right
                    verts[1] = Round(generatedLevel.moveArea.transform.InverseTransformPoint(new Vector3(4050f, 1110f, moveAreaZ)));
                    // lower left
                    verts[2] = Round(generatedLevel.moveArea.transform.InverseTransformPoint(new Vector3(3990f, 950f, moveAreaZ)));
                    // lower right
                    verts[3] = Round(generatedLevel.moveArea.transform.InverseTransformPoint(new Vector3(4050f, 950f, moveAreaZ)));

                    // create clockwise triangles based on the vertices we just created
                    tri[0] = 0;
                    tri[1] = 1;
                    tri[2] = 2;
                    tri[3] = 1;
                    tri[4] = 3;
                    tri[5] = 2;

                    Mesh m = CreateMoveAreaMesh(verts, tri);
                    mr.sharedMesh = m;
                    //LevelLoader.SaveCustomLevel(LevelEditor.CreateLevelData());
                }

                // set the movearea as the selected object
                if (Main.currentScene == Main.ActiveScene.editor)
                {
                    LevelEditor.SetSelectedObject(generatedLevel.moveArea);
                }
                return true;
            }
            else
            {
                print("[LevelLoader] level already placed or the LevelData trying to be generated is null");
            }
            return false;
        }

        // used to round movearea vertices to 0.001, means three decimals after the comma
        private Vector3 Round(Vector3 v)
        {
            float x = float.Parse(v.x.ToString("F3"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            float y = float.Parse(v.y.ToString("F3"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            float z = float.Parse(v.z.ToString("F3"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            return new Vector3(x, y, z);
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