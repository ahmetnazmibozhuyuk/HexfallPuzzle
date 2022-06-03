using UnityEngine;
using TMPro;

namespace Hexfall.Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        private int _score;
        public void UpdateScore()
        {
            if (GameManager.instance.CurrentState == GameState.GameAwaitingStart) return;
            _score += 5;
            scoreText.SetText("Score: " + _score);
        }
        public void ResetScore()
        {
            _score = 0;
            scoreText.SetText("Score: " + _score);
        }
    }
}
