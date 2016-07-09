using System.Collections;
using UnityEngine;

public class UIDeathscreen : MonoBehaviour
{
    public static UIDeathscreen uiDeathscren;

    public void Awake()
    {
        uiDeathscren = this;
    }

    private void Start()
    {
    }

    public void Appear()
    {
        uiDeathscren.gameObject.SetActive(true);
    }

    public void Hide()
    {
        uiDeathscren.gameObject.SetActive(true);
    }
}