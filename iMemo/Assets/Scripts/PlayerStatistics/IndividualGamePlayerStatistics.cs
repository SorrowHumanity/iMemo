namespace Assets.Scripts.PlayerStatistics
{
    using UnityEngine;
    using UnityEngine.UI;

    public class IndividualGamePlayerStatistics : MonoBehaviour
    {
        private Text _gameHighScoreText;
        private Text _timerText;

        public static string GameName;

        private void Update()
        {
            _gameHighScoreText = GameObject.Find("Score").GetComponent<Text>();
            _timerText = GameObject.Find("PlayTime").GetComponent<Text>();

            GetIndividualPlayerStatistics();
        }

        /// <summary>
        ///     Get local player high score and played time from player preferences for an individual game.
        ///     Set the score text to the saved high scored in PlayerPrefs
        ///     Set the timer text to the saved played time in PlayerPrefs
        /// </summary>
        public void GetIndividualPlayerStatistics()
        {
            if (!PlayerStatisticsUtil.IsReturningUser(GameName))
            {
                _gameHighScoreText.text = PlayerStatisticsUtil.FirstTimeUserMessage;
                _timerText.text = PlayerStatisticsUtil.FirstTimeUserMessage;
                return;
            }
            var highScore = PlayerStatisticsUtil.GetGameHighScoreFromPlayerPreferences(GameName);
            _gameHighScoreText.text = highScore.ToString();
            var playedTime = PlayerStatisticsUtil.GetPlayedTimeFromPlayerPreferences(GameName);
            _timerText.text = PlayerStatisticsUtil.PlayedTimeToFormatedTimeSpan(playedTime);
        }
    }
}