using Impulse.Audio;
using Impulse.Levels;
using Impulse.Progress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all UI Elements of the respective scene
/// </summary>

namespace Impulse.UI
{
    public class UIShopManager : MonoBehaviour
    {
        public static UIShopManager _instance;
        public static List<UIProduct> uiProducts;

        public Animator animator;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            Main.onSceneChange.AddListener(SceneChanging);

            // collect all UIProducts, maybe do this in coroutine
            uiProducts = new List<UIProduct>();
            UIProduct[] products = GetComponentsInChildren<UIProduct>();

            foreach (UIProduct p in products)
            {
                uiProducts.Add(p);
            }
        }

        private void SceneChanging(Main.Scene scene)
        {
            animator.SetTrigger("fadeout");
        }

        public void HomeButtonClicked()
        {
            SoundManager.ButtonClicked();
            Main.SetScene(Main.Scene.home);
        }
    }
}