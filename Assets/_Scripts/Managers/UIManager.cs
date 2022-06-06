using UnityEngine;
using TMPro;

namespace Hexfall.Managers
{
    public class UIManager : MonoBehaviour
    {
        public int Score { get; private set; }

        [SerializeField] private int singleExplosionScore = 5;

        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI bombText;
        [SerializeField] private GameObject lostText;
        public void UpdateScore()
        {
            if (GameManager.instance.CurrentState == GameState.GameAwaitingStart) return;
            Score += singleExplosionScore;
            scoreText.SetText("Score: " + Score);
        }
        private void ResetScore()
        {
            Score = 0;
            scoreText.SetText("Score: " + Score);
        }
        public void InitializeGame()
        {
            ResetScore();
            lostText.SetActive(false);
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
            lostText.SetActive(true);
        }
    }
}
