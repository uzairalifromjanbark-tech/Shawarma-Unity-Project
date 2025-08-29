using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ShawarmaLegend
{
    [System.Serializable]
    public class Ingredient
    {
        public string id;
        public Sprite icon;
        public float prepTimeSeconds = 0.5f;
    }

    [System.Serializable]
    public class Order
    {
        public List<string> ingredientIds = new List<string>();
        public float maxWaitSeconds = 20f;
        public int baseReward = 5;
        public string orderId;
        public float remainingSeconds;
    }

    public class OrderManager : MonoBehaviour
    {
        [Header("Catalog")]
        [SerializeField] private List<Ingredient> ingredients = new List<Ingredient>();

        [Header("Flow")]
        [SerializeField] private float initialSpawnInterval = 5f;
        [SerializeField] private float minSpawnInterval = 1.5f;
        [SerializeField] private int maxConcurrentOrders = 5;

        private readonly List<Order> activeOrders = new List<Order>();
        private Coroutine spawnRoutine;

        public event Action<Order> OnOrderAdded;
        public event Action<Order> OnOrderRemoved;
        public event Action<Order> OnOrderUpdated;

        public IReadOnlyList<Order> ActiveOrders => activeOrders;

        private void Awake()
        {
            Debug.Log("[OrderManager] Awake called");
            // Add sample ingredients if the list is empty
            if (ingredients.Count == 0)
            {
                ingredients.Add(new Ingredient { id = "chicken", prepTimeSeconds = 0.5f });
                ingredients.Add(new Ingredient { id = "mayonnaise", prepTimeSeconds = 0.5f });
                ingredients.Add(new Ingredient { id = "cucumber", prepTimeSeconds = 0.3f });
                ingredients.Add(new Ingredient { id = "chips", prepTimeSeconds = 0.3f });
                Debug.Log($"[OrderManager] Added {ingredients.Count} sample ingredients");
            }
            else
            {
                Debug.Log($"[OrderManager] Using existing {ingredients.Count} ingredients");
            }
        }

        public void BeginServiceForDay(int dayIndex)
        {
            Debug.Log($"[OrderManager] BeginServiceForDay called for day {dayIndex}");
            if (spawnRoutine != null) StopCoroutine(spawnRoutine);
            spawnRoutine = StartCoroutine(SpawnOrdersRoutine(dayIndex));
            Debug.Log("[OrderManager] Spawn routine started");
        }

        public void EndServiceForDay(int dayIndex)
        {
            Debug.Log($"[OrderManager] EndServiceForDay called for day {dayIndex}");
            if (spawnRoutine != null)
            {
                StopCoroutine(spawnRoutine);
                spawnRoutine = null;
                Debug.Log("[OrderManager] Spawn routine stopped");
            }
            activeOrders.Clear();
            Debug.Log($"[OrderManager] Cleared {activeOrders.Count} active orders");
        }

        private IEnumerator SpawnOrdersRoutine(int dayIndex)
        {
            Debug.Log($"[OrderManager] SpawnOrdersRoutine started for day {dayIndex}");
            float interval = Mathf.Max(minSpawnInterval, initialSpawnInterval - dayIndex * 0.25f);
            Debug.Log($"[OrderManager] Spawn interval: {interval:F1}s");
            var wait = new WaitForSeconds(interval);
            
            while (true)
            {
                if (activeOrders.Count < maxConcurrentOrders)
                {
                    var order = GenerateOrderForDay(dayIndex);
                    activeOrders.Add(order);
                    Debug.Log($"[OrderManager] Generated order {order.orderId} with {order.ingredientIds.Count} ingredients");
                    OnOrderAdded?.Invoke(order);
                    Debug.Log($"[OrderManager] OnOrderAdded event fired. Active orders: {activeOrders.Count}");
                }
                else
                {
                    Debug.Log($"[OrderManager] Max orders reached ({activeOrders.Count}/{maxConcurrentOrders}), waiting...");
                }
                yield return wait;
            }
        }

        private Order GenerateOrderForDay(int dayIndex)
        {
            var order = new Order();
            int count = Mathf.Clamp(1 + dayIndex / 3, 1, 5);
            for (int i = 0; i < count; i++)
            {
                if (ingredients.Count == 0) break;
                var ing = ingredients[UnityEngine.Random.Range(0, ingredients.Count)];
                order.ingredientIds.Add(ing.id);
            }
            order.baseReward += Mathf.RoundToInt(dayIndex * 1.5f);
            order.maxWaitSeconds = Mathf.Max(8f, 20f - dayIndex * 0.5f);
            order.remainingSeconds = order.maxWaitSeconds;
            order.orderId = Guid.NewGuid().ToString("N");
            return order;
        }

        public bool TryServeOrder(List<string> ingredientIds)
        {
            for (int i = 0; i < activeOrders.Count; i++)
            {
                if (IsMatch(activeOrders[i], ingredientIds))
                {
                    GameManager.Instance.AddCoins(activeOrders[i].baseReward);
                    var removed = activeOrders[i];
                    activeOrders.RemoveAt(i);
                    OnOrderRemoved?.Invoke(removed);
                    return true;
                }
            }
            return false;
        }

        private bool IsMatch(Order order, List<string> ingredientIds)
        {
            if (order.ingredientIds.Count != ingredientIds.Count) return false;
            // unordered match
            var temp = new List<string>(order.ingredientIds);
            foreach (var id in ingredientIds)
            {
                if (!temp.Remove(id)) return false;
            }
            return temp.Count == 0;
        }

        private void Update()
        {
            if (!GameManager.Instance || !GameManager.Instance.IsDayRunning) return;
            if (activeOrders.Count == 0) return;
            float dt = Time.deltaTime;
            for (int i = activeOrders.Count - 1; i >= 0; i--)
            {
                var o = activeOrders[i];
                o.remainingSeconds -= dt;
                if (o.remainingSeconds <= 0f)
                {
                    var removed = o;
                    activeOrders.RemoveAt(i);
                    OnOrderRemoved?.Invoke(removed);
                    continue;
                }
                OnOrderUpdated?.Invoke(o);
            }
        }
    }
}


