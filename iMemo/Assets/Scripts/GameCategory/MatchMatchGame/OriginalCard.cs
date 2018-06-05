namespace Assets.Scripts.GameCategory.MatchMatchGame
{
    using UnityEngine;

    public class OriginalCard : MonoBehaviour
    {
        [SerializeField] public GameObject CardBack;
        [SerializeField] public MatchMatchSceneController MatchMatchSceneController;

        public int Id { get; set; }

        /// <summary>
        ///     Reveal card if the card is not already revealed(Car back is active) and only one card is revealed.
        ///     The player cannot reveal more than two cards at the same time.
        /// </summary>
        public void OnMouseDown()
        {
            if (CardBack.activeSelf && MatchMatchSceneController.IsSecondCardRevealed)
            {
                CardBack.SetActive(false);
                MatchMatchSceneController.CanReveal(this);
            }
        }

        /// <summary>
        ///     Change card sprite to the given sprite. Used when creating the card grid.
        /// </summary>
        /// <param name="cardId">Id of the card which sprite needs to be changed.</param>
        /// <param name="sprite">Sprite to be set as the new card sprite.</param>
        public void ChangeSprite(int cardId, Sprite sprite)
        {
            Id = cardId;
            // Get the sprice renderer component and change the property of it's sprite.
            GetComponent<SpriteRenderer>().sprite = sprite;
        }

        /// <summary>
        ///     Set Card back as active whenever the revealed cards are not a match.
        /// </summary>
        public void UnrevealCard()
        {
            CardBack.SetActive(true);
        }
    }
}