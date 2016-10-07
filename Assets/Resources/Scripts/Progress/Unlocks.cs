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
        public bool editorUnlocked;

        public Unlocks()
        {
            skins = new List<ThemeManager.Skin>();
            editorUnlocked = false;
        }

        public void UnlockSkin(ThemeManager.Skin skin)
        {
            if (!skins.Any(x => x == skin))
                skins.Add(skin);
        }

        public void UnlockEditor()
        {
            editorUnlocked = true;
        }
    }
}