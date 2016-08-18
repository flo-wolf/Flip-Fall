using Sliders.Cam;
using System.Collections;
using UnityEngine;

/// <summary>
/// Plays sounds, only referenced through the SoundManager
/// </summary>

namespace Sliders.Audio
{
    public class SoundPlayer : MonoBehaviour
    {
        public static SoundPlayer _instance = null;     //Allows other scripts to call functions from SoundManager.

        public float playSingleVolume = 1F;
        public AudioSource sfxSource;                   //Drag a reference to the audio source which will play the sound effects.
        public AudioSource sfxSourceRdmPitch;
        public AudioSource musicSource;                 //Drag a reference to the audio source which will play the music.

        public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
        public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.

        public AudioListener audioListener;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else if (_instance != this)
                //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
                Destroy(gameObject);

            //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
            DontDestroyOnLoad(gameObject);
        }

        //Used to play single sound clips.
        public void PlaySingle(AudioClip clip)
        {
            if (sfxSource.clip == null)
            {
            }
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            sfxSource.pitch = 1;

            //Play the clip.
            sfxSource.PlayOneShot(clip, playSingleVolume);
        }

        //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
        public void RandomizeSfx(params AudioClip[] clips)
        {
            //Generate a random number between 0 and the length of our array of clips passed in.
            int randomIndex = Random.Range(0, clips.Length);

            //Choose a random pitch to play back our clip at between our high and low pitch ranges.
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);

            //Set the pitch of the audio source to the randomly chosen pitch.
            sfxSourceRdmPitch.pitch = randomPitch;

            //Set the clip to the clip at our randomly chosen index.
            sfxSourceRdmPitch.PlayOneShot(clips[randomIndex], playSingleVolume);
        }
    }
}