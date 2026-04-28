using UnityEngine;

namespace PocketDungeons.Monetization.IAP
{
    public enum IAPProductType
    {
        Consumable,     // Gems, gold packs
        NonConsumable,  // Remove ads, starter pack
        Subscription    // Battle Pass
    }

    [CreateAssetMenu(fileName = "NewIAPProduct", menuName = "Pocket Dungeons/Monetization/IAP Product")]
    public class IAPProduct : ScriptableObject
    {
        [Header("Identity")]
        public string ProductId;
        public string DisplayName;
        [TextArea(1, 2)]
        public string Description;
        public Sprite Icon;

        [Header("Type & Pricing")]
        public IAPProductType Type;
        public string PriceString;

        [Header("Reward")]
        public int GemAmount;
        public int GoldAmount;
        public string UnlockId;

        [Header("Display")]
        public bool ShowBestValue;
        public bool ShowMostPopular;
        public float DiscountPercent;
    }
}
