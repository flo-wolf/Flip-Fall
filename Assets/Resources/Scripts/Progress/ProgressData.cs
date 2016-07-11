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
                lastPlayedLevelID = LevelManager.levelManager.GetID();
            else lastPlayedLevelID = LevelManager.levelManager.defaultlevel.id;
            scoreboards = new List<Scoreboard>();
        }

        public Scoreboard NewScoreboard(int id)
        {
            Debug.Log("[ProgresssData]: Creating new Scoreboard: " + id);
            Scoreboard sc = new Scoreboard();
            sc.levelId = id;
            scoreboards.Add(sc);
            return sc;
        }

        public Scoreboard GetScoreboard(int id)
        {
            if (scoreboards.Count < 1 || scoreboards.Any(x => x.levelId != id))
            {
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
            Debug.Log("[ProgressData]: GetScoreboard() returns null - there is no scoreboard yet");
            return null;
        }
    }
}