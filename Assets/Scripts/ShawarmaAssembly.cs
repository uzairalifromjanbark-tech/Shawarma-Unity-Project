using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditorInternal;
using TMPro;

namespace ShawarmaLegend
{
    public class ShawarmaAssembly : MonoBehaviour
    {
        [Header("Assembly")]
        [SerializeField] private ShawarmaSlicer slicer;
        [SerializeField] private Transform assemblyContainer;
        [SerializeField] private Button wrapButton;
        [SerializeField] private Button clearButton;
        
        [Header("Ingredients")]
        [SerializeField] private List<string> currentIngredients = new List<string>();
        [SerializeField] private int[] maxIngredients = { };
        [SerializeField] private int[] maxIngredientsCounter = { };
        
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI ingredientsListText;
        [SerializeField] private TextMeshProUGUI statusText;

        private void Start()
        {
            Debug.Log("[ShawarmaAssembly] Start called - Assembly system initialized");
            
            if (slicer == null)
            {
                slicer = FindObjectOfType<ShawarmaSlicer>();
                Debug.Log($"[ShawarmaAssembly] Found slicer: {(slicer != null ? "SUCCESS" : "FAILED")}");
            }
            
            if (wrapButton != null)
            {
                wrapButton.onClick.AddListener(OnWrapClicked);
                Debug.Log("[ShawarmaAssembly] Wrap button listener added");
            }
            
            if (clearButton != null)
            {
                clearButton.onClick.AddListener(OnClearClicked);
                Debug.Log("[ShawarmaAssembly] Clear button listener added");
            }
            
            UpdateUI();
        }

        /*public void AddIngredient(string ingredientId)
        {
            if (currentIngredients.Count >= maxIngredients)
            {
                Debug.LogWarning("[ShawarmaAssembly] Cannot add ingredient - assembly is full!");
                return;
            }
            
            currentIngredients.Add(ingredientId);
            Debug.Log($"[ShawarmaAssembly] Added ingredient: {ingredientId}. Total: {currentIngredients.Count}");
            UpdateUI();
        }*/

        public void AddIngredient(int ingredientId)
        {
            Debug.Log("here");
            if (maxIngredientsCounter[ingredientId] >= maxIngredients[ingredientId])
            {
                Debug.Log("[ShawarmaAssembly] Cannot add ingredient - assembly is full!");
                return;
            }
            maxIngredientsCounter[ingredientId]++;
            //currentIngredients.Add(ingredientId);
            Debug.Log($"[ShawarmaAssembly] Added ingredient: {ingredientId}. Total: {maxIngredientsCounter[ingredientId]}");
            UpdateUI();
        }

        public void AddMeatFromSlicer()
        {
            if (slicer == null)
            {
                Debug.LogWarning("[ShawarmaAssembly] No slicer found!");
                return;
            }
            
            if (slicer.HasMeat())
            {
                float meatAmount = slicer.GetMeatAmount();
                string meatType = "chicken"; // TODO: Make this configurable
                
                //AddIngredient(meatType);
                slicer.ResetMeat();
                Debug.Log($"[ShawarmaAssembly] Added {meatAmount:F1} meat from slicer");
            }
            else
            {
                Debug.Log("[ShawarmaAssembly] No meat available from slicer");
            }
        }

        /*private*/ void OnWrapClicked()
        {
            Debug.Log("[ShawarmaAssembly] Wrap button clicked");
            
            if (currentIngredients.Count == 0)
            {
                Debug.LogWarning("[ShawarmaAssembly] Cannot wrap - no ingredients!");
                return;
            }
            
            // Try to serve the order
            var orderManager = FindObjectOfType<OrderManager>();
            if (orderManager != null)
            {
                bool served = orderManager.TryServeOrder(currentIngredients);
                if (served)
                {
                    Debug.Log($"[ShawarmaAssembly] Order served successfully with {currentIngredients.Count} ingredients!");
                    ClearAssembly();
                }
                else
                {
                    Debug.LogWarning("[ShawarmaAssembly] No matching order found for these ingredients");
                }
            }
            else
            {
                Debug.LogError("[ShawarmaAssembly] OrderManager not found!");
            }
        }

        private void OnClearClicked()
        {
            Debug.Log("[ShawarmaAssembly] Clear button clicked");
            ClearAssembly();
        }

        private void ClearAssembly()
        {
            Debug.Log("[ShawarmaAssembly] Clearing assembly");
            currentIngredients.Clear();
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (ingredientsListText != null)
            {
                string ingredientsText = string.Join(", ", currentIngredients);
                ingredientsListText.text = ingredientsText.Length > 0 ? ingredientsText : "No ingredients";
            }
            
            if (statusText != null)
            {
                statusText.text = $"Ingredients: {currentIngredients.Count}/{maxIngredients}";
            }
            
            if (wrapButton != null)
            {
                wrapButton.interactable = currentIngredients.Count > 0;
            }
            
            Debug.Log($"[ShawarmaAssembly] UI updated. Ingredients: {currentIngredients.Count}");
        }

        public List<string> GetCurrentIngredients()
        {
            return new List<string>(currentIngredients);
        }

        public bool IsAssemblyEmpty()
        {
            return currentIngredients.Count == 0;
        }
    }
}
