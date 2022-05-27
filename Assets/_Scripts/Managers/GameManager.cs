using Hexfall.HexElements;
using UnityEngine;

namespace Hexfall.Managers
{
    public class GameManager : Singleton<GameManager>
    {

        //oyun stateleri: başlama öncesi, taşların oturması, rotasyon ve kontrol



        public HexGridLayout MainGrid { get; private set; }
        

        public void AssignMainGrid( HexGridLayout gridToAssign)
        {
            MainGrid = gridToAssign;

        }

        public void SelectHexagon(Vector2Int hexCoordinates, Vector2Int neighborIndex)
        {
            MainGrid.ShowNeighbors(hexCoordinates, neighborIndex);

        }
    }

}
