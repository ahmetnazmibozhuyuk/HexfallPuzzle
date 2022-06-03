using UnityEngine;
using Hexfall.Managers;

namespace Hexfall.Control
{
    public class Controller : MonoBehaviour
    {
        private Vector2 _pressPosition;
        private Vector2 _releasePosition;

        private readonly float _maxPositionDifference = 30;

        private void Update()
        {
            if (GameManager.instance.CurrentState != GameState.CanInteract) return;
            SelectHex();
        }
        private void SelectHex()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _pressPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _releasePosition = Input.mousePosition;
                SwipeOrPress();
            }


        }
        private void SwipeOrPress()
        {
            if (Vector2.Distance(_releasePosition, _pressPosition) > _maxPositionDifference)
            {
                GameManager.instance.SwipeAction( _pressPosition.y > _releasePosition.y);
            }
            else
            {
                GameManager.instance.PressAction();
            }
        }
    }
}
