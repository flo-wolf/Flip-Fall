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

        public Button playBtn;
        public Button homeBtn;
        public Button backBtn;
        public Button pauseBtn;
        public Button resumeBtn;
        public Button storeBtn;
        public Button infoBtn;
        public Button unlockBtn;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
        }

        public static void Show(Button b)
        {
            //FadeInAnimations
            b.gameObject.SetActive(true);
        }

        public static void Hide(Button b)
        {
            //FadeOutAnimations
            b.gameObject.SetActive(false);
        }

        public void PlayBtnClicked(Button b)
        {
            if (b == playBtn)
            {
                onButtonClick.Invoke(b);
                Game.SetGameState(Game.GameState.playing);
                b.gameObject.SetActive(false);
            }
            else
                Debug.LogError("[UIButtonManager] Button tries to call the wrong function.");
        }

        public void HomeBtnClicked(Button b)
        {
            onButtonClick.Invoke(b);
        }

        public void PauseBtnClicked(Button b)
        {
            onButtonClick.Invoke(b);
        }

        public class UIButtonClick : UnityEvent<Button> { }

        public class UIButtonRelease : UnityEvent<Button> { }
    }
}