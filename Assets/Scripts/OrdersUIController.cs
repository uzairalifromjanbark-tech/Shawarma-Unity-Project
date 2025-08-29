using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace ShawarmaLegend
{
    public class OrdersUIController : MonoBehaviour
    {
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private RectTransform ticketsContainer;
        [SerializeField] private OrderTicketView ticketPrefab;

        private readonly Dictionary<string, OrderTicketView> idToTicket = new Dictionary<string, OrderTicketView>();

        private void OnEnable()
        {
            Debug.Log("[OrdersUIController] OnEnable called");
            if (!orderManager) 
            {
                orderManager = FindObjectOfType<OrderManager>();
                Debug.Log($"[OrdersUIController] Found OrderManager: {(orderManager != null ? "SUCCESS" : "FAILED")}");
            }
            
            if (orderManager != null)
            {
                orderManager.OnOrderAdded += HandleAdded;
                orderManager.OnOrderRemoved += HandleRemoved;
                orderManager.OnOrderUpdated += HandleUpdated;
                Debug.Log("[OrdersUIController] Event listeners attached to OrderManager");
            }
            else
            {
                Debug.LogError("[OrdersUIController] OrderManager is null!");
            }
        }

        private void OnDisable()
        {
            Debug.Log("[OrdersUIController] OnDisable called");
            if (orderManager != null)
            {
                orderManager.OnOrderAdded -= HandleAdded;
                orderManager.OnOrderRemoved -= HandleRemoved;
                orderManager.OnOrderUpdated -= HandleUpdated;
                Debug.Log("[OrdersUIController] Event listeners removed from OrderManager");
            }
        }

        private void HandleAdded(Order order)
        {
            Debug.Log($"[OrdersUIController] Order added: {order.orderId} with {order.ingredientIds.Count} ingredients");
            
            if (ticketPrefab == null)
            {
                Debug.LogError("[OrdersUIController] Ticket prefab is null!");
                return;
            }
            
            if (ticketsContainer == null)
            {
                Debug.LogError("[OrdersUIController] Tickets container is null!");
                return;
            }
            
            var view = Instantiate(ticketPrefab, ticketsContainer);
            Debug.Log($"[OrdersUIController] Ticket instantiated: {view.name}");
            
            view.Bind(order);
            idToTicket[order.orderId] = view;
            Debug.Log($"[OrdersUIController] Ticket bound and stored. Total tickets: {idToTicket.Count}");
        }

        private void HandleRemoved(Order order)
        {
            if (order == null) 
            {
                Debug.LogWarning("[OrdersUIController] HandleRemoved called with null order");
                return;
            }
            
            Debug.Log($"[OrdersUIController] Order removed: {order.orderId}");
            if (idToTicket.TryGetValue(order.orderId, out var view))
            {
                Destroy(view.gameObject);
                idToTicket.Remove(order.orderId);
                Debug.Log($"[OrdersUIController] Ticket destroyed and removed. Remaining tickets: {idToTicket.Count}");
            }
            else
            {
                Debug.LogWarning($"[OrdersUIController] No ticket found for order {order.orderId}");
            }
        }

        private void HandleUpdated(Order order)
        {
            if (order == null) 
            {
                //Debug.LogWarning("[OrdersUIController] HandleUpdated called with null order");
                return;
            }
            
            if (idToTicket.TryGetValue(order.orderId, out var view))
            {
                view.RefreshTimer(order.remainingSeconds);
                //Debug.Log($"[OrdersUIController] Timer updated for order {order.orderId}: {order.remainingSeconds:F1}s");
            }
            else
            {
                //Debug.LogWarning($"[OrdersUIController] No ticket found for order update {order.orderId}");
            }
        }
    }
}



