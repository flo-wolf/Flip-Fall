using Impulse.Background;
using Impulse.Theme;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// information about what items were bought/unlocked, including skins and editoritems

namespace Impulse.Progress
{
    [Serializable]
    public class Unlocks
    {
        public List<ThemeManager.Skin> skins;

        public Unlocks()
        {
            skins = new List<ThemeManager.Skin>();
        }

        public void AddSkin(ThemeManager.Skin skin)
        {
            if (!skins.Any(x => x == skin))
                skins.Add(skin);
        }
    }
}