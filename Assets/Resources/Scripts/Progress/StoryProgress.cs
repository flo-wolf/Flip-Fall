using FlipFall.Levels;
using FlipFall.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlipFall.Progress
{
    [Serializable]
    public class StoryProgress
    {
        public int lastUnlockedLevel;
        public int lastPlayedLevelID;

        public StoryProgress()
        {
            lastUnlockedLevel = 0;
            lastPlayedLevelID = 0;
        }

        /// <param name="id">Current level, used for comparison if next level can be unlocked</param>
        public bool TryUnlockNextLevel(int id)
        {
            if (lastUnlockedLevel <= id && lastUnlockedLevel + 1 <= LevelManager.GetLastStoryLevel())
            {
                // First Steps
                if (lastUnlockedLevel == 0)
                {
                    Social.ReportProgress("CgkIqIqqjZYFEAIQBw", 100.0f, (bool success) =>
                    {
                        if (success)
                            Main.onAchievementUnlock.Invoke();
                    });
                }

                lastUnlockedLevel++;
                UILevelselectionManager.unlockNextLevel = true;
                ProgressManager.SaveProgressData();
                return true;
            }
            return false;
        }
    }
}