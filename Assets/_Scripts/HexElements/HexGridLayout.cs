using System.Collections.Generic;
using UnityEngine;
using Hexfall.Managers;

namespace Hexfall.HexElements
{
    //odd q vertical layout
    public class HexGridLayout : MonoBehaviour
    {
        //not: eğer x %2 = 1 ise y'si altta demektir koordinatta x 1 arttığında sağ üste erişir.
        //collider'la almayı düşünebilirsin

        public Hexagon[,] HexArray { get; private set; }

        private const float SQRT_3 = 1.732050808f;

        private Vector2Int _gridSize;
        private GameObject _hexagon;

        private float _distanceMultiplier;

        private Color[] _tileColor;


        private List<Hexagon> _selectedHexList = new List<Hexagon>();

        public void Initialize(Vector2Int gridSize, GameObject hexagonObject, float disanceBetweenHex, Color[] tileColor)
        {
            _gridSize = gridSize;
            _hexagon = hexagonObject;
            _distanceMultiplier = disanceBetweenHex;
            _tileColor = tileColor;
            LayoutGrid();
            GameManager.instance.AssignMainGrid(this);

        }
        private void LayoutGrid()
        {
            HexArray = new Hexagon[_gridSize.x, _gridSize.y];
            for (int y = 0; y < _gridSize.y; y++)
            {
                for (int x = 0; x < _gridSize.x; x++)
                {
                    Hexagon newTile = Instantiate(_hexagon, HexPosition(x, y) * _distanceMultiplier, transform.rotation, transform).GetComponent<Hexagon>();
                    newTile.Initialize(_tileColor[Random.Range(0,_tileColor.Length)], new Vector2Int(x, y));
                    HexArray[x, y] = newTile;
                    HexArray[x, y].SetNeighbors(_gridSize);
                }
            }
        }
        public void ShowNeighbors(Vector2Int coordinate, Vector2Int neighborIndex)
        {
            DeselectHexagon();

            Vector2Int[] neighborCoordinate = HexArray[coordinate.x, coordinate.y].NeighborCoordinate;
            //int startPoint = 0, endPoint = 1; // todo :  bu kısım Vector2.Angle kısmından gelecek
            SelectValidNeighbors(neighborIndex.x, neighborIndex.y, neighborCoordinate);


            _selectedHexList.Add(HexArray[coordinate.x, coordinate.y]);

            SelectHexagon();
        }
        private void SelectValidNeighbors(int startPoint, int endPoint, Vector2Int[] neighborCoordinate) // get neighboor values, rotate if not valid
        {
            if (neighborCoordinate[startPoint].x < 0 || neighborCoordinate[startPoint].y < 0 || neighborCoordinate[endPoint].x < 0 || neighborCoordinate[endPoint].y < 0)
            {
                _selectedHexList.Clear();
                //Debug.Log("not valid coordinate" + startPoint + endPoint);
                startPoint++;
                if (startPoint > 5) startPoint = 0;
                endPoint++;
                if (endPoint > 5) endPoint = 0;
                SelectValidNeighbors(startPoint, endPoint, neighborCoordinate);
                return;
            }

            if (neighborCoordinate[startPoint].x > _gridSize.x - 1 || neighborCoordinate[startPoint].y > _gridSize.y - 1 ||
                neighborCoordinate[endPoint].x > _gridSize.x - 1 || neighborCoordinate[endPoint].y > _gridSize.y - 1)
            {
                _selectedHexList.Clear();
                //Debug.Log("not valid coordinate" + startPoint + endPoint);
                startPoint++;
                if (startPoint > 5) startPoint = 0;
                endPoint++;
                if (endPoint > 5) endPoint = 0;
                SelectValidNeighbors(startPoint, endPoint, neighborCoordinate);
                return;
            }

            _selectedHexList.Add(HexArray[neighborCoordinate[startPoint].x, neighborCoordinate[startPoint].y]);
            _selectedHexList.Add(HexArray[neighborCoordinate[endPoint].x, neighborCoordinate[endPoint].y]);
        }
        private void DeselectHexagon()
        {
            for (int i = 0; i < _selectedHexList.Count; i++)
            {
                _selectedHexList[i].DeselectHexagon();

            }
            _selectedHexList.Clear();
        }
        private void SelectHexagon()
        {
            for (int i = 0; i < _selectedHexList.Count; i++)
            {
                _selectedHexList[i].SelectHexagon();

            }
        }
        public void ClearEverything()
        {
            for(int i = 0; i < HexArray.GetLength(0); i++)
            {
                for(int j = 0; j < HexArray.GetLength(1); j++)
                {
                    Destroy(HexArray[i, j].gameObject);
                    HexArray[i, j] = null;
                }
            }
            Destroy(this);
        }

        private Vector2 HexPosition(float x, float y)
        {
            if (x % 2 == 1)
            {
                return new Vector2(x * 3, y * 2 * SQRT_3 - SQRT_3);
            }
            else
            {
                return new Vector2(x * 3, y * 2 * SQRT_3);
            }
        }
    }
    public enum TileType
    {

    }
}
