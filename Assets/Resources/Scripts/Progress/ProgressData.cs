using Impulse.Levels;
using Impulse.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is the actual Progress Data that is saved and loaded.
/// It consists out of an array of scoreboars, each of them containing the scores for a level.
/// It also holds other variables important for the players progress like achievemnts or settings.
/// </summary>

namespace Impulse.Progress
{
    [Serializable]
    public class ProgressData
    {
        public List<Highscore> highscores;
        public Settings settings;
        public Unlocks unlocks;

        //public HighscoreEvent;

        //public List<Achievements>
        //public List<Unlocks>
        //public List<Settings>
        public int lastUnlockedLevel;
        public int lastPlayedLevelID;
        public int totalStars;
        //add: unlocks, achievements, stats etc...

        public ProgressData()
        {
            settings = new Settings();
            highscores = new List<Highscore>();
            unlocks = new Unlocks();
            lastUnlockedLevel = 1;
            totalStars = 0;
            //if (LevelLoader.IsLoaded)
            //{
            //    lastPlayedLevelID = LevelManager.lastPlayedID;
            //    Debug.Log("[ProgressData] ProgressData(): LastPlayedID = " + lastPlayedLevelID);
            //}
            //else
            lastPlayedLevelID = 1;
            highscores = new List<Highscore>();
        }

        //Updates existing highscores (if the score is better) or creates a new one if it doesnt exist already
        public void EnterHighscore(int id, double time)
        {
            Highscore hs;
            //doesnt exist
            if (!highscores.Any(x => x.levelId == id))
            {
                hs = new Highscore(id, time);
                highscores.Add(hs);
                Debug.Log("[ProgresssData]: Creating new Highscore of level " + id);
            }
            //exists, thus try to edit this one
            else
            {
                highscores.Find(x => x.levelId == id).PlaceTime(time);
                Debug.Log("[ProgresssData]: Updating existing Highscore of level " + id);
            }
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
    }
}