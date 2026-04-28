using UnityEngine;

namespace PocketDungeons.Meta.BattlePass
{
    public enum BattlePassRewardType
    {
        Gold,
        Gems,
        CosmeticSkin,
        PowerUpUnlock,
        TitleBadge,
        EmoteSlot
    }

    [System.Serializable]
    public struct BattlePassTier
    {
        public int Level;
        public int XPRequired;
        public BattlePassRewardType FreeReward;
        public string FreeRewardId;
        public int FreeRewardAmount;
        public BattlePassRewardType PremiumReward;
        public string PremiumRewardId;
        public int PremiumRewardAmount;
    }

    [CreateAssetMenu(fileName = "BattlePassSeason", menuName = "Pocket Dungeons/Meta/Battle Pass Season")]
    public class BattlePassData : ScriptableObject
    {
        [Header("Season Info")]
        public string SeasonName;
        public int SeasonNumber;
        public string ThemeName;
        public Sprite SeasonBanner;

        [Header("Tiers")]
        public BattlePassTier[] Tiers;

        [Header("Pricing")]
        public string PremiumProductId;
        public float PremiumPrice = 4.99f;

        [Header("Duration")]
        public int DurationDays = 42;
    }
}
