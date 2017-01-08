using FlipFall.Background;
using FlipFall.Theme;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dont save this in progress, save it as an extra file

namespace FlipFall.Progress
{
    [Serializable]
    public class Settings
    {
        public bool chargeOnLeftSide;
        public float fxVolume;
        public float musicVolume;
        public bool imageEffects;
        public float backgroundSpeed;
        public int cameraZoomStep;

        public Settings()
        {
            backgroundSpeed = 8F;
            chargeOnLeftSide = true;
            fxVolume = 1F;
            musicVolume = 0.1F;
            cameraZoomStep = 2;
            imageEffects = true;
        }
    }
}