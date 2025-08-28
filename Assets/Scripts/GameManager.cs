using UnityEngine;
using System.Collections.Generic;
using System;

namespace ShawarmaLegend
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private OrderManager orderManager;
        [SerializeField] private float dayDurationSeconds = 120f;

        private float dayTimerSeconds;
        private bool dayRunning;

        public bool IsDayRunning => dayRunning;
        public float RemainingDaySeconds => Mathf.Max(0f, dayDurationSeconds - dayTimerSeconds);

        public event Action OnDayEnded;

        public int CurrentDay { get; private set; } = 1;
        public int Coins { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() { }

        private void Update()
        {
            if (!dayRunning) return;

            dayTimerSeconds += Time.deltaTime;
            if (dayTimerSeconds >= dayDurationSeconds)
            {
                EndDay();
            }
        }

        public void StartDay()
        {
            dayTimerSeconds = 0f;
            dayRunning = true;
            orderManager?.BeginServiceForDay(CurrentDay);
        }

        public void EndDay()
        {
            dayRunning = false;
            orderManager?.EndServiceForDay(CurrentDay);
            CurrentDay++;
            // Hook for results UI and upgrades
            OnDayEnded?.Invoke();
        }

        public void AddCoins(int amount)
        {
            Coins = Mathf.Max(0, Coins + amount);
        }
    }
}


