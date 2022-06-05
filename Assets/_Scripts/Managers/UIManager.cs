using UnityEngine;
using TMPro;

namespace Hexfall.Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI bombText;
        [SerializeField] private TextMeshProUGUI lostText;
        public int Score { get; private set; }

        public void UpdateScore()
        {
            if (GameManager.instance.CurrentState == GameState.GameAwaitingStart) return;
            Score += 5;
            scoreText.SetText("Score: " + Score);
        }
        public void ResetScore()
        {
            Score = 0;
            scoreText.SetText("Score: " + Score);
        }
        public void ActivateBomb(int counter)
        {
            bombText.gameObject.SetActive(true);
            bombText.SetText("BOMB IS ACTIVE\nRemaining Moves: " + counter);
        }
        public void DeactivateBomb()
        {
            bombText.gameObject.SetActive(false);
        }
        public void BombTick(int counter)
        {
            bombText.SetText("BOMB IS ACTIVE\nRemaining Moves: " + counter);

        }
        public void LoseGame()
        {
            lostText.gameObject.SetActive(true);
        }
    }
}
