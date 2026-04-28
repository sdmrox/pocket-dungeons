using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Monetization.IAP
{
    /// <summary>
    /// Manages in-app purchases via StoreKit 2 (iOS) wrapper.
    /// All digital goods go through Apple IAP (15-30% commission).
    /// Products: Gem packs, Starter Pack, Battle Pass, Remove Ads.
    /// No real-money randomized purchases (loot compliance).
    /// </summary>
    public class IAPManager : MonoBehaviour
    {
        public static IAPManager Instance { get; private set; }

        [SerializeField] private IAPProduct[] _products;

        private readonly Dictionary<string, IAPProduct> _productMap = new();
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public event Action<string, bool> OnPurchaseComplete; // productId, success

        private void Awake()
        {
            Instance = this;

            foreach (var product in _products)
            {
                if (product != null && !string.IsNullOrEmpty(product.ProductId))
                    _productMap[product.ProductId] = product;
            }
        }

        public void Initialize()
        {
            // TODO: Initialize StoreKit 2 via native plugin
            // In Unity: use UnityPurchasing or custom StoreKit 2 bridge
            _isInitialized = true;
            Debug.Log("[IAP] Store initialized with " + _products.Length + " products");
        }

        public void PurchaseProduct(string productId)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[IAP] Store not initialized");
                return;
            }

            if (!_productMap.TryGetValue(productId, out var product))
            {
                Debug.LogError($"[IAP] Unknown product: {productId}");
                return;
            }

            Debug.Log($"[IAP] Initiating purchase: {product.DisplayName}");
            // TODO: Call native StoreKit 2 purchase
            // On success: ProcessPurchase(product);
        }

        public void ProcessPurchase(IAPProduct product)
        {
            switch (product.Type)
            {
                case IAPProductType.Consumable:
                    GrantConsumable(product);
                    break;
                case IAPProductType.NonConsumable:
                    GrantNonConsumable(product);
                    break;
                case IAPProductType.Subscription:
                    GrantSubscription(product);
                    break;
            }

            // Log analytics
            Core.Services.Services.Get<Core.Services.IAnalyticsService>()?.LogEvent("iap_purchase",
                new Dictionary<string, object>
                {
                    { "product_id", product.ProductId },
                    { "product_type", product.Type.ToString() },
                    { "price", product.PriceString }
                });

            OnPurchaseComplete?.Invoke(product.ProductId, true);
        }

        public void RestorePurchases()
        {
            Debug.Log("[IAP] Restoring purchases...");
            // TODO: StoreKit 2 restore transactions
        }

        public IAPProduct GetProduct(string productId)
        {
            return _productMap.TryGetValue(productId, out var product) ? product : null;
        }

        private void GrantConsumable(IAPProduct product)
        {
            if (product.GemAmount > 0)
                Debug.Log($"[IAP] Granted {product.GemAmount} gems");

            if (product.GoldAmount > 0)
                Meta.Town.TownManager.Instance?.AddGold(product.GoldAmount);
        }

        private void GrantNonConsumable(IAPProduct product)
        {
            if (!string.IsNullOrEmpty(product.UnlockId))
                Debug.Log($"[IAP] Unlocked: {product.UnlockId}");
        }

        private void GrantSubscription(IAPProduct product)
        {
            Meta.BattlePass.BattlePassManager.Instance?.ActivatePremium();
        }
    }
}
