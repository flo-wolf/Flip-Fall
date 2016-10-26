using Impulse.Theme;
using System.Collections;
using UnityEngine;

namespace Impulse.Levels
{
    public class Spawn : MonoBehaviour
    {
        public bool facingLeftOnSpawn;

        private void Start()
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr != null)
                mr.material.SetColor("_Color2", ThemeManager.theme.speedstripColor);
            else
                Debug.LogError("No MeshRenderer attached to the Spawn, can't set the color.");
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}