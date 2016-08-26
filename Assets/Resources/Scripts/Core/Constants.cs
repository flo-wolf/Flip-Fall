/// <summary>
/// Constants storage, do not change
/// </summary>
namespace Sliders
{
    public static class Constants
    {
        public const float twoStarPercantage = 0.1F;
        public const float cameraZ = -20f;
        public const float playerZ = 0f;
        public const float ghostZ = 0f;
        public const int scoreboardSize = 10;
        public const string timerFormat = "{0:D1}.{1:D2}";
        public const float velocityThreshhold = 5F;
        public const int lastPage = 6; //placeholder, find better solution, used as levelselection scroll boundery check,
        public const int itemsPerPage = 6; //Do not modify

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
    }
}