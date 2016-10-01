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

        private float despawnDelay = 0.5F;
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

            Debug.Log("[UILevelNumber] Start() ID: " + id + "  Position: " + position);
            animator.SetInteger("Position", position);
            animator.SetBool("FirstSet", false);

            Main.onSceneChange.AddListener(SceneChanged);
        }

        private void SceneChanged(Main.Scene s)
        {
            animator.SetTrigger("fadeout");
        }

        public void Right()
        {
            position++;
            animator.SetInteger("Position", position);
            animator.SetTrigger("Right");

            if (position > 2)
            {
                StartCoroutine(cDestroyAfterAnimation());
            }
        }

        public void Left()
        {
            position--;
            animator.SetInteger("Position", position);
            animator.SetTrigger("Left");

            if (position < -2)
            {
                StartCoroutine(cDestroyAfterAnimation());
            }
        }

        // destroy the object after the fade-out animation has finished
        private IEnumerator cDestroyAfterAnimation()
        {
            yield return new WaitForSeconds(despawnDelay);
            UILevelPlacer.placedLevelNumbers.Remove(this);
            Destroy(gameObject);
            yield break;
        }
    }
}