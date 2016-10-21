using System;
using System.Collections;
using System.Collections.Generic;
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
        // the scrollRect
        public ScrollRect scrollRect;

        public bool vertical = true;

        // ignores the insideArea property and uses the scrollable area of the ScrollRect instead
        public bool useScollRectAsInsideArea = false;

        // the upper bound for the area in which elements are "faded-in". Everything above will be "faded-out"
        public float upperInsideBound;

        // lower bound, everything below will be "faded-out"
        public float lowerInsideBound;

        // visualize the bounds in the Editor
        public bool visualizeBounds = true;

        // list of elements attached as a child to the scrollrect
        private List<UIScrollElement> scrollElements;

        // is the user dragging? If so, check the inside values
        private bool dragging = false;

        private bool dragDownPossible = true;
        private bool dragUpPossible = true;
        private bool dragStopped = false;
        private float vertNormalPosition;

        private void Start()
        {
            StartCoroutine(cGetScrollElements());

            if (useScollRectAsInsideArea)
            {
                //insideBounds = new Bounds();
                //RectTransform r = scrollRect.content;
                //insideBounds.center = scrollRect.content.transform.position;
                //insideBounds.size = new Vector3(r.rect.width, r.rect.height);
            }

            StartCoroutine(cCorrectInsideSetting());
        }

        private void OnDrawGizmos()
        {
            if (visualizeBounds)
            {
                Gizmos.color = Color.blue;

                // upper Line
                Vector3 from = new Vector3(0, upperInsideBound, 0);
                Vector3 to = new Vector3(1200, upperInsideBound, 0);
                Gizmos.DrawLine(from, to);

                // lower line
                from = new Vector3(0, lowerInsideBound, 0);
                to = new Vector3(1200, lowerInsideBound, 0);
                Gizmos.DrawLine(from, to);
            }
        }

        private IEnumerator cGetScrollElements()
        {
            scrollElements = new List<UIScrollElement>();
            for (int i = 0; i < scrollRect.content.childCount; i++)
            {
                if (scrollRect.content.GetChild(i).GetComponent<UIScrollElement>() != null)
                {
                    // set the inside value to fit its first position
                    UIScrollElement element = scrollRect.content.GetChild(i).GetComponent<UIScrollElement>();
                    if (!IsInside(element.transform.position))
                    {
                        // begin fading animations if object is actually outside but still set to inside
                        if (element.inside)
                            element.FadeOut();
                    }
                    else
                    {
                        // begin fading animations if object is actually inside but not set to inside
                        if (!element.inside)
                            element.FadeIn();
                    }

                    // reference the element
                    scrollElements.Add(element);
                }
            }
            yield break;
        }

        private void FixedUpdate()
        {
            // is the user dragging
            if (scrollRect.velocity != Vector2.zero)
            {
                StartCoroutine(cCorrectInsideSetting());
            }
        }

        private IEnumerator cCorrectInsideSetting()
        {
            for (int i = 0; i < scrollElements.Count; i++)
            {
                UIScrollElement element = scrollElements[i];

                // element is not inside
                if (!IsInside(element.transform.position))
                {
                    // begin fading animations if object is actually outside but still set to inside
                    if (element.inside)
                        element.FadeOut();

                    // first product
                    if (element.position == UIScrollElement.Position.first)
                    {
                        dragDownPossible = true;
                    }
                    // last product
                    else if (element.position == UIScrollElement.Position.last)
                    {
                        dragUpPossible = true;
                    }
                }
                else
                {
                    // begin fading animations if object is actually inside but not set to inside
                    if (!element.inside)
                        element.FadeIn();

                    // the first item is inside, which means we cant drag
                    if (i == 0)
                    {
                        dragDownPossible = false;
                    }
                    else if (i == scrollElements.Count - 1)
                    {
                        dragUpPossible = false;
                    }
                }
            }
            yield break;
        }

        //checks if a position is inside the scrollArea
        private bool IsInside(Vector3 position)
        {
            // in between the bounds => inside
            if (upperInsideBound > position.y && lowerInsideBound < position.y)
                return true;
            return false;
        }

        // while dragging
        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("dragging " + eventData.delta.y);

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
            dragging = true;
        }

        // end of drag
        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
            scrollRect.enabled = true;
            scrollRect.vertical = true;
        }
    }
}