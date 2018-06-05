namespace Assets.Scripts.UI
{
    using Levels;
    using MainMenu;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using PlayerStatistics;

    public class UIController : MonoBehaviour
    {
        #region UIControllerConstants

        private const string ContainerButtons = "ContainerButtons";
        private const string Container = "Container";
        private const string YourScore = "YourScore";
        private const string Timer = "Timer";
        private const string Canvas = "Canvas";
        private const string Score = "Score";
        private const string TotalScoreText = "TotalScoreText";
        private const string TimerText = "TimerText";

        private const string LevelsScene = "LevelsScene";

        #endregion UIControllerConstants

        #region Instantiations

        public GameObject PauseContainer;
        public GameObject CompletedContainer;
        public GameObject FailedContainer;

        public static string SceneName;
        public static string GameName;
        public static float TotalPlayedTime;

        private LevelManager _levelManager;
        private int _score, _totalScore;

        #endregion Instantiations

        private void Start()
        {
            _levelManager = FindObjectOfType<LevelManager>();
        }

        /*private void Awake()
    {
        if (!GameObject.Find("UI(Clone)"))
        {
            Instantiate(ui, Default(), Identity());

            GameObject.Find("Score").transform.localScale = new Vector3((float)0.5, (float)0.5, (float)0.75);

            GameObject.Find("FailedContainer").transform.localScale = Reset();

            GameObject.Find("PauseContainer").transform.localScale = Reset();

            GameObject.Find("CompletedContainer").transform.localScale = Reset();

            DontDestroyOnLoad(GameObject.FindGameObjectWithTag("UI"));
        }
        //Destroy on Load makes duplicates once you use the back button.
    }*/

        #region ContainerButtons

        public void PauseButton()
        {
            Time.timeScale = 0;

            var obj = Instantiate(PauseContainer, Default(), Identity());

            obj.transform.SetParent(GameObject.Find(Canvas).transform, true);

            obj.transform.localScale = new Vector3(20, 20, 20);

            ScaleControlButtons();
        }

        public void ExitButton()
        {
            SceneManager.LoadScene(MenuController.MainMenuScene, LoadSceneMode.Single);
        }

        public void Resume()
        {
            Time.timeScale = 1;

            GameObject.Find(ContainerButtons).transform.localScale = new Vector3(1, 1, 0);

            GameObject.Find(Container).transform.localScale = new Vector3(1, 1, 0);

            GameObject.Find(YourScore).transform.localScale = new Vector3(1, 1, 0);

            GameObject.Find(Timer).transform.localScale = new Vector3(1, 1, 0);

            Destroy(GameObject.Find("PauseContainer(Clone)"));
        }

        #endregion ContainerButtons

        #region Prefixes

        public static Vector3 Reset()
        {
            return new Vector3(0, 0, 0);
        }

        public static Vector2 Default()
        {
            return new Vector3(0, 0);
        }

        public static Quaternion Identity()
        {
            return Quaternion.identity;
        }

        private static void SetScale(string objectName)
        {
            GameObject.Find(objectName).transform.localScale = Default();
        }

        private static void ScaleControlButtons()
        {
            SetScale(ContainerButtons);

            SetScale(Container);

            SetScale(YourScore);

            SetScale(Timer);
        }

        #endregion Prefixes

        #region Failed&Complete Buttons

        public void RepeatLevel()
        {
            Time.timeScale = 1;

            Application.LoadLevel(Application.loadedLevel);
        }

        public void GoToLevels()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(LevelsScene, LoadSceneMode.Single);
        }

        public void GoToNextLevel()
        {
            SceneManager.LoadScene(SceneName);
        }

        #endregion Failed&Complete Buttons

        #region Failed&Complete

        public void Failed()
        {
            var obj = Instantiate(FailedContainer, Default(), Identity());

            obj.transform.SetParent(GameObject.Find(Canvas).transform, true);

            obj.transform.localScale = new Vector3(20, 20, 20);

            GetPlayerStatistics();

            obj.transform.Find(Score).GetComponentInChildren<TextMesh>().text = _score.ToString();

            GameObject.Find(TotalScoreText).GetComponentInChildren<TextMesh>().text = _totalScore.ToString();

            GameObject.Find(TimerText).GetComponentInChildren<TextMesh>().text =
                PlayerStatisticsUtil.PlayedTimeToFormatedTimeSpan(TotalPlayedTime);

            ScaleControlButtons();
        }

        public void Completed()
        {
            var obj = Instantiate(CompletedContainer, Default(), Identity());

            obj.transform.SetParent(GameObject.Find(Canvas).transform, true);

            obj.transform.localScale = new Vector3(20, 20, 20);

            GetPlayerStatistics();

            obj.transform.Find(Score).GetComponentInChildren<TextMesh>().text = _score.ToString();

            GameObject.Find(TotalScoreText).GetComponentInChildren<TextMesh>().text = _totalScore.ToString();

            GameObject.Find(TimerText).GetComponentInChildren<TextMesh>().text =
                PlayerStatisticsUtil.PlayedTimeToFormatedTimeSpan(TotalPlayedTime);

            _levelManager.GameLevel++;

            ScaleControlButtons();
        }

        #endregion Failed&Complete

        private void GetPlayerStatistics()
        {
            _score = LevelUtil.GetLevelScoreFromPlayerPreferences(GameName, _levelManager.GameLevel);
            _totalScore = PlayerStatisticsUtil.GetGameHighScoreFromPlayerPreferences(GameName);
        }
    }
}