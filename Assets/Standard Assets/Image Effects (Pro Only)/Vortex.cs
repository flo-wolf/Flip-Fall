using System;

namespace UnityStandardAssets.ImageEffects
{
    [UnityEngine.ExecuteInEditMode]
    [UnityEngine.AddComponentMenu("Image Effects/Displacement/Vortex")]
    public class Vortex : ImageEffectBase
    {
        public UnityEngine.Vector2 radius = new UnityEngine.Vector2(0.4F, 0.4F);
        public float angle = 50;
        public UnityEngine.Vector2 center = new UnityEngine.Vector2(0.5F, 0.5F);

        // Called by camera to apply image effect
        private void OnRenderImage(UnityEngine.RenderTexture source, UnityEngine.RenderTexture destination)
        {
            ImageEffects.RenderDistortion(material, source, destination, angle, center, radius);
        }
    }
}