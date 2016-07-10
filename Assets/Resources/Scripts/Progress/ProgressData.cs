using Sliders.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sliders.Progress
{
    [Serializable]
    public class ProgressData
    {
        public List<Scoreboard> scoreboards = new List<Scoreboard>();
        public int lastPlayedLevelID;
        public int coins;
        //add: unlocks, achievements, stats etc...

        public ProgressData()
        {
            coins = -1;
            if (LevelLoader.IsLoaded)
                lastPlayedLevelID = LevelManager.activeLevel.id;
            else lastPlayedLevelID = 1;
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
                Debug.Log("2");
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
            Debug.Log("null");
            return null;
        }
    }
}