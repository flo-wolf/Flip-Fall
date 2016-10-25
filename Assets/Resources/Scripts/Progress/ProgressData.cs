using Impulse.Levels;
using Impulse.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is the actual Progress Data that is saved and loaded.
/// It consists out of an array of scoreboars, each of them containing the scores for a level.
/// It also holds other variables important for the players progress like achievemnts or settings.
/// </summary>

namespace Impulse.Progress
{
    [Serializable]
    public class WalletUpdateEvent : UnityEvent { }

    [Serializable]
    public class ProgressData
    {
        // level highscores and stars
        public List<Highscore> highscores;

        public static WalletUpdateEvent onWalletUpdate = new WalletUpdateEvent();

        //[SerializeField]

        // sound settings etc.
        public Settings settings;

        // bought/unlocked items
        public Unlocks unlocks;

        // reached achievements
        public Achievements achievements;

        public int lastUnlockedLevel;
        public int lastPlayedLevelID;

        // amount of currently owned stars
        public int starsOwned;
        public int starsSpent;
        public int starsEarned;

        // pro version owned? => no ads, editor access
        public bool proVersion;

        //add: unlocks, achievements, stats etc...

        public ProgressData()
        {
            settings = new Settings();
            highscores = new List<Highscore>();
            unlocks = new Unlocks();

            lastUnlockedLevel = 1;
            starsOwned = 0;
            starsSpent = 0;
            starsEarned = 0;
            proVersion = false;
            lastPlayedLevelID = 1;
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

        public int GetStarsSpent()
        {
            // get from google
            int spent = 0;
            return spent;
        }

        /// <param name="id">Current level, used for comparison if next level can be unlocked</param>
        public bool TryUnlockNextLevel(int id)
        {
            if (lastUnlockedLevel <= id && lastUnlockedLevel + 1 <= Constants.lastLevel)
            {
                // first level finished
                if (lastUnlockedLevel == 1)
                {
                    Social.ReportProgress("CgkIqIqqjZYFEAIQDA", 100.0f, (bool success) =>
                    {
                        if (success)
                            Main.onAchievementUnlock.Invoke();
                    });
                }

                // first turret level
                if (lastUnlockedLevel == 6)
                {
                    Social.ReportProgress("CgkIqIqqjZYFEAIQBw", 100.0f, (bool success) =>
                    {
                        if (success)
                            Main.onAchievementUnlock.Invoke();
                    });
                }

                // first attractor level finished
                if (lastUnlockedLevel == 10)
                {
                    Social.ReportProgress("CgkIqIqqjZYFEAIQBg", 100.0f, (bool success) =>
                    {
                        if (success)
                            Main.onAchievementUnlock.Invoke();
                    });
                }

                // first speedstrip level finished
                else if (lastUnlockedLevel == 12)
                {
                    Social.ReportProgress("CgkIqIqqjZYFEAIQCA", 100.0f, (bool success) =>
                    {
                        if (success)
                            Main.onAchievementUnlock.Invoke();
                    });
                }

                // first portal level finished
                else if (lastUnlockedLevel == 7)
                {
                    Social.ReportProgress("CgkIqIqqjZYFEAIQEQ", 100.0f, (bool success) =>
                    {
                        if (success)
                            Main.onAchievementUnlock.Invoke();
                    });
                }
                else if (lastUnlockedLevel == Constants.lastLevel)
                {
                    Social.ReportProgress("CgkIqIqqjZYFEAIQAQ", 100.0f, (bool success) =>
                    {
                        if (success)
                            Main.onAchievementUnlock.Invoke();
                    });
                }

                lastUnlockedLevel++;
                UILevelselectionManager.unlockNextLevel = true;
                return true;
            }
            return false;
        }

        public void AddStarsToWallet(int stars)
        {
            starsOwned += stars;
            onWalletUpdate.Invoke();
        }

        //Updates existing highscores (if the score is better) or creates a new one if it doesnt exist already
        public Highscore EnterHighscore(int id, double time)
        {
            Highscore hs;
            //doesnt exist
            if (!highscores.Any(x => x.levelId == id))
            {
                hs = new Highscore(id, time);
                AddStarsToWallet(hs.starCount);
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
                AddStarsToWallet(newStars - oldStars);
                Debug.Log("[ProgresssData]: Updating existing Highscore of level " + id);
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
    }
}