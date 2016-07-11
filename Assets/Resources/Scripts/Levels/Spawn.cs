using System.Collections;
using UnityEngine;

namespace Sliders.Levels
{
    public class Spawn : MonoBehaviour
    {
        public Vector3 spawnLocation;
        public Quaternion spawnRotation;
        public bool facingLeftOnSpawn;

        // Use this for initialization
        private void Start()
        {
            spawnLocation = transform.position;
            spawnRotation = transform.rotation;
        }
    }
}