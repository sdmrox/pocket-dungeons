using UnityEngine;

namespace PocketDungeons.Meta.Quests
{
    public enum QuestType
    {
        Daily,
        Weekly
    }

    public enum QuestObjective
    {
        KillEnemies,
        ClearFloors,
        CollectGold,
        CompleteDungeonRuns,
        UseAbilities,
        DodgeAttacks,
        DefeatBoss,
        CollectPowerUps,
        PlayAsHero,
        ReachFloor
    }

    [CreateAssetMenu(fileName = "NewQuest", menuName = "Pocket Dungeons/Meta/Quest")]
    public class QuestData : ScriptableObject
    {
        [Header("Identity")]
        public string DisplayName;
        [TextArea(1, 2)]
        public string Description;
        public QuestType Type;
        public QuestObjective Objective;

        [Header("Target")]
        public int TargetAmount = 10;
        public string TargetParameter;

        [Header("Reward")]
        public int GoldReward;
        public int GemReward;
        public int BattlePassXP;
    }
}
