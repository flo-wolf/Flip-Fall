using Sliders;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sliders.Levels
{
    public class LevelPlacer : MonoBehaviour
    {
        public static Level Place(Level level)
        {
            Debug.Log("Try to Place Level: " + level.id);
            Level t = new Level();
            t = (Level)Instantiate(level, new Vector3(-0f, -2.0f, 7.8f), Quaternion.identity);
            return t;
        }

        public static void Remove(Level level)
        {
        }

        public static void Replace(Level levelOld, Level levelNew)
        {
        }
    }
}