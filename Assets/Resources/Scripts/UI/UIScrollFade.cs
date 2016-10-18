using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Calls an UIScrollElement attached to a scrollRect, whenever it leaves an area
/// Checks wheter or not an UIScrollElement needs to be animated
/// </summary>

namespace FlipFall.UI
{
    public class UIScrollFade : MonoBehaviour
    {
        // the scrollRect
        public ScrollRect scrollRect;

        // ignores the insideArea property and uses the scrollable area of the ScrollRect instead
        public bool useScollRectAsInsideArea = false;

        // the area in which elements are "faded-in"
        public Bounds insideBounds = new Bounds();

        public bool visualizeBounds = true;

        public List<UIScrollElement> scrollElements;

        private bool dragging = false;

        private void Start()
        {
            StartCoroutine(GetScrollElements());

            if (useScollRectAsInsideArea)
            {
                insideBounds = new Bounds();
                insideBounds.center = scrollRect.viewport.position;
                insideBounds.size = scrollRect.viewport.sizeDelta;
            }

            if (visualizeBounds)
            {
                Gizmos.DrawWireCube(insideBounds.center, insideBounds.size);
            }
        }

        private IEnumerator GetScrollElements()
        {
            scrollElements = new List<UIScrollElement>();
            for (int i = 0; i < scrollRect.transform.childCount; i++)
            {
                if (scrollRect.transform.GetChild(i).GetComponent<UIScrollElement>() != null)
                {
                    scrollElements.Add(scrollRect.transform.GetChild(i).GetComponent<UIScrollElement>());
                }
            }
            yield break;
        }

        private void FixedUpdate()
        {
            // check the validity of the inside statement
            if (dragging)
            {
                foreach (UIScrollElement element in scrollElements)
                {
                    if (!IsInside(element.transform.position))
                    {
                    }
                    // begin fading animations if needed
                }
            }
        }

        //checks if a position is inside the scrollArea
        private bool IsInside(Vector3 position)
        {
            if (insideBounds.Contains(position))
                return true;
            return false;
        }

        // Event Listener, called when the elements get moved
        private void OnDrag()
        {
            dragging = true;
        }

        private void OnRelease()
        {
            dragging = false;
        }
    }
}