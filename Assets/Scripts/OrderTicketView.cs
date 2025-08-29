using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

namespace ShawarmaLegend
{
    public class OrderTicketView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI ingredientsText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Button serveButton;

        private Order boundOrder;

        private void Awake()
        {
            Debug.Log($"[OrderTicketView] Awake called on {gameObject.name}");
            
            if (serveButton != null)
            {
                serveButton.onClick.AddListener(OnServeClicked);
                Debug.Log($"[OrderTicketView] Serve button listener added to {gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"[OrderTicketView] Serve button is null on {gameObject.name}");
            }
        }

        public void Bind(Order order)
        {
            Debug.Log($"[OrderTicketView] Binding order {order.orderId} to {gameObject.name}");
            boundOrder = order;
            RefreshIngredients(order);
            RefreshTimer(order.remainingSeconds);
        }

        public void RefreshIngredients(Order order)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < order.ingredientIds.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(order.ingredientIds[i]);
            }
            if (ingredientsText != null)
            {
                ingredientsText.text = sb.ToString();
                Debug.Log($"[OrderTicketView] Ingredients updated: {sb.ToString()}");
            }
            else
            {
                Debug.LogWarning($"[OrderTicketView] Ingredients text is null on {gameObject.name}");
            }
        }

        public void RefreshTimer(float seconds)
        {
            if (timerText != null)
            {
                int s = Mathf.CeilToInt(seconds);
                timerText.text = s.ToString();
                //Debug.Log($"[OrderTicketView] Timer updated: {s}s on {gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"[OrderTicketView] Timer text is null on {gameObject.name}");
            }
        }

        private void OnServeClicked()
        {
            Debug.Log($"[OrderTicketView] Serve button clicked on {gameObject.name}");
            if (boundOrder != null)
            {
                var orderManager = FindObjectOfType<OrderManager>();
                if (orderManager != null)
                {
                    bool served = orderManager.TryServeOrder(boundOrder.ingredientIds);
                    if (served)
                    {
                        Debug.Log($"[OrderTicketView] Order served! Earned {boundOrder.baseReward} coins!");
                    }
                    else
                    {
                        Debug.LogWarning($"[OrderTicketView] Failed to serve order on {gameObject.name}");
                    }
                }
                else
                {
                    Debug.LogError($"[OrderTicketView] OrderManager not found!");
                }
            }
            else
            {
                Debug.LogWarning($"[OrderTicketView] No bound order on {gameObject.name}");
            }
        }

        private void OnDestroy()
        {
            if (serveButton != null)
            {
                serveButton.onClick.RemoveListener(OnServeClicked);
                Debug.Log($"[OrderTicketView] Cleanup completed on {gameObject.name}");
            }
        }
    }
}
