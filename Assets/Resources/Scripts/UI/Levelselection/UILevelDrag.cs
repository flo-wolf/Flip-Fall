using Impulse.Progress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Impulse.UI
{
    public class UILevelDrag : MonoBehaviour
    {
        public static UILevelDrag _instance;
        public GameObject dragObject;
        public float fallBackDuration = 0.5F;

        //if a drag's length is smaller than the percantage value of the screen width it wont switch to the next item
        public float minScreenWidthPercentToSwitch = 0.3f;

        private Vector2 touchCache;
        private Vector3 originDragObjectPos;
        private Vector3 newDragObjectPos;

        //Drag length registration
        private Vector2 dragBeginPos;
        private Vector2 dragEndPos;
        private float dragLength;

        //controls drag length registration
        private bool dragging = false;

        //block input when objects get lerped
        private bool collectInput = true;

        //Allows fixedupdate transformations
        private bool touched = false;
        private int screenHeight;
        private int screenWidth;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
        }

        private void Start()
        {
            screenHeight = Screen.height;
            screenWidth = Screen.width;
            originDragObjectPos = dragObject.transform.position;
        }

        private void Update()
        {
            if (collectInput)
            {
                //If running game in editor
#if UNITY_EDITOR
                //If mouse button 0 is down
                if (Input.GetMouseButton(0))
                {
                    Vector2 mouseCache = Input.mousePosition;
                    if (!dragging)
                    {
                        dragBeginPos = mouseCache;
                        dragging = true;
                    }

                    //claculate the length of the current drag
                    dragLength = (dragBeginPos.x - mouseCache.x) * -1;

                    if (Mathf.Abs(dragLength) >= screenWidth * minScreenWidthPercentToSwitch)
                    {
                        Debug.Log("Drag long enough to switch");
                        StartCoroutine(lerpBackToOrigin());
                        //UILevelselectionManager.SwitchLeft();
                    }

                    newDragObjectPos = new Vector3(originDragObjectPos.x + dragLength, originDragObjectPos.y, originDragObjectPos.z);
                    touched = true;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Vector2 mouseCache = Input.mousePosition;
                    dragging = false;

                    if (dragLength <= screenWidth * minScreenWidthPercentToSwitch)
                    {
                        StartCoroutine(lerpBackToOrigin());
                    }
                }
#endif
                ////If a touch is detected
                //if (Input.touchCount >= 1)
                //{
                //    //For each touch
                //    foreach (Touch touch in Input.touches)
                //    {
                //        touchCache = touch.position;
                //        dragObjectPos = new Vector3(touchCache.x, dragObjectPos.y, dragObjectPos.z);
                //    }
                //    touched = true;
                //}
            }
        }

        private void FixedUpdate()
        {
            if (touched)
            {
                dragObject.transform.position = newDragObjectPos;
                touched = false;
            }
        }

        private IEnumerator lerpBackToOrigin()
        {
            collectInput = false;
            float t = 0;
            Vector3 dragPos = dragObject.transform.position;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / fallBackDuration);
                dragObject.transform.position = Vector3.Lerp(dragPos, originDragObjectPos, t);
                yield return 0;
            }
            dragLength = 0;
            dragging = false;
            collectInput = true;
            yield break;
        }
    }
}