using Sliders;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sliders.UI
{
    public class UIButtonManager : MonoBehaviour
    {
        public static UIButtonManager _instance;
        public static UIButtonClick onButtonClick = new UIButtonClick();
        public static UIButtonRelease onButtonRelease = new UIButtonRelease();
        public Player player;

        public Button playBtn;
        public Button homeBtn;
        public Button backBtn;
        public Button pauseBtn;
        public Button resumeBtn;
        public Button storeBtn;
        public Button infoBtn;
        public Button unlockBtn;

        private bool chargeOnLeftSide = true;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            player = Player._instance;
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
            onButtonClick.Invoke(b);
        }

        public void LevelButtonClicked(Button b, UILevel level)
        {
            onButtonClick.Invoke(b);
        }

        public void PauseBtnClicked(Button b)
        {
            onButtonClick.Invoke(b);
        }

        public void LeftHalfClicked()
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

        public void LeftHalfReleased()
        {
            if (chargeOnLeftSide && player.charging)
            {
                player.Decharge();
            }
        }

        public void RightHalfClicked()
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

        public void RightHalfReleased()
        {
            if (!chargeOnLeftSide && player.charging)
            {
                player.Decharge();
            }
        }

        //Inputs, alles in den Input Manager!
        private void Update()
        {
            if (player.IsAlive())
            {
                //Keyboard
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    player.Die();
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