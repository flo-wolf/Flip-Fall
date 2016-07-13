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
        private int lastPlayedLevelID;
        public int coins;
        //add: unlocks, achievements, stats etc...

        public ProgressData()
        {
            coins = -1;
            if (LevelLoader.IsLoaded)
            {
                lastPlayedLevelID = LevelManager.levelManager.GetID();
                Debug.Log("[ProgressData] ProgressData(): LastPlayedID = " + lastPlayedLevelID);
            }
            else lastPlayedLevelID = LevelManager.levelManager.defaultlevel.id;
            scoreboards = new List<Scoreboard>();
        }

        public void SetLastPlayedID(int id)
        {
            lastPlayedLevelID = id;
        }

        public int GetLastPlayedID()
        {
            return lastPlayedLevelID;
        }

        public Scoreboard NewScoreboard(int id)
        {
            Scoreboard sc = new Scoreboard();
            if (!scoreboards.Any(x => x.levelId == id))
            {
                sc.levelId = id;
                scoreboards.Add(sc);
                Debug.Log("[ProgresssData]: Creating new Scoreboard: " + id);
            }
            return sc;
        }

        public Scoreboard GetScoreboard(int id)
        {
            if (scoreboards.Count < 0 || !scoreboards.Any(x => x.levelId == id))
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
                        Debug.Log("[ProgresssData]: Loading old Scoreboard: " + id);
                        return s;
                    }
                }
            }
            Debug.Log("[ProgressData]: GetScoreboard() returns null - there is no scoreboard yet");
            return null;
        }
    }
}