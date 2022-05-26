using UnityEngine;
using Hexfall.Managers;

namespace Hexfall.HexElements
{
    public class Hexagon : MonoBehaviour
    {
        // komşuları almak yerine komşuları ver mesela bu düştüğünde sol altındakinin sağ üst komşusunu kendisi yapsın
        public Color HexColor { get; private set; } // random seçilecek
        public Vector2Int Coordinate { get; private set; }
        public Vector2Int[] NeighborCoordinate { get; private set; }

        public void Initialize(Color hexColor, Vector2Int coordinate) // color yerine tile type al
        {

            SetCoordinates(coordinate);
            SetColor(hexColor);
            NeighborCoordinate = new Vector2Int[6];
        }

        public void SetCoordinates(Vector2Int coordinate)
        {
            Coordinate = coordinate;
        }

        private void SetColor(Color hexColor)
        {
            gameObject.GetComponent<SpriteRenderer>().color = hexColor;
            HexColor = hexColor;
        }
        public void SetNeighbors(Vector2Int gridSize)
        {
            //if()
            //gridSize ile kenarda mı değil mi vs kontrol et
            if(Coordinate.x % 2 == 0)
            {
                NeighborCoordinate[0] = new Vector2Int(Coordinate.x, Coordinate.y + 1);     //Top
                NeighborCoordinate[1] = new Vector2Int(Coordinate.x + 1, Coordinate.y + 1);     //Top right
                NeighborCoordinate[2] = new Vector2Int(Coordinate.x + 1, Coordinate.y);        //Bot Right
                NeighborCoordinate[3] = new Vector2Int(Coordinate.x, Coordinate.y - 1);         //Bot
                NeighborCoordinate[4] = new Vector2Int(Coordinate.x - 1, Coordinate.y );     //Bot left
                NeighborCoordinate[5] = new Vector2Int(Coordinate.x - 1, Coordinate.y+1);        //Top Left
            }
            else
            {
                NeighborCoordinate[0] = new Vector2Int(Coordinate.x, Coordinate.y + 1);     //Top
                NeighborCoordinate[1] = new Vector2Int(Coordinate.x + 1, Coordinate.y);     //Top right
                NeighborCoordinate[2] = new Vector2Int(Coordinate.x + 1, Coordinate.y - 1);        //Bot Right
                NeighborCoordinate[3] = new Vector2Int(Coordinate.x, Coordinate.y - 1);         //Bot
                NeighborCoordinate[4] = new Vector2Int(Coordinate.x - 1, Coordinate.y - 1);     //Bot left
                NeighborCoordinate[5] = new Vector2Int(Coordinate.x - 1, Coordinate.y);        //Top Left
            }

        }
        private float SetCoordinate(int maxGridSize)
        {
            return 0; // coordiante.x yerine bu methoddan çağır sınır dışındaysa -1 olarak dönsün.
        }
        public void SelectHexagon()
        {
            SetColor(Color.red);
        }

        private void OnMouseDown()
        {
            //Debug.Log(Coordinate+" GameManager'dan Coordinate girerek çağır seçme fonksiyonunu"); // Button kullanılabilir mi?
            GameManager.instance.SelectHexagon(Coordinate);

        }
    }
}
