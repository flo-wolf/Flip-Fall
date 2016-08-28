using Sliders.Progress;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.UI
{
    public class UIStarCount : MonoBehaviour
    {
        public static UIStarCount _instance;
        public Text starText;

        private void Start()
        {
            _instance = this;
            UpdateStarCount();
        }

        public static void Show()
        {
            _instance.gameObject.SetActive(true);
            _instance.UpdateStarCount();
            //animation
        }

        public static void Hide()
        {
            _instance.gameObject.SetActive(false);
            //animation
        }

        public void UpdateStarCount()
        {
            starText.text = ProgressManager.GetProgress().totalStars.ToString();
        }
    }
}