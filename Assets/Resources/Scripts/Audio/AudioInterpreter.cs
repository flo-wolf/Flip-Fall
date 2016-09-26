using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Impulse.Audio
{
    public class AudioInterpreter : MonoBehaviour
    {
        public static AudioInterpreter _instance;
        public static AudioSource source;

        public static float[] spectrum;
        public static float currentValue;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            float[] spectrum = new float[256];
            if (SoundPlayer._instance != null)
            {
                SoundPlayer._instance.musicSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
                currentValue = spectrum[0];
            }
        }

        //Left out: draw debug curves of current audio input - Update()
        //for (int i = 1; i < spectrum.Length - 1; i++)
        //{
        //    Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
        //    Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
        //    Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
        //    Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        //}
    }
}