using Sliders.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sliders
{
    [SerializeField]
    public class LevelManager : MonoBehaviour
    {
        //if 0 - infinite
        public int maxLevelObjects;

        public static List<Level> _levels = new List<Level>();

        // Use this for initialization
        private void LoadLevel()
        {
            //ProgressManager.pr
            //load levels from file, one of them is marked as lastPlayed
        }

        // Update is called once per frame
        private void SaveLevel()
        {
        }
    }
}