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
        //rotate by 180, because moveArea shader flips levelObjects upside down => PLATFORM SPECIFIC!!!
        //transform.rotation = new Quaternion(targetCam.transform.rotation.x, targetCam.transform.rotation.y, targetCam.transform.rotation.z + 180, targetCam.transform.rotation.w);

        transform.rotation = targetCam.transform.rotation;
        cam.orthographicSize = targetCam.orthographicSize;
    }
}