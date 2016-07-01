using Sliders.UI;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void NewScoreboard(int id)
        {
            if (scoreboards.Any(x => x.levelId == id))
            {
                Scoreboard sc = new Scoreboard();
                sc.levelId = id;
                scoreboards.Add(sc);
            }
        }

        public Scoreboard GetScoreboard(int id)
        {
            foreach (Scoreboard s in scoreboards)
            {
                if (s.levelId == id)
                {
                    //Scoreboard found
                    return s;
                }
            }
            return null;
        }
    }
}