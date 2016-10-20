using Impulse.Progress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Impulse.UI
{
    public class UIStarCount : MonoBehaviour
    {
        public static UIStarCount _instance;
        public Text starText;

        public Animator animator;

        private void Start()
        {
            _instance = this;
            UpdateStarCount();
            ProgressData.onStarUpdate.AddListener(UpdateStarCount);
            UIProduct.onBuy.AddListener(ProductBought);
        }

        public static void Show()
        {
            _instance.gameObject.SetActive(true);
            _instance.UpdateStarCount();
            //animation
        }

        private void ProductBought(UIProduct product)
        {
            animator.SetTrigger("buy");
        }

        private void ProductBuyFail(UIProduct product)
        {
            animator.SetTrigger("buyfail");
        }

        public static void Hide()
        {
            _instance.gameObject.SetActive(false);
            //animation
        }

        public void UpdateStarCount()
        {
            starText.text = ProgressManager.GetProgress().starsOwned.ToString();
            // update aniamtion
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