using System.Collections;
using UnityEngine;

namespace Sliders.Levels
{
    public class Finish : MonoBehaviour
    {
        public Vector2 finishLocation;

        // Use this for initialization
        private void Start()
        {
            finishLocation = (Vector2)this.gameObject.transform.position;
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}