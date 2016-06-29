using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.UI
{
    public class UIInstructions : MonoBehaviour
    {
        public Text text;

        // Use this for initialization
        private void Start()
        {
            //create listeneer listening to changes in gamestate
            //if gamestate switches to ready enable instruction text and make it blink until it changes back to something else.
        }

        public void Blink()
        {
            //coroutine
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}