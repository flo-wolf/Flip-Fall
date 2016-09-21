using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCopy : MonoBehaviour
{
    public Camera targetCam;

    private Camera cam;

    private void Start()
    {
        if (cam == null)
        {
            cam = this.GetComponent<Camera>();
        }
    }

    private void FixedUpdate()
    {
        this.transform.position = targetCam.transform.position;
        this.transform.rotation = targetCam.transform.rotation;
        cam.orthographicSize = targetCam.orthographicSize;
    }
}