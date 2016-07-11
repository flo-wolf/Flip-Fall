using Sliders.Progress;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Sliders.Levels
{
    [Serializable]
    public class Level : MonoBehaviour
    {
        public static LevelUpdateEvent onGameStateChange = new LevelUpdateEvent();

        public class LevelUpdateEvent : UnityEvent<Level> { }

        public int id;
        public string title;
        private double timeGold { get; set; }
        private double timeSilver { get; set; }
        private double timeBronze { get; set; }
        public Spawn spawn;
        public Finish finish;
        public DateTime created { get; set; }
        public DateTime updated { get; set; }

        public void Start()
        {
            timeSilver = 1;
            id = ProgressManager.progress.lastPlayedLevelID;
        }

        public void AddObject(LevelObject obj)
        {
        }

        public void SetName(String newTitle)
        {
            title = newTitle;
        }

        public string GetName()
        {
            return title;
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

        public void RemoveLevelObject(LevelObject obj)
        {
        }

        public void AddLevelObject(LevelObject levelObject)
        {
        }
    }
}