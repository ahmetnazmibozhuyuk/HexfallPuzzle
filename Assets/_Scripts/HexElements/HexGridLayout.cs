using System.Collections.Generic;
using UnityEngine;
using Hexfall.Managers;
using DG.Tweening;

namespace Hexfall.HexElements
{
    // "odd q" Vertical Layout

    // creation, selection, remove vs şeklinde ufak classlara ayır

    //eş bir renk bulduğunda bir eksiği ve bir fazlasını kontrol et

    //merkeze bir obje yerleştir select dediğinde aktive olsun merkez pozisyona geçsin deaktivede deaktive olsun
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

        private List<Hexagon> _hexagonToDestroy = new List<Hexagon>();

        private TrioSelection _trioSelection;



        #region Grid Layout Initialization - Creation - Clearing
        public void Initialize(Vector2Int gridSize, GameObject hexagonObject, float disanceBetweenHex, float offscreenOffset, Color[] tileColor)
        {
            _gridSize = gridSize;
            _hexagon = hexagonObject;
            _distanceMultiplier = disanceBetweenHex;
            _offscreenOffset = offscreenOffset;
            _tileColor = tileColor;
            CreateGrid();


            _trioSelection = new TrioSelection(HexArray, _gridSize);


            GameManager.instance.AssignMainGrid(this);
        }

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
            Hexagon newTile = ObjectPool.Spawn(_hexagon, HexPosition(coordinateX, coordinateY), transform.rotation).GetComponent<Hexagon>();

            newTile.Initialize(_tileColor[Random.Range(0, _tileColor.Length)], new Vector2Int(coordinateX, coordinateY), false);

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
            _trioSelection.DeselectHexagon();

            Vector2Int[] neighborCoordinate = HexArray[coordinate.x, coordinate.y].NeighborCoordinate;
            _trioSelection.SelectValidNeighbors(neighborIndex.x, neighborIndex.y, neighborCoordinate);
            _selectedHexList.Add(HexArray[coordinate.x, coordinate.y]);
            _trioSelection.SelectHexagon();
        }
        private Hexagon Neighbor(Vector2Int baseCoordinate, int neighborIndex)
        {
            return HexArray[HexArray[baseCoordinate.x, baseCoordinate.y].NeighborCoordinate[neighborIndex].x, HexArray[baseCoordinate.x, baseCoordinate.y].NeighborCoordinate[neighborIndex].y];
        }
        public void ShouldExplode(Vector2Int coordinate)
        {
            Color selectedColor = HexArray[coordinate.x, coordinate.y].HexColor;
            Vector2Int[] neighborCoordinate = HexArray[coordinate.x, coordinate.y].NeighborCoordinate;
            for (int i = 0; i < 6; i++)
            {
                if (!NeighborIsValid(i, neighborCoordinate)) continue;
                if (selectedColor == Neighbor(coordinate, i).HexColor)
                {
                    CheckNeighborConnection(coordinate, i, selectedColor);
                }
            }
        }
        private bool NeighborIsValid(int neighborIndex, Vector2Int[] neighborCoordinate)
        {
            return neighborCoordinate[neighborIndex].x >= 0 && neighborCoordinate[neighborIndex].y >= 0 &&
                neighborCoordinate[neighborIndex].x <= _gridSize.x - 1 && neighborCoordinate[neighborIndex].y <= _gridSize.y - 1;
        }


        private void CheckNeighborConnection(Vector2Int coordinate, int index, Color colorToMatch)
        {
            List<Hexagon> hexagonToExplode = new List<Hexagon>();
            hexagonToExplode.Add(HexArray[coordinate.x, coordinate.y]);
            int forwardIndex = index;
            for (int i = 0; i < 2; i++)
            {
                if (!NeighborIsValid(forwardIndex, HexArray[coordinate.x, coordinate.y].NeighborCoordinate)) break;
                if (Neighbor(coordinate, forwardIndex).HexColor == colorToMatch)
                {
                    if (hexagonToExplode.Contains(Neighbor(coordinate, forwardIndex)))
                    {
                        forwardIndex = _trioSelection.NextNeighborIndex(forwardIndex);
                        continue;
                    }
                    hexagonToExplode.Add(Neighbor(coordinate, forwardIndex));
                    forwardIndex = _trioSelection.NextNeighborIndex(forwardIndex);
                    continue;
                }
                break;
            }
            int backwardIndex = index;
            for (int i = 0; i < 3; i++)
            {
                if (!NeighborIsValid(backwardIndex, HexArray[coordinate.x, coordinate.y].NeighborCoordinate)) break;
                if (Neighbor(coordinate, backwardIndex).HexColor == colorToMatch)
                {
                    if (hexagonToExplode.Contains(Neighbor(coordinate, backwardIndex)))
                    {
                        backwardIndex = _trioSelection.PreviousNeighborIndex(backwardIndex);
                        continue;
                    }
                    hexagonToExplode.Add(Neighbor(coordinate, backwardIndex));
                    backwardIndex = _trioSelection.PreviousNeighborIndex(backwardIndex);
                    continue;
                }

                break;
            }
            if (hexagonToExplode.Count > 2)
            {
                for (int i = 0; i < hexagonToExplode.Count; i++)
                {
                    //RemoveHexagon(hexagonToExplode[i].Coordinate);
                    hexagonToExplode[i].SelectHexagon();
                }
            }
            else
            {
                for (int i = 0; i < hexagonToExplode.Count; i++)
                {
                    //RemoveHexagon(hexagonToExplode[i].Coordinate);
                    hexagonToExplode[i].DeselectHexagon();
                }
                hexagonToExplode.Clear();
            }
            for (int i = 0; i < hexagonToExplode.Count; i++)
            {
                Debug.Log(hexagonToExplode[i].Coordinate);
            }
            Debug.Log(hexagonToExplode.Count);
            for (int i = 0; i < hexagonToExplode.Count; i++)
            {
                RemoveHexagon(hexagonToExplode[i].Coordinate);
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
    public class TrioSelection
    {
        private List<Hexagon> _selectedHexList = new List<Hexagon>();

        private Hexagon[,] _hexArray;
        private Vector2Int _gridSize;
        public TrioSelection(Hexagon[,] hexArray, Vector2Int gridSize)
        {
            //burdan dependency inject
            _hexArray = hexArray;
            _gridSize = gridSize;
        }
        public void SelectValidNeighbors(int startPoint, int endPoint, Vector2Int[] neighborCoordinate) // get neighboor values, rotate if not valid
        {
            if (NeighborIsValid(startPoint, neighborCoordinate) && NeighborIsValid(endPoint, neighborCoordinate))
            {
                _selectedHexList.Add(_hexArray[neighborCoordinate[startPoint].x, neighborCoordinate[startPoint].y]);
                _selectedHexList.Add(_hexArray[neighborCoordinate[endPoint].x, neighborCoordinate[endPoint].y]);
                return;
            }
            _selectedHexList.Clear();
            SelectValidNeighbors(NextNeighborIndex(startPoint), NextNeighborIndex(endPoint), neighborCoordinate);
        }
        public int NextNeighborIndex(int index)
        {
            index++;
            if (index > 5) return 0;
            return index;
        }
        public int PreviousNeighborIndex(int index)
        {
            index--;
            if (index < 0) return 5;
            return index;
        }
        private bool NeighborIsValid(int neighborIndex, Vector2Int[] neighborCoordinate)
        {
            return neighborCoordinate[neighborIndex].x >= 0 && neighborCoordinate[neighborIndex].y >= 0 &&
                neighborCoordinate[neighborIndex].x <= _gridSize.x - 1 && neighborCoordinate[neighborIndex].y <= _gridSize.y - 1;
        }
        public void DeselectHexagon()
        {
            for (int i = 0; i < _selectedHexList.Count; i++)
            {
                if (_selectedHexList[i] != null)
                    _selectedHexList[i].DeselectHexagon();

            }
            _selectedHexList.Clear();
        }
        public void SelectHexagon()
        {
            for (int i = 0; i < _selectedHexList.Count; i++)
            {
                _selectedHexList[i].SelectHexagon();
            }
        }


    }
}
