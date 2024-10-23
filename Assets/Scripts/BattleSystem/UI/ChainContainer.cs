using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class ChainContainer : MonoBehaviour
{
    [SerializeField] private ChainUIProduct chainUIProductPrefab;
    [SerializeField] private RectTransform container; // Parent container for UI products
    [SerializeField] private float spacing = 80f; // Space between elements
    [SerializeField] private float moveDuration = 0.3f;
    [SerializeField] private float scaleDuration = 0.3f;
    [SerializeField] private int maxElements = 5;

    private List<ChainUIProduct> chainUIProducts = new List<ChainUIProduct>();
    private Vector2 spawnPosition = new Vector2(400f, 0f); // Start off-screen right
    private Vector2 firstPosition = new Vector2(0f, 0f); // Leftmost position

    private void Start()
    {
        ChainManager.Instance.OnEnqueueSuitChain += HandleNewSuit;
    }

    private void OnDestroy()
    {
        if (ChainManager.Instance != null)
            ChainManager.Instance.OnEnqueueSuitChain -= HandleNewSuit;
    }

    private void HandleNewSuit(Suit suit)
    {
        // Create new UI product
        ChainUIProduct newProduct = Instantiate(chainUIProductPrefab, container);
        newProduct.transform.localPosition = spawnPosition;
        newProduct.Init(suit);
        
        // Set initial scale to 1
        newProduct.transform.localScale = Vector3.one;

        // Add to list
        chainUIProducts.Add(newProduct);

        // Animate all elements
        AnimateChain();
    }

    private void AnimateChain()
    {
        int count = chainUIProducts.Count;

        // Remove excess elements
        if (count > maxElements)
        {
            // Destroy the leftmost element
            Destroy(chainUIProducts[0].gameObject);
            chainUIProducts.RemoveAt(0);
            count--;
        }

        // Animate remaining elements
        for (int i = 0; i < count; i++)
        {
            ChainUIProduct product = chainUIProducts[i];
            Vector2 targetPos = firstPosition + new Vector2(i * spacing, 0f);

            // Move animation
            product.transform
                .DOLocalMove(targetPos, moveDuration)
                .SetEase(Ease.OutQuad);

            // Scale animation
            float targetScale = (i == count - 1) ? 1f : 0.5f;
            
            // If this was previously the newest element (now second-newest)
            if (i == count - 2)
            {
                product.transform
                    .DOScale(targetScale, scaleDuration)
                    .SetEase(Ease.OutQuad);
            }
            // For all other existing elements, maintain their scale
            else if (i < count - 1)
            {
                product.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
    }

    // Optional: Method to clear all chain UI products
    public void ClearChainUI()
    {
        foreach (var product in chainUIProducts)
        {
            if (product != null)
                Destroy(product.gameObject);
        }
        chainUIProducts.Clear();
    }
}