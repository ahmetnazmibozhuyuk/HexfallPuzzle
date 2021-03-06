using System.Collections.Generic;
using UnityEngine;
using Hexfall.Managers;
using DG.Tweening;

namespace Hexfall.HexElements
{
    // "odd q" Vertical Layout
    public class HexGridLayout : MonoBehaviour
    {
        public Hexagon[,] HexArray { get; private set; }
        public int ScoreTresholdForBomb { get; private set; }

        private const float SQRT_3 = 1.732050808f;

        private Vector2Int _gridSize;
        private GameObject _hexagon;

        private float _distanceMultiplier;
        private float _offscreenOffset;

        private Color[] _tileColor;


        private List<Hexagon> _selectedHexList = new List<Hexagon>();

        private List<Hexagon> _hexagonToDestroy = new List<Hexagon>();


        private readonly float _movePositionDuration = 0.25f;
        private readonly float _explodeDelay = 0.4f;

        private int _rotationCounter = 0;
        private bool _clockwise;

        public bool ShouldSpawnBomb = false;

        private bool _moveIsMade = false;


        #region Grid Layout Initialization - Creation - Clearing
        public void Initialize(Vector2Int gridSize, GameObject hexagonObject, float disanceBetweenHex, float offscreenOffset, Color[] tileColor, int scoreTresholdForBomb)
        {
            _gridSize = gridSize;
            _hexagon = hexagonObject;
            _distanceMultiplier = disanceBetweenHex;
            _offscreenOffset = offscreenOffset;
            _tileColor = tileColor;
            ScoreTresholdForBomb = scoreTresholdForBomb;
            CreateGrid();
            GameManager.instance.AssignMainGrid(this);

            Invoke(nameof(ExplodeMatchingHexagons), _explodeDelay);
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
            Hexagon newTile = ObjectPool.Spawn(_hexagon, HexInitialPosition(coordinateX, coordinateY), transform.rotation).GetComponent<Hexagon>();

            newTile.Initialize(_tileColor[Random.Range(0, _tileColor.Length)], new Vector2Int(coordinateX, coordinateY));
            HexArray[coordinateX, coordinateY] = newTile;
            newTile.transform.DOMoveY(HexInitialPosition(coordinateX, coordinateY).y - _offscreenOffset * _distanceMultiplier, _movePositionDuration).SetEase(Ease.OutCubic);

            if (ShouldSpawnBomb)
            {
                if (GameManager.instance.BombHexagon != null)
                {
                    ShouldSpawnBomb = false;
                }
                else
                {
                    newTile.SetAsBombHexagon();
                    ShouldSpawnBomb = false;
                }
            }
        }
        private Vector2 HexInitialPosition(int x, int y)
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
        private Hexagon Neighbor(Vector2Int baseCoordinate, int neighborIndex)
        {
            return HexArray[HexArray[baseCoordinate.x, baseCoordinate.y].NeighborCoordinate[neighborIndex].x, HexArray[baseCoordinate.x, baseCoordinate.y].NeighborCoordinate[neighborIndex].y];
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
            for (int i = 0; i < 3; i++)
            {
                if (!NeighborIsValid(forwardIndex, HexArray[coordinate.x, coordinate.y].NeighborCoordinate)) break;
                if (Neighbor(coordinate, forwardIndex).HexColor == colorToMatch)
                {
                    if (hexagonToExplode.Contains(Neighbor(coordinate, forwardIndex)))
                    {
                        forwardIndex = NextNeighborIndex(forwardIndex);
                        continue;
                    }
                    hexagonToExplode.Add(Neighbor(coordinate, forwardIndex));
                    forwardIndex = NextNeighborIndex(forwardIndex);
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
                        backwardIndex = PreviousNeighborIndex(backwardIndex);
                        continue;
                    }
                    hexagonToExplode.Add(Neighbor(coordinate, backwardIndex));
                    backwardIndex = PreviousNeighborIndex(backwardIndex);
                    continue;
                }

                break;
            }
            if (hexagonToExplode.Count <= 2)
            {
                hexagonToExplode.Clear();
            }
            for (int i = 0; i < hexagonToExplode.Count; i++)
            {
                if (!_hexagonToDestroy.Contains(hexagonToExplode[i]))
                    _hexagonToDestroy.Add(hexagonToExplode[i]);
            }
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
        public void SelectValidNeighbors(int startPoint, int endPoint, Vector2Int[] neighborCoordinate)
        {
            if (NeighborIsValid(startPoint, neighborCoordinate) && NeighborIsValid(endPoint, neighborCoordinate))
            {
                _selectedHexList.Add(HexArray[neighborCoordinate[startPoint].x, neighborCoordinate[startPoint].y]);
                _selectedHexList.Add(HexArray[neighborCoordinate[endPoint].x, neighborCoordinate[endPoint].y]);
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
        #endregion

        #region Rotation
        public void ResetRotationCounter()
        {
            _rotationCounter = 2;
        }
        public void RotateSelection(bool clockwise)
        {
            if (_selectedHexList.Count <= 0) return;
            GameManager.instance.ChangeState(GameState.Rotation);
            Vector2Int[] previousCoordinate = new Vector2Int[3] { _selectedHexList[0].Coordinate, _selectedHexList[1].Coordinate, _selectedHexList[2].Coordinate };
            Hexagon[] previousHexagon = new Hexagon[3] { _selectedHexList[0], _selectedHexList[1], _selectedHexList[2] };

            _clockwise = clockwise;
            if (clockwise)
            {
                _selectedHexList[0].transform.DOMove(HexPosition(previousCoordinate[1].x, previousCoordinate[1].y), _movePositionDuration);
                _selectedHexList[1].transform.DOMove(HexPosition(previousCoordinate[2].x, previousCoordinate[2].y), _movePositionDuration);
                _selectedHexList[2].transform.DOMove(HexPosition(previousCoordinate[0].x, previousCoordinate[0].y), _movePositionDuration);


                HexArray[_selectedHexList[1].Coordinate.x, _selectedHexList[1].Coordinate.y] = previousHexagon[0];
                HexArray[_selectedHexList[2].Coordinate.x, _selectedHexList[2].Coordinate.y] = previousHexagon[1];
                HexArray[_selectedHexList[0].Coordinate.x, _selectedHexList[0].Coordinate.y] = previousHexagon[2];

                _selectedHexList[0].SetCoordinates(previousCoordinate[1]);
                _selectedHexList[1].SetCoordinates(previousCoordinate[2]);
                _selectedHexList[2].SetCoordinates(previousCoordinate[0]);
            }
            else
            {
                _selectedHexList[0].gameObject.transform.DOMove(HexPosition(previousCoordinate[2].x, previousCoordinate[2].y), _movePositionDuration);
                _selectedHexList[1].transform.DOMove(HexPosition(previousCoordinate[0].x, previousCoordinate[0].y), _movePositionDuration);
                _selectedHexList[2].transform.DOMove(HexPosition(previousCoordinate[1].x, previousCoordinate[1].y), _movePositionDuration);

                HexArray[_selectedHexList[2].Coordinate.x, _selectedHexList[2].Coordinate.y] = previousHexagon[0];
                HexArray[_selectedHexList[0].Coordinate.x, _selectedHexList[0].Coordinate.y] = previousHexagon[1];
                HexArray[_selectedHexList[1].Coordinate.x, _selectedHexList[1].Coordinate.y] = previousHexagon[2];

                _selectedHexList[0].SetCoordinates(previousCoordinate[2]);
                _selectedHexList[1].SetCoordinates(previousCoordinate[0]);
                _selectedHexList[2].SetCoordinates(previousCoordinate[1]);
            }
            AssignAllNeighbors();
            Invoke(nameof(ExplodeMatchingHexagons), _movePositionDuration + 0.1f);
        }
        private void AssignAllNeighbors()
        {
            for (int i = 0; i < HexArray.GetLength(0); i++)
            {
                for (int j = 0; j < HexArray.GetLength(1); j++)
                {
                    HexArray[i, j].SetNeighbors();
                }
            }
        }
        private Vector2 HexPosition(int x, int y)
        {
            if (x % 2 == 1)
            {
                return new Vector2(x * 3, y * 2 * SQRT_3 - SQRT_3) * _distanceMultiplier;
            }
            else
            {
                return new Vector2(x * 3, y * 2 * SQRT_3) * _distanceMultiplier;
            }
        }
        #endregion

        #region Explosion
        public void ExplodeMatchingHexagons()
        {
            for (int i = 0; i < HexArray.GetLength(0); i++)
            {
                for (int j = 0; j < HexArray.GetLength(1); j++)
                {
                    ShouldExplode(HexArray[i, j].Coordinate);
                }
            }
            if (_hexagonToDestroy.Count > 2)
            {
                for (int i = 0; i < _hexagonToDestroy.Count; i++)
                {
                    RemoveHexagon(_hexagonToDestroy[i].Coordinate);
                    GameManager.instance.UpdateScore();
                }
                _hexagonToDestroy.Clear();
                _rotationCounter = 0;
                DeselectHexagon();
                Invoke(nameof(ExplodeMatchingHexagons), _explodeDelay);
                _moveIsMade = true;

            }
            else
            {
                _hexagonToDestroy.Clear();
                if (_rotationCounter > 0)
                {
                    RotateSelection(_clockwise);
                    _rotationCounter--;
                }
                else
                {
                    DeselectHexagon();
                    GameManager.instance.ChangeState(GameState.CanInteract);
                    if (_moveIsMade)
                    {
                        GameManager.instance.ExplosionTick();
                        _moveIsMade = false;
                    }
                }
            }
        }
        public void RemoveHexagon(Vector2Int hexCoordinate)
        {
            ObjectPool.Despawn(HexArray[hexCoordinate.x, hexCoordinate.y].gameObject);
            for (int y = hexCoordinate.y + 1; y < HexArray.GetLength(1); y++)
            {
                Vector2Int newCoordinates = new Vector2Int(hexCoordinate.x, y - 1);

                HexArray[hexCoordinate.x, y].SetCoordinates(newCoordinates);

                HexArray[newCoordinates.x, newCoordinates.y] = HexArray[hexCoordinate.x, y];
                HexArray[hexCoordinate.x, y].transform.DOMoveY(HexInitialPosition(newCoordinates.x, newCoordinates.y).y - _offscreenOffset * _distanceMultiplier, _movePositionDuration);
            }
            CreateTile(hexCoordinate.x, HexArray.GetLength(1) - 1);
        }
        private void ShouldExplode(Vector2Int coordinate)
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
        #endregion
    }
}
