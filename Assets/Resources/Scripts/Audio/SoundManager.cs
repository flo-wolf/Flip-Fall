using Sliders.Cam;
using Sliders.Levels;
using Sliders.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Listens to game events and plays sounds accordingly through the SoundPlayer class
/// </summary>
namespace Sliders.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager _instance;
        public SoundPlayer soundPlayer;
        public Player player;
        public CamManager camManager;
        public UITimer uiTimer;

        [Header("Game Sounds")]
        public AudioClip playSound;
        public AudioClip spawnSound;
        public AudioClip deathSound;
        public AudioClip reflectSound;
        public AudioClip chargeSound;
        public AudioClip finishSound;

        [Header("UI Sounds")]
        public AudioClip buttonClickSound;
        public AudioClip buttonReleaseSound;
        public AudioClip levelChangeSound;
        public AudioClip timerSound;
        public AudioClip defaultButtonSound;
        public AudioClip scoreScreenAppearSound;
        public AudioClip camTransitionSound;

        [Header("Music")]
        public AudioClip backgroundSound;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            Game.onGameStateChange.AddListener(GameStateChanged);
            Player.onPlayerAction.AddListener(PlayerAction);
            Player.onPlayerStateChange.AddListener(PlayerStateChanged);
            CamMove.onCamMoveStateChange.AddListener(CamMoveStateChanged);
            LevelManager.onLevelChange.AddListener(LevelChanged);
            UIButtonManager.onButtonClick.AddListener(ButtonClicked);
            UIButtonManager.onButtonRelease.AddListener(ButtonReleased);
        }

        //Listener
        private void PlayerAction(Player.PlayerAction playerAction)
        {
            switch (playerAction)
            {
                case Player.PlayerAction.reflect:
                    soundPlayer.RandomizeSfx(reflectSound);
                    break;

                case Player.PlayerAction.charge:
                    soundPlayer.PlaySingle(chargeSound);
                    break;

                case Player.PlayerAction.decharge:
                    break;

                default:
                    break;
            }
        }

        private void CamMoveStateChanged(CamMove.CamMoveState moveState)
        {
            switch (moveState)
            {
                case CamMove.CamMoveState.transitioning:
                    soundPlayer.RandomizeSfx(camTransitionSound);
                    break;

                default:
                    break;
            }
        }

        //Listener
        private void PlayerStateChanged(Player.PlayerState playerState)
        {
            switch (playerState)
            {
                case Player.PlayerState.alive:
                    soundPlayer.PlaySingle(spawnSound);
                    break;

                case Player.PlayerState.dead:
                    soundPlayer.PlaySingle(deathSound);
                    break;

                default:
                    break;
            }
        }

        private void GameStateChanged(Game.GameState gameState)
        {
            switch (gameState)
            {
                case Game.GameState.playing:
                    soundPlayer.PlaySingle(playSound);
                    break;

                case Game.GameState.scorescreen:
                    soundPlayer.PlaySingle(scoreScreenAppearSound);
                    break;

                case Game.GameState.finishscreen:
                    soundPlayer.PlaySingle(finishSound);
                    //play win sound
                    break;

                case Game.GameState.ready:
                    //soundPlayer.PlaySingle(spawnSound);
                    break;

                default:
                    break;
            }
        }

        private void ButtonClicked(Button b)
        {
            soundPlayer.PlaySingle(buttonClickSound);
        }

        private void ButtonReleased(Button b)
        {
            soundPlayer.PlaySingle(buttonReleaseSound);
        }

        public void PlayTimerSound()
        {
            soundPlayer.PlaySingle(timerSound);
        }

        private void LevelChanged(Level level)
        {
            soundPlayer.PlaySingle(levelChangeSound);
        }
    }
}