﻿using Impulse;
using Impulse.Progress;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Impulse.UI
{
    public class UIGameButtons : MonoBehaviour
    {
        public static UIGameButtons _instance;
        public static UIButtonClick onButtonClick = new UIButtonClick();
        public static UIButtonRelease onButtonRelease = new UIButtonRelease();
        public Player player;

        public Toggle pauseToggle;
        //public Button resumeBtn;

        //public Animator pauseAnimator;
        public Animation timerAnimator;

        public float buttonSwitchDelay = 0.25F;

        private bool chargeOnLeftSide = true;

        private void Awake()
        {
            _instance = this;
            //pauseAnimator.SetBool("fadeout", false);
        }

        private void Start()
        {
            player = Player._instance;
            chargeOnLeftSide = ProgressManager.GetProgress().settings.chargeOnLeftSide;
            timerAnimator.Play("uiLevelselectionFadeIn");
            //pauseAnimator.SetBool("fadeout", false);
            Main.onSceneChange.AddListener(SceneChanged);
        }

        public void SceneChanged(Main.Scene sc)
        {
            timerAnimator.Play("uiLevelselectionFadeOut");
            //pauseAnimator.SetBool("fadeout", true);
        }

        public static void Show(Button b)
        {
            //FadeInAnimations
            //b.gameObject.SetActive(true);
        }

        public static void Hide(Button b)
        {
            //FadeOutAnimations
            //b.gameObject.SetActive(false);
        }

        public void HomeBtnClicked(Button b)
        {
            Main.SetScene(Main.Scene.home);
            onButtonClick.Invoke(b);
        }

        public void PauseBtnClicked(Toggle t)
        {
            if (Player._instance.IsAlive())
            {
                if (t.isOn)
                {
                    Time.timeScale = 0;
                    //pauseAnimator.SetBool("fadeout", true);
                    //StartCoroutine(cPauseResumeSwitch(pauseBtn, resumeBtn));
                    Game.SetGameState(Game.GameState.pause);
                }
                else
                {
                    Time.timeScale = 1;
                    //pauseAnimator.SetBool("fadeout", true);
                    //StartCoroutine(cPauseResumeSwitch(pauseBtn, resumeBtn));
                    Game.SetGameState(Game.GameState.playing);
                }
            }
        }

        //public void ResumeBtnClicked()
        //{
        //    if (Player._instance.IsAlive())
        //    {
        //        Time.timeScale = 1;
        //        //pauseAnimator.SetBool("fadeout", false);
        //        //StartCoroutine(cPauseResumeSwitch(resumeBtn, pauseBtn));
        //        Game.SetGameState(Game.GameState.playing);
        //    }
        //}

        private IEnumerator cPauseResumeSwitch(Button deactivateBtn, Button activateBtn)
        {
            //since we have paused the timescale "new WaitForSeconds" wont work, this is a nice workaround
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(buttonSwitchDelay));
            deactivateBtn.gameObject.SetActive(false);
            activateBtn.gameObject.SetActive(true);
            yield break;
        }

        public void LevelButtonClicked(Button b, UILevel level)
        {
            onButtonClick.Invoke(b);
        }

        private bool IsPaused()
        {
            if (Game.gameState == Game.GameState.pause)
                return true;
            else
                return false;
        }

        public void LeftHalfClicked()
        {
            if (player.IsAlive() && !IsPaused())
            {
                if (chargeOnLeftSide && !player.charging)
                {
                    player.Charge();
                }
                else if (!chargeOnLeftSide)
                {
                    player.Reflect();
                }
            }
        }

        public void LeftHalfReleased()
        {
            if (player.IsAlive())
            {
                if (chargeOnLeftSide && player.charging)
                {
                    player.Decharge();
                }
            }
        }

        public void RightHalfClicked()
        {
            if (player.IsAlive() && !IsPaused())
            {
                if (chargeOnLeftSide)
                {
                    player.Reflect();
                }
                else if (!chargeOnLeftSide)
                {
                    player.Charge();
                }
            }
        }

        public void RightHalfReleased()
        {
            if (player.IsAlive() && !IsPaused())
            {
                if (!chargeOnLeftSide && player.charging)
                {
                    player.Decharge();
                }
            }
        }

        //Inputs, alles in den Input Manager!
        private void Update()
        {
            if (player.IsAlive() && !IsPaused())
            {
                //Keyboard
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    player.Die(player.transform.position);
                }
                else if (Input.GetKeyDown(KeyCode.M))
                {
                    player.Reflect();
                }
                else if (Input.GetKeyDown(KeyCode.Y) && !player.charging)
                {
                    player.Charge();
                }
                else if (Input.GetKeyUp(KeyCode.Y) && player.charging)
                {
                    player.Decharge();
                }
            }
        }

        public class UIButtonClick : UnityEvent<Button> { }

        public class UIButtonRelease : UnityEvent<Button> { }
    }
}