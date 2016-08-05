using Sliders;
using System.Collections;
using UnityEngine;

//Handels the Camera Zoom depending on the Players velocity
//Higher Velocity => greater size
public class CameraZoom : MonoBehaviour
{
    public Player player;
    public float maxZoom;
    public float minZoom;

    private float size;

    private void Start()
    {
        size = Camera.main.orthographicSize;
    }

    private void FixedUpdate()
    {
        Camera.main.orthographicSize = Mathf.Lerp(minZoom, maxZoom, Mathf.InverseLerp(0F, player.maxChargeVelocity, System.Math.Abs(player.rBody.velocity.x)));
    }
}