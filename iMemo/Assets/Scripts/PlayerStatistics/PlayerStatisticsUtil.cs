namespace Assets.Scripts.PlayerStatistics
{
    using UnityEngine;
    using System;

    public static class PlayerStatisticsUtil
    {
        public static string PlayerGeneralHighScoreKey = "GeneralHighScore";
        public static string FirstTimeUserMessage = "No registered scores";
        private const string PlayedTimeKey = "PlayedTime";
        private const string HighScoreKey = "HighScore";

        /// <summary>
        ///     Checks if the player preferences contain the key passed as parameter.
        /// </summary>
        /// <param name="gameName"> The game name of the game which high score is to be returned. </param>
        /// <returns> true, if the player preferences contains the key. Otherwise, false. </returns>
        public static bool IsReturningUser(string gameName)
        {
            var gameScorePlayerPrefsKey = gameName + HighScoreKey;

            return PlayerPrefs.HasKey(gameScorePlayerPrefsKey);
        }

        /// <summary>
        ///     Retrieves the score saved for the given game at the given level.
        /// </summary>
        /// <param name="gameName">Name of the game which score needs to be returned.</param>
        /// <returns>Score stored in player preferences for the given game at the given level.</returns>
        public static int GetGameHighScoreFromPlayerPreferences(string gameName)
        {
            var gameScorePlayerPrefsKey = gameName + HighScoreKey;

            return PlayerPrefs.GetInt(gameScorePlayerPrefsKey);
        }

        /// <summary>
        ///     Retrieve the total played time for the given game.
        /// </summary>
        /// <param name="gameName">Name of the game which played time needs to be returned.</param>
        /// <returns>Played Time stored in player preferences for the given game at the given level.</returns>
        public static float GetPlayedTimeFromPlayerPreferences(string gameName)
        {
            var playedTimePlayerPrefsKey = gameName + PlayedTimeKey;

            return PlayerPrefs.GetFloat(playedTimePlayerPrefsKey);
        }

        public static int GeTotaltHighScoreFromPlayerPreferences()
        {
            return PlayerPrefs.GetInt(PlayerGeneralHighScoreKey);
        }

        public static string PlayedTimeToFormatedTimeSpan(float playedTime)
        {
            var playedTimeSpan = TimeSpan.FromSeconds(playedTime);
            var formatedTimeSpan = playedTimeSpan.Hours + ":" + playedTimeSpan.Minutes + ":" +
                              playedTimeSpan.Seconds.ToString("n0");

            return formatedTimeSpan;
        }
    }
}