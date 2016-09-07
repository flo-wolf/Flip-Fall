using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dont save this in progress, save it as an extra file

namespace Impulse.Progress
{
    [Serializable]
    public class Settings
    {
        public bool chargeOnLeftSide;
        public float fxVolume;
        public float musicVolume;
        public bool imageEffects;

        public Settings()
        {
            chargeOnLeftSide = true;
            fxVolume = 1F;
            musicVolume = 1F;
            imageEffects = true;
        }
    }
}