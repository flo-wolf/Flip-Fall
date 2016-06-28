using Sliders.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sliders.Models
{
    [Serializable]
    public class LevelDataModel : MonoBehaviour
    {
        public int id { get; set; }
        public string name { get; set; }
        public double timeTop { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public static List<LevelObject> levelObjects { get; set; }

        public void AddObject(LevelObject obj)
        {
            levelObjects.Add(obj);
        }

        public void SetName(String newName)
        {
            name = newName;
        }

        public string GetName()
        {
            return name;
        }

        public void SetTime(double newTime)
        {
            timeTop = newTime;
        }

        public void RemoveLevelObject(int index)
        {
            levelObjects.RemoveAt(index);
        }

        public void RemoveLevelObject(LevelObject obj)
        {
            levelObjects.Remove(obj);
        }

        public void AddLevelObject(LevelObject levelObject)
        {
            levelObjects.Add(levelObject);
        }

        public LevelObject GetAddLevelObject(LevelObject levelObject)
        {
            levelObjects.Add(levelObject);
            return levelObject;
        }
    }
}