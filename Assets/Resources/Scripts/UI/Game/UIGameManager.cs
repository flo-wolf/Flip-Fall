using FlipFall;
using FlipFall.Progress;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FlipFall.UI
{
    public class UIGameManager : MonoBehaviour
    {
        public static UIGameManager _instance;
        public static UIButtonClick onButtonClick = new UIButtonClick();
        public static UIButtonRelease onButtonRelease = new UIButtonRelease();
        public Player player;

        public Toggle pauseToggle;
        //public Button resumeBtn;

        //public Animator pauseAnimator;
        public Animation timerAnimaton;

        public Animator animator;

        public float buttonSwitchDelay = 0.25F;

        private bool chargeOnLeftSide = true;

        private bool leftHold = false;
        private bool rightHold = false;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
        }

        private void Start()
        {
            player = Player._instance;
            chargeOnLeftSide = ProgressManager.GetProgress().settings.chargeOnLeftSide;
            timerAnimaton.Play("uiLevelselectionFadeIn");
            //pauseAnimator.SetBool("fadeout", false);
            Main.onSceneChange.AddListener(SceneChanged);

            UIGameTimer.Show();
        }

        public void SceneChanged(Main.ActiveScene sc)
        {
            animator.SetTrigger("fadeout");
            timerAnimaton.Play("uiLevelselectionFadeOut");
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
            Main.SetScene(Main.ActiveScene.home);
            onButtonClick.Invoke(b);
        }

        public void PauseBtnClicked(Toggle t)
        {
            Debug.Log("PauseCLicked");
            if (Player._instance.IsAlive())
            {
                if (t.isOn)
                {
                    Time.timeScale = 0;
                    animator.ResetTrigger("pause");
                    animator.SetTrigger("pause");
                    //pauseAnimator.SetBool("fadeout", true);
                    //StartCoroutine(cPauseResumeSwitch(pauseBtn, resumeBtn));
                    Game.SetGameState(Game.GameState.pause);
                }
                else
                {
                    animator.SetTrigger("resume");
                    Time.timeScale = Game._instance.timestep;
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

        // the left contols-half got clicked
        public void LeftHalfClicked()
        {
            if (player.IsAlive() && !IsPaused())
            {
                leftHold = true;
                player.ReflectToLeft();
            }
        }

        // the left contols-half got released
        public void LeftHalfReleased()
        {
            if (player.IsAlive())
            {
                if (player.charging)
                {
                    leftHold = false;
                    player.Decharge();
                }
            }
        }

        // the right contols-half got clicked
        public void RightHalfClicked()
        {
            if (player.IsAlive() && !IsPaused())
            {
                rightHold = true;
                player.ReflectToRight();
            }
        }

        // the right contols-half got released
        public void RightHalfReleased()
        {
            if (player.IsAlive() && !IsPaused())
            {
                if (player.charging)
                {
                    rightHold = false;
                    player.Decharge();
                }
            }
        }

        /* Old Controls
        // the left contols-half got clicked
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

        // the left contols-half got released
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

        // the right contols-half got clicked
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

        // the right contols-half got released
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
            */

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
                    player.ReflectToRight();
                }
                else if (Input.GetKeyUp(KeyCode.M) && player.charging)
                {
                    player.Decharge();
                }
                else if (Input.GetKeyDown(KeyCode.Y))
                {
                    player.ReflectToLeft();
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