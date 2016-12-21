using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlipFall.Progress
{
    [Serializable]
    public class Highscores
    {
        public List<Highscore> highscores;

        public Highscores()
        {
            highscores = new List<Highscore>();
        }

        //Updates existing highscores (if the score is better) or creates a new one if it doesnt exist already
        public Highscore EnterHighscore(int id, double time)
        {
            Highscore hs;
            //doesnt exist
            if (!highscores.Any(x => x.levelId == id))
            {
                hs = new Highscore(id, time);
                ProgressManager.GetProgress().AddStarsToWallet(hs.starCount);
                highscores.Add(hs);
                Debug.Log("[ProgresssData]: Creating new Highscore of level " + id);
            }
            //exists, thus try to edit this one
            else
            {
                hs = highscores.Find(x => x.levelId == id);
                int oldStars;
                oldStars = hs.starCount;
                if (oldStars < 0)
                    oldStars = 0;
                hs.PlaceTime(time);
                int newStars = hs.starCount;
                ProgressManager.GetProgress().AddStarsToWallet(newStars - oldStars);
                Debug.Log("[ProgresssData]: Updating existing Highscore of level " + id);
            }

            if (time >= 45F)
            {
                // slow but steady
                Social.ReportProgress("CgkIqIqqjZYFEAIQCA ", 100.0f, (bool success) =>
                {
                    if (success)
                        Main.onAchievementUnlock.Invoke();
                });
            }
            return hs;
        }

        public Highscore GetHighscore(int id)
        {
            if (highscores.Count < 0 || !highscores.Any(x => x.levelId == id))
            {
                return null;
            }
            else
            {
                foreach (Highscore h in highscores)
                {
                    if (h.levelId == id)
                    {
                        //Highscore found
                        return h;
                    }
                }
            }
            Debug.LogError("[ProgressData] You try to load a Highscore that doesnt exist!");
            return null;
        }

        public int GetStarsEarned()
        {
            // get from google and use this as validation
            int total = 0;
            for (int i = 0; i < highscores.Count; i++)
            {
                if (highscores[i].starCount > 0)
                {
                    total += highscores[i].starCount;
                }
            }
            return total;
        }
    }
}