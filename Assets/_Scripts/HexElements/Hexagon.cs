using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexfall.HexElements
{
    public class Hexagon : MonoBehaviour
    {
        // komşuları almak yerine komşuları ver mesela bu düştüğünde sol altındakinin sağ üst komşusunu kendisi yapsın
        public Color HexColor { get; private set; } // random seçilecek
        public Vector2 Coordinate { get; private set; }
        private GameObject _hexObject;

        private void Awake()
        {
            _hexObject = GetComponent<GameObject>();
            HexColor = _hexObject.GetComponent<SpriteRenderer>().color;
        }
        public void Initialize(Color hexColor, Vector2 coordinate) // color yerine tile type al
        {
            HexColor = hexColor;
            SetCoordinates(coordinate);
        }
        public void SetCoordinates(Vector2 coordinate)
        {
            Coordinate = coordinate;
        }
    }
}
