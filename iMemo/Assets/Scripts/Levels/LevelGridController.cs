namespace Assets.Scripts.Levels
{
    using System;
    using SelectCategory;
    using UnityEngine;

    public class LevelGridController : MonoBehaviour
    {
        #region SerializeFields

        [SerializeField] public GameObject CompletedLevelSprite;
        [SerializeField] public Sprite LockedLevelSprite;
        [SerializeField] public Sprite NextAvailableLevelSprite;

        #endregion SerializeFields

        #region Constants

        private const float OffsetX = 2f;
        private const float OffsetY = 2f;
        private const int TotalNumberOfLevels = 9;
        private const string LevelCanvasName = "Canvas";

        #endregion Constants

        private int _gridColumns;
        private int _gridRows;
        private string _gameName;

        private int _lastCompletedLevel;

        #region MonoBehaviour

        private void Start()
        {
            GetSelectedGameName();
            SetLastCompletedLevel();
            SetGridLimits();
            SpawnCompletedLevelGameObjects();
        }

        #endregion MonoBehaviour

        #region Setup

        /// <summary>
        ///     Get the Game Name corresponding to the game name selected in the CategoryScene.
        /// </summary>
        private void GetSelectedGameName()
        {
            _gameName = CategoryController.GameName;
        }

        /// <summary>
        ///    Set last completed level to the value returned by player preferences for the given game.
        /// </summary>
        public void SetLastCompletedLevel()
        {
            _lastCompletedLevel = LevelUtil.GetLastCompletedLevelFromPlayerPreferences(_gameName);
        }

        /// <summary>
        ///     Get number of needed rows and columns needs to construct the grid.
        /// </summary>
        private void SetGridLimits()
        {
            _gridColumns = (int)Math.Sqrt(TotalNumberOfLevels);
            _gridRows = (int)Math.Ceiling((double)TotalNumberOfLevels / _gridColumns);
        }

        #endregion Setup

        #region Grid

        /// <summary>
        ///     Spawn level objects in Grid.
        /// </summary>
        public void SpawnCompletedLevelGameObjects()
        {
            var startPosition = CompletedLevelSprite.transform.position;

            for (var currentColumn = 0; currentColumn < _gridColumns; currentColumn++)
            {
                for (var currentRow = 0; currentRow < _gridRows; currentRow++)
                {
                    GameObject currentLevelObject;

                    if (currentColumn == 0 && currentRow == 0)
                    {
                        currentLevelObject = CompletedLevelSprite;
                    }
                    else
                    {
                        currentLevelObject = Instantiate(CompletedLevelSprite);
                    }

                    var positionX = (OffsetX * currentColumn) + startPosition.x;
                    var positionY = -((OffsetY * currentRow) + startPosition.y);

                    var levelIndex = (currentRow * _gridColumns + currentColumn) + 1;

                    SetSpriteForCurrentAndUnlockedLevels(levelIndex, currentLevelObject);

                    currentLevelObject.transform.SetParent(GameObject.Find(LevelCanvasName).transform, false);
                    currentLevelObject.GetComponentInChildren<TextMesh>().text = levelIndex.ToString();
                    currentLevelObject.transform.position = new Vector3(positionX, positionY, startPosition.z);
                }
            }
        }

        /// <summary>
        ///     Change Level Sprite depending on level index.
        ///     If the level index is higher than the player's last completed level,
        ///     the sprite will change to locked level sprite.
        ///     If the level sprite is 1 value higher than the player's last completed level,
        ///     the sprite will change to current level sprite.
        /// </summary>
        /// <param name="levelIndex">Level index representing the currentLevelObject.</param>
        /// <param name="currentLevelObject">Level object being spawned in the grid. </param>
        private void SetSpriteForCurrentAndUnlockedLevels(int levelIndex, GameObject currentLevelObject)
        {
            if (levelIndex > _lastCompletedLevel)
            {
                currentLevelObject.GetComponent<SpriteRenderer>().sprite = LockedLevelSprite;
            }
            if (levelIndex == _lastCompletedLevel + 1)
            {
                currentLevelObject.GetComponent<SpriteRenderer>().sprite = NextAvailableLevelSprite;
            }
        }

        #endregion Grid
    }
}