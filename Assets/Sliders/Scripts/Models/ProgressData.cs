using Sliders.UI;
using System;
using System.Collections.Generic;

namespace Sliders.Models
{
    [Serializable]
    public class ProgressData
    {
        public static List<Scoreboard> scoreboards { get; set; }
        public int lastPlayedLevel;
        public int coins;
        //add: unlocks, achievements, stats etc...
    }
}