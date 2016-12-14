using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FlipFall.UI
{
    public class UIScrollRectContentResizer : MonoBehaviour
    {
        public static ContentChangeEvent onContentChange = new ContentChangeEvent();

        public class ContentChangeEvent : UnityEvent { }

        public ScrollRect scrollRect;

        private void Start()
        {
            onContentChange.AddListener(ResizeNeeded);
        }

        private void ResizeNeeded()
        {
            if (scrollRect != null)
            {
            }
        }

        private void Update()
        {
        }
    }
}