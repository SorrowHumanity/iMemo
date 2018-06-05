namespace Assets.Scripts.GameCategory.NumberGames.MentalrobicsGame
{
    using System.Collections.Generic;
    using System.Linq;
    using Levels;
    using UnityEngine;
    using UnityEngine.UI;
    using UI;
    using System.Diagnostics;

    public class MentalrobicsSceneController : MonoBehaviour
    {
        [SerializeField] public GameObject NumberTextField;
        [SerializeField] public Text TimerTextField;
        [SerializeField] public InputField InputField;
        [SerializeField] public Button CheckAnswersButton;
        [SerializeField] public Text ResultTextField;
        [SerializeField] public Button NextLevelButton;
        [SerializeField] public Button RepeatLevelButton;
        [SerializeField] public Text ScoreTextField;

        private LevelManager _levelManager;
        private List<int> _orderedNumbers;
        private Timer _memoTimer;
        private bool _hasMemorizationTime;
        private string _expectedInput;
        private int _score;
        private Stopwatch _sessionTimer;

        #region MentalrobicsSceneControllerConstants

        private const string GameName = "Mentalrobics";
        private const string SceneName = "MentalrobicsScene";

        private const string WinMessage = "Correct!";
        private const string LossMessage = "Wrong!";

        private const int MinNumValue = 0;
        private const int MaxNumValue = 100;

        #endregion MentalrobicsSceneControllerConstants

        public void Start()
        {
            InitializeComponents();
            StartOrRestartLevel();
        }

        public void Update()
        {
            // stop updating timer or asking for input over and
            // over again when memo timer is expired
            if (!_hasMemorizationTime) return;
            
            // timer has ended
            if (_memoTimer.TimerEnded)
            {
                AskForInput();
            }
            else // timer still running
            {
                UpdateMemoTimer();
            }
        }

        public void OnDisable()
        {
            UpdateTotalPlayedTime();
        }

        /// <summary>
        ///     Initializes scene components. Locates an instance of the LevelManager. Starts the total 
        ///     played time timer. Disables InputField, NextLevelButton, RepeatLevelButton and 
        ///     CheckAnswersButton game objects.
        /// </summary>
        private void InitializeComponents()
        {
            UIController.GameName = GameName;
            UIController.SceneName = SceneName;

            _sessionTimer = new Stopwatch();
            _sessionTimer.Start();

            _levelManager = FindObjectOfType<LevelManager>();
            _memoTimer = _levelManager.GenerateMentalrobicsTimer();
            _orderedNumbers = new List<int>();

            // intial score
            _score = 0;

            // disable input field, next level button, repeat level button & check answer button
            InputField.gameObject.SetActive(false);
            NextLevelButton.gameObject.SetActive(false);
            RepeatLevelButton.gameObject.SetActive(false);
            CheckAnswersButton.gameObject.SetActive(false);
        }

        /// <summary>
        ///     Used in every frame. Updates the timer object and the timer
        ///     text field.
        /// </summary>
        private void UpdateMemoTimer()
        {
            _memoTimer.UpdateTimer();

            // timer becomes red if not much time is left
            if (_memoTimer.TargetTime < 5f)
            {
                TimerTextField.color = Color.red;
            }
            TimerTextField.text = _memoTimer.TargetTime.ToString("0.0");
        }

        /// <summary>
        ///     Shows the numbers that the player will be asked to remember on the scene.
        /// </summary>
        private void ShowNumbers()
        {
            // convert int array to a string
            string[] numArray = _orderedNumbers.Select(i => i.ToString()).ToArray();
            _expectedInput = string.Join(" ", numArray);

            // set text field properties
            Text numberTextField = NumberTextField.GetComponent<Text>();
            numberTextField.text = _expectedInput;
            numberTextField.color = Color.white;
            numberTextField.transform.SetParent(GameObject.Find("Container").transform, false);

            // enable timer and number text fields
            NumberTextField.GetComponent<Text>().enabled = true;
            TimerTextField.GetComponent<Text>().enabled = true;
            _hasMemorizationTime = true;
        }

        /// <summary>
        ///     Generates a timer and a sequence of numbers for the level that is about ot be started.
        ///     Shows the generated sequence of numbers to the player.
        /// </summary>
        private void StartOrRestartLevel()
        {
            // timer field green by default
            TimerTextField.color = Color.green;

            // generate a timer
            _memoTimer = _levelManager.GenerateMentalrobicsTimer();

            // generate numbers
            _orderedNumbers = _levelManager.GenerateRandomNumberList(MinNumValue, MaxNumValue, _levelManager.GameLevel);

            // show them to the player
            ShowNumbers();
        }

        /// <summary>
        ///     Enables the input field for the player, and disables
        ///     the timer and number text field game objects. Sets the hasMemorizationTime
        ///     flag to true.
        /// </summary>
        private void AskForInput()
        {
            // clear number text field and timer text field to not obscure the view
            NumberTextField.GetComponent<Text>().text = string.Empty;
            TimerTextField.GetComponent<Text>().text = string.Empty;

            // activate the input field and check answers button
            InputField.gameObject.SetActive(true);
            InputField.Select();
            CheckAnswersButton.gameObject.SetActive(true);

            _hasMemorizationTime = false;
        }

        /// <summary>
        ///     Called when the Check answers button is touched. Retrieves the player input, and compares it
        ///     to the exptected input. Updates the UI, according to the outcome of the level.
        /// </summary>
        public void CheckAnswersOnClick()
        {
            string playerInput = InputField.text;

            // player loses
            if (string.IsNullOrEmpty(playerInput))
            {
                OnInvalidInput();
                return;
            }
            
            bool hasWon = DecideLevelOutcome(playerInput);
            ShowResult(hasWon);

            // player wins
            if (hasWon)

            {
                OnWin();
            }
            else // player loses
            {
                OnLoss();
            }
            CheckAnswersButton.gameObject.SetActive(false);
        }

        /// <summary>
        ///     Called when the player wins. Increments the score, updates the level
        ///     in the level manager, updates the ScoreTextField and enables 
        ///     the NextLevel button.
        /// </summary>
        private void OnWin()
        {
            LevelUp();
            ScoreTextField.text = "SCORE: " + _score;
            NextLevelButton.gameObject.SetActive(true);
        }

        /// <summary>
        ///     Compares the actual player input to the expected player input. Used to decide if
        ///     a level has been completed or failed.
        /// </summary>
        /// <param name="playerInput"> The actual player input. </param>
        /// <returns> true, if the actual input and the expected input match. Otherwise, false. </returns>
        private bool DecideLevelOutcome(string playerInput) 
        {
            return playerInput.Equals(_expectedInput);
        }

        /// <summary>
        ///     Called when the player loses. Enables the RepeatLevel button.
        /// </summary>
        private void OnLoss()
        {
            RepeatLevelButton.gameObject.SetActive(true);
        }

        /// <summary>
        ///     Called when the player enters invalid input (null or empty string). 
        ///     Notifies the player that the level has been failed, disables the CheckAnswers button
        ///     and enables the RepeatLevel button.
        /// </summary>
        private void OnInvalidInput()
        {
            ShowResult(false);
            CheckAnswersButton.gameObject.SetActive(false);
            RepeatLevelButton.gameObject.SetActive(true);
        }

        /// <summary>
        ///     Called when Next level button has been touched. Disables the next level button and the 
        ///     input field. Clears the ResultTextField and InputField. Finally, starts the next level.
        /// </summary>
        public void NextLevelOnClick()
        {
            InputField.Select();
            InputField.text = ResultTextField.text = string.Empty;
            InputField.gameObject.SetActive(false);
            NextLevelButton.gameObject.SetActive(false);
            StartOrRestartLevel();
        }

        /// <summary>
        ///     Called when the Repeat level button has been selected. Disables the Repeat level button and the 
        ///     input field. Clears the ResultTextField and InputField. Finally, starts the same level again.
        /// </summary>
        public void RepeatLevelOnClick()
        {
            InputField.Select();
            InputField.text = ResultTextField.text = string.Empty;
            InputField.gameObject.SetActive(false);
            RepeatLevelButton.gameObject.SetActive(false);
            StartOrRestartLevel();
        }

        /// <summary>
        ///     Increases the score counter, and updates the level value in the level manager &
        ///     Player preferences. Also updates the high score in the level manager if the currect score
        ///     is the highest ever achieved.
        /// </summary>
        private void LevelUp()
        {
            ++_score; ++_levelManager.GameLevel;
            _levelManager.RestartGame(GameName);
            _levelManager.UpdateHighScore(GameName, _score);
        }

        /// <summary>
        ///     Notifies the player if the game level has been completed successfully or not by
        ///     showing a message in the ResultTextField.
        /// </summary>
        /// <param name="hasWon"> Indicates if the player has successfully completed the level or not. </param>
        private void ShowResult(bool hasWon)
        {
            if (hasWon)
            {
                ResultTextField.text = WinMessage;
                ResultTextField.color = Color.green;
            }
            else
            {
                ResultTextField.text = LossMessage;
                ResultTextField.color = Color.red;
            }
        }

        /// <summary>
        ///     Updates the total played time value in the statistics.
        /// </summary>
        private void UpdateTotalPlayedTime()
        {
            _levelManager.UpdatePlayedTime(GameName, (float)_sessionTimer.Elapsed.TotalSeconds);
        }
        
    }
}