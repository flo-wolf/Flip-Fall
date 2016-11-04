using FlipFall.Levels;
using FlipFall.Progress;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FlipFall.UI
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