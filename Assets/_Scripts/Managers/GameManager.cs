using Hexfall.HexElements;
using UnityEngine;

namespace Hexfall.Managers
{
    
    public class GameManager : Singleton<GameManager>
    {
        public GameState CurrentState { get; private set; }

        public HexGridLayout MainGrid { get; private set; }
        
        public Vector2Int ActiveHexCoordinate { get; private set; }
        public Vector2Int SelectDirection { get; private set; }
        public void SetActiveHex( Vector2Int activeHexCoordinate)
        {
            ActiveHexCoordinate = activeHexCoordinate;
        }
        public void SetSelectDirection(Vector2Int selectDirection)
        {
            SelectDirection = selectDirection;
        }

        public void AssignMainGrid( HexGridLayout gridToAssign)
        {
            MainGrid = gridToAssign;

        }

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
            //SelectHexagon();
            MainGrid.RotateSelection(clockwise);
        }



        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;
            switch (newState)
            {
                case GameState.BreakAndFill:

                    break;
                case GameState.CanInteract:

                    break;
                case GameState.Rotation:

                    break;
                case GameState.GameWon:

                    break;
                case GameState.GameLost:

                    break;
                default:
                    throw new System.ArgumentException("Invalid game state selection.");
            }
        }
    }
    public enum GameState
    {
        GamePreStart = 0,
        BreakAndFill = 1,
        CanInteract = 2,
        Rotation = 3,
        GameWon = 4,
        GameLost = 5,
    }

}
