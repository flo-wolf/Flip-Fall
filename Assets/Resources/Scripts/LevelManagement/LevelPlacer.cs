using FlipFall.LevelObjects;
using FlipFall.Levels;
using Impulse;
using Impulse.Theme;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Parses LevelData and creates the fitting gameobjects based on it and instantiates them.
/// </summary>

namespace Impulse.Levels
{
    public class LevelPlacer : MonoBehaviour
    {
        public static Transform placingParent;

        public static LevelPlaceEvent onLevelPlace = new LevelPlaceEvent();

        public class LevelPlaceEvent : UnityEvent<Level> { }

        public static Level placedLevel;

        [Header("LevelObject Prefabs")]
        public Attractor attractor;
        public GameObject moveArea;
        public Finish finish;
        public Spawn spawn;

        private void OnEnable()
        {
            placingParent = this.transform;
        }

        public static Level Place(Level level)
        {
            Level t = null;
            if (level != null && !IsPlaced(level.id))
            {
                DestroyChildren(placingParent);
                //t = (Level)Instantiate(level, new Vector3(-0f, -2.0f, 7.8f), Quaternion.identity);
                t = (Level)PrefabUtility.InstantiatePrefab(level);

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

        public static Level PlaceCustom(LevelData level)
        {
            return null;
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

        public static void Remove(Level level)
        {
        }

        public static void Replace(Level levelOld, Level levelNew)
        {
        }
    }
}