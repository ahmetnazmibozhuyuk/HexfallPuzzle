using UnityEngine;
using Hexfall.Managers;

namespace Hexfall.HexElements
{
    public class Hexagon : MonoBehaviour
    {
        public Color HexColor { get; private set; } // random seçilecek
        public Vector2Int Coordinate { get; private set; }
        public Vector2Int[] NeighborCoordinate { get; private set; }

        [SerializeField] private GameObject highLightSprite;

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
        public void SelectHexagon()
        {
            highLightSprite.SetActive(true);
        }
        public void DeselectHexagon()
        {
            highLightSprite.SetActive(false);
        }

        private void OnMouseDown() // nasılsa ray ile yapıyosun inputtan düzgün hallet

        {
            GameManager.instance.SelectHexagon(Coordinate,GetSelectDirection(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }

        private Vector2Int GetSelectDirection(Vector2 startPoint, Vector2 endPoint)
        {
            float angle = Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y,
                Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x) * 180 / Mathf.PI;
            if (angle < -120)
            {
                return new Vector2Int(4, 5);
            }
            else if(angle >= -120 && angle < -60)
            {
                return new Vector2Int(3, 4);
            }
            else if(angle >= -60 && angle < 0)
            {
                return new Vector2Int(2, 3);
            }
            else if(angle>= 0  && angle < 60)
            {
                return new Vector2Int(1, 2);
            }
            else if(angle>=60 && angle < 120)
            {
                return new Vector2Int(0, 1);
            }
            else
            {
                return new Vector2Int(5, 0);
            }

        }
    }
}
