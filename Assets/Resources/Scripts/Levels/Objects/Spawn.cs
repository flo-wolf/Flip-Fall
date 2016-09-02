using System.Collections;
using UnityEngine;

namespace Sliders.Levels
{
    public class Spawn : MonoBehaviour
    {
        public bool facingLeftOnSpawn;

        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}