using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps the outline of a selected object at a constant size
/// Dynamically checks for the parent objects scale and listens for changes and computes them to keep a constant size
/// </summary>

namespace FlipFall.Editor
{
    public class OutlineScaler : MonoBehaviour
    {
        // fixed outline size in unity units
        public static float fixedSize = 4F;

        private Transform parentTransform;

        // the last size of the parentTransform, prevents the scaling when not needed
        private Vector3 lastParentSize;

        private void Start()
        {
            // get the parents transform
            parentTransform = transform.parent.transform;
            lastParentSize = parentTransform.localScale;
            ScaleSize();
        }

        // fix the outlines size when the parent object changes size
        private void FixedUpdate()
        {
            if (lastParentSize != parentTransform.localScale)
            {
                lastParentSize = parentTransform.localScale;
                ScaleSize();
            }
        }

        // computes the parents objects size and sets this objects size so that the overlapping area has always the same size
        private void ScaleSize()
        {
            // assuming x and y are the same, we take the x value for further calculations
            float parentSize = lastParentSize.x;

            // solve for x: (parentSize/x = fixedSize) => 1 + 1/x = desired size of the outline
            // example: parentsize 100, fixedSize 4
            // => 100/x=4 => x=25 => 1/25=0,04 => 1 + 0,04=1,04 => thats our outline size
            float x = parentSize / fixedSize;
            x = 1 / x;

            float outlineSize = 1 + x;
            transform.localScale = new Vector3(outlineSize, outlineSize, transform.localScale.z);
        }
    }
}