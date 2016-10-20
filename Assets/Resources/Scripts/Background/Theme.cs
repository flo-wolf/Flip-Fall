using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Impulse.Theme
{
    public class Theme : MonoBehaviour
    {
        public ThemeManager.Skin horizonSkin;
        public Color backgorundColor;

        // change to colors??? => pro: less materials spamming the list; contra: no shader variation possible => per theme special fx
        public Material playerMaterial;
        public Material finishMaterial;
        public Material turretMaterial;
        public Material moveZoneMaterial;

        public Mesh playerMesh;
    }
}