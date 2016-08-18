using Sliders.Levels;
using System.Collections;
using UnityEngine;

namespace Sliders.Cam
{
    public enum InterpolationType { smoothstep, linear }

    public class CamManager : MonoBehaviour
    {
        public static CamManager _instance;
        public Player player;

        public static float defaultTransitionDuration = 1F;

        //Duration camera events use after the player's death, i.e. zooming and rotation times
        public float deathTransitionDuration = 1F;

        //Duration between the appearance of the score screen and the transition back to the game start
        public float scoreScreenTransitionDuration = 1F;

        //Duration between the appearance of the finish screen and the transition back to the game start
        public float finishScreenTransitionDuration = 1F;

        public float reflectRotationSwitchDuration = 1F;

        public float rotateToVelocityDuration = 1F;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            Game.onGameStateChange.AddListener(GameStateChanged);
            player.onPlayerAction.AddListener(PlayerAction);
            player.onPlayerStateChange.AddListener(PlayerStateChanged);
        }

        //Player Listener
        private void PlayerAction(Player.PlayerAction playerAction)
        {
            switch (playerAction)
            {
                case Player.PlayerAction.reflect:
                    CamRotation.ReflectRotationSwitch(player, reflectRotationSwitchDuration);
                    break;

                case Player.PlayerAction.charge:
                    CamZoom.ZoomToVelocity(player, defaultTransitionDuration);
                    CamRotation.RotateToVelocity(player, defaultTransitionDuration);
                    break;

                case Player.PlayerAction.decharge:
                    break;

                default:
                    break;
            }
        }

        //Player Listener
        private void PlayerStateChanged(Player.PlayerState playerState)
        {
            switch (playerState)
            {
                case Player.PlayerState.alive:
                    CamMove.StartFollowing();
                    CamZoom.ZoomToVelocity(player, deathTransitionDuration);
                    //CamShake.VelocityShake(player);
                    CamRotation.VelocityRotation(player); // currently deactivated in CamRotation
                    break;

                case Player.PlayerState.dead:
                    CamZoom.DeathZoom(deathTransitionDuration);
                    CamShake.DeathShake();
                    CamRotation.RotateToDefault(deathTransitionDuration);
                    //CamRotation.DeathRotation(); //change to camshake
                    //CamMove.StopFollowing();
                    break;

                case Player.PlayerState.ready:
                    break;

                default:
                    break;
            }
        }

        //Game Listener
        private void GameStateChanged(Game.GameState gameState)
        {
            Vector3 spawnPos = LevelManager.GetSpawnPosition();

            switch (gameState)
            {
                case Game.GameState.playing:
                    break;

                case Game.GameState.scorescreen:
                    CamZoom.ZoomToMinimum(scoreScreenTransitionDuration);
                    CamRotation.RotateToDefault(scoreScreenTransitionDuration);
                    CamMove.MoveCamTo(spawnPos, defaultTransitionDuration);
                    break;

                case Game.GameState.finishscreen:
                    CamZoom.ZoomToMinimum(finishScreenTransitionDuration);
                    CamRotation.RotateToDefault(scoreScreenTransitionDuration);
                    CamMove.MoveCamTo(spawnPos, defaultTransitionDuration);
                    break;

                default:
                    break;
            }
        }
    }
}