namespace Assets.Scripts.GameCategory.NumberGames.NumberSnakeGame
{
    using System.Collections.Generic;
    using Levels;
    using UI;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Diagnostics;

    public class NumberSnakeSceneController : MonoBehaviour
    {
        [SerializeField] public GameObject NumberObjectPrefab;
        [SerializeField] public TextMesh ScoreTextField;
        [SerializeField] public UIController Ui;

        private List<GameObject> _instantiatedObjects;
        private List<int> _orderedNumbers;
        private int _correctAnswerStreak, _score;
        private LevelManager _levelManager;
        private Stopwatch _sessionTimer;

        #region NumberSnakeConstants

        private const string GameName = "NumberSnake";
        private const string SceneName = "NumberSnakeScene";
        private const float ScreenHeight = 250;
        private const float ScreenWidth = 650;

        private const int MinNumValue = 1;
        private const int MaxNumValue = 25;

        private static readonly Vector2 ScreenDimens = new Vector2(ScreenWidth, ScreenHeight);

        #endregion NumberSnakeConstants

        public void Start()
        {
            InitializeComponents();
            StartOrRestartLevel();
        }

        public void OnDisable()
        {
            UpdateTotalPlayedTime();
        }

        /// <summary>
        ///     Validates that a selected number by the player matches the 
        ///     predefined sequence of numbers. If it does, score is increased
        ///     and it is checked if it is the last number of the sequence. If it
        ///     is the last number of the sequence, the level is completed successfully.
        ///     In all other cases, the level is failed.
        /// </summary>
        /// <param name="selectedNumber"> The number that the player selected. </param>
        public void SelectNumber(int selectedNumber)
        {
            int correctNumber = _orderedNumbers[_correctAnswerStreak];
            
            // player has selected correct number
            if (selectedNumber == correctNumber)
            {
                IncreaseScore();
                
                // player wins
                if (_correctAnswerStreak == _levelManager.GameLevel)
                {
                    OnWin();
                }

                return;
            }

            // player has selected wrong number
            OnLoss();
        }

        /// <summary>
        ///     Increases the player score. If it is a high score, save it to the level manager & 
        ///     Player preferences. Finally, update the score text field.
        /// </summary>
        private void IncreaseScore()
        {
            ++_correctAnswerStreak; ++_score;

            // update score in level manager
            _levelManager.UpdateHighScore(GameName, _score);

            // update score text field
            ScoreTextField.text = _score.ToString();
        }

        /// <summary>
        ///     Called when the player completes a level successfully. All number objects are 
        ///     destroyed, UI dialog is shown.
        /// </summary>
        private void OnWin()
        {
            DestroyNumberObjects();
            UIController.TotalPlayedTime = (float) _sessionTimer.Elapsed.TotalSeconds;
            _levelManager.RestartGame(GameName);
            Ui.Completed();
        }

        /// <summary>
        ///     Called when the player fails a level. All number objects are destroyed,
        ///     score is decreased and UI dialog is shown.
        /// </summary>
        private void OnLoss()
        {
            DestroyNumberObjects();
            --_score;
            _levelManager.UpdateHighScore(GameName, _score);
            UIController.TotalPlayedTime = (float)_sessionTimer.Elapsed.TotalSeconds;
            Ui.Failed();
        }

        /// <summary>
        ///     Starts or restarts the game. Instantiates the required amount of 
        ///     number game objects.
        /// </summary>
        private void StartOrRestartLevel()
        {
            _orderedNumbers = _levelManager.GenerateRandomNumberList(MinNumValue, MaxNumValue, _levelManager.GameLevel);
            InstantiateNumberObjects(_levelManager.GameLevel);
        }

        /// <summary>
        ///     Instantiates selectable number objects in the scene. Number of objects
        ///     depends on the difficulty level. Number of objects = game level.
        /// <param name="gameLevel"> The game level. </param>
        /// </summary>
        private void InstantiateNumberObjects(int gameLevel)
        {
            // initialization
            Vector2 randomPosition = Vector2.zero;

            for (int i = 0; i < gameLevel; i++)
            {
                // set number value, color & instantiate number object
                NumberObjectPrefab.GetComponent<Text>().text = _orderedNumbers[i].ToString();
                NumberObjectPrefab.GetComponent<Text>().color = _levelManager.GetRandomColor();

                // generate position for the number game object
                while (true)
                {
                    float x = Random.Range(-ScreenWidth / 2, ScreenWidth / 2);
                    float y = Random.Range(-ScreenHeight / 2, (ScreenHeight - 100) / 2);

                    randomPosition.x = x;
                    randomPosition.y = y;

                    bool canSpawnHere = PhysicsUtil.CheckBounds2D(randomPosition, ScreenDimens);

                    // is the position valid?
                    if (canSpawnHere) break;  
                }
                GameObject newObject = Instantiate(NumberObjectPrefab, randomPosition, UIController.Identity());
                _instantiatedObjects.Add(newObject);

                // make the number object a child of the canvas
                _instantiatedObjects[i].transform.SetParent(GameObject.Find("Container").transform, false);
            }
        }

        /// <summary>
        ///     Destroys all instantiated number objects and clears the number sequence.
        /// </summary>
        private void DestroyNumberObjects()
        {
            foreach (GameObject obj in _instantiatedObjects)
            {
                Destroy(obj);
            }
            _orderedNumbers.Clear();
            _instantiatedObjects.Clear();
        }

        /// <summary>
        ///     Updates the total played time in the player statistics.
        /// </summary>
        private void UpdateTotalPlayedTime()
        {
            _levelManager.UpdatePlayedTime(GameName, _sessionTimer.Elapsed.Seconds);
        }

        /// <summary>
        ///     Initializes instance variables and starts the total played time timer.
        /// </summary>
        private void InitializeComponents()
        {
            UIController.GameName = GameName;
            UIController.SceneName = SceneName;

            // get the level manager
            _levelManager = FindObjectOfType<LevelManager>();

            // measure total time spent in the level
            _sessionTimer = new Stopwatch();
            _sessionTimer.Start();

            _instantiatedObjects = new List<GameObject>();
            _correctAnswerStreak = 0;
        }
    }
}