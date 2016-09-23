using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRenderMask : MonoBehaviour
{
    public static LevelRenderMask _instance;
    public RenderTexture renderTexture;

    // Use this for initialization
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        //renderTexture = new RenderTexture(1080, 1920, 32, RenderTextureFormat.ARGB32);
        //renderTexture.Create();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //renderTexture.Create();
        //renderTexture = new RenderTexture(1080, 1920, 1, RenderTextureFormat.ARGB32);
        //renderTexture.Create();
    }
}