using FlipFall.LevelObjects;
using FlipFall.Levels;
using FlipFall.Progress;
using FlipFall.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles zooming, moving and vertex adding input inside the editor
/// zoom by using the numpad + and - keys in Unity or pinch the screen on mobile.
/// </summary>

namespace FlipFall.Editor
{
    public class EditorInput : MonoBehaviour
    {
        public float zoomSpeed = 20f;        // The rate of change of the orthographic size in orthographic mode.
        public float panThreshold = 0.75F;

        public float minSize = 500F;
        public float maxSize = 3000F;

        // only relevant on the PC, zoom factor for each keypress
        public float keyboardZoomStep = 250F;

        private Camera cam;

        public static bool itemDragged = false;

        private Vector2 currentPosition;
        private Vector2 deltaPositon;
        private Vector2 lastPositon;

        // the time needed between to clicks to account for a double click/tap
        public float doubleClickDelay = 0.3F;

        // the time of the last registered click
        private float doubleClickTime;

        private void Start()
        {
            cam = GetComponent<Camera>();
            doubleClickTime = 0F;
        }

        // Input Control
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                if (cam.orthographicSize + keyboardZoomStep <= maxSize)
                    cam.orthographicSize += keyboardZoomStep;
            }
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if (cam.orthographicSize - keyboardZoomStep >= minSize)
                    cam.orthographicSize -= keyboardZoomStep;
            }
#endif
#if UNITY_ANDROID
            // No items or verticies get curretly dragged
            if (!itemDragged)
            {
                // If there are two touches on the device...
                if (Input.touchCount == 2)
                {
                    // Store both touches.
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchMag = (touchZero.position - touchOne.position).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchMag - touchMag;

                    // fingers moving fast towards each other or fast away from each other => zooming
                    if (touchMag + panThreshold < prevTouchMag || touchMag > prevTouchMag + panThreshold)
                    {
                        // Change the orthographic size based on the change in distance between the touches.
                        float sizeToChange = deltaMagnitudeDiff * zoomSpeed;
                        if ((sizeToChange > 0 && (cam.orthographicSize + sizeToChange) < maxSize) || (sizeToChange < 0 && (cam.orthographicSize - sizeToChange) > minSize))
                        {
                            cam.orthographicSize += sizeToChange;
                        }
                    }
                    // panning => move the camera
                    else
                    {
                        //old one, working
                        Vector3 touchDeltaPosition = new Vector3(-touchZero.deltaPosition.x * Time.deltaTime * 500, -touchZero.deltaPosition.y * Time.deltaTime * 500, 0);
                        transform.position += touchDeltaPosition;
                        //transform.Translate(touchDeltaPosition.x * Time.deltaTime, touchDeltaPosition.y * Time.deltaTime, 0);
                    }
                }
                else if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        Vector3 position = Camera.main.ScreenToWorldPoint(touch.position);
                        ClickHandler(position);
                    }
                }
#if UNITY_EDITOR
                // both mouse buttons clicked => drag view
                else if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
                {
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3 deltaPos = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 40f;
                    deltaPos.z = 0F;
                    transform.position += deltaPos;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    ClickHandler(position);
                }
#endif
            }
        }

        private void ClickHandler(Vector3 position)
        {
            bool objectSelected = false;

            if (LevelEditor.editorMode == LevelEditor.EditorMode.select)
            {
                // double click
                if (Time.time - doubleClickTime < doubleClickDelay && doubleClickTime > 0)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector2.zero);
                    Debug.DrawRay(position, Vector3.forward * 200, Color.green, 20F, false);

                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit.collider != null)
                        {
                            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EditorSelectionColliders"))
                            {
                                LevelObject levelObject = hit.transform.parent.gameObject.GetComponent<LevelObject>();
                                if (levelObject != null)
                                {
                                    Debug.Log("SELECTED OBJ " + levelObject.name);
                                    LevelEditor.SetSelectedObject(levelObject);
                                    objectSelected = true;
                                    break;
                                }
                            }
                        }
                    }

                    // the raycast didn't select any levelobjects
                    if (!objectSelected)
                    {
                        Vector3 localPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(position);
                        bool clickInsideMesh = VertHelper.IsInsideMesh(LevelPlacer.generatedLevel.moveArea.meshFilter.mesh, Vector3.zero, localPos);
                        // we double clicked inside the mesh => select the mesh
                        if (clickInsideMesh)
                        {
                            LevelEditor.SetSelectedObject(LevelPlacer.generatedLevel.moveArea);
                            objectSelected = true;
                        }
                    }
                }
                else
                {
                    //normal click
                }
            }
            else if (LevelEditor.editorMode == LevelEditor.EditorMode.edit)
            {
                if (Time.time - doubleClickTime < doubleClickDelay && doubleClickTime > 0)
                {
                    // double click
                    RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector2.zero);
                    Debug.DrawRay(position, Vector3.forward * 200, Color.green, 20F, false);

                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit.collider != null)
                        {
                            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EditorSelectionColliders"))
                            {
                                LevelObject levelObject = hit.transform.parent.gameObject.GetComponent<LevelObject>();
                                Debug.Log("LevelObject hit " + levelObject);
                                if (levelObject != null)
                                {
                                    // same object got selected again, deselect it
                                    if (levelObject == LevelEditor.selectedObject)
                                    {
                                        LevelEditor.SetSelectedObject(null);
                                        objectSelected = true;
                                    }
                                    // a new object gets selected, replace the old selection with the new one.
                                    else if (levelObject != LevelEditor.selectedObject)
                                    {
                                        LevelEditor.SetSelectedObject(levelObject);
                                        objectSelected = true;
                                    }
                                }
                            }
                        }
                    }
                    // the raycast didn't select any levelobjects
                    if (!objectSelected)
                    {
                        Vector3 localPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(position);
                        bool clickInsideMesh = VertHelper.IsInsideMesh(LevelPlacer.generatedLevel.moveArea.meshFilter.mesh, Vector3.zero, localPos);
                        // we double clicked inside the mesh => select the mesh
                        if (clickInsideMesh)
                        {
                            if (LevelEditor.selectedObject.objectType != LevelObject.ObjectType.moveArea)
                                LevelEditor.SetSelectedObject(LevelPlacer.generatedLevel.moveArea);
                            else
                                LevelEditor.SetSelectedObject(null);
                            objectSelected = true;
                        }
                    }
                }
                // there was no object selected, try to add a vertex
                //if (!objectSelected || LevelEditor.selectedObject.objectType == LevelObject.ObjectType.moveArea)
                VertHandler._instance.VertexAdd(position);
            }
            else if (LevelEditor.editorMode == LevelEditor.EditorMode.place)
            {
                // snapping
                Vector3 snapPos = VertHelper.Snap(position, false);
                LevelObject.ObjectType objectType = UILevelObject.currentSelectedObject.objectType;
                if (snapPos != Vector3.zero)
                {
                    LevelPlacer.generatedLevel.AddObject(objectType, snapPos);
                }
            }
            doubleClickTime = Time.time;
        }

#endif
    }
}