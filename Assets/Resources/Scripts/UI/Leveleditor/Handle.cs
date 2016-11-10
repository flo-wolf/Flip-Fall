using FlipFall.Levels;
using FlipFall.UI;
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
            EditorInput.itemDragged = true;
            // this is a valid double tap => select the clicked vertex
            if ((Time.time - lastTap) < tapTime)
            {
                // this handle isn't selected yet, add it to the selection
                if (!VertHandler.selectedHandles.Any(x => x == this))
                {
                    VertHandler.SelectHandle(this);
                    GetComponent<Image>().color = selectedColor;
                    //print("selected this handle at " + transform.position + " there are " + VertHandler.selectedHandles.Count + " elements in the selection");
                }
                // this handle is already selected, deselect it
                else
                {
                    VertHandler.DeselectHandle(this);
                    GetComponent<Image>().color = unselectedColor;
                    //print("selected this handle at " + transform.position + " there are " + VertHandler.selectedHandles.Count + " elements in the selection");
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

            StartCoroutine(cDragDisableDelayed());
        }

        // begin of drag, called once
        public void OnBeginDrag(PointerEventData eventData)
        {
            EditorInput.itemDragged = true;
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

            Vector3 newPos;

            // alter positions based on the original position plus the drag delta
            for (int i = 0; i < VertHandler.selectedHandles.Count; i++)
            {
                newPos = beforeDragPositions[i] - dragDelta;

                // check if the desired position is not crossing the triangle
                if (VertHelper.IsHandlerPositionValid(transform.position, newPos))
                {
                    // update the selection triangle verts
                    Vector3 localOldPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(VertHandler.selectedHandles[i].transform.position);
                    Vector3 localnewPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(newPos);
                    if (VertHandler.selectionTriangleVerts.Any(x => x == localOldPos))
                    {
                        // get all vector entries that are equal to this position and replace them with the newest position
                        List<int> indexes = Enumerable.Range(0, VertHandler.selectionTriangleVerts.Count).Where(k => VertHandler.selectionTriangleVerts[k] == localOldPos).ToList();

                        foreach (int ind in indexes)
                        {
                            VertHandler.selectionTriangleVerts[ind] = localnewPos;
                        }
                    }
                    VertHandler.selectedHandles[i].transform.position = newPos;
                }
            }
            if (VertHandler.quickDragHandle != null)
            {
                newPos = quickDragBefore - dragDelta;
                if (VertHelper.IsHandlerPositionValid(transform.position, newPos))
                    VertHandler.quickDragHandle.transform.position = newPos;
            }
        }

        // end of drag
        public void OnEndDrag(PointerEventData eventData)
        {
            // alter positions based on the original position plus the drag delta
            for (int i = 0; i < VertHandler.selectedHandles.Count; i++)
            {
                Vector3 newPos = VertHandler.selectedHandles[i].transform.position;

                // snapping enabled?
                if (GridOverlay._instance != null && GridOverlay._instance.snapToGrid)
                {
                    newPos = VertHelper.Snap(newPos);
                }

                VertHandler.selectedHandles[i].transform.position = newPos;
            }

            if (VertHandler.quickDragHandle != null)
            {
                Vector3 newPos = VertHandler.quickDragHandle.transform.position;
                // snapping enabled?
                if (GridOverlay._instance != null && GridOverlay._instance.snapToGrid)
                {
                    newPos = VertHelper.Snap(newPos);
                }
                VertHandler.quickDragHandle.transform.position = newPos;
            }

            StartCoroutine(cDragDisableDelayed());
            VertHandler.quickDragHandle = null;
        }

        private IEnumerator cDragDisableDelayed()
        {
            yield return new WaitForSecondsRealtime(0.1F);
            EditorInput.itemDragged = false;
            yield break;
        }
    }
}

// snap-----------
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

// calc triangle crossing -----------
//print("i: " + i + ", current: " + currentPos + ", p1: " + p1 + ", p2: " + p2 + ", p3: " + p3);

//// the triangle contains this objects position, check if the desired destination is outside the allowed area
//if (currentPos == p1)
//{
//    // we drag to the right....
//    if (dragToRight)
//    {
//        // ... and the destination is to the right of the allowed line => dont allow the drag
//        if (AngleDir((p2 - p3), destination, transform.up) > 0)
//            return false;
//    }
//    // we drag to the left....
//    else
//    {
//        // ... and the destination is to the left of the allowed line => dont allow the drag
//        if (AngleDir((p2 - p3), destination, transform.up) < 0)
//            return false;
//    }
//}
//// same as above, in this case we just draw the line with the other thwo vectors
//else if (currentPos == p2)
//{
//    if (dragToRight)
//    {
//        if (AngleDir((p1 - p3), destination, transform.up) > 0)
//            return false;
//    }
//    else
//    {
//        if (AngleDir((p1 - p3), destination, transform.up) < 0)
//            return false;
//    }
//}
//// same as above, in this case we just draw the line with the other thwo vectors
//else if (currentPos == p3)
//{
//    if (dragToRight)
//    {
//        if (AngleDir((p2 - p3), destination, transform.up) > 0)
//            return false;
//    }
//    else
//    {
//        if (AngleDir((p2 - p3), destination, transform.up) < 0)
//            return false;
//    }
//}

// check if a point is to the left or right of a line
// returns -1 when to the left, 1 to the right, and 0 for forward/backward
//public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
//{
//    Vector3 perp = Vector3.Cross(fwd, targetDir);
//    float dir = Vector3.Dot(perp, up);

//    if (dir > 0.0f)
//    {
//        return 1.0f;
//    }
//    else if (dir < 0.0f)
//    {
//        return -1.0f;
//    }
//    else {
//        return 0.0f;
//    }
//}