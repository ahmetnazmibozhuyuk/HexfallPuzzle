using Hexfall.HexElements;
using UnityEngine;

namespace Hexfall.Managers
{
    [RequireComponent(typeof(UIManager))]
    public class GameManager : Singleton<GameManager>
    {
        public GameState CurrentState { get; private set; }

        public HexGridLayout MainGrid { get; private set; }
        
        public Vector2Int ActiveHexCoordinate { get; private set; }
        public Vector2Int SelectDirection { get; private set; }
        public Hexagon BombHexagon { get; private set; }

        private UIManager _uiManager;

        private int _explosionCounter = 5;

        private bool _bombIsActive = false;
        protected override void Awake()
        {
            base.Awake();
            _uiManager = GetComponent<UIManager>();
        }
        private void Start()
        {
            ChangeState(GameState.GameAwaitingStart);
        }
        public void AssignMainGrid(HexGridLayout gridToAssign)
        {
            MainGrid = gridToAssign;
        }
        #region Game State Related
        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;
            switch (newState)
            {
                case GameState.GameAwaitingStart:
                    _uiManager.InitializeGame();
                    break;
                case GameState.CanInteract:

                    break;
                case GameState.Rotation:

                    break;
                case GameState.GameWon:

                    break;
                case GameState.GameLost:
                    GameLost();
                    break;
                default:
                    throw new System.ArgumentException("Invalid game state selection.");
            }
        }
        private void GameLost()
        {
            _uiManager.LoseGame();
        }
        #endregion

        #region Selection Related
        public void SetActiveHex( Vector2Int activeHexCoordinate)
        {
            ActiveHexCoordinate = activeHexCoordinate;
        }
        public void SetSelectDirection(Vector2Int selectDirection)
        {
            SelectDirection = selectDirection;
        }
        #endregion

        #region Input Related
        private void SelectHexagon()
        {
            MainGrid.ShowNeighbors(ActiveHexCoordinate, SelectDirection);
        }
        public void PressAction()
        {
            SelectHexagon();
        }
        public void SwipeAction(bool clockwise)
        {
            MainGrid.ResetRotationCounter();
            MainGrid.RotateSelection(clockwise);
        }

        #endregion

        #region Explosion Related
        public void CancelExplosion()
        {
            _bombIsActive = false;
            _uiManager.DeactivateBomb();
        }
        public void StartExplosionClock(int explosionCounter)
        {
            _explosionCounter = explosionCounter;
            _bombIsActive = true;
            _uiManager.ActivateBomb(explosionCounter);
        }
        public void SetBombHexagon(Hexagon hexagon)
        {
            BombHexagon = hexagon;
        }
        public void RemoveBombHexagon()
        {
            BombHexagon = null;
        }
        public void ExplosionTick()
        {
            if (!_bombIsActive) return;
            _explosionCounter--;
            _uiManager.BombTick(_explosionCounter);
            if (_explosionCounter < 1)
                ChangeState(GameState.GameLost);
        }
        #endregion

        public void UpdateScore()
        {
            _uiManager.UpdateScore();
            if (_uiManager.Score % MainGrid.ScoreTresholdForBomb == 0 && _uiManager.Score != 0)
            {
                MainGrid.ShouldSpawnBomb = true;
            }
        }

    }
    public enum GameState
    {
        GamePreStart = 0,
        GameAwaitingStart = 1,
        CanInteract = 2,
        Rotation = 3,
        GameWon = 4,
        GameLost = 5,
    }

}
