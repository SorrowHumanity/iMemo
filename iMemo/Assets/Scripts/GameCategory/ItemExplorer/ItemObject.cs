namespace Assets.Scripts.GameCategory.ItemExplorer
{
    using UnityEngine;

    public class ItemObject : MonoBehaviour
    {
        [SerializeField] public ItemExplorerSceneController ItemExplorerSceneController;

        public void OnMouseDown()
        {
            StartCoroutine(ItemExplorerSceneController.CheckPlayerInput(GetComponent<SpriteRenderer>().sprite));
        }

        public void ChangeSprite(Sprite sprite)
        {
            // Get the sprice renderer component and change the property of it's sprite.
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}