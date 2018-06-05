namespace Assets.Scripts.MainMenu
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System.Globalization;
    using PlayerStatistics;

    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        public GameObject aboutUs;

        [SerializeField] public TextMesh ScoreText;

        public const string MainMenuScene = "MainMenuScene";
        private const string SelectCategorySceneName = "SelectCategoryScene";
        private const string PlayerStatisticsScene = "PlayerStatisticsScene";

        private void Start()
        {
            GetTotalScore();
        }

        public void LoadGameCategory()
        {
            SceneManager.LoadScene(SelectCategorySceneName, LoadSceneMode.Single);
        }

        public void ViewStatistics()
        {
            SceneManager.LoadScene(PlayerStatisticsScene, LoadSceneMode.Single);
        }

        public void AboutUs()
        {
            GameObject.Find("MenuButtons").transform.localScale = new Vector3(0, 0, 0);

            aboutUs.SetActive(true);
        }

        public void AboutUsBack()
        {
            aboutUs.SetActive(false);

            GameObject.Find("MenuButtons").transform.localScale = new Vector3(1, 1, 1);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void GetTotalScore()
        {
            ScoreText.text = PlayerStatisticsUtil.GeTotaltHighScoreFromPlayerPreferences().ToString(CultureInfo.InvariantCulture);
        }
    }
}