namespace Assets.Scripts.PlayerStatistics
{
    using System;
    using MainMenu;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using System.Globalization;

    public class GeneralPlayerStatistics : MonoBehaviour
    {
        public GameObject AllHighScoresHolder;
        public GameObject IndividualScoreHolder;
        public Text GeneralHighScoreText;

        #region Constants

        private const int MatchMatchGameId = 0;
        private const int NumberSnakeGameId = 1;
        private const int MentalrobicsGameId = 2;
        private const int ItemExplorerGameId = 3;
        private const int PatternMemoGameId = 4;

        #endregion Constants

        private void Start()
        {
            GetPlayerGeneralHighScore();
        }

        /// <summary>
        ///     Get local player general high score from player preferences.
        ///     The general high score refers to the highest score achieved in all of the available games.
        /// </summary>
        public void GetPlayerGeneralHighScore()
        {
            if (!PlayerStatisticsUtil.IsReturningUser(PlayerStatisticsUtil.PlayerGeneralHighScoreKey))
            {
                GeneralHighScoreText.text = PlayerStatisticsUtil.FirstTimeUserMessage;
                return;
            }
            var highScore = PlayerStatisticsUtil.GeTotaltHighScoreFromPlayerPreferences();
            GeneralHighScoreText.text = highScore.ToString(CultureInfo.InvariantCulture);
        }

        #region ButtonListeners

        public void LoadMainMenuScene()
        {
            SceneManager.LoadScene(MenuController.MainMenuScene, LoadSceneMode.Single);
        }

        public void LoadIndividualGamePlayerStatistics(int gameId)
        {
            AllHighScoresHolder.SetActive(false);

            IndividualGamePlayerStatistics.GameName = GetGameName(gameId);
            IndividualScoreHolder.SetActive(true);
        }

        private string GetGameName(int gameId)
        {
            switch (gameId)
            {
                case MatchMatchGameId:
                    return "MatchMatch";

                case NumberSnakeGameId:
                    return "NumberSnake";

                case MentalrobicsGameId:
                    return "Mentalrobics";

                case PatternMemoGameId:
                    return "PatternMemo";

                case ItemExplorerGameId:
                    return "ItemExplorer";

                default:
                    return "";
            }
        }

        [Obsolete("Whatever it's crying for")]
        public void GoBack()
        {
            if (AllHighScoresHolder.active)
            {
                SceneManager.LoadScene(MenuController.MainMenuScene, LoadSceneMode.Single);
            }
            else
            {
                GoBackToSelectedScene();
            }
        }

        private void GoBackToSelectedScene()
        {
            IndividualScoreHolder.SetActive(false);

            AllHighScoresHolder.SetActive(true);
        }

        #endregion ButtonListeners
    }
}