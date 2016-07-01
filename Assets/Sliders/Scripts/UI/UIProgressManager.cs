using Sliders;
using UnityEngine;
using UnityEngine.UI;

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

        public void CreateScoreboard()
        {
        }
    }
}