using System.Collections;
using UnityEngine;

namespace Sliders.Cam
{
    public enum InterpolationType { smoothstep, linear }

    public class CamManager : MonoBehaviour
    {
        public Player player;

        private void Start()
        {
            Game.onGameStateChange.AddListener(GameStateChanged);
            player.onPlayerAction.AddListener(PlayerAction);
            player.onPlayerStateChange.AddListener(PlayerStateChanged);
        }

        //Listener
        private void PlayerAction(Player.PlayerAction playerAction)
        {
            switch (playerAction)
            {
                case Player.PlayerAction.reflect:
                    break;

                case Player.PlayerAction.charge:
                    CamZoom.ZoomToVelocity(player);
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
                    CamMove.StartFollowing();
                    CamZoom.ZoomToVelocity(player);
                    break;

                case Player.PlayerState.dead:
                    CamShake.DeathShake();
                    //CamRotation.DeathRotation(); //change to camshake
                    CamMove.StopFollowing();
                    break;

                case Player.PlayerState.ready:
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
                    //play spawn sound
                    break;

                case Game.GameState.scorescreen:
                    //play deathscreen sound
                    break;

                case Game.GameState.finishscreen:
                    //play win sound
                    break;

                default:
                    break;
            }
        }
    }
}