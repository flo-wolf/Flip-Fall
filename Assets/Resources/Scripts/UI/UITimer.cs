using UnityEngine;
using UnityEngine.UI;

namespace Sliders.UI
{
    public class UITimer : MonoBehaviour
    {
        public static UITimer instance;
        public Text textSec;
        public Text textMil;

        public AudioClip countingSound;

        private void Start()
        {
            instance = this;
        }

        // Use this for initialization
        public void Run()
        {
            Sliders.UI.Timer.Start(this, textSec, textMil);
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

        public void PlayCountingSound()
        {
            SoundManager.instance.PlaySingle(countingSound);
        }

        public double GetTime()
        {
            double time = Sliders.UI.Timer.pauseTime;
            return time;
        }
    }
}