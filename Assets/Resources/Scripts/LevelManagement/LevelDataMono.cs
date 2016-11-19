﻿using FlipFall.Cam;
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
        public void AddObject(LevelObject.ObjectType type, Vector3 position)
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
                        break;

                    case LevelObject.ObjectType.attractor:
                        Attractor a = (Attractor)Instantiate(LevelPlacer._instance.attractorPrefab, Vector3.zero, Quaternion.identity);
                        a.transform.parent = LevelPlacer.generatedLevel.transform;
                        Vector2 aPos = new Vector3(position.x, position.y, LevelPlacer.levelObjectZ);
                        a.transform.position = aPos;
                        attractors.Add(a);
                        break;

                    case LevelObject.ObjectType.portal:
                        Portal p = (Portal)Instantiate(LevelPlacer._instance.portalPrefab, Vector3.zero, Quaternion.identity);
                        p.transform.parent = LevelPlacer.generatedLevel.transform;
                        Vector2 pPos = new Vector3(position.x, position.y, LevelPlacer.levelObjectZ);
                        p.transform.position = pPos;
                        portals.Add(p);
                        break;

                    case LevelObject.ObjectType.speedStrip:
                        SpeedStrip s = (SpeedStrip)Instantiate(LevelPlacer._instance.speedStripPrefab, Vector3.zero, Quaternion.identity);
                        s.transform.parent = LevelPlacer.generatedLevel.transform;
                        Vector2 sPos = new Vector3(position.x, position.y, LevelPlacer.levelObjectZ);
                        s.transform.position = sPos;
                        speedStrips.Add(s);
                        break;

                    default:
                        //Debug.Log("Wasnt able to add the levelobject to the LevelDataMono of type " + lo.objectType);
                        break;
                }
            }
            else
            {
                Debug.LogError("LevelPlacer needed to add an object to the level.");
            }
        }

        public void RemoveObject(LevelObject lo)
        {
        }
    }
}