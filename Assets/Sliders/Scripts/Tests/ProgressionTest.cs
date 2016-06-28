using Impulse;
using UnityEngine;

public class ProgressionTest : MonoBehaviour
{
    public void Load()
    {
        Progression.LoadProgressData();
    }

    public void Save()
    {
        Progression.SaveProgressData();
    }

    public void Clear()
    {
        Progression.ClearProgress();
        Progression.SaveProgressData();
    }

    public void CreateEntry()
    {
        Progression.SetLevelProgress(1, 2D, true);
    }
}