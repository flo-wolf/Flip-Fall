using Impulse.Levels;
using Impulse.Progress;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Impulse.UI
{
    public class UILevelNumber : MonoBehaviour
    {
        public int id = -1;
        public int position = 6;
        public Text levelNumberText;
        private Animator animator;

        private Highscore highscore;

        public void Start()
        {
            levelNumberText.text = id.ToString();
            animator = GetComponent<Animator>();

            if (position == 6 || animator == null)
            {
                Debug.Log("No UILevelNumber position set, switching to -1");
                position = -1;
            }

            Debug.Log("Nsssssssssss" + id + "  p " + position);
            animator.SetInteger("Position", position);
            animator.SetBool("FirstSet", false);
        }

        public void Right()
        {
            animator.SetTrigger("Right");
            position++;

            if (position >= 3)
            {
                UILevelPlacer.placedLevelNumbers.Remove(this);
                //UILevelPlacer.PlaceLevelNumber(this.id - 5, -2);
                Destroy(this);
                // destroy and remove out of list
            }
        }

        public void Left()
        {
            animator.SetTrigger("Left");
            position--;

            if (position <= -3)
            {
                UILevelPlacer.placedLevelNumbers.Remove(this);
                //UILevelPlacer.PlaceLevelNumber(this.id + 5, 2);
                Destroy(this);
                // destroy and remove out of list
            }
        }
    }
}