using System.Collections;
using UnityEngine;

namespace Sliders.Levels
{
    public class Finish : MonoBehaviour
    {
        public static Vector2 spawnLocation;

        // Use this for initialization
        private void Start()
        {
            spawnLocation = (Vector2)this.gameObject.transform.position;
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}