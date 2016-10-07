using Impulse.Background;
using Impulse.Theme;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Achievements that got unlocked
/// </summary>

namespace Impulse.Progress
{
    [Serializable]
    public class Achievements
    {
        // general info
        public float completedPercent;

        // actual achievements - dictionary might be the better container option.
        public List<Achievement> achievements;

        // constructor
        public Achievements()
        {
            completedPercent = 0f;
            achievements = new List<Achievement>();
        }

        public void UnlockAchievement(int id)
        {
            achievements.Find(x => x.id == id).Complete();
        }
    }
}