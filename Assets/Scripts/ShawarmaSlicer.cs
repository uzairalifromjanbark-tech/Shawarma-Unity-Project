using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ShawarmaLegend
{
    public class ShawarmaSlicer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Slicing")]
        [SerializeField] private Image meatFillImage;
        [SerializeField] private float sliceSpeed = 1f;
        [SerializeField] private float maxMeatAmount = 100f;
        
        [Header("Debug")]
        [SerializeField] private bool isSlicing = false;
        [SerializeField] private float currentMeatAmount = 0f;
        
        private Vector2 lastMousePos;
        private float sliceProgress = 0f;

        private void Start()
        {
            Debug.Log("[ShawarmaSlicer] Start called - Slicer initialized");
            if (meatFillImage == null)
            {
                Debug.LogWarning("[ShawarmaSlicer] Meat fill image not assigned!");
            }
            else
            {
                Debug.Log("[ShawarmaSlicer] Meat fill image found and ready");
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("[ShawarmaSlicer] Pointer down - Starting slice");
            isSlicing = true;
            lastMousePos = eventData.position;
            sliceProgress = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("[ShawarmaSlicer] Pointer up - Stopping slice");
            isSlicing = false;
            
            if (currentMeatAmount > 0)
            {
                Debug.Log($"[ShawarmaSlicer] Slice completed! Meat amount: {currentMeatAmount:F1}");
                // TODO: Add meat to assembly system
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isSlicing) return;
            
            Vector2 currentPos = eventData.position;
            float distance = Vector2.Distance(currentPos, lastMousePos);
            
            if (distance > 5f) // Minimum drag distance
            {
                sliceProgress += distance * sliceSpeed * Time.deltaTime;
                currentMeatAmount = Mathf.Min(maxMeatAmount, sliceProgress);
                
                if (meatFillImage != null)
                {
                    meatFillImage.fillAmount = currentMeatAmount / maxMeatAmount;
                }
                
                lastMousePos = currentPos;
                
                Debug.Log($"[ShawarmaSlicer] Slicing progress: {sliceProgress:F1}/{maxMeatAmount:F1}");
            }
        }

        public float GetMeatAmount()
        {
            return currentMeatAmount;
        }

        public void ResetMeat()
        {
            Debug.Log("[ShawarmaSlicer] Resetting meat amount");
            currentMeatAmount = 0f;
            sliceProgress = 0f;
            if (meatFillImage != null)
            {
                meatFillImage.fillAmount = 0f;
            }
        }

        public bool HasMeat()
        {
            return currentMeatAmount > 0f;
        }
    }
}
