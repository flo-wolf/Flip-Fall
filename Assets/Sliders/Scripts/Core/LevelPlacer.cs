using Sliders;
using Sliders.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelPlacer : MonoBehaviour
{
    private static Transform t;

    private static void Place(Level level)
    {
        Debug.Log("Try to Place Level: " + level.id);
        Level l = LevelManager.levels.Find(x => x == level);
        t = (Transform)(Instantiate(l, new Vector3(-8.0f, -2.0f, 7.8f), Quaternion.identity));
        LevelManager.levels.Add(l);
        Debug.Log("Level Added to Levels: " + LevelManager.levels.Any(x => x == level));
    }

    private static void Remove(Level level)
    {
        Level l = LevelManager.levels.Find(x => x == level);
        LevelManager.levels.Remove(l);
        Debug.Log("Try to remove Level: " + l.id);
        Transform t = (Transform)(Instantiate(level, new Vector3(-8.0f, -2.0f, 7.8f), Quaternion.identity));
        GameObject go = t.gameObject;
        LevelManager.levels.Add(level);
        Debug.Log("Level Added to Levels: " + LevelManager.levels.Any(x => x == level));
    }

    private static void Replace(Level levelOld, Level levelNew)
    {
    }
}