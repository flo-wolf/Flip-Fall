using Sliders;
using UnityEngine;

namespace Sliders.UI
{
    public class UIProgressManager : MonoBehaviour
    {
        public void Load()
        {
            ProgressManager.LoadProgressData();
        }

        public void Save()
        {
            ProgressManager.SaveProgressData();
        }

        public void Clear()
        {
            ProgressManager.ClearProgress();
            ProgressManager.SaveProgressData();
        }

        public void CreateEntry()
        {
            ProgressManager.SetLevelProgress(1, 2D, true);
        }
    }
}