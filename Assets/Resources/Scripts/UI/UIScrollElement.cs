using FlipFall;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Needs to be attached to a GameObject which s child to a ScrollRect and thus effected by touch input.
/// Needs a Referenced Animator to function.
/// A "fade" trigger gets called in the Animator and causes Fade-in and -outs.
/// Thus each element can have different or even none fading animations.
///
/// This script gets called by the UIScrollFade script.
/// </summary>

namespace FlipFall.UI
{
    public class UIScrollElement : MonoBehaviour
    {
        public enum ElementType { editorLevel, product }
        public ElementType elementType = ElementType.product;

        // Fading Animation
        public Animation anim;

        // Controls wether or not is is needed to start a fading animation
        // if an element is actually outside an insideArea, but set to inside, a fadeout will be called while correcting inside to false.
        // [HideInInspector]
        public bool isFadedIn = false;

        //  [HideInInspector]
        public bool canBeAnimated = true;

        // Use this for initialization
        private void Awake()
        {
            //isFadedIn = false;
            //anim.Play("scrollElementFadein");
            Main.onSceneChange.AddListener(SceneChanged);
            canBeAnimated = true;
        }

        private void SceneChanged(Main.ActiveScene scene)
        {
            if (isFadedIn)
            {
                //print("fadeOut scene changed");
                FadeOut();
            }
            canBeAnimated = false;
        }

        // begins fade-in animation. Animator needs to have a "fadein" trigger and transitions using it.
        public void FadeIn()
        {
            if (canBeAnimated && !isFadedIn)
            {
                if (elementType == ElementType.editorLevel && UIScrollFade.IsInside(this.transform.position))
                {
                    anim["scrollElementFadein"].speed = 1;
                    anim["scrollElementFadein"].time = 0F;
                    anim.Play("scrollElementFadein");
                }
                else if (UIScrollFade.IsInside(this.transform.position))
                {
                    anim["productFade"].speed = 1;
                    anim["productFade"].time = 0F;
                    anim.Play("productFade");
                }
                isFadedIn = true;
            }
        }

        // begins fade-out animation. Animator needs to have a "fadeout" trigger and transitions using it.
        public void FadeOut()
        {
            if (canBeAnimated && isFadedIn)
            {
                isFadedIn = false;
                if (elementType == ElementType.editorLevel)
                {
                    anim.Play("scrollElementFadeout");
                }
                else
                {
                    anim["productFade"].speed = -1;
                    anim["productFade"].time = anim["productFade"].length;
                    anim.Play("productFade");
                }
            }
        }

        public void InstantFadeOut()
        {
            if (canBeAnimated)
            {
                if (elementType == ElementType.editorLevel)
                {
                    //anim["scrollElementFadeout"].normalizedTime = 1F;
                    anim["scrollElementFadein"].speed = -1;
                    anim["scrollElementFadein"].time = 0F;
                    anim.Play("scrollElementFadein");
                    Debug.Log("instant fadeout");
                }
                else
                {
                    //anim["productFade"].normalizedTime = 0F;
                    anim["productFade"].speed = -1;
                    anim["productFade"].time = 0F;
                    anim.Play("productFade");
                }
                isFadedIn = false;
            }
        }
    }
}