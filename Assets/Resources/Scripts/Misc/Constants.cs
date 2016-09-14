using UnityEngine;

/// <summary>
/// Constants storage, do not change
/// </summary>
namespace Impulse
{
    public static class Constants
    {
        public const float twoStarPercantage = 0.2F;
        public const float cameraZ = -20f;
        public const float playerZ = 0f;
        public const float ghostZ = 0f;
        public const int scoreboardSize = 10;
        public const string timerFormat = "{0:D1}.{1:D2}";
        public const float velocityThreshhold = 5F;
        public const int firstLevel = 1;
        public const int lastLevel = 6;

        //Tags
        public const string portalTag = "Portal";
        public const string moveAreaTag = "MoveArea";
        public const string finishTag = "Finish";
        public const string killTag = "Kill";

        /// <summary>
        /// Converts any Double into the 00:00 timer format used for displaying highscores
        /// </summary>
        /// <param name="t"></param>
        /// <returns>/returns>
        public static string FormatTime(double t)
        {
            string timeString = "--.--";
            int secs = (int)t;
            int milSecs = (int)((t - (int)t) * 100);
            timeString = string.Format(timerFormat, secs, milSecs);
            return timeString;
        }

        /// <summary>
        /// Used to round timer floats to two decimal places
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static double FloatTo2DP(float f)
        {
            return ((double)Mathf.Round((float)f * 100f) / 100f);
        }
    }
}