using Impulse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Impulse.UI
{
    public class UIHomeManager : MonoBehaviour
    {
        public static UIHomeManager _instance;

        public Animation fadeAnimation;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            FadeIn();
        }

        public void GameButton()
        {
            Main.SetScene(Main.Scene.game);
            FadeOut();
        }

        private void FadeIn()
        {
            fadeAnimation.Play("fadeFromBlack");
        }

        private void FadeOut()
        {
            fadeAnimation.Play("fadeToBlack");
        }
    }
}