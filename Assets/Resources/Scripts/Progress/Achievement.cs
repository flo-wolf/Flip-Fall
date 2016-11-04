using FlipFall.Background;
using FlipFall.Theme;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A single Achievement, part of the Achievements collection. Identified through id.
/// </summary>

namespace FlipFall.Progress
{
    [Serializable]
    public class Achievement
    {
        public static AchievementCompletedEvent onComplete = new AchievementCompletedEvent();

        /// <summary>
        /// <int> Fires int for amount of stars awarded for completition</int>
        /// </summary>
        public class AchievementCompletedEvent : UnityEvent<int> { }

        // identifier
        public int id;

        // displayed name
        public string name;

        // descriptive information, aka. how to get the achievement
        public string description;

        // is this achievement completed?
        public bool completed;

        // amount of stars that get awarded upon completition
        public int award;

        // constructor
        public Achievement(int _id, string _name, int _award)
        {
            name = _name;
            id = _id;
            award = _award;
            completed = false;
        }

        public void Complete()
        {
            completed = true;
            onComplete.Invoke(award);
        }
    }
}