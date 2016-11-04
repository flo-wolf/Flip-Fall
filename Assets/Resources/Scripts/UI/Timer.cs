using FlipFall.Audio;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FlipFall.UI
{
    public static class Timer
    {
        #region Properties

        public enum TimerState { running, paused };

        public static string format { get; set; }
        public static bool isPaused { get; private set; }
        public static bool isStarted { get; private set; }
        public static Text textObjectS { get; private set; }
        public static Text textObjectM { get; private set; }
        public static double passedTime { get; private set; }
        public static double pauseTime;

        private static MonoBehaviour Behaviour { get; set; }

        #endregion Properties

        #region Public Methods

        public static void Start(MonoBehaviour mono, Text _textObjectS = null, Text _textObjectM = null)
        {
            isPaused = false;
            Reset();
            if (mono == null)
            {
                Debug.LogError("Behaviour may not be null!");
                return;
            }

            if (_textObjectS != null && _textObjectM != null)
            {
                textObjectM = _textObjectM;
                textObjectS = _textObjectS;
            }
            else
            {
                Debug.LogError("No Text object!");
                return;
            }
            Behaviour = mono;
            Behaviour.StartCoroutine(WaitForTimerUpdate());
            isStarted = true;
        }

        public static void Pause()
        {
            pauseTime = passedTime;
            isPaused = true;
        }

        public static void Reset()
        {
            passedTime = 0;
        }

        public static void Continue()
        {
            isPaused = false;
            Behaviour.StartCoroutine(WaitForTimerUpdate());
        }

        public static void Stop()
        {
            isStarted = false;
        }

        #endregion Public Methods

        private static IEnumerator WaitForTimerUpdate()
        {
            while (!isPaused)
            {
                passedTime += Time.fixedDeltaTime;
                var time = TimeSpan.FromSeconds(passedTime);
                try
                {
                    textObjectS.text = string.Format("{0}", time.Seconds);
                    textObjectM.text = string.Format("{0:D2}", time.Milliseconds / 10);
                }
                catch (ArgumentNullException anex)
                {
                    Debug.LogError(anex.Message);
                }
                catch (FormatException fex)
                {
                    Debug.LogError(fex.Message);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
                if (time.Milliseconds / 10 == 0)
                {
                    SoundManager._instance.PlayTimerSound();
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }
}