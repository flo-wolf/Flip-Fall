using UnityEngine;
using UnityEngine.UI;

namespace Impulse.UI
{
    public class UITimer : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        // Use this for initialization
        public void Run()
        {
            Impulse.UI.Timer.Start(this, text);
        }

        public void Pause()
        {
            Impulse.UI.Timer.Pause();
        }

        public void Continue()
        {
            Impulse.UI.Timer.Continue();
        }

        public void Stop()
        {
            Impulse.UI.Timer.Stop();
        }

        public void Reset()
        {
            Impulse.UI.Timer.Reset();
        }

        public double GetTime()
        {
            double time = Impulse.UI.Timer.PassedTime;
            return time;
        }
    }
}