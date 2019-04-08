using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPrivacyPolicy : MonoBehaviour {

	public string fileName = "PrivacyPolicyURL";

	private void Start()
	{
	}

	public void OpenPrivacyPolicyURL()
	{
		Application.OpenURL(GetURL());
	}

	public string GetURL()
	{
		TextAsset privacyPolicyFile = Resources.Load<TextAsset>(fileName);
		
		string privacyPolicy = privacyPolicyFile.text;
		return privacyPolicy;
	}
}
