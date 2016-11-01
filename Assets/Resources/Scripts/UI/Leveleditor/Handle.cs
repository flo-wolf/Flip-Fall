using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

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
                    print("selected this handle at " + transform.position + " there are " + VertHandler.selectedHandles.Count + " elements in the selection");
                }
                // this handle is already selected, deselect it
                else
                {
                    VertHandler.selectedHandles.Remove(this);
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

        // while dragging
        public void OnDrag(PointerEventData eventData)
        {
            if (LevelEditor.editorMode == LevelEditor.EditorMode.moveVertex)
            {
                // alter positions based on the original position plus the drag delta
                //foreach (Handle h in VertHandler.selectedHandles)
                //{
                //    h.transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
                //}

                for (int i = 0; i < VertHandler.selectedHandles.Count; i++)
                {
                    VertHandler.selectedHandles[i].transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
                }
            }
        }

        // end of drag
        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
        }
    }
}