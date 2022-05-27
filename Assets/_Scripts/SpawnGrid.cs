using UnityEngine;

namespace Hexfall.HexElements
{
    public class SpawnGrid : MonoBehaviour
    {
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private GameObject hexagonObject;
        [SerializeField] private float distanceBetweenHex;
        [SerializeField] private float offscreenOffset;

        [SerializeField] private Color[] tileColor;

        private HexGridLayout _hexGridMainLayout;


        public void SpawnNewGrid()
        {

            ClearGrid();
            _hexGridMainLayout = gameObject.AddComponent<HexGridLayout>();
            _hexGridMainLayout.Initialize(gridSize, hexagonObject, distanceBetweenHex,offscreenOffset, tileColor);
        }
        public void ClearGrid()
        {
            if (_hexGridMainLayout != null) _hexGridMainLayout.ClearEverything();
        }


    }
}
