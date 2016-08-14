//TODO
//Per-Cloud-Movement
//Cloud: Unterschiedliche Cloud-Geschwindigkeiten, abhänig von gegebener Transparenz (verstärkte Tiefenwirkung)
//Menu-Play: Add Level selection
//Menu: Add Shop
//Game: Replace obsticles
//Game: Add wildlife, wildlife animations and wildlife AI (predator/frienldy)
//Game: Add water animations
//Post-Processing: Add a Retro CRT Distortion Effect Using RGB Shifting
//Audio: Add "character-dive-in", "player-death", "level-done", (...)

using Sliders.Cam;
using System.Collections;
using UnityEngine;

namespace Sliders.Unused
{
    public enum GameState { menu, editor, playing }

    public class Manager : MonoBehaviour
    {
        //Importierung
        public Player player;

        public CamMove cm;

        //Camera Defaults, put these into camera script!
        public static float lockedCameraZ = -10F;

        public static float CameraDistanceToPlayer = -40f;

        public static GameState state;

        public static int level = 0;

        private void Update()
        {
            //Zurück-Tatse
            //if (Input.GetKeyDown(KeyCode.Escape) && CameraMovement.IsResting() && Player.IsReady())
            //{
            //Debug.Log("Game Closed");
            //Save stuff in here
            //Application.Quit();
            //}
        }
    }
}