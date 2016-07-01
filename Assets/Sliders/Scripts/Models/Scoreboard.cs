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
        public List<ScoreboardElement> elements;
        public DateTime created { get; set; }
        public DateTime updated { get; set; }

        //scoreboards = LevelManager.level.getScores();

        public Scoreboard()
        {
            elements = new List<ScoreboardElement>();
            created = DateTime.UtcNow;
            updated = DateTime.UtcNow;
        }

        public ScoreboardElement Top()
        {
            if (elements.Count > 0)
                return elements[0];
            return null;
        }

        public void AddTime(double t)
        {
            ScoreboardElement se = new ScoreboardElement();
            se.time = t;
            elements.Add(se);
        }

        public void TryPlacingTime(double newTime)
        {
            Debug.Log("1");
            ScoreboardElement newElement = new ScoreboardElement();

            //list filled? not wking, else works
            if (elements.Count > 0)
            {
                foreach (ScoreboardElement s in elements)
                {
                    if (newTime > s.time)
                    {
                        //ScoreboardElement e = elements.Find(s);
                        newElement = s;
                        newElement.time = newTime;
                        //elements.Insert(elements.FindIndex(s), newElement);
                        break;
                    }
                }
            }
            else
            {
                newElement.time = newTime;
                Debug.Log("New ScoreboardElement (" + newElement.time + ") added to Scoreboard " + levelId);
                elements.Add(newElement);
            }
        }

        public bool IsPlacingTimePossible(double t)
        {
            foreach (ScoreboardElement s in elements)
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