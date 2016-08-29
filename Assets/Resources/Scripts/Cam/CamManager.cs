using Sliders.Levels;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages camera actions based on externally fired events
/// </summary>
namespace Sliders.Cam
{
    public enum InterpolationType { smoothstep, linear }

    public class CamManager : MonoBehaviour
    {
        public static CamManager _instance;
        public Player player;

        public static float defaultTransitionDuration = 1F;

        //Duration camera events use after the player's death, i.e. zooming and rotation times

        public float reflectRotationSwitchDuration = 1F;

        public float rotateToVelocityDuration = 1F;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            Game.onGameStateChange.AddListener(GameStateChanged);
            Player.onPlayerAction.AddListener(PlayerAction);
            Player.onPlayerStateChange.AddListener(PlayerStateChanged);
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
                    CamZoom.ZoomToVelocity(player, defaultTransitionDuration);
                    //CamShake.VelocityShake(player);
                    CamRotation.VelocityRotation(player); // currently deactivated in CamRotation
                    break;

                case Player.PlayerState.dead:
                    CamZoom.DeathZoom(Game.deathDelay);
                    CamShake.DeathShake();
                    CamRotation.RotateToDefault(Game.deathDelay);
                    //CamRotation.DeathRotation(); //change to camshake
                    //CamMove.StopFollowing();
                    break;

                case Player.PlayerState.fin:
                    CamZoom.DeathZoom(Game.deathDelay);
                    CamRotation.RotateToDefault(Game.deathDelay);
                    //CamRotation.DeathRotation(); //change to camshake
                    //CamMove.StopFollowing();
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

                case Game.GameState.deathscreen:
                    CamZoom.ZoomToMinimum(Game.levelselectionDelay);
                    CamRotation.RotateToDefault(Game.levelselectionDelay);
                    CamMove.MoveCamTo(spawnPos, Game.levelselectionDelay);
                    break;

                case Game.GameState.finishscreen:
                    CamZoom.ZoomToMinimum(Game.levelselectionDelay);
                    CamRotation.RotateToDefault(Game.levelselectionDelay);
                    CamMove.MoveCamTo(spawnPos, Game.levelselectionDelay);
                    break;

                default:
                    break;
            }
        }
    }
}