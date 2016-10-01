using Impulse.Levels;
using Impulse.Progress;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Impulse.UI
{
    public class UILevelStar : MonoBehaviour
    {
        private Toggle starToggle;
        private int starScore = 0;

        private void Start()
        {
            starToggle = GetComponent<Toggle>();
        }
    }
}