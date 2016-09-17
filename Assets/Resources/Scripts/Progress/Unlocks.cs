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
        public List<HorizonManager.Skin> skins;

        public Unlocks()
        {
            skins = new List<HorizonManager.Skin>();
        }

        public void AddSkin(HorizonManager.Skin skin)
        {
            if (!skins.Any(x => x == skin))
                skins.Add(skin);
        }
    }
}