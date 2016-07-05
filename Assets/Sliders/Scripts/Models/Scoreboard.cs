using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.Models
{
    [SerializeField]
    public class Scoreboard
    {
        public int levelId;
        public bool finished;
        public bool unlocked;
        public List<Highscore> elements;
        public DateTime created { get; set; }
        public DateTime updated { get; set; }

        //scoreboards = LevelManager.level.getScores();

        public Scoreboard()
        {
            elements = new List<Highscore>();
            created = DateTime.UtcNow;
            updated = DateTime.UtcNow;
        }

        public Highscore Top()
        {
            if (elements.Count > 0)
                return elements[0];
            return null;
        }

        public void AddTime(double t)
        {
            Highscore se = new Highscore();
            se.time = t;
            elements.Add(se);
        }

        public void TryPlacingTime(double newTime)
        {
            Highscore newElement = new Highscore();

            //list filled? not wking, else works
            if (elements.Count > 0)
            {
                foreach (Highscore s in elements)
                {
                    if (newTime < s.time)
                    {
                        Debug.Log("lockaaa");
                        //Highscore e = elements.Find(s);
                        newElement = s;
                        newElement.time = newTime;
                        elements.Insert(elements.IndexOf(s), newElement);
                        break;
                    }
                }
            }
            else
            {
                newElement.time = newTime;
                Debug.Log("New Highscore with time: (" + newElement.time + ") added to Scoreboard of Level: (" + levelId + ") at position: (" + elements.IndexOf(newElement) + ")");
                elements.Add(newElement);
            }
        }

        public bool IsPlacingTimePossible(double t)
        {
            foreach (Highscore s in elements)
            {
                if (s.time < t)
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateLevelTime(double t)
        {
            //scoreboards = LevelManager.level.getScores();
        }

        public void Display()
        {
        }

        public void UpdateScoreboards()
        {
            //foreach (Scoreboard ls in _elements)
            //{
            //}
        }
    }
}