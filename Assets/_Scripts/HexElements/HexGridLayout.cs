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

        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private GameObject hexagon;

        [SerializeField] private float distanceMultiplier;


        private List<Hexagon> _selectedHexList = new List<Hexagon>();

        private void Start()
        {
            LayoutGrid();
            GameManager.instance.AssignMainGrid(this);
        }
        public void Initialize()
        {
            //Spawn grid tarzı bir yerden initialize edilecek hale getir. Grid seçme vs düzgün ayarlansın
        }
        private void LayoutGrid()
        {
            HexArray = new Hexagon[gridSize.x, gridSize.y];
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    Hexagon newTile = Instantiate(hexagon, HexPosition(x, y) * distanceMultiplier, transform.rotation, transform).GetComponent<Hexagon>();
                    newTile.Initialize(Color.cyan, new Vector2Int(x, y));
                    HexArray[x, y] = newTile;
                    HexArray[x, y].SetNeighbors(gridSize);
                }
            }
        }
        public void ShowNeighbors(Vector2Int coordinate)
        {
            DeselectHexagon();

            Vector2Int[] neighborCoordinate = HexArray[coordinate.x, coordinate.y].NeighborCoordinate;
            int startPoint = 0, endPoint = 1; // todo :  bu kısım Vector2.Angle kısmından gelecek
            SelectValidNeighbors(startPoint, endPoint, neighborCoordinate);


            _selectedHexList.Add(HexArray[coordinate.x, coordinate.y]);

            SelectHexagon();
        }
        private void SelectValidNeighbors(int startPoint, int endPoint, Vector2Int[] neighborCoordinate)
        {
            if(neighborCoordinate[startPoint].x < 0 || neighborCoordinate[startPoint].y < 0 || neighborCoordinate[endPoint].x < 0 || neighborCoordinate[endPoint].y < 0)
            {
                _selectedHexList.Clear();
                Debug.Log("not valid coordinate" + startPoint + endPoint);
                startPoint++;
                if (startPoint > 5) startPoint = 0;
                endPoint++;
                if (endPoint > 5) endPoint = 0;
                SelectValidNeighbors(startPoint, endPoint, neighborCoordinate);
                return;
            }

            if (neighborCoordinate[startPoint].x > gridSize.x-1  || neighborCoordinate[startPoint].y > gridSize.y -1 ||
                neighborCoordinate[endPoint].x > gridSize.x - 1 || neighborCoordinate[endPoint].y > gridSize.y - 1)
            {
                _selectedHexList.Clear();
                Debug.Log("not valid coordinate" + startPoint + endPoint);
                startPoint++;
                if (startPoint > 5) startPoint = 0;
                endPoint++;
                if (endPoint > 5) endPoint = 0;
                SelectValidNeighbors(startPoint, endPoint, neighborCoordinate);
                return;
            }

                _selectedHexList.Add(HexArray[neighborCoordinate[startPoint].x, neighborCoordinate[startPoint].y]);
                Debug.Log(startPoint + " " + endPoint);
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
