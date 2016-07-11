using System.Collections;
using UnityEngine;

namespace Sliders.Levels
{
    public class Spawn : MonoBehaviour
    {
        public Vector3 position;
        public bool facingLeftOnSpawn;

        // Use this for initialization
        private void Awake()
        {
            position = transform.position;
        }
    }
}