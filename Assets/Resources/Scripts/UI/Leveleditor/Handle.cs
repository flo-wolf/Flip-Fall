using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attached to a Handler Button
/// Controls the handler button movement, responsible for moving mesh verticies, and restricts it to allowed areas.
/// See UIScrollFade on how to use corcoutines to check for allowed drag positions
/// </summary>

namespace FlipFall.Editor
{
    public class Handle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
    {
        [HideInInspector]
        public bool dragging;
        public static bool snapToGrid = false;
        public Color unselectedColor;
        public Color selectedColor;

        private float fadeDuration = 0.5F;
        private Image image;

        private Vector3[] beforeDragPositions;  // selected handler positions before the drag
        private Vector3 quickDragBefore;
        private Vector3 dragStart;

        private float tapTime = 0.4F;       // allowed timeframe to register a double tap
        private float lastTap;              // time since the last tap

        public void Awake()
        {
            image = GetComponent<Image>();
            StartCoroutine(cFadeIn());
        }

        private IEnumerator cFadeIn()
        {
            if (image != null)
            {
                Color oColor = image.color;
                Color transColor = image.color;
                transColor.a = 0F;

                float t = 0F;
                while (t < 1.0f)
                {
                    t += Time.deltaTime * (Time.timeScale / fadeDuration);
                    image.color = Color.Lerp(transColor, oColor, t);
                    yield return 0;
                }
            }
            else
                Debug.Log("Color Fading Handlers failed because there is no handler image attached.");
            yield break;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // this is a valid double tap => select the clicked vertex
            if ((Time.time - lastTap) < tapTime)
            {
                // this handle isn't selected yet, add it to the selection
                if (!VertHandler.selectedHandles.Any(x => x == this))
                {
                    VertHandler.selectedHandles.Add(this);
                    GetComponent<Image>().color = selectedColor;
                    print("selected this handle at " + transform.position + " there are " + VertHandler.selectedHandles.Count + " elements in the selection");
                }
                // this handle is already selected, deselect it
                else
                {
                    VertHandler.selectedHandles.Remove(this);
                    GetComponent<Image>().color = unselectedColor;
                    print("selected this handle at " + transform.position + " there are " + VertHandler.selectedHandles.Count + " elements in the selection");
                }
            }
            else if (!VertHandler.selectedHandles.Any(x => x == this))
            {
                VertHandler.quickDragHandle = this;
            }
            else
            {
                VertHandler.quickDragHandle = null;
            }
            lastTap = Time.time;
        }

        // begin of drag, called once
        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;
            dragStart = Camera.main.ScreenToWorldPoint(eventData.position);
            dragStart.z = 0;

            // save original positions
            beforeDragPositions = new Vector3[VertHandler.selectedHandles.Count];
            for (int i = 0; i < VertHandler.selectedHandles.Count; i++)
            {
                beforeDragPositions[i] = VertHandler.selectedHandles[i].transform.position;
            }

            if (VertHandler.quickDragHandle != null)
            {
                quickDragBefore = VertHandler.quickDragHandle.transform.position;
            }
        }

        // while dragging, transform the selected handles
        public void OnDrag(PointerEventData eventData)
        {
            Vector3 dragPos = Camera.main.ScreenToWorldPoint(eventData.position);
            dragPos.z = 0;

            Vector3 dragDelta = (dragStart - dragPos);
            dragDelta.z = 0;

            print(dragPos);

            // alter positions based on the original position plus the drag delta
            for (int i = 0; i < VertHandler.selectedHandles.Count; i++)
            {
                //Vector3 newPos = VertHandler.selectedHandles[i].transform.position;
                //newPos += new Vector3(eventData.delta.x / 5, eventData.delta.y / 5, 0);
                VertHandler.selectedHandles[i].transform.position = beforeDragPositions[i] - dragDelta;
            }
            if (VertHandler.quickDragHandle != null)
            {
                VertHandler.quickDragHandle.transform.position = quickDragBefore - dragDelta;
            }
        }

        // end of drag
        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
            // alter positions based on the original position plus the drag delta
            for (int i = 0; i < VertHandler.selectedHandles.Count; i++)
            {
                Vector3 newPos = VertHandler.selectedHandles[i].transform.position;

                // snapping enabled?
                if (GridOverlay._instance != null && GridOverlay._instance.snapToGrid)
                {
                    newPos = Snap(newPos, GridOverlay._instance.smallStep);
                }

                VertHandler.selectedHandles[i].transform.position = newPos;
            }

            if (VertHandler.quickDragHandle != null)
            {
                Vector3 newPos = VertHandler.quickDragHandle.transform.position;
                // snapping enabled?
                if (GridOverlay._instance != null && GridOverlay._instance.snapToGrid)
                {
                    newPos = Snap(newPos, GridOverlay._instance.smallStep);
                }
                VertHandler.quickDragHandle.transform.position = newPos;
            }

            VertHandler.quickDragHandle = null;
        }

        // snapping
        private Vector3 Snap(Vector3 v, float snapValue)
        {
            return new Vector3
            (
                snapValue * Mathf.Round(v.x / snapValue),
                snapValue * Mathf.Round(v.y / snapValue),
                v.z
            );

            //float step = GridOverlay._instance.smallStep;
            //Vector2 start = GridOverlay._instance.start;
            //Vector2 end = GridOverlay._instance.end;
            //float closestY = 0;
            //float closestX = 0;

            //// Find closest vertical snappoint
            //for (float j = start.y; j <= end.y; j += step)
            //{
            //    if (Mathf.Abs(v.y - j) < Mathf.Abs(v.y - closestY))
            //    {
            //        closestY = j;
            //    }
            //}

            //// Find closest horizontal snappoint
            //for (float i = start.x; i <= end.x; i += step)
            //{
            //    if (Mathf.Abs(v.x - i) < Mathf.Abs(v.x - closestX))
            //    {
            //        closestX = i;
            //    }
            //}

            //return new Vector3(closestX, closestY, v.z);
        }
    }
}