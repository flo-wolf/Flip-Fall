using FlipFall.Progress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlipFall.UI
{
    public class UIStarCount : MonoBehaviour
    {
        public static UIStarCount _instance;
        public Text starText;

        public Animator animator;

        private void Start()
        {
            _instance = this;
            WalletUpdate();
            ProgressData.onWalletUpdate.AddListener(WalletUpdate);
        }

        public static void Show()
        {
            _instance.gameObject.SetActive(true);
            _instance.WalletUpdate();
            //animation
        }

        public static void Hide()
        {
            _instance.gameObject.SetActive(false);
            //animation
        }

        public void WalletUpdate()
        {
            starText.text = ProgressManager.GetProgress().starsOwned.ToString();
        }

        public void StarsChanged()
        {
        }

        // old way of doing it, calculate the starcount by adding all highscore stars together
        //public void UpdateStarCount()
        //{
        //    int starCount = 0;
        //    List<Highscore> highscores = ProgressManager.GetProgress().highscores;
        //    foreach (Highscore h in highscores)
        //    {
        //        starCount = starCount + h.starCount;
        //    }
        //    ProgressManager.GetProgress().starsOwned = starCount;
        //    starText.text = starCount.ToString();
        //}
    }
}