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
            for(int y = 0; y <  gridSize.y; y++)
            {
                for(int x = 0; x < gridSize.x; x++)
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
            for(int i = 0; i < HexArray[coordinate.x, coordinate.y].NeighborCoordinate.Length; i++)
            {
               //Debug.Log( HexArray[coordinate.x, coordinate.y].NeighborCoordinate[i]);
                HexArray[HexArray[coordinate.x, coordinate.y].NeighborCoordinate[i].x, HexArray[coordinate.x, coordinate.y].NeighborCoordinate[i].y].SelectHexagon();
            }

        }

        // todo : seçim için alınacak objeleri bir listeye doldur sonra o listedekileri aktive et. yeni seçimde önceki listeyi deaktive edip boşalt
        //sonra tekrar doldur.

       private Vector2 HexPosition(float x, float y)
        {
            if(x %2 == 1)
            {
                return new Vector2(x*3,y*2* SQRT_3 - SQRT_3);
            }
            else
            {
                return new Vector2(x*3, y*2 * SQRT_3) ;
            }

        }
    }
    public enum TileType
    {

    }
}
