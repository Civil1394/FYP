using System;
using UnityEngine;
using System.Collections.Generic;

public class CostUIContainer : MonoBehaviour
{
    [SerializeField] private CostUIProduct costUIProductPrefab;
    [SerializeField] private Transform container; // Parent transform for the UI products

    private List<CostUIProduct> activeProducts = new List<CostUIProduct>();
    private Queue<CostUIProduct> inactiveProducts = new Queue<CostUIProduct>();

    private void Start()
    {
        // Remove all existing children under the container
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Clear any existing products in our lists
        activeProducts.Clear();
        inactiveProducts.Clear();

        CostManager.Instance.OnCostUpdated += OnCostUpdated;
        // Initial update
        OnCostUpdated();
    }

    private void OnDestroy()
    {
        if (CostManager.Instance != null)
        {
            CostManager.Instance.OnCostUpdated -= OnCostUpdated;
        }
    }

    private void OnCostUpdated()
    {
        int availCost = CostManager.Instance.availCost;
        UpdateUIProducts(availCost);
    }

    private void UpdateUIProducts(int count)
    {
        // Add products if we need more
        while (activeProducts.Count < count)
        {
            CostUIProduct product;
            if (inactiveProducts.Count > 0)
            {
                product = inactiveProducts.Dequeue();
                product.gameObject.SetActive(true);
            }
            else
            {
                product = Instantiate(costUIProductPrefab, container);
            }
            activeProducts.Add(product);
        }

        // Remove products if we have too many
        while (activeProducts.Count > count)
        {
            CostUIProduct product = activeProducts[activeProducts.Count - 1];
            activeProducts.RemoveAt(activeProducts.Count - 1);
            product.gameObject.SetActive(false);
            inactiveProducts.Enqueue(product);
        }
    }
}