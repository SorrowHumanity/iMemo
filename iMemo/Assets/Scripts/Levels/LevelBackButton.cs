namespace Assets.Scripts.Levels
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class LevelBackButton : MonoBehaviour
    {
        public void GoBack()
        {
            SceneManager.LoadScene("SelectCategoryScene");
        }
    }
}