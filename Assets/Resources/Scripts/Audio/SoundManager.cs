﻿using Sliders.Cam;
using Sliders.Levels;
using System.Collections;
using UnityEngine;

/// <summary>
/// Listens to game events and plays sounds accordingly through the SoundPlayer class
/// </summary>
namespace Sliders.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public SoundPlayer soundPlayer;
        public Player player;
        public CamManager camManager;

        [Header("Game Sounds")]
        public AudioClip spawnSound;
        public AudioClip deathSound;
        public AudioClip reflectSound;
        public AudioClip chargeSound;
        public AudioClip finishSound;

        [Space(10)]
        [Header("UI Sounds")]
        public AudioClip levelChangeSound;
        public AudioClip clockSound;
        public AudioClip defaultButtonSound;

        [Space(10)]
        [Header("Music")]
        public AudioClip backgroundSound;

        private void Start()
        {
            Game.onGameStateChange.AddListener(GameStateChanged);
            player.onPlayerAction.AddListener(PlayerAction);
            player.onPlayerStateChange.AddListener(PlayerStateChanged);
            LevelManager.onLevelChange.AddListener(LevelChanged);
        }

        //Listener
        private void PlayerAction(Player.PlayerAction playerAction)
        {
            switch (playerAction)
            {
                case Player.PlayerAction.reflect:
                    soundPlayer.PlaySingle(reflectSound);
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

                    break;

                case Game.GameState.deathscreen:
                    //play deathscreen sound
                    break;

                case Game.GameState.finishscreen:
                    soundPlayer.PlaySingle(finishSound);
                    //play win sound
                    break;

                default:
                    break;
            }
        }

        private void LevelChanged(Level level)
        {
            soundPlayer.PlaySingle(levelChangeSound);
        }
    }
}