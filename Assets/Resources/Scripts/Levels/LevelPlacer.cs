using Sliders;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sliders.Levels
{
    public class LevelPlacer : MonoBehaviour
    {
        public static LevelData Place(LevelData level)
        {
            Debug.Log("Try to Place LevelData: " + level.id);
            LevelData t = new LevelData();
            //(LevelData)Instantiate(level, new Vector3(-8.0f, -2.0f, 7.8f), Quaternion.identity);
            return t;
        }

        public static void Remove(LevelData level)
        {
            LevelData l = LevelManager.loadedLevels.Find(x => x == level);
            LevelManager.loadedLevels.Remove(l);
            Debug.Log("Try to remove LevelData: " + l.id);
            Transform t = (Transform)(Instantiate(level, new Vector3(-8.0f, -2.0f, 7.8f), Quaternion.identity));
            GameObject go = t.gameObject;
            LevelManager.loadedLevels.Add(level);
            Debug.Log("LevelData Added to loadedLevels: " + LevelManager.loadedLevels.Any(x => x == level));
        }

        public static void Replace(LevelData levelOld, LevelData levelNew)
        {
        }
    }
}