using FlipFall.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardClick : MonoBehaviour
{
    public void LeaderboardClicked()
    {
        UILevelselectionManager._instance.OpenLeaderboard();
    }
}