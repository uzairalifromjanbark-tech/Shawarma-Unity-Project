using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ShawarmaLegend
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Button startDayButton;
        [SerializeField] private Button endDayButton;
        [SerializeField] private GameObject dayEndPanel;
        [SerializeField] private Button nextDayButton;

        private void Awake()
        {
            if (startDayButton != null) startDayButton.onClick.AddListener(OnStartDay);
            if (endDayButton != null) endDayButton.onClick.AddListener(OnEndDay);
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;
            if (dayText != null) dayText.text = $"Day {GameManager.Instance.CurrentDay}";
            if (coinsText != null) coinsText.text = $"$ {GameManager.Instance.Coins}";
            if (timerText != null)
            {
                var t = GameManager.Instance.RemainingDaySeconds;
                int m = Mathf.FloorToInt(t / 60f);
                int s = Mathf.FloorToInt(t % 60f);
                timerText.text = $"{m:00}:{s:00}";
            }
        }

        private void OnStartDay()
        {
            GameManager.Instance.StartDay();
            if (dayEndPanel != null) dayEndPanel.SetActive(false);
        }

        private void OnEndDay()
        {
            GameManager.Instance.EndDay();
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDayEnded += HandleDayEnded;
            }
            if (nextDayButton != null) nextDayButton.onClick.AddListener(OnStartDay);
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDayEnded -= HandleDayEnded;
            }
            if (nextDayButton != null) nextDayButton.onClick.RemoveListener(OnStartDay);
        }

        private void HandleDayEnded()
        {
            if (dayEndPanel != null) dayEndPanel.SetActive(true);
        }
    }
}


