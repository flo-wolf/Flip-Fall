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
                else if (lastUnlockedLevel == LevelManager.GetLastStoryLevel())
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
    }
}