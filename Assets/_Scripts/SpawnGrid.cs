using UnityEngine;
using Hexfall.Managers;

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
            GameManager.instance.ChangeState(GameState.GameAwaitingStart);
            ClearGrid();
            _hexGridMainLayout = gameObject.AddComponent<HexGridLayout>();
            _hexGridMainLayout.Initialize(gridSize, hexagonObject, distanceBetweenHex,offscreenOffset, tileColor);
        }
        private void ClearGrid()
        {
            if (_hexGridMainLayout != null) _hexGridMainLayout.ClearEverything();
        }
    }
}
