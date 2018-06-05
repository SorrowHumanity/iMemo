namespace Assets.Scripts.SelectCategory
{
    using MainMenu;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class CategoryController : MonoBehaviour
    {
        public static string GameName { get; set; }

        private const string LevelSceneName = "LevelsScene";

        /// <summary>
        ///     Load the LevelScene for the selected game.
        ///     Each button in the CategoryScene corresponds to one game.
        ///     The button clicked will set the parameter game name to the clicked button game name in the CategoryScene.
        ///     The game name parameter will later be used by the LevelsController to load
        ///     the required game levels information from player preferences.
        /// </summary>
        /// <param name="gameName">Name of the game which levelScene needs to be loaded.</param>
        public void LoadSelectedGameLevelScene(string gameName)
        {
            GameName = gameName;
            SceneManager.LoadScene(LevelSceneName, LoadSceneMode.Single);
        }

        public void GoBack()
        {
            SceneManager.LoadScene(MenuController.MainMenuScene, LoadSceneMode.Single);
        }
    }
}