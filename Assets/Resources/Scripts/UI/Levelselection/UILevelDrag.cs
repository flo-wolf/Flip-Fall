using Impulse.Progress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Impulse.UI
{
    public class UILevelDrag : MonoBehaviour
    {
        public static UILevelDrag _instance;
        public float fallBackDuration = 0.5F;

        //if a drag's length is smaller than the percantage value of the screen width it wont switch to the next item
        public float minScreenWidthPercentToSwitch = 0.3f;

        //the object that is to be dragged
        private GameObject dragObject;

        private Vector2 touchCache;
        private Vector3 newDragObjectPos;
        private Vector3 defaultPosition = new Vector3(0, 0, 0);

        public static BounceBackEvent onBounceBack = new BounceBackEvent();

        public class BounceBackEvent : UnityEvent { }

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

            _instance.dragObject = UILevelPlacer.placingParent;
            _instance.dragging = false;
            _instance.dragLength = 0;
            _instance.collectInput = true;
            // UpdateDragObject();
        }

        public static void UpdateDragObject()
        {
            _instance.dragObject = UILevelPlacer.placingParent;
            //_instance.dragging = false;
            ////_instance.dragLength = 0;
            ////_instance.collectInput = true;
            _instance.StartCoroutine(_instance.lerpBackToOrigin());
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

                    //Drag is long enough to switch to the next level
                    if (Mathf.Abs(dragLength) >= screenWidth * minScreenWidthPercentToSwitch)
                    {
                        //drag to the right
                        if (dragLength > 0)
                        {
                            if (!UILevelselectionManager.LastLevel())
                            {
                                StartCoroutine(lerpBackToOrigin());
                            }
                        }
                        //drag to the left
                        else
                        {
                            if (!UILevelselectionManager.NextLevel())
                            {
                                StartCoroutine(lerpBackToOrigin());
                            }
                        }
                        onBounceBack.Invoke();
                    }
                    newDragObjectPos = new Vector3(defaultPosition.x + dragLength, defaultPosition.y, defaultPosition.z);
                    //Debug.Log(dragLength + " " + originDragObjectPos.x);
                    touched = true;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Vector2 mouseCache = Input.mousePosition;
                    dragging = false;

                    if (dragLength <= screenWidth * minScreenWidthPercentToSwitch)
                    {
                        StartCoroutine(lerpBackToOrigin());
                        if (dragLength > 20 || dragLength < -20)
                            onBounceBack.Invoke();
                    }
                }
#endif
                //If a touch is detected
                if (Input.touchCount >= 1)
                {
                    Touch touch = Input.touches[0];

                    //finger is on screen
                    if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                    {
                        touchCache = touch.position;
                        if (!dragging)
                        {
                            dragBeginPos = touchCache;
                            dragging = true;
                        }

                        dragLength = (dragBeginPos.x - touchCache.x) * -1;

                        if (Mathf.Abs(dragLength) >= screenWidth * minScreenWidthPercentToSwitch)
                        {
                            StartCoroutine(lerpBackToOrigin());
                        }
                        newDragObjectPos = new Vector3(defaultPosition.x + dragLength, defaultPosition.y, defaultPosition.z);
                        touched = true;
                    }

                    //finger got released
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        dragging = false;
                        if (dragLength <= screenWidth * minScreenWidthPercentToSwitch)
                        {
                            StartCoroutine(lerpBackToOrigin());
                        }
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (touched)
            {
                //Debug.Log(newDragObjectPos);
                dragObject.transform.localPosition = newDragObjectPos;
                touched = false;
            }
        }

        private IEnumerator lerpBackToOrigin()
        {
            collectInput = false;
            float t = 0;
            Vector3 dragPos = dragObject.transform.localPosition;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / fallBackDuration);
                dragObject.transform.localPosition = Vector3.Lerp(dragPos, new Vector3(0, 0, 0), t);
                yield return 0;
            }
            dragLength = 0;
            dragging = false;
            collectInput = true;
            yield break;
        }
    }
}