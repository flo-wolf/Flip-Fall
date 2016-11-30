using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVersionNumber : MonoBehaviour
{
    public Text text;

    private void Start()
    {
        text.text = "v" + Application.version;
    }
}