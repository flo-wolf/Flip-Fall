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

        //id is unique, no doubles allowed!
        public int id;

        public string title;
        public Ghost goldGhost;
        public Ghost silverGhost;
        public Ghost bronzeGhost;

        public Spawn spawn;
        public Finish finish;
        public DateTime created { get; set; }
        public DateTime updated { get; set; }

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
        }

        public void SetSilverTime(double newTime)
        {
        }

        public void SetBronzeTime(double newTime)
        {
        }
    }
}