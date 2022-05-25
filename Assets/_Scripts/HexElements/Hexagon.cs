using UnityEngine;

namespace Hexfall.HexElements
{
    public class Hexagon : MonoBehaviour
    {
        // komşuları almak yerine komşuları ver mesela bu düştüğünde sol altındakinin sağ üst komşusunu kendisi yapsın
        public Color HexColor { get; private set; } // random seçilecek
        public Vector2 Coordinate { get; private set; }

        public void Initialize(Color hexColor, Vector2 coordinate) // color yerine tile type al
        {
            SetCoordinates(coordinate);
            SetColor(hexColor);
        }

        public void SetCoordinates(Vector2 coordinate)
        {
            Coordinate = coordinate;
        }
        private void SetColor(Color hexColor)
        {
            gameObject.GetComponent<SpriteRenderer>().color = hexColor;
            HexColor = hexColor;
        }
    }
}
