using System.Globalization;

namespace Assets.Scripts.GameCategory.PatternMemoGame
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Levels;
    using UI;
    using UnityEngine;
    using Random = System.Random;

    public class PatternMemoSceneController : MonoBehaviour
    {
        private int _gridRows;
        private int _gridColumns;
        private int _numberOfCells;
        private int _score;
        private Material _initialMaterial;

        #region Constants

        private const float OffsetX = 3.5f;
        private const float OffsetY = 3.5f;
        private const string GameName = "PatternMemo";
        private const string PatternMemoSceneName = "PatternMemoScene";
        private const string PatternMemoContainer = "Container";
        private const int LastLevelWithCellsIncremented = 5;

        #endregion Constants

        private LevelManager _levelManager;
        private List<int> _selectedCellIds;

        #region SerializeFields

        [SerializeField] public OriginalCell OriginalCell;
        [SerializeField] public UIController UIController;
        [SerializeField] public TextMesh ScoreText;

        #endregion SerializeFields

        private Timer _timer;
        private List<OriginalCell> _selectedCells;
        private List<OriginalCell> _clickedCells;
        private Stopwatch _stopwatch;

        #region MonoBehaviour

        private void Start()
        {
            _initialMaterial = OriginalCell.Material;
            _stopwatch = new Stopwatch();
            _selectedCells = new List<OriginalCell>();
            _clickedCells = new List<OriginalCell>();
            _selectedCellIds = new List<int>();

            _levelManager = FindObjectOfType<LevelManager>();
            _timer = new Timer(_levelManager.GenerateTimerTargetForPatternMemo());

            OriginalCell.CellsHidden = false;

            SetNumberOfCellsAccordingToDifficulty();
            StartCoroutine(GenerateRandomSelectedCells());
        }

        private void Update()
        {
            _stopwatch.Start();
        }

        #endregion MonoBehaviour

        #region Setup

        /// <summary>
        ///     Get number of cells for the selected level and difficulty using the level manager logic
        ///     and create grid limits depending on the number of cells.
        /// </summary>
        private void SetNumberOfCellsAccordingToDifficulty()
        {
            _numberOfCells = _levelManager.GeneratePatternMemoNumberOfCells();
            SetGridLimits();
        }

        /// <summary>
        ///     Get number of rows and columns needed to construct the grid.
        /// </summary>
        private void SetGridLimits()
        {
            _gridColumns = (int)Math.Sqrt(_numberOfCells);
            _gridRows = (int)Math.Ceiling((double)_numberOfCells / _gridColumns);
        }

        /// <summary>
        ///     Generate a random list of cell ids that will be selected for the player to remember.
        ///     If the level the player is currently playing is igual or higher than 5 (LastLevelWithCellsIncremented)
        ///     the number of selected cells will be 3 plus the current level because from level 5 the number of cells dont increment.
        /// </summary>
        private IEnumerator GenerateRandomSelectedCells()
        {
            var level = _levelManager.GameLevel;
            var random = new Random();

            if (level >= LastLevelWithCellsIncremented)
            {
                level = level + 3;
            }

            for (var i = 0; i <= level; i++)
            {
                var selectedCellId = random.Next(0, _numberOfCells);
                while (_selectedCellIds.Contains(selectedCellId))
                {
                    selectedCellId = random.Next(0, _numberOfCells);
                }
                _selectedCellIds.Add(selectedCellId);
            }

            yield return StartCoroutine(CreateCellGrid());
        }

        #endregion Setup

        #region Grid

        /// <summary>
        ///     Create Card Grid from the grid limits stablished.
        /// </summary>
        private IEnumerator CreateCellGrid()
        {
            var startPosition = OriginalCell.transform.position;

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
                    OriginalCell cell;
                    if (currentColumn == 0 && currentRow == 0)
                    {
                        cell = OriginalCell;
                    }
                    else
                    {
                        cell = Instantiate(OriginalCell);
                    }

                    var cellId = currentRow * _gridColumns + currentColumn;

                    if (_selectedCellIds.Contains(cellId))
                    {
                        cell.ChangeCellMaterial(OriginalCell.ColoredMaterial);
                        UnityEngine.Debug.Log(cellId);
                        _selectedCells.Add(cell);
                    }
                    else
                    {
                        cell.ChangeCellMaterial(_initialMaterial);
                    }

                    var positionX = (OffsetX * currentColumn) + startPosition.x;
                    var positionY = (OffsetY * currentRow) + startPosition.y;
                    cell.transform.SetParent(GameObject.Find(PatternMemoContainer).transform, false);
                    cell.transform.position = new Vector3(positionX, positionY, startPosition.z);
                }
            }

            yield return new WaitForSeconds(_timer.TargetTime);
            HideSelectedCells();
        }

        /// <summary>
        ///     Hide selected cells changing its material to the initial material.
        /// </summary>
        private void HideSelectedCells()
        {
            UnityEngine.Debug.Log("q");
            foreach (var selectedCell in _selectedCells)
            {
                selectedCell.ChangeCellMaterial(_initialMaterial);
            }

            OriginalCell.CellsHidden = true;
        }

        #endregion Grid

        #region UserInput

        /// <summary>
        ///     This method gets called every time the user clicks on a cell.
        ///     If the number of clicked cells is equal to the number of selected cells, which
        ///     means that the player has clicked the amount of cells he was supposed to, check if
        ///     the clicked and selected cells are a match.
        ///
        ///     Otherwise, if the player has not finished clicking all the cells, add it to the clicked cells
        ///     list.
        /// </summary>
        /// <param name="selectedCell">Cell clicked by the player.</param>
        public void ExecuteClickCheck(OriginalCell selectedCell)
        {
            selectedCell.ChangeCellMaterial(OriginalCell.ColoredMaterial);

            if (_selectedCells.Contains(selectedCell))
            {
                _score++;
                ScoreText.text = "SCORE: " + _score;
            }
            if (!_clickedCells.Contains(selectedCell))
            {
                _clickedCells.Add(selectedCell);
            }
            if (_clickedCells.Count == _selectedCells.Count)
            {
                StartCoroutine(CheckClickedCellsMatch());
            }
        }

        /// <summary>
        ///     When the player has clicked the same amount of cells as he was shown, check if the clicked cells
        ///     match the selected cells(cells the user was shown in the beginning of the game).
        ///
        ///     If all the cards are a match, increase and update the high score and restart the game at a new level.
        ///     If the cards are not a match, redirect the user to the fail screen.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckClickedCellsMatch()
        {
            if (_clickedCells.All(_selectedCells.Contains))
            {
                yield return new WaitForSeconds(0.5f);

                var score = _clickedCells.Count;
                _stopwatch.Stop();
                _levelManager.UpdatePlayedTime(GameName, (float)_stopwatch.Elapsed.TotalSeconds);
                _levelManager.UpdateHighScore(GameName, score);
                _levelManager.RestartGame(GameName);
                SetUIControllerSettings();
                UIController.Completed();
            }
            else
            {
                yield return new WaitForSeconds(0.5f);

                _selectedCellIds.Clear();
                _clickedCells.Clear();
                _selectedCells.Clear();

                SetUIControllerSettings();
                UIController.Failed();
            }
        }

        #endregion UserInput

        private void SetUIControllerSettings()
        {
            UIController.SceneName = PatternMemoSceneName;
            UIController.GameName = GameName;
            UIController.TotalPlayedTime = (float)_stopwatch.Elapsed.TotalSeconds;
        }
    }
}