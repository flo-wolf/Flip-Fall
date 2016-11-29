using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Calls an UIScrollElement attached to a scrollRect, whenever it leaves an area
/// Checks wheter or not an UIScrollElement needs to be animated
/// </summary>

namespace FlipFall.UI
{
    public class UIScrollFade : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public static UIScrollFade _instance;

        // the scrollRect
        public ScrollRect scrollRect;

        public RectTransform canvasRect;
        public Canvas canvas;

        public bool vertical = true;

        // ignores the insideArea property and uses the scrollable area of the ScrollRect instead
        public bool useScollRectAsInsideArea = false;

        // the upper bound for the area in which elements are "faded-in". Everything above will be "faded-out"
        public float upperInsideHeight;

        // the upper height border on which elements are faded back in
        public float upperFadeHeight;

        // lower bound, everything below will be "faded-out"
        public float lowerInsideHeight;

        // the lower height border on which elements are faded back in
        public float lowerFadeHeight;

        // visualize the bounds in the Editor
        public bool visualizeHeights = true;

        // list of elements attached as a child to the scrollrect
        public static List<UIScrollElement> scrollElements;

        // drag direction
        private bool dragUp = true;

        private bool dragDownPossible = true;
        private bool dragUpPossible = true;
        private bool dragStopped = false;
        private float vertNormalPosition;

        private float upInside;
        private float lowInside;
        private float upFade;
        private float lowFade;

        private void Start()
        {
            if (_instance == null)
                _instance = this;

            // get worldposition inside positions to check

            // middle of the canvas
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(canvas.transform.position);

            upInside = (canvasRect.sizeDelta.y * canvasRect.localScale.y) * upperInsideHeight;
            lowInside = (canvasRect.sizeDelta.y * canvasRect.localScale.y) * lowerInsideHeight;
            upFade = (canvasRect.sizeDelta.y * canvasRect.localScale.y) * upperFadeHeight;
            lowFade = (canvasRect.sizeDelta.y * canvasRect.localScale.y) * lowerFadeHeight;

            UpdateScrollElements();

            if (useScollRectAsInsideArea)
            {
                //insideBounds = new Bounds();
                //RectTransform r = scrollRect.content;
                //insideBounds.center = scrollRect.content.transform.position;
                //insideBounds.size = new Vector3(r.rect.width, r.rect.height);
            }
        }

        public void UpdateScrollElements()
        {
            StartCoroutine(cGetScrollElements());
        }

        private void OnDrawGizmos()
        {
            if (visualizeHeights)
            {
                Gizmos.color = Color.blue;

                float width = (canvasRect.sizeDelta.x * canvasRect.localScale.x);

                // upper inside Line
                Vector3 from = new Vector3(0, upInside, 0);
                Vector3 to = new Vector3(width, upInside, 0);
                Gizmos.DrawLine(from, to);

                // lower inside line
                from = new Vector3(0, lowInside, 0);
                to = new Vector3(width, lowInside, 0);
                Gizmos.DrawLine(from, to);

                // upper fade Line
                from = new Vector3(0, upFade, 0);
                to = new Vector3(width, upFade, 0);
                Gizmos.DrawLine(from, to);

                // lower fade line
                from = new Vector3(0, lowFade, 0);
                to = new Vector3(width, lowFade, 0);
                Gizmos.DrawLine(from, to);
            }
        }

        private IEnumerator cGetScrollElements()
        {
            yield return new WaitForEndOfFrame();
            scrollElements = new List<UIScrollElement>();
            for (int i = 0; i < scrollRect.content.childCount; i++)
            {
                UIScrollElement element = scrollRect.content.GetChild(i).GetComponent<UIScrollElement>();
                if (element != null)
                {
                    // set the inside value to fit its first position

                    print("INSIDE - element " + i + ", position " + element.transform.position);

                    if (!IsInside(element.transform.position))
                    {
                        // begin fading animations if object is actually outside but still set to inside
                        //element.gameObject.SetActive(false);
                        element.InstantFadeOut();
                    }
                    else
                    {
                        element.gameObject.SetActive(true);
                        element.FadeIn();
                        // element.FadeIn();
                    }

                    // reference the element
                    scrollElements.Add(element);
                }
            }
            StartCoroutine(cCorrectInsideSetting());
            yield break;
        }

        private void Update()
        {
            // is the user dragging
            Vector2 vel = scrollRect.velocity;
            if (vel != Vector2.zero)
            {
                if (vel.y > 0)
                    dragUp = true;
                else
                    dragUp = false;
                StartCoroutine(cCorrectInsideSetting());
            }
        }

        // correct the current state of an scrollelement, if it should be faded out(-> outside the inside area) and its not, fade it and vice versa
        private IEnumerator cCorrectInsideSetting()
        {
            for (int i = 0; i < scrollElements.Count; i++)
            {
                UIScrollElement element = scrollElements[i];

                //if (i == scrollElements.Count - 1 && element.transform.position.y >= lowInside)
                //    dragUpPossible = false;
                //else if (i == 0 && element.transform.position.y <= upInside)
                //    dragDownPossible = false;

                bool inside = IsInside(element.transform.position);

                if (inside && element.isFadedIn)
                {
                    // first product
                    if (i == 0 && !dragUp)
                    {
                        dragDownPossible = true;
                        dragUpPossible = false;
                    }
                    // last product
                    else if (i == scrollElements.Count - 1 && dragUp)
                    {
                        dragUpPossible = false;
                        dragDownPossible = true;
                    }
                }

                // element is not inside => perform checks if thats correct
                else if (!inside)
                {
                    // first product
                    if (i == 0 && dragUp && IsInside(scrollElements[scrollElements.Count - 1].transform.position))
                    {
                        dragDownPossible = true;
                        dragUpPossible = false;
                    }
                    // last product
                    else if (i == scrollElements.Count - 1 && !dragUp && IsInside(scrollElements[0].transform.position))
                    {
                        dragUpPossible = true;
                        dragDownPossible = false;
                    }

                    // begin fading animations if object is actually outside but still set to inside
                    else if (element.isFadedIn)
                        element.FadeOut();
                    else
                    {
                        dragDownPossible = true;
                        dragUpPossible = true;
                    }
                }
                // element is between the fade lines, and outside the inside area
                else if (CanFade(element.transform.position, element) && !element.isFadedIn)
                {
                    // begin fading animations if object is actually inside but not set to inside
                    element.gameObject.SetActive(true);
                    element.FadeIn();

                    //// the first item is inside, which means we cant drag
                    //if (i == 0)
                    //{
                    //    dragDownPossible = false;
                    //}
                    //else if (i == scrollElements.Count - 1 && scrollElements.Count > 2)
                    //{
                    //    dragUpPossible = false;
                    //}
                }
            }
            yield break;
        }

        //checks if a position is inside the scrollArea
        private bool IsInside(Vector3 position)
        {
            // in between the bounds => inside
            if (upInside > position.y && lowInside < position.y)
                //if (upInside > position.y && dragUp || lowInside < position.y && !dragUp)
                return true;
            return false;
        }

        // checks if a position is indie the fade area
        private bool CanFade(Vector3 position, UIScrollElement element)
        {
            // in between the bounds => inside
            //if ((int)position.y == (int)upFade || (int)position.y == (int)lowFade)
            //if (upFade > position.y && !dragUp || lowFade < position.y && dragUp)
            if ((upFade > position.y && lowFade < position.y) && !element.isFadedIn)
                return true;
            return false;
        }

        // while dragging
        public void OnDrag(PointerEventData eventData)
        {
            //check if a drag is neccassary, i.e. if any elements are outside
            if (scrollElements.Any(x => x.isFadedIn == false))
            {
                // dragging up while dragging up shouldn't be possible
                if (eventData.delta.y < 0 && !dragDownPossible)
                {
                    scrollRect.StopMovement();
                    scrollRect.enabled = false;
                    scrollRect.vertical = false;
                    vertNormalPosition = scrollRect.verticalNormalizedPosition;
                    dragStopped = true;
                }
                // dragging down while dragging down shouldn't be possible
                else if (eventData.delta.y > 0 && !dragUpPossible)
                {
                    scrollRect.StopMovement();
                    scrollRect.vertical = false;
                    scrollRect.enabled = false;
                    vertNormalPosition = scrollRect.verticalNormalizedPosition;
                    dragStopped = true;
                }
            }
            else
            {
                scrollRect.StopMovement();
                scrollRect.vertical = false;
                scrollRect.enabled = false;
            }
            //else
            //{
            //    scrollRect.enabled = true;
            //    scrollRect.vertical = true;
            //}

            //// drag was stopped, but the drag direction is allowed
            //if (dragStopped && (eventData.delta.y > 0 && dragUpPossible || eventData.delta.y < 0 && dragDownPossible))
            //{
            //    scrollRect.StopMovement();
            //    scrollRect.verticalNormalizedPosition = vertNormalPosition;
            //    scrollRect.vertical = true;
            //    scrollRect.enabled = true;
            //    dragStopped = false;
            //}

            //else if (dragStopped)
            //{
            //    scrollRect.verticalNormalizedPosition = vertNormalPosition;
            //}
        }

        // begin of drag
        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        // end of drag
        public void OnEndDrag(PointerEventData eventData)
        {
            scrollRect.enabled = true;
            scrollRect.vertical = true;
        }
    }
}