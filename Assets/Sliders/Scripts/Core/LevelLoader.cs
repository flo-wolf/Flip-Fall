using Sliders.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sliders
{
    public class LevelLoader : MonoBehaviour
    {
        public GameObject myPrefab;
        private static List<Level> levelsLoading;

        public static List<Level> Load()
        {
            return levelsLoading;
        }

        private void Start()
        {
            GameObject go = (GameObject)Instantiate(myPrefab);
        }
    }
}