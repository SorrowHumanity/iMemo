namespace Assets.Scripts.GameCategory.NumberGames.NumberSnakeGame
{
    using UnityEngine;
    using UnityEngine.UI;

    public class NumberObject : MonoBehaviour
    {
        [SerializeField] public NumberSnakeSceneController SceneController;

        public void OnMouseDown()
        {
            // get value from clicked game object
            int number = int.Parse(GetComponent<Text>().text);

            // pass value to the controller
            SceneController.SelectNumber(number);

            // destroy the number game object
            Destroy(gameObject);
        }
    }
}