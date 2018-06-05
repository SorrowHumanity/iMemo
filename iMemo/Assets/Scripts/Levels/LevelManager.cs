namespace Assets.Scripts.Levels
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class LevelManager : MonoBehaviour
    {
        private static LevelManager _instance;

        public static LevelManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LevelManager();
                }

                return _instance;
            }
        }

        #region MatchMatchAndPatternMemoSettings

        private const int MinimumNumberOfCardsAndCells = 4;
        public const int ConstantLevel = 6;
        private const int LastLevelWithCardsIncremented = 4;

        #endregion MatchMatchAndPatternMemoSettings

        #region NumberSnakeSettings

        private static readonly Color[] TextColors =
            { Color.blue, Color.green, Color.gray, Color.red, Color.white, Color.yellow };

        #endregion NumberSnakeSettings

        #region CommonSettings

        public int GameLevel { get; set; }
        public int GameDifficulty { get; set; }

        #endregion CommonSettings

        #region PatternMemoSettings

        private const int LastLevelWithCellsIncremented = 5;

        #endregion PatternMemoSettings

        #region MatchMatchLevelLogic

        /// <summary>
        ///     Generate Match Match number of cards depending on the given level.
        ///     The number of cards will increment until level 5. From level 6 the number of cards
        ///     will remain the same (being this one the number of cards of level 5)
        /// </summary>
        /// <returns>Number of cards to be used to generate the card grid.</returns>
        public int GenerateMatchMatchNumberOfCards()
        {
            int numberOfCards;
            if (GameLevel >= ConstantLevel)
            {
                numberOfCards = MinimumNumberOfCardsAndCells + (4 * LastLevelWithCardsIncremented);
            }
            else
            {
                numberOfCards = MinimumNumberOfCardsAndCells + (4 * GameLevel);
            }

            return numberOfCards;
        }

        #endregion MatchMatchLevelLogic

        #region PatternMemoLevelLogic

        /// <summary>
        ///     Generate Timer depending on the given level.
        ///     The higher the level is, the lower the timer would be.
        ///
        ///     This timer will be used to stablish the amount of time the play has to memorize
        ///     the pattern before it becomes invisible.
        /// </summary>
        /// <returns>Timer target generated.</returns>
        public float GenerateTimerTargetForPatternMemo()
        {
            var timerTarget = 6.0f - Math.Sqrt(2 * GameLevel);
            return (float)timerTarget;
        }

        /// <summary>
        ///     Generate PatternMemo number of cells depending on the given level.
        /// </summary>
        /// <returns>Number of cells to be used to generate the cell grid.</returns>
        public int GeneratePatternMemoNumberOfCells()
        {
            int numberOfCells;
            if (GameLevel < ConstantLevel)
            {
                numberOfCells = MinimumNumberOfCardsAndCells + (4 * GameLevel);
            }
            else
            {
                numberOfCells = MinimumNumberOfCardsAndCells + (4 * LastLevelWithCellsIncremented);
            }

            return numberOfCells;
        }

        #endregion PatternMemoLevelLogic

        #region CommonLevelLogic

        /// <summary>
        ///     Generate Timer depending on the given level and difficulty.
        ///     The higher the level and the difficulty are, the lower the timer would be.
        /// </summary>
        /// <returns>Timer target generated.</returns>
        public float GenerateTimerForLevel()
        {
            var timerTarget = 60.0f - (2 * GameDifficulty);
            return timerTarget;
        }

        /// <summary>
        ///     Instantiate Timer when the game starts using the level manager logic.
        /// </summary>
        public Timer CreateTimer()
        {
            var timerTarget = GenerateTimerForLevel();
            return new Timer(timerTarget);
        }

        /// <summary>
        ///     Update the total high score for the given game in Player Preferences - using the level util class.
        /// </summary>
        /// <param name="gameName">Name of the game which score has to be updated.</param>
        ///<param name="score">Score to be updated.</param>
        public void UpdateHighScore(string gameName, int score)
        {
            LevelUtil.UpdateLevelScoreInPlayerPreferences(gameName, GameLevel, score);
        }

        /// <summary>
        ///     Update the total played time for the given game in Player Preferences - using the level util class.
        /// </summary>
        /// <param name="gameName">Name of the game which score has to be updated.</param>
        /// <param name="playedTime">Played time for the given game.</param>
        public void UpdatePlayedTime(string gameName, float playedTime)
        {
            LevelUtil.UpdatePlayedTimeInPlayerPreferences(gameName, playedTime);
        }

        /// <summary>
        ///     Restart the given game whenever a level is completed.
        ///     Update the last compleyed level in player preferences using the level util class.
        /// </summary>
        /// <param name="gameName">Name of the game to be restarted.</param>
        public void RestartGame(string gameName)
        {
            var lastPlayedLevel = GameLevel;
            var lastCompletedLevel = LevelUtil.GetLastCompletedLevelFromPlayerPreferences(gameName);

            if (lastPlayedLevel > lastCompletedLevel)
            {
                LevelUtil.UpdateLastCompletedLevelInPlayerPreferences(gameName);
            }

            GameLevel = lastPlayedLevel;
        }

        /// <summary>
        ///     Generates random, non - duplicate numbers within a certain range. The number of
        ///     the generated numbers depends on the game level.
        /// </summary>
        /// <param name="lowerBound"> The minimum number value that could be generated. </param>
        /// <param name="upperBound"> The maximum number value that could be generated. </param>
        /// <param name="size"> The amount of numbers that will be generated. </param>
        /// <returns> A sorted list containing the numbers. </returns>
        public List<int> GenerateRandomNumberList(int lowerBound, int upperBound, int size)
        {
            if (lowerBound > upperBound || size > upperBound)
            {
                throw new ArgumentOutOfRangeException("Upper bound or size is too high!");
            }
            HashSet<int> uniqueNumbers = new HashSet<int>();
            do
            {
                uniqueNumbers.Add(Random.Range(lowerBound, upperBound));
            }
            while (uniqueNumbers.Count < size);
            List<int> randomNumbers = new List<int>(uniqueNumbers);
            randomNumbers.Sort();
            return randomNumbers;
        }

        #endregion CommonLevelLogic

        #region NumberSnakeLevelLogic

        /// <summary>
        ///     Returns a random color. Could be green, yellow, red, blue, grey or red.
        /// </summary>
        /// <returns> A color object. </returns>
        public Color GetRandomColor()
        {
            int chosenOne = Random.Range(0, TextColors.Length);
            return TextColors[chosenOne];
        }

        #endregion NumberSnakeLevelLogic

        #region MentalrobicsLevelLogic

        /// <summary>
        ///     Generates a time for a player turn in Mentalrobics.
        /// </summary>
        /// <returns> The time value. </returns>
        public Timer GenerateMentalrobicsTimer()
        {
            float time = (GameLevel + 5) - GameDifficulty;
            return new Timer(time);
        }

        #endregion MentalrobicsLevelLogic

        #region ItemExplorerLevelLogic

        /// <summary>
        ///     Generate the number of inner levels inside the main level depending on the player's current level.
        /// </summary>
        /// <returns>Number of inner levels for the current game level.</returns>
        public int GenerateItemExplorerNumberOfInnerLevels()
        {
            return GameLevel * 4;
        }

        #endregion ItemExplorerLevelLogic
    }
}