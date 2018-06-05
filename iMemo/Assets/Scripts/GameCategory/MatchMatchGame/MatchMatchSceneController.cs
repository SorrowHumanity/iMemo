namespace Assets.Scripts.GameCategory.MatchMatchGame
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Levels;
    using UI;
    using UnityEngine;
    using Random = System.Random;

    public class MatchMatchSceneController : MonoBehaviour
    {
        private int _gridRows;
        private int _gridColumns;
        private int _numberOfCards;

        #region Constants

        private const string EnsightAssetsFolder = "ensight_assets";
        private const string MatchMatchSceneName = "MatchMatchScene";
        private const string MatchMatchGameContainer = "Container";
        private const string GameName = "MatchMatch";
        private const float OffsetX = 4f;
        private const float OffsetY = 4.5f;

        #endregion Constants

        private OriginalCard _firstRevealedCard;
        private OriginalCard _secondRevealedCard;
        private int _score;

        #region SerializeFields

        [SerializeField] public TextMesh ScoreLabel;
        [SerializeField] public TextMesh TimerLabel;
        [SerializeField] public OriginalCard OriginalCard;

        #endregion SerializeFields

        public UIController UIController;
        private List<Sprite> _cardSprites;
        private LevelManager _levelManager;
        private Timer _timer;
        public static bool LevelCompleted;
        private int _numberOfTries;
        private int _triesBeforeScoresDecreases;
        private bool _locked;
        private float _initialTime;

        public bool IsSecondCardRevealed
        {
            get { return _secondRevealedCard == null; }
        }

        #region MonoBehaviour

        private void Start()
        {
            _score = _numberOfTries = 0;
            _levelManager = FindObjectOfType<LevelManager>();

            _timer = _levelManager.CreateTimer();
            _initialTime = _timer.TargetTime;

            SetNumberOfCardsAccordingToDifficulty();
            _cardSprites = GetNeccessarySprites();

            _triesBeforeScoresDecreases = _numberOfCards - (_levelManager.GameLevel * _levelManager.GameDifficulty);

            CreateCardGrid();
        }

        /// <summary>
        ///     Update timer within every frame.
        ///     When time target time goes bellow 15, an animation will be enabled to show the
        ///     player that time is running low.
        ///
        ///     When the time target has ended, the game will be restarted and the high score will be updated,
        ///     redirecting the user to the completed or failed scene, depending on if the level was completed.
        /// </summary>
        private void Update()
        {
            _timer.UpdateTimer();

            if (!_timer.TimerEnded)
            {
                if (_timer.TargetTime <= 15.0f)
                {
                    TimerLabel.GetComponent<Animator>().enabled = true;
                }
                TimerLabel.text = _timer.TargetTime.ToString("n2");
            }
            else
            {
                if (_locked) return;

                _levelManager.UpdatePlayedTime(GameName, _timer.TargetTime);
                _levelManager.UpdateHighScore(GameName, _score);

                if (!LevelCompleted && !GameObject.Find("FailedContainer(Clone)"))
                {
                    SetUIControllerSettings();
                    UIController.Failed();
                }
                else
                {
                    _levelManager.RestartGame(GameName);

                    SetUIControllerSettings();
                    UIController.Completed();
                }

                _locked = true;
            }
        }

        #endregion MonoBehaviour

        #region Setup

        /// <summary>
        ///     Get number of cards for the selected level and difficulty using the level manager logic
        ///     and create grid limits depending on the number of cards.
        /// </summary>
        private void SetNumberOfCardsAccordingToDifficulty()
        {
            _numberOfCards = _levelManager.GenerateMatchMatchNumberOfCards();
            SetGridLimits();
        }

        /// <summary>
        ///     Get number of rows and columns needed to construct the grid.
        /// </summary>
        private void SetGridLimits()
        {
            _gridColumns = (int)Math.Sqrt(_numberOfCards);
            _gridRows = (int)Math.Ceiling((double)_numberOfCards / _gridColumns);
        }

        /// <summary>
        ///     Generate the number of card ids needed depending on the number of cards.
        ///     The length of the card id list returned will be equal to half the value of the number of cards
        ///     so that every card can have a duplicate, this will allow the player to find a pair for each of
        ///     the cards in the grid.
        ///
        ///     Each card id will be twice in the list, one id will represent the original card and the other one
        ///     the index of the card pair. This makes it easier to check if the cards are a match.
        /// </summary>
        /// <returns>List of card ids needed to form the grid.</returns>
        public List<int> GenerateCardIds()
        {
            var cardIds = new List<int>();
            var currentCardId = 0;

            while (currentCardId < _numberOfCards / 2)
            {
                cardIds.Add(currentCardId);
                cardIds.Add(currentCardId);
                currentCardId += 1;
            }

            return cardIds;
        }

        /// <summary>
        ///     Shuffle card ids so that everytime the game starts the cards are in a new position.
        /// </summary>
        /// <param name="cardIds">Initial list containing the cards to be shuffled.</param>
        /// <returns>Shuffled list of card ids.</returns>
        public List<int> ShuffleCardIds(IEnumerable<int> cardIds)
        {
            var random = new Random();
            var shuffledList = cardIds.OrderBy(x => random.Next()).ToList();
            return shuffledList;
        }

        /// <summary>
        ///     Get the number of neccessary sprites needed depending on the number of cards.
        ///     The length of the sprite list returned will be equal to half the value of the number of cards
        ///     so that every card can have a duplicate, this will allow the player to find a pair
        ///     for each of the cards in the grid.
        ///     The length of the sprite list will be the same as the length of the card ids list.
        /// </summary>
        /// <returns>List of sprites needed.</returns>
        public List<Sprite> GetNeccessarySprites()
        {
            _cardSprites = Resources.LoadAll<Sprite>(EnsightAssetsFolder).ToList();

            var neccessarySprites = _cardSprites.GetRange(0, _numberOfCards / 2);
            return neccessarySprites;
        }

        #endregion Setup

        #region CreateGrid

        /// <summary>
        ///     Create Card Grid from the grid limits stablished.
        /// </summary>
        private void CreateCardGrid()
        {
            var startPosition = OriginalCard.transform.position;

            var cardIds = GenerateCardIds();

            cardIds = ShuffleCardIds(cardIds);

            if (_gridColumns < _gridRows)
            {
                var temp = _gridRows;
                _gridRows = _gridColumns;
                _gridColumns = temp;
            }

            for (var currentColumn = 0; currentColumn < _gridColumns; currentColumn++)
            {
                for (var currentRow = 0; currentRow < _gridRows; currentRow++)
                {
                    OriginalCard card;
                    if (currentColumn == 0 && currentRow == 0)
                    {
                        card = OriginalCard;
                    }
                    else
                    {
                        card = Instantiate(OriginalCard);
                    }

                    var cardIndex = currentRow * _gridColumns + currentColumn;

                    var cardId = cardIds[cardIndex];
                    card.ChangeSprite(cardId, _cardSprites[cardId]);

                    var positionX = (OffsetX * currentColumn) + startPosition.x;
                    var positionY = (OffsetY * currentRow) + startPosition.y;
                    card.transform.SetParent(GameObject.Find(MatchMatchGameContainer).transform, false);
                    card.transform.position = new Vector3(positionX, positionY, startPosition.z);
                }
            }
        }

        #endregion CreateGrid

        #region UserInput

        /// <summary>
        ///     Set the clicked card as first or second revealed card.
        ///     If the clicked card is the second revealed card check if the revealed cards are a match.
        ///
        ///     Increase numberOfTries within every click -> used for level 6 and above.
        /// </summary>
        /// <param name="card">Revealed Clicked Card.</param>
        public void CanReveal(OriginalCard card)
        {
            _numberOfTries++;
            if (_firstRevealedCard == null)
            {
                _firstRevealedCard = card;
            }
            else
            {
                _secondRevealedCard = card;
                StartCoroutine(CheckCardsMatch());
            }
        }

        /// <summary>
        ///     When two cards are revealed, check if they are a match by comparing their ids.
        ///     If the revealed card ids are the same, increment the score and continue the game
        ///     unless all the cards are revealed, which means that the game ended.
        ///     In that case update high score (if higher than previous saved one) and restart game.
        ///
        ///     If not all the cards are revealed, reset first and second revealed cards so that the game can continue.
        ///
        ///     If the revealed cards ids are not the same, unreveal cards and reset first and second
        ///     revealed cards values so that the game can continue.
        ///
        ///     If the current game level is 6 or higher, we keep track of his number of tries, and  everytime
        ///     the player chooses two cards that aren't a match and the number of tries goes above the available number
        ///     of tries, the score will start to decrease.
        ///
        /// </summary>
        public IEnumerator CheckCardsMatch()
        {
            if (_firstRevealedCard.Id == _secondRevealedCard.Id)
            {
                _score++;
                ScoreLabel.text = "SCORE: " + _score;
                if (_score == _numberOfCards / 2)
                {
                    LevelCompleted = true;
                    _levelManager.UpdatePlayedTime(GameName, _timer.TargetTime);
                    _levelManager.UpdateHighScore(GameName, _score);
                    _levelManager.RestartGame(GameName);

                    SetUIControllerSettings();
                    UIController.Completed();
                }
            }
            else
            {
                if (_levelManager.GameLevel >= LevelManager.ConstantLevel && _numberOfTries >= _triesBeforeScoresDecreases)
                {
                    _score--;
                    ScoreLabel.text = "SCORE: " + _score;
                }

                yield return new WaitForSeconds(0.5f);

                _firstRevealedCard.UnrevealCard();
                _secondRevealedCard.UnrevealCard();
            }

            _firstRevealedCard = null;
            _secondRevealedCard = null;
        }

        #endregion UserInput

        private void SetUIControllerSettings()
        {
            UIController.SceneName = MatchMatchSceneName;
            UIController.GameName = GameName;
            UIController.TotalPlayedTime = _initialTime;
        }
    }
}