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
        public double timeGold;
        public double timeSilver;
        public double timeBronze;
        public Spawn spawn;
        public Finish finish;
        public DateTime created { get; set; }
        public DateTime updated { get; set; }

        public void Start()
        {
            timeGold = 10D;
            timeSilver = 11D;
            timeBronze = 12D;
        }

        public void AddObject(GameObject go)
        {
        }

        public void RemoveObject(GameObject go)
        {
        }

        public void SetTitle(String newTitle)
        {
            title = newTitle;
        }

        public string GetTitle()
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
    }
}