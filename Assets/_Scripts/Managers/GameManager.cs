using Hexfall.HexElements;
using UnityEngine;

namespace Hexfall.Managers
{
    public class GameManager : Singleton<GameManager>
    {

        public HexGridLayout MainGrid { get; private set; }
        

        public void AssignMainGrid( HexGridLayout gridToAssign)
        {
            MainGrid = gridToAssign;

        }

        public void SelectHexagon(Vector2Int hexCoordinates)
        {
            MainGrid.ShowNeighbors(hexCoordinates);

        }
    }

}
