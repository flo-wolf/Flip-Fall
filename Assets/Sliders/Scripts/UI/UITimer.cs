using UnityEngine;
using UnityEngine.UI;

namespace Sliders.UI
{
    public class UITimer : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        // Use this for initialization
        public void Run()
        {
            Sliders.UI.Timer.Start(this, text);
        }

        public void Pause()
        {
            Sliders.UI.Timer.Pause();
        }

        public void Continue()
        {
            Sliders.UI.Timer.Continue();
        }

        public void Stop()
        {
            Sliders.UI.Timer.Stop();
        }

        public void Reset()
        {
            Sliders.UI.Timer.Reset();
        }

        public double GetTime()
        {
            double time = Sliders.UI.Timer.PassedTime;
            return time;
        }
    }
}