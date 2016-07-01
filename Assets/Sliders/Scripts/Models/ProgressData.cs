using Sliders.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sliders.Models
{
    [Serializable]
    public class ProgressData
    {
        public List<Scoreboard> scoreboards = new List<Scoreboard>();
        public static int lastPlayedLevelID;
        public static int coins;
        //add: unlocks, achievements, stats etc...

        public ProgressData()
        {
            coins = -1;
            lastPlayedLevelID = 1;
            scoreboards = new List<Scoreboard>();
        }

        public Scoreboard NewScoreboard(int id)
        {
            Scoreboard sc = new Scoreboard();
            sc.levelId = id;
            scoreboards.Add(sc);
            return sc;
        }

        public Scoreboard GetScoreboard(int id)
        {
            Debug.Log("1");
            if (scoreboards.Count < 1 || scoreboards.Any(x => x.levelId != id))
            {
                Debug.Log("3");
                return NewScoreboard(id);
            }
            else
            {
                foreach (Scoreboard s in scoreboards)
                {
                    if (s.levelId == id)
                    {
                        //Scoreboard found
                        return s;
                    }
                }
            }
            Debug.Log("2");
            return null;
        }
    }
}