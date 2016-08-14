using Sliders.Audio;
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
            Timer.Start(this, textSec, textMil);
        }

        public void Pause()
        {
            Timer.Pause();
        }

        public void Continue()
        {
            Timer.Continue();
        }

        public void Stop()
        {
            Timer.Stop();
        }

        public void Reset()
        {
            Timer.Reset();
        }

        public void PlayCountingSound()
        {
            SoundPlayer.instance.PlaySingle(countingSound);
        }

        public double GetTime()
        {
            //maybe change passedTime to pauseTime
            double time = Timer.passedTime;
            return time;
        }
    }
}