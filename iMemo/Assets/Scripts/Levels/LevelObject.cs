namespace Assets.Scripts.Levels
{
    using SelectCategory;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class LevelObject : MonoBehaviour
    {
        #region SerializeField

        public LevelManager LevelManager = LevelManager.Instance;
        [SerializeField] public Sprite LockedLevelSprite;

        #endregion SerializeField

        #region Constants

        private const string LevelCanvasName = "Canvas";
        private const string DifficultyCanvasName = "DifficultyCanvas";
        private const string BeginnerButtonName = "BeginnerButton";
        private const string IntermediateButtonName = "IntermediateButton";
        private const string AdvancedButtonName = "AdvancedButton";
        private const string PlayButtonName = "PlayGameButton";

        #endregion Constants

        private string _gameName;
        private Sprite _selectedLevelSprite;

        #region MonoBehaviour

        private void Start()
        {
            GetSelectedGameName();
            _selectedLevelSprite = GetComponent<SpriteRenderer>().sprite;
        }

        /// <summary>
        ///     Set selected level in level manager so that it can later be used
        ///     by the level manager to generate the level logic for the selected game
        ///     if the level clicked is available (completed or next level to be completed).
        ///     Display the difficulty canvas and hide the level canvas.
        /// </summary>
        private void OnMouseDown()
        {
            if (_selectedLevelSprite.Equals(LockedLevelSprite))
            {
                return;
            }

            var selectedLevel = int.Parse(GetComponentInChildren<TextMesh>().text);
            LevelManager.GameLevel = selectedLevel;
            GameObject.Find(LevelCanvasName).SetActive(false);
            ActivateDifficultyButton(BeginnerButtonName);
            ActivateDifficultyButton(IntermediateButtonName);
            ActivateDifficultyButton(AdvancedButtonName);
        }

        #endregion MonoBehaviour

        #region UserInput

        /// <summary>
        ///     Get difficulty from button clicked and pass it to the levels manager.
        ///     The play button will only be set to active after the player has selected a difficulty.
        /// </summary>
        /// <param name="difficulty">Difficulty key of the difficulty button clicked.</param>
        public void LoadDifficulty(int difficulty)
        {
            LevelManager.GameDifficulty = difficulty;
            ActivateDifficultyButton(PlayButtonName);
        }

        /// <summary>
        ///     The player cannot play a game until the difficulty has been selected.
        ///     Load game selected in the CategoryController.
        /// </summary>
        public void LoadGameScene()
        {
            if (FindObjectsOfType<LevelManager>().Length <= 1)
            {
                DontDestroyOnLoad(LevelManager);
            }
            else // level manager already exists.(Level scene started from back button in game scene)
            {
                // set game level of the current levelManager to the selectedLevel
                FindObjectOfType<LevelManager>().GameLevel = LevelManager.GameLevel;
                FindObjectOfType<LevelManager>().GameDifficulty = LevelManager.GameDifficulty;
            }

            SceneManager.LoadScene(_gameName + "Scene");
        }

        #endregion UserInput

        /// <summary>
        ///     Get the Game Name corresponding to the game name selected in the CategoryScene.
        /// </summary>
        private void GetSelectedGameName()
        {
            _gameName = CategoryController.GameName;
        }

        /// <summary>
        ///     Set difficuly button which name is equal to the parameter to active.
        /// </summary>
        /// <param name="difficultyButtonName">button name of the button to be set active.</param>
        public void ActivateDifficultyButton(string difficultyButtonName)
        {
            GameObject.Find(DifficultyCanvasName).transform.Find(difficultyButtonName).gameObject.SetActive(true);
        }
    }
}