namespace Assets.Scripts.Levels
{
    using PlayerStatistics;
    using UnityEngine;

    public class LevelUtil : MonoBehaviour
    {
        private const string LastCompletedLevelKey = "LastCompletedLevel";
        private const string HighScoreLevelKey = "HighScoreLevel_";
        private const string HighScoreKey = "HighScore";
        private const string PlayedTimeKey = "PlayedTime";

        #region LastCompletedLevel

        /// <summary>
        ///     Retrieves the last played level value for game selected in the CategoryController.
        /// </summary>
        /// <param name="gameName">Name of the game which last played level needs to be returned.</param>
        /// <returns>Last played level for the given game.</returns>
        public static int GetLastCompletedLevelFromPlayerPreferences(string gameName)
        {
            var lastCompletedLevelPlayerPrefsKey = gameName + LastCompletedLevelKey;

            return PlayerPrefs.GetInt(lastCompletedLevelPlayerPrefsKey);
        }

        /// <summary>
        ///     Updates the last completed level value whenever a level has been completed for the given game.
        /// </summary>
        /// <param name="gameName">Name of the game which level has been completed.</param>
        public static void UpdateLastCompletedLevelInPlayerPreferences(string gameName)
        {
            var lastCompletedLevelPlayerPrefsKey = gameName + LastCompletedLevelKey;
            var lastCompletedLevel = GetLastCompletedLevelFromPlayerPreferences(gameName) + 1;
            PlayerPrefs.SetInt(lastCompletedLevelPlayerPrefsKey, lastCompletedLevel);
            PlayerPrefs.Save();
        }

        #endregion LastCompletedLevel

        #region LevelScore

        /// <summary>
        ///     Retrieves the score saved for the given game at the given level.
        /// </summary>
        /// <param name="gameName">Name of the game which score needs to be returned.</param>
        /// <param name="level">Level for the given game which score needs to be returned.</param>
        /// <returns>Score stored in player preferences for the given game at the given level.</returns>
        public static int GetLevelScoreFromPlayerPreferences(string gameName, int level)
        {
            var levelScorePlayerPrefsKey = gameName + HighScoreLevelKey + level;

            return PlayerPrefs.GetInt(levelScorePlayerPrefsKey);
        }

        /// <summary>
        ///     Updates the level high score for a given game and level if the given score is higher than the saved score.
        ///     Requests a general high score update and a general game high score(total high score of all games) update.
        /// </summary>
        /// <param name="gameName">Name of the game which level has been completed.</param>
        /// <param name="level">Level of the game which score has to be updated.</param>
        /// <param name="newLevelScore">Score to be updated.</param>
        public static void UpdateLevelScoreInPlayerPreferences(string gameName, int level, int newLevelScore)
        {
            var levelScorePlayerPrefsKey = gameName + HighScoreLevelKey + level;
            var savedLevelScore = GetLevelScoreFromPlayerPreferences(gameName, level);
            if (newLevelScore > savedLevelScore)
            {
                PlayerPrefs.SetInt(levelScorePlayerPrefsKey, newLevelScore);
                PlayerPrefs.Save();
            }

            UpdateGeneralGameHighScoreInPlayerPreferences(gameName, newLevelScore);
            UpdateGeneralHighScore(newLevelScore);
        }

        #region HighScore

        /// <summary>
        ///     Updates the general score for a given game if the achieved score is higher than the saved score.
        ///     This method will be called whenever the user completes a level.
        ///     It is used so that the player can keep track of his highest score at any given level.
        /// </summary>
        /// <param name="gameName">Name of the game which level has been completed.</param>
        /// <param name="newLevelScore">Score to be updated.</param>
        private static void UpdateGeneralGameHighScoreInPlayerPreferences(string gameName, int newLevelScore)
        {
            var totalHighScorePlayerPrefsKey = gameName + HighScoreKey;
            var savedHighScore = PlayerStatisticsUtil.GetGameHighScoreFromPlayerPreferences(gameName);

            if (newLevelScore > savedHighScore)
            {
                PlayerPrefs.SetInt(totalHighScorePlayerPrefsKey, newLevelScore);
                PlayerPrefs.Save();
            }
        }

        public static void UpdateGeneralHighScore(int highScore)
        {
            var totalHighScore = PlayerStatisticsUtil.GeTotaltHighScoreFromPlayerPreferences();
            if (highScore > totalHighScore)
            {
                PlayerPrefs.SetInt(PlayerStatisticsUtil.PlayerGeneralHighScoreKey, highScore);
                PlayerPrefs.Save();
            }
        }

        #endregion HighScore

        #endregion LevelScore

        #region PlayedTime

        /// <summary>
        ///     Update the time the player has spent playing the given game.
        /// </summary>
        /// <param name="gameName">Name of the game which time is to be updated.</param>
        /// <param name="playedTime">Time to be saved in Player Preferences.</param>
        public static void UpdatePlayedTimeInPlayerPreferences(string gameName, float playedTime)
        {
            var playedTimePlayerPrefsKey = gameName + PlayedTimeKey;
            var savedPlayedTime = PlayerStatisticsUtil.GetPlayedTimeFromPlayerPreferences(gameName);

            PlayerPrefs.SetFloat(playedTimePlayerPrefsKey, savedPlayedTime + playedTime);
            PlayerPrefs.Save();
        }

        #endregion PlayedTime
    }
}