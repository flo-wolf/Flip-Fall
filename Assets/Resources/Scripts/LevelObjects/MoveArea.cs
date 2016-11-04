using FlipFall.Levels;
using FlipFall.Theme;
using System.Collections;
using UnityEngine;

namespace FlipFall.LevelObjects
{
    public class MoveArea : MonoBehaviour
    {
        public MeshFilter meshFilter;
        private PolygonCollider2D poly2D;
        private MeshRenderer mr;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.material.SetColor("_Color", ThemeManager.theme.moveZoneColor);
            mr.material.SetFloat("_SliceAmount", 0F);
        }

        private void UpdateColliders()
        {
        }

        public void DissolveLevel()
        {
            StartCoroutine(cDissolveLevel(LevelManager._instance.DissolveLevelDuration));
        }

        private IEnumerator cDissolveLevel(float duration)
        {
            if (mr != null)
            {
                yield return new WaitForSeconds(LevelManager._instance.DissolveDelay);
                Material m = mr.material;
                float t = 0F;
                while (t < 1.0f)
                {
                    t += Time.deltaTime * (Time.timeScale / duration);
                    m.SetFloat("_SliceAmount", t);
                    yield return 0;
                }
            }
            else
                Debug.Log("Dissolving Level failed, moveAreaGo MeshRenderer not found.");
            yield break;
        }
    }
}