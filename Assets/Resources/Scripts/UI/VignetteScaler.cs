using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// scales the vignette image to always fit the current resolution
/// </summary>

namespace FlipFall.UI
{
    [ExecuteInEditMode]
    public class VignetteScaler : MonoBehaviour
    {
        private Image image;

        private void Start()
        {
            Camera cam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
            image = GetComponent<Image>();
            if (cam != null && image != null)
            {
                //float height = cam.orthographicSize * 2.0F;
                //float width = height * Screen.width / Screen.height;
                //image.rectTransform.sizeDelta = new Vector2(Screen.width / 2, Screen.height / 2);
            }
        }
    }
}