using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PocketDungeons.Gameplay;

namespace PocketDungeons.UI.Views
{
    /// <summary>
    /// Results screen shown after death or run completion.
    /// Displays stats, score, gold earned, and quick-restart button.
    /// </summary>
    public class ResultsScreenView : MonoBehaviour
    {
        [Header("Title")]
        [SerializeField] private TextMeshProUGUI _titleText;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI _floorsText;
        [SerializeField] private TextMeshProUGUI _enemiesText;
        [SerializeField] private TextMeshProUGUI _goldText;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Header("Buttons")]
        [SerializeField] private Button _playAgainButton;
        [SerializeField] private Button _mainMenuButton;

        public System.Action OnPlayAgain;
        public System.Action OnMainMenu;

        private void Awake()
        {
            _playAgainButton?.onClick.AddListener(() => OnPlayAgain?.Invoke());
            _mainMenuButton?.onClick.AddListener(() => OnMainMenu?.Invoke());
            gameObject.SetActive(false);
        }

        public void Show(RunData data)
        {
            gameObject.SetActive(true);

            if (_titleText != null)
                _titleText.text = data.DefeatedBoss ? "VICTORY!" : "DEFEATED";

            if (_floorsText != null)
                _floorsText.text = $"Floors: {data.FloorsCleared}";

            if (_enemiesText != null)
                _enemiesText.text = $"Enemies: {data.EnemiesKilled}";

            if (_goldText != null)
                _goldText.text = $"Gold: {data.GoldCollected}";

            if (_timeText != null)
            {
                int minutes = (int)(data.TotalTimeSeconds / 60);
                int seconds = (int)(data.TotalTimeSeconds % 60);
                _timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }

            if (_scoreText != null)
                _scoreText.text = $"Score: {data.ScoreTotal}";
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
