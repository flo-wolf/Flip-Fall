using UnityEngine;
using System.Collections;

public class LevelChange : MonoBehaviour {

	public static float fadeTime = 0.5f;
	public GameObject[] levels;
	GameObject level;
	GameObject newLevel;
	Component[] spriteRenderers;

	bool isLevelsCorrupt(){
		if(levels.Length > 0)
		{
			foreach(GameObject level in levels)
			{
				if (level == null) 
					return true;
			}
			return false;
		}
		else 
			return true;
	}

	public void deconstructCurrentLevel()
	{

	}

	//0 < levelNumber < levels.Length
	public void buildLevel(int levelNumber)
	{
		if(0 < levelNumber && levelNumber < levels.Length && !isLevelsCorrupt()){
			level = levels[levelNumber];
			newLevel = (GameObject)Instantiate(level);
			spriteRenderers = newLevel.GetComponentsInChildren<SpriteRenderer>();

			int loop = 1;
			foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
				StartCoroutine(FadeIn(spriteRenderer, loop));
				loop++;
			}
		}
	}

	IEnumerator FadeIn(SpriteRenderer sr, int loop) 
	{ 
		yield return new WaitForSeconds(loop/(spriteRenderers.Length/2));

		float t = 0.0f; 
		
		while (t < 1.0f) 
		{ 
			t += Time.deltaTime * (Time.timeScale/fadeTime);
			
			float fade = t / 1;
			fade = fade * fade * (3f - 2f*t); //Smoothstep formula: t = t*t * (3f - 2f*t)

			sr.color = new Color(1f,1f,1f, Mathf.SmoothStep(0f, 1f, fade));
		}
	}

	
	void FixedUpdate() {
	
	}
}




