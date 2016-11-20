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

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        private Vector2 currentPosition;
        private Vector2 deltaPositon;
        private Vector2 lastPositon;

        // the time needed between to clicks to account for a double click/tap
        public float doubleClickDelay = 0.3F;

        // the time of the last registered click
        private float doubleClickTime = 0F;

        // cycle through objects when multiple are within the same raycast by double tapping again
        private int cycleSelection = 0;

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

                        //debug1.text = "moving";
                        //debug2.text = touchDeltaPosition.ToString();
                        //transform.Translate(touchDeltaPosition.x * Time.deltaTime, touchDeltaPosition.y * Time.deltaTime, 0);
                    }
                }
                else if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    Vector3 position = Camera.main.ScreenToWorldPoint(touch.position);

                    if (LevelEditor.editorMode == LevelEditor.EditorMode.edit)
                    {
                        VertHandler._instance.VertexAdd(position);
                    }
                    else if (LevelEditor.editorMode == LevelEditor.EditorMode.place)
                    {
                        // snapping
                        Vector3 snapPos = VertHelper.Snap(position, false);
                        LevelObject.ObjectType objectType = UILevelObject.currentSelectedObject.objectType;
                        if (snapPos != Vector3.zero)
                        {
                            ProgressManager.GetProgress().unlocks.inventory.Add(objectType, -1);
                            LevelPlacer.generatedLevel.AddObject(objectType, snapPos);
                        }
                    }
                }
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
                    if (LevelEditor.editorMode == LevelEditor.EditorMode.select)
                    {
                        // double click
                        if (Time.time - doubleClickTime < doubleClickDelay)
                        {
                            Debug.Log("double click");
                            Vector3 rayOrigin = position;
                            rayOrigin.z = -50F;
                            // get the object clicked onto by raycasting
                            //RaycastHit hitInfo = new RaycastHit();
                            //bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 50000F);

                            RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector2.zero);
                            Debug.DrawRay(position, Vector3.forward * 200, Color.green, 20F, false);

                            int i = 0;

                            foreach (RaycastHit2D hit in hits)
                            {
                                if (hit.collider != null)
                                {
                                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EditorSelectionColliders"))
                                    {
                                        i++;
                                        LevelObject levelObject = hit.transform.parent.gameObject.GetComponent<LevelObject>();
                                        Debug.Log("LevelObject hit " + levelObject);
                                        if (levelObject != null && i == cycleSelection + 1)
                                        {
                                            cycleSelection = i;
                                            if (i + 1 >= hits.Length - 1)
                                            {
                                                cycleSelection = 0;
                                            }
                                            Debug.Log("cycleselection: " + cycleSelection);
                                            Debug.Log("SELECTED OBJ " + levelObject.name);
                                            LevelEditor.selectedObject = levelObject;
                                            LevelEditor.editorMode = LevelEditor.EditorMode.edit;
                                            break;
                                        }
                                    }
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
                        if (Time.time - doubleClickTime < doubleClickDelay)
                        {
                            /// double click
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
                                            if (levelObject == LevelEditor.selectedObject)
                                            {
                                                LevelEditor.selectedObject = null;
                                                LevelEditor.editorMode = LevelEditor.EditorMode.select;
                                            }
                                            else if (levelObject != LevelEditor.selectedObject)
                                            {
                                                LevelEditor.selectedObject = levelObject;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        VertHandler._instance.VertexAdd(position);
                    }
                    else if (LevelEditor.editorMode == LevelEditor.EditorMode.place)
                    {
                        // snapping
                        Vector3 snapPos = VertHelper.Snap(position, false);
                        LevelObject.ObjectType objectType = UILevelObject.currentSelectedObject.objectType;
                        if (snapPos != Vector3.zero)
                        {
                            ProgressManager.GetProgress().unlocks.inventory.Add(objectType, -1);
                            LevelPlacer.generatedLevel.AddObject(objectType, snapPos);
                        }
                    }
                    doubleClickTime = Time.time;
                }
            }
        }

#endif
    }
}