using Impulse.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonClick : MonoBehaviour
{
    public void PlayClicked()
    {
        UILevelselectionManager._instance.PlayLevel();
    }
}