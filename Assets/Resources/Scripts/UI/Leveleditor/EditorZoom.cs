using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Editor Zooming script, zoom by using the numpad + and - keys in the editor or pinch the screen on mobile.
/// </summary>

namespace FlipFall.Editor
{
    public class EditorZoom : MonoBehaviour
    {
        public float zoomSpeed = 20f;        // The rate of change of the orthographic size in orthographic mode.
        public float panThreshold = 0.75F;

        public float minSize = 500F;
        public float maxSize = 3000F;

        // only relevant on the PC, zoom factor for each keypress
        public float keyboardZoomStep = 250F;

        private Camera cam;

        public Text debug1;
        public Text debug2;

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        private Vector2 currentPosition;
        private Vector2 deltaPositon;
        private Vector2 lastPositon;

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
                    debug1.text = "pinching";
                    // Change the orthographic size based on the change in distance between the touches.
                    float sizeToChange = deltaMagnitudeDiff * zoomSpeed;
                    if ((sizeToChange > 0 && cam.orthographicSize + sizeToChange < maxSize) || (sizeToChange < 0 && cam.orthographicSize - sizeToChange > minSize))
                    {
                        debug1.text = "zooming";
                        cam.orthographicSize += sizeToChange;
                    }
                }
                // panning => move the camera
                else
                {
                    //old one, working
                    Vector3 touchDeltaPosition = new Vector3(-touchZero.deltaPosition.x * Time.deltaTime * 3000, -touchZero.deltaPosition.y * Time.deltaTime * 3000, 0);
                    transform.position += touchDeltaPosition;

                    //debug1.text = "moving";
                    //debug2.text = touchDeltaPosition.ToString();
                    //transform.Translate(touchDeltaPosition.x * Time.deltaTime, touchDeltaPosition.y * Time.deltaTime, 0);
                }
            }
            else
                debug1.text = "nothing";
        }

#endif
    }
}