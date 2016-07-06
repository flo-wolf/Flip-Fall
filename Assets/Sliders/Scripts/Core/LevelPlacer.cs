using Sliders;
using Sliders.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelPlacer : MonoBehaviour
{
    public static Level Place(Level level)
    {
        Debug.Log("Try to Place Level: " + level.id);
        Level t = (Level)Instantiate(level, new Vector3(-8.0f, -2.0f, 7.8f), Quaternion.identity);
        return t;
    }

    public static void Remove(Level level)
    {
        Level l = LevelManager.loadedLevels.Find(x => x == level);
        LevelManager.loadedLevels.Remove(l);
        Debug.Log("Try to remove Level: " + l.id);
        Transform t = (Transform)(Instantiate(level, new Vector3(-8.0f, -2.0f, 7.8f), Quaternion.identity));
        GameObject go = t.gameObject;
        LevelManager.loadedLevels.Add(level);
        Debug.Log("Level Added to loadedLevels: " + LevelManager.loadedLevels.Any(x => x == level));
    }

    public static void Replace(Level levelOld, Level levelNew)
    {
    }
}