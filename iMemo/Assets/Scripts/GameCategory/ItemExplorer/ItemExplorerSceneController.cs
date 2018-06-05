namespace Assets.Scripts.GameCategory.ItemExplorer
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Levels;
    using UI;
    using UnityEngine;
    using Random = System.Random;

    public class ItemExplorerSceneController : MonoBehaviour
    {
        private List<Sprite> _nonUsedSprites, _usedSprites;
        private int _numberOfSprites;
        private Sprite _correctSprite;
        private float _radius;
        private LevelManager _levelManager;
        private Timer _timer;
        private bool _locked, _levelCompleted;
        private float _initialTime;

        private static float _containerWidth;
        private static float _containerHeight;

        #region SerializeField

        [SerializeField] public UIController UiController;
        [SerializeField] public ItemObject ItemObject;
        [SerializeField] public TextMesh ScoreLabel;
        [SerializeField] public TextMesh TimerLabel;

        #endregion SerializeField

        #region Constants

        private const string ItemExplorerSceneName = "ItemExplorerScene";
        private const string GameName = "ItemExplorer";
        private const string ItemExplorerGameContainer = "Container";
        private const string ItemCloneTag = "ItemClone";
        private const string ResourcesFolder = "ItemExplorer";

        #endregion Constants

        private int _score;
        private int _numberOfInnerLevels, _innerLevel;

        #region MonoBehaviour

        private void Start()
        {
            SetContainerBounds();

            _radius = _containerWidth / 2;
            _levelManager = FindObjectOfType<LevelManager>();

            _timer = _levelManager.CreateTimer();
            _initialTime = _timer.TargetTime;

            _numberOfInnerLevels = _levelManager.GenerateItemExplorerNumberOfInnerLevels();
            _nonUsedSprites = Resources.LoadAll<Sprite>(ResourcesFolder).ToList();

            _usedSprites = new List<Sprite>();

            InitializeGame();
        }

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

                _levelManager.UpdatePlayedTime(GameName, _score);
                _levelManager.UpdateHighScore(GameName, _score);

                if (!_levelCompleted && !GameObject.Find("FailedContainer(Clone)"))
                {
                    SetUIControllerSettings();
                    UiController.Failed();
                }
                else
                {
                    _levelManager.RestartGame(GameName);

                    SetUIControllerSettings();
                    UiController.Completed();
                }

                _locked = true;
            }
        }

        #endregion MonoBehaviour

        #region SetUp

        /// <summary>
        ///     Set the container bounds so that when spawning object the object position can always be inside the container bounds.
        /// </summary>
        private static void SetContainerBounds()
        {
            _containerWidth = GameObject.Find("GamesContainer").GetComponent<SpriteRenderer>().size.x;
            _containerHeight = GameObject.Find("GamesContainer").GetComponent<SpriteRenderer>().size.y;
        }

        /// <summary>
        ///     Initialize game whenever an inner level is completed.
        ///         - Get number of sprites depending of the current inner level.
        ///         - Get the sprite representing the correct answer.
        ///         - Spawn Items in the game canvas.
        /// </summary>
        public void InitializeGame()
        {
            GenerateNumberOfSprites();

            GenerateCorrectSprite();

            SpawnSpriteItems();
        }

        /// <summary>
        ///     Generate number of neccessary sprites depending on the current inner level.
        ///     If the inner level is 1, only one sprite will be spawned.
        /// </summary>
        public void GenerateNumberOfSprites()
        {
            _numberOfSprites = _innerLevel == 0 ? 1 : _innerLevel + 1;
        }

        /// <summary>
        ///     Generate the sprite representing the correct answer.
        ///     The correct sprite will be a sprite from the non used sprites at a random index.
        ///     When the correct sprite has been selected, it will be removed from the non used sprites so that
        ///     it can later be added to the used sprite whenever the user completes an inner level.
        /// </summary>
        public void GenerateCorrectSprite()
        {
            var random = new Random();
            var itemIndex = random.Next(0, _nonUsedSprites.Count);

            _correctSprite = _nonUsedSprites[itemIndex];

            _nonUsedSprites.RemoveAt(itemIndex);
        }

        #endregion SetUp

        #region SpawnItems

        /// <summary>
        ///     Generate the position in which the sprites will be spawned.
        ///     The position will be inside the container bounds and the camara frame and will not overlap with other sprite positions.
        /// </summary>
        /// <returns>Generated Spawn position.</returns>
        private static Vector2 GenerateSpawnPosition()
        {
            var positionX = UnityEngine.Random.Range(-_containerWidth / 2, _containerWidth / 2);
            var positionY = UnityEngine.Random.Range(-_containerHeight / 2, _containerHeight / 2 - 2.5f);

            var spawnPosition = new Vector2(positionX, positionY);

            return spawnPosition;
        }

        /// <summary>
        ///     Spawn as many Sprites as the _numberOfSprites indicates(number of neccessary sprites for the current inner level)
        ///     The sprite at index 0 will always have the correct sprite. Since the position is random, the correct sprite will
        ///     always be spawned at a random position even if it is always at index 0.
        ///
        ///     The rest of the sprites will be taken from the used sprite list - containing sprites that the player has already
        ///     seen before in previous inner levels.
        /// </summary>
        public void SpawnSpriteItems()
        {
            var canSpawn = false;
            var spawnPosition = Vector2.zero;
            var usedSpritesIndex = 0;

            for (var i = 0; i < _numberOfSprites; i++)
            {
                while (!canSpawn)
                {
                    spawnPosition = GenerateSpawnPosition();
                    canSpawn = PreventSpawnOverlap(spawnPosition);
                }

                ItemObject itemObject;

                if (i == 0)
                {
                    itemObject = ItemObject;
                    itemObject.ChangeSprite(_correctSprite);
                }
                else
                {
                    itemObject = Instantiate(ItemObject);
                    itemObject.ChangeSprite(_usedSprites[usedSpritesIndex]);
                    usedSpritesIndex++;
                    itemObject.tag = ItemCloneTag;
                }

                itemObject.transform.localScale = new Vector3(8f, 8f, 0);
                itemObject.transform.SetParent(GameObject.Find(ItemExplorerGameContainer).transform, false);
                itemObject.transform.position = spawnPosition;
                canSpawn = false;
            }
        }

        /// <summary>
        ///     Prevent two objects from overlaping when spawning at a random position.
        /// </summary>
        /// <param name="spawnPosition">Position of the itemObject to be spawned.</param>
        /// <returns></returns>
        private bool PreventSpawnOverlap(Vector2 spawnPosition)
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, _radius).ToList();

            foreach (var spriteCollider in colliders)
            {
                var centerPoint = spriteCollider.bounds.center;
                var width = spriteCollider.bounds.extents.x;
                var height = spriteCollider.bounds.extents.y;

                var leftExtend = centerPoint.x - width;
                var rightExtend = centerPoint.x + width;
                var lowerExtend = centerPoint.y - height;
                var upperExtend = centerPoint.y + height;

                if (!(spawnPosition.x >= leftExtend) || !(spawnPosition.x <= rightExtend)) continue;
                if (spawnPosition.y >= lowerExtend && spawnPosition.y <= upperExtend)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion SpawnItems

        #region PlayerInput

        /// <summary>
        ///     Perform different actions depending on the player input.
        ///     If the player has completed all the inner levels with all correct answers,
        ///     restart game and update high score for the current level.
        ///     Otherwise, if the player fails any of the inner levels, redirect him to the fail scene.
        ///
        ///     The moment the user fails a inner level, the whole level is not completed.
        /// </summary>
        /// <param name="selectedItemSprite">Sprite the player has clicked.</param>
        public IEnumerator CheckPlayerInput(Sprite selectedItemSprite)
        {
            if (selectedItemSprite == _correctSprite)
            {
                _usedSprites.Add(_correctSprite);

                if (_innerLevel == _numberOfInnerLevels)
                {
                    _levelCompleted = true;

                    yield return new WaitForSeconds(0.5f);

                    _levelManager.UpdatePlayedTime(GameName, _timer.TargetTime);
                    _levelManager.UpdateHighScore(GameName, _score);
                    _levelManager.RestartGame(GameName);

                    SetUIControllerSettings();
                    UiController.Completed();
                }
                else // player hasn't finished completing the inner levels.
                {
                    _innerLevel++;
                    _score++;
                    ScoreLabel.text = "SCORE: " + _score.ToString();

                    DestroyItemClones();
                    InitializeGame();
                }
            }
            else // player failed any of the inner levels.
            {
                yield return new WaitForSeconds(0.5f);

                SetUIControllerSettings();
                UiController.Failed();
            }
        }

        #endregion PlayerInput

        /// <summary>
        ///     Destroy ItemObject clones whenever an inner level starts.
        ///     Used to avoid having repeating items on the screen when starting a new inner level.
        /// </summary>
        private static void DestroyItemClones()
        {
            var clones = GameObject.FindGameObjectsWithTag(ItemCloneTag);

            foreach (var clone in clones)
            {
                Destroy(clone);
            }
        }

        private void SetUIControllerSettings()
        {
            UIController.SceneName = ItemExplorerSceneName;
            UIController.GameName = GameName;
            UIController.TotalPlayedTime = _initialTime;
        }
    }
}