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

        private Vector3[] oPositions;

        public void OnPointerDown(PointerEventData eventData)
        {
            // we are selecting verticies
            if (LevelEditor.editorMode == LevelEditor.EditorMode.selectVertex)
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
        }

        // begin of drag, called once
        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;

            // save original positions
            if (LevelEditor.editorMode == LevelEditor.EditorMode.moveVertex)
            {
                oPositions = new Vector3[VertHandler.selectedHandles.Count];
                for (int i = 0; i < VertHandler.selectedHandles.Count; i++)
                {
                    oPositions[i] = VertHandler.selectedHandles[i].transform.position;
                }
            }
        }

        // while dragging, transform the selected handles
        public void OnDrag(PointerEventData eventData)
        {
            if (LevelEditor.editorMode == LevelEditor.EditorMode.moveVertex)
            {
                // alter positions based on the original position plus the drag delta
                for (int i = 0; i < VertHandler.selectedHandles.Count; i++)
                {
                    Vector3 dragPos = Camera.main.ScreenToWorldPoint(eventData.position);
                    dragPos.z = 0;
                    //Vector3 newPos = VertHandler.selectedHandles[i].transform.position;
                    //newPos += new Vector3(eventData.delta.x / 5, eventData.delta.y / 5, 0);
                    VertHandler.selectedHandles[i].transform.position = dragPos;
                }
            }
        }

        // end of drag
        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
            if (LevelEditor.editorMode == LevelEditor.EditorMode.moveVertex)
            {
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
            }
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