using System.Collections;
using UnityEngine;

namespace Sliders.Levels
{
    public class Spawn : MonoBehaviour
    {
        public static Vector2 spawnLocation;
        public bool facingLeftOnSpawn;

        // Use this for initialization
        private void Start()
        {
            spawnLocation = (Vector2)this.gameObject.transform.position;
        }
    }
}