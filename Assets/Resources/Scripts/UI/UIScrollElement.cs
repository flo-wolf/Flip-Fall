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
            isFadedIn = false;
            //anim.Play("scrollElementFadein");
            Main.onSceneChange.AddListener(SceneChanged);
            canBeAnimated = true;
        }

        private void SceneChanged(Main.Scene scene)
        {
            FadeOut();
            canBeAnimated = false;
        }

        // begins fade-in animation. Animator needs to have a "fadein" trigger and transitions using it.
        public void FadeIn()
        {
            if (canBeAnimated && !isFadedIn)
            {
                isFadedIn = true;
                anim.Play("scrollElementFadein");
            }
        }

        // begins fade-out animation. Animator needs to have a "fadeout" trigger and transitions using it.
        public void FadeOut()
        {
            if (canBeAnimated && isFadedIn)
            {
                isFadedIn = false;
                anim.Play("scrollElementFadeout");
            }
        }
    }
}