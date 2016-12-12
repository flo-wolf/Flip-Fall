using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wavetest : MonoBehaviour {

    public static wavetest instance;
    private static bool started;
    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
            started = true;
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
            

        DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
