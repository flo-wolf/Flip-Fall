using Impulse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Impulse.UI
{
    public class UIHomeManager : MonoBehaviour
    {
        public static UIHomeManager _instance;

        private void Awake()
        {
            _instance = this;
        }

        public void GameButton()
        {
            Main.SetScene(Main.Scene.game);
        }

        private void Start()
        {
        }
    }
}