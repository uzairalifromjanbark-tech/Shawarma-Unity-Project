using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        public void BeginServiceForDay(int dayIndex)
        {
            if (spawnRoutine != null) StopCoroutine(spawnRoutine);
            spawnRoutine = StartCoroutine(SpawnOrdersRoutine(dayIndex));
        }

        public void EndServiceForDay(int dayIndex)
        {
            if (spawnRoutine != null)
            {
                StopCoroutine(spawnRoutine);
                spawnRoutine = null;
            }
            activeOrders.Clear();
        }

        private IEnumerator SpawnOrdersRoutine(int dayIndex)
        {
            float interval = Mathf.Max(minSpawnInterval, initialSpawnInterval - dayIndex * 0.25f);
            var wait = new WaitForSeconds(interval);
            while (true)
            {
                if (activeOrders.Count < maxConcurrentOrders)
                {
                    var order = GenerateOrderForDay(dayIndex);
                    activeOrders.Add(order);
                    // TODO: Notify UI
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
                var ing = ingredients[Random.Range(0, ingredients.Count)];
                order.ingredientIds.Add(ing.id);
            }
            order.baseReward += Mathf.RoundToInt(dayIndex * 1.5f);
            order.maxWaitSeconds = Mathf.Max(8f, 20f - dayIndex * 0.5f);
            return order;
        }

        public bool TryServeOrder(List<string> ingredientIds)
        {
            for (int i = 0; i < activeOrders.Count; i++)
            {
                if (IsMatch(activeOrders[i], ingredientIds))
                {
                    GameManager.Instance.AddCoins(activeOrders[i].baseReward);
                    activeOrders.RemoveAt(i);
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
    }
}


