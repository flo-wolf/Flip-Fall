using Sliders.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sliders.Models
{
    [Serializable]
    public class Level : MonoBehaviour
    {
        public int id { get; set; }
        public string title { get; set; }
        public double timeGold { get; set; }
        public double timeSilver { get; set; }
        public double timeBronze { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public List<LevelObject> levelObjects { get; set; }

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

        public void SetGoldTime(double newTime)
        {
            timeGold = newTime;
        }

        public void SetSilverTime(double newTime)
        {
            timeSilver = newTime;
        }

        public void SetBronzeTime(double newTime)
        {
            timeBronze = newTime;
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