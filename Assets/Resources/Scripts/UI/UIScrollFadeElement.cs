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
        // Fading Animator
        public Animator animator;

        // Controls wether or not is is needed to start a fading animation
        // if an element is outside the scrollArea, but set to inside, a fadeout will be called - setting it to false, to prevent more fades from happening
        public bool inside;

        // Use this for initialization
        private void Start()
        {
            if (animator == null)
            {
                animator = gameObject.GetComponent<Animator>();
            }
        }

        public void FadeIn()
        {
            animator.SetTrigger("fadein");
        }

        public void FadeOut()
        {
            animator.SetTrigger("fadeout");
        }

        public void Destroy()
        {
            GameObject.Destroy(this);
        }
    }
}