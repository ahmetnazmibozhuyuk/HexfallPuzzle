using System.Collections.Generic;
using UnityEngine;
using Hexfall.Managers;
using DG.Tweening;

namespace Hexfall.HexElements
{
    // "odd q" Vertical Layout

    // creation, selection, remove vs şeklinde ufak classlara ayır
    public class HexGridLayout : MonoBehaviour
    {
        public Hexagon[,] HexArray { get; private set; }

        private const float SQRT_3 = 1.732050808f;

        private Vector2Int _gridSize;
        private GameObject _hexagon;

        private float _distanceMultiplier;
        private float _offscreenOffset;

        private Color[] _tileColor;


        private List<Hexagon> _selectedHexList = new List<Hexagon>();

        public void Initialize(Vector2Int gridSize, GameObject hexagonObject, float disanceBetweenHex, float offscreenOffset, Color[] tileColor)
        {
            _gridSize = gridSize;
            _hexagon = hexagonObject;
            _distanceMultiplier = disanceBetweenHex;
            _offscreenOffset = offscreenOffset;
            _tileColor = tileColor;
            CreateGrid();
            GameManager.instance.AssignMainGrid(this);

        }
        #region Grid Layout Creation - Clearing
        private void CreateGrid()
        {
            HexArray = new Hexagon[_gridSize.x, _gridSize.y];
            for (int y = 0; y < _gridSize.y; y++)
            {
                for (int x = 0; x < _gridSize.x; x++)
                {
                    CreateTile(x, y);
                }
            }
        }
        private void CreateTile(int coordinateX, int coordinateY)
        {
            //Hexagon newTile = Instantiate(_hexagon, HexPosition(coordinateX, coordinateY), transform.rotation, transform).GetComponent<Hexagon>();

            Hexagon newTile = ObjectPool.Spawn(_hexagon, HexPosition(coordinateX, coordinateY), transform.rotation).GetComponent<Hexagon>();

            newTile.Initialize(_tileColor[Random.Range(0, _tileColor.Length)], new Vector2Int(coordinateX, coordinateY),false);

            HexArray[coordinateX, coordinateY] = newTile;
            HexArray[coordinateX, coordinateY].SetNeighbors(_gridSize);
            newTile.transform.DOMoveY(HexPosition(coordinateX, coordinateY).y - _offscreenOffset * _distanceMultiplier, 0.2f).SetEase(Ease.OutCubic);
        }
        private Vector2 HexPosition(int x, int y)
        {
            if (x % 2 == 1)
            {
                return new Vector2(x * 3, y * 2 * SQRT_3 - SQRT_3 + _offscreenOffset) * _distanceMultiplier;
            }
            else
            {
                return new Vector2(x * 3, y * 2 * SQRT_3 + _offscreenOffset) * _distanceMultiplier;
            }
        }
        public void ClearEverything()
        {
            for (int i = 0; i < HexArray.GetLength(0); i++)
            {
                for (int j = 0; j < HexArray.GetLength(1); j++)
                {
                    if (HexArray[i, j] == null) continue;
                    //Destroy(HexArray[i, j].gameObject);
                    ObjectPool.Despawn(HexArray[i, j].gameObject);
                    HexArray[i, j] = null;
                }
            }
            Destroy(this);
        }
        #endregion

        #region Selection

        public void ShowNeighbors(Vector2Int coordinate, Vector2Int neighborIndex)
        {
            DeselectHexagon();

            Vector2Int[] neighborCoordinate = HexArray[coordinate.x, coordinate.y].NeighborCoordinate;
            SelectValidNeighbors(neighborIndex.x, neighborIndex.y, neighborCoordinate);
            _selectedHexList.Add(HexArray[coordinate.x, coordinate.y]);
            SelectHexagon();
        }
        private void SelectValidNeighbors(int startPoint, int endPoint, Vector2Int[] neighborCoordinate) // get neighboor values, rotate if not valid
        {
            if(NeighborIsValid(startPoint,neighborCoordinate) && NeighborIsValid(endPoint, neighborCoordinate))
            {
                _selectedHexList.Add(HexArray[neighborCoordinate[startPoint].x, neighborCoordinate[startPoint].y]);
                _selectedHexList.Add(HexArray[neighborCoordinate[endPoint].x, neighborCoordinate[endPoint].y]);
                return;
            }
            _selectedHexList.Clear();
            SelectValidNeighbors(NextNeighbor(startPoint), NextNeighbor(endPoint), neighborCoordinate);
        }
        private int NextNeighbor(int index)
        {
            index++;
            if (index > 5) return 0;
            return index;
        }
        private bool NeighborIsValid(int neighborIndex, Vector2Int[] neighborCoordinate)
        {
            return neighborCoordinate[neighborIndex].x >= 0 && neighborCoordinate[neighborIndex].y >= 0 &&
                neighborCoordinate[neighborIndex].x <= _gridSize.x - 1 && neighborCoordinate[neighborIndex].y <= _gridSize.y - 1;
        }
        private void DeselectHexagon()
        {
            for (int i = 0; i < _selectedHexList.Count; i++)
            {
                if (_selectedHexList[i] != null)
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
        #endregion

        public void RemoveHexagon(Vector2Int hexCoordinate)
        {
            ObjectPool.Despawn(HexArray[hexCoordinate.x, hexCoordinate.y].gameObject);
            for (int y = hexCoordinate.y + 1; y < HexArray.GetLength(1); y++)
            {
                Vector2Int newCoordinates = new Vector2Int(hexCoordinate.x, y - 1);

                HexArray[hexCoordinate.x, y].SetCoordinates(newCoordinates);
                HexArray[hexCoordinate.x, y].SetNeighbors(_gridSize);

                HexArray[newCoordinates.x, newCoordinates.y] = HexArray[hexCoordinate.x, y];
                HexArray[hexCoordinate.x, y].transform.DOMoveY(HexPosition(newCoordinates.x, newCoordinates.y).y - _offscreenOffset * _distanceMultiplier, 0.2f);
            }
            CreateTile(hexCoordinate.x, HexArray.GetLength(1) - 1);
        }
    }
}
