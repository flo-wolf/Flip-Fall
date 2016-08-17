using System.Collections;
using UnityEngine;

namespace Sliders.Levels
{
    public class Spawn : MonoBehaviour
    {
        public bool facingLeftOnSpawn;
        public Vector3 spawnPosition;

        private void Start()
        {
            spawnPosition = transform.position;
        }

        public Vector3 GetLocation()
        {
            return spawnPosition;
        }
    }
}