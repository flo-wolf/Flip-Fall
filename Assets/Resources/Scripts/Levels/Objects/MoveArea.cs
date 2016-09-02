using System.Collections;
using UnityEngine;

namespace Sliders.Objects
{
    public class MoveArea : MonoBehaviour
    {
        public MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = this.GetComponent<MeshRenderer>();
        }
    }
}