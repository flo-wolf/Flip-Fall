using FlipFall.Cam;
using FlipFall.Editor;
using FlipFall.LevelObjects;
using FlipFall.Progress;
using FlipFall.Theme;
using FlipFall.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Generated Level class. When a level gets loaded all moveArea meshes will be combined into one object, the moveAreaGo.
/// It is referenced in the LevelPlacer.placedLevel.
/// </summary>

namespace FlipFall.Levels
{
    public class LevelDataMono : MonoBehaviour
    {
        public static LevelUpdateEvent onGameStateChange = new LevelUpdateEvent();

        public class LevelUpdateEvent : UnityEvent<Level> { }

        public LevelData levelData;
        public MoveArea moveArea;
        public Spawn spawn;
        public Finish finish;
        public List<Attractor> attractors;
        public List<Turret> turrets;
        public List<SpeedStrip> speedStrips;
        public List<Portal> portals;
        public List<Bouncer> bouncers;

        private void Awake()
        {
        }

        public void OnEnable()
        {
            // reset the dissolve shader to zero
        }

        private void FixedUpdate()
        {
        }

        // add an object to the level. this step of reference is needed for later deconstruction of the level and serialization
        public LevelObject AddObject(LevelObject.ObjectType type, Vector3 position)
        {
            if (LevelPlacer._instance != null)
            {
                switch (type)
                {
                    case LevelObject.ObjectType.turret:
                        Turret turret = (Turret)Instantiate(LevelPlacer._instance.turretPrefab, Vector3.zero, Quaternion.identity);
                        turret.transform.parent = LevelPlacer.generatedLevel.transform;
                        Vector2 turretPos = new Vector3(position.x, position.y, LevelPlacer.levelObjectZ);
                        turret.transform.position = turretPos;
                        turrets.Add(turret);
                        // ProgressManager.GetProgress().unlocks.inventory.Add(type, -1);
                        UndoManager.AddUndoPoint();
                        return turret;

                    case LevelObject.ObjectType.attractor:
                        Attractor a = (Attractor)Instantiate(LevelPlacer._instance.attractorPrefab, Vector3.zero, Quaternion.identity);
                        a.transform.parent = LevelPlacer.generatedLevel.transform;
                        Vector2 aPos = new Vector3(position.x, position.y, LevelPlacer.levelObjectZ);
                        a.transform.position = aPos;
                        attractors.Add(a);
                        //ProgressManager.GetProgress().unlocks.inventory.Add(type, -1);
                        UndoManager.AddUndoPoint();
                        return a;

                    case LevelObject.ObjectType.portal:
                        Portal p = (Portal)Instantiate(LevelPlacer._instance.portalPrefab, Vector3.zero, Quaternion.identity);
                        p.transform.parent = LevelPlacer.generatedLevel.transform;
                        Vector2 pPos = new Vector3(position.x, position.y, LevelPlacer.levelObjectZ);
                        p.transform.position = pPos;
                        p.portalID = GetFreePortalID(0);
                        p.linkedPortalID = -1;
                        portals.Add(p);
                        // ProgressManager.GetProgress().unlocks.inventory.Add(type, -1);
                        UndoManager.AddUndoPoint();
                        return p;

                    case LevelObject.ObjectType.speedStrip:
                        SpeedStrip s = (SpeedStrip)Instantiate(LevelPlacer._instance.speedStripPrefab, Vector3.zero, Quaternion.identity);
                        s.transform.parent = LevelPlacer.generatedLevel.transform;
                        Vector2 sPos = new Vector3(position.x, position.y, LevelPlacer.levelObjectZ);
                        s.transform.position = sPos;
                        s.transform.rotation = Quaternion.Euler(0, 0, 180);
                        speedStrips.Add(s);
                        // ProgressManager.GetProgress().unlocks.inventory.Add(type, -1);
                        UndoManager.AddUndoPoint();
                        return s;

                    case LevelObject.ObjectType.bouncer:
                        Bouncer b = (Bouncer)Instantiate(LevelPlacer._instance.bouncerPrefab, Vector3.zero, Quaternion.identity);
                        b.transform.parent = LevelPlacer.generatedLevel.transform;
                        Vector2 bPos = new Vector3(position.x, position.y, LevelPlacer.levelObjectZ);
                        b.transform.position = bPos;
                        b.transform.rotation = Quaternion.Euler(0, 0, 0);
                        bouncers.Add(b);
                        // ProgressManager.GetProgress().unlocks.inventory.Add(type, -1);
                        UndoManager.AddUndoPoint();
                        return b;

                    default:
                        //Debug.Log("Wasnt able to add the levelobject to the LevelDataMono of type " + lo.objectType);
                        break;
                }
            }
            else
            {
                Debug.LogError("LevelPlacer needed to add an object to the level.");
            }
            return null;
        }

        // add an object to the level. this step of reference is needed for later deconstruction of the level and serialization
        public void DeleteObject(LevelObject levelObj)
        {
            if (LevelPlacer._instance != null)
            {
                switch (levelObj.objectType)
                {
                    case LevelObject.ObjectType.turret:
                        turrets.Remove((Turret)levelObj);
                        break;

                    case LevelObject.ObjectType.attractor:
                        attractors.Remove((Attractor)levelObj);
                        break;

                    case LevelObject.ObjectType.portal:
                        portals.Remove((Portal)levelObj);
                        levelObj.GetComponent<Portal>().Unlink();
                        break;

                    case LevelObject.ObjectType.speedStrip:
                        speedStrips.Remove((SpeedStrip)levelObj);
                        break;

                    case LevelObject.ObjectType.bouncer:
                        bouncers.Remove((Bouncer)levelObj);
                        break;

                    default:
                        //Debug.Log("Wasnt able to add the levelobject to the LevelDataMono of type " + lo.objectType);
                        break;
                }

                //ProgressManager.GetProgress().unlocks.inventory.Add(levelObj.objectType, 1);
                DestroyImmediate(levelObj.gameObject);
                UndoManager.AddUndoPoint();
            }
            else
            {
                Debug.LogError("LevelPlacer needed to add an object to the level.");
            }
        }

        // returns a free portal id that is not yet used within the current level
        private int GetFreePortalID(int i)
        {
            foreach (Portal p in portals)
            {
                if (p.portalID == i)
                    i = GetFreePortalID(i + 1);
                else
                    break;

                if (i > portals.Count)
                    break;
            }
            Debug.Log(i);
            return i;
        }
    }
}