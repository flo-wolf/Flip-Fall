using Sliders;
using System.Collections;
using UnityEngine;

namespace Sliders
{
    public class CameraRotation : MonoBehaviour
    {
        public Player player;

        private Camera cam;

        // Use this for initialization
        private void Start()
        {
            cam = Camera.main;
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}