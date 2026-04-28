using System;
using System.Collections.Generic;
using UnityEngine;
using PocketDungeons.Data;

namespace PocketDungeons.Gameplay.PowerUps
{
    /// <summary>
    /// Manages power-up selection during a run. Presents 3 choices after clearing a floor.
    /// Tracks active power-ups with stacking support and synergy detection.
    /// </summary>
    public class PowerUpManager : MonoBehaviour
    {
        public static PowerUpManager Instance { get; private set; }

        [Header("Config")]
        [SerializeField] private PowerUpData[] _allPowerUps;
        [SerializeField] private int _choicesPerSelection = 3;

        private readonly Dictionary<PowerUpData, int> _activePowerUps = new();
        private PlayerStatModifiers _currentModifiers;

        public PlayerStatModifiers CurrentModifiers => _currentModifiers;
        public int TotalPowerUpsCollected { get; private set; }

        public event Action<PowerUpData[]> OnSelectionReady;
        public event Action<PowerUpData> OnPowerUpApplied;

        private void Awake()
        {
            Instance = this;
            _currentModifiers = new PlayerStatModifiers();
        }

        public void TriggerSelection()
        {
            var choices = GenerateChoices();
            OnSelectionReady?.Invoke(choices);
        }

        public void ApplyChoice(PowerUpData chosen)
        {
            if (_activePowerUps.TryGetValue(chosen, out int stacks))
            {
                if (!chosen.Stackable || stacks >= chosen.MaxStacks)
                    return;
                _activePowerUps[chosen] = stacks + 1;
            }
            else
            {
                _activePowerUps[chosen] = 1;
            }

            TotalPowerUpsCollected++;
            RecalculateModifiers();
            OnPowerUpApplied?.Invoke(chosen);
        }

        public void Reset()
        {
            _activePowerUps.Clear();
            _currentModifiers = new PlayerStatModifiers();
            TotalPowerUpsCollected = 0;
        }

        private PowerUpData[] GenerateChoices()
        {
            var eligible = new List<PowerUpData>();

            foreach (var pu in _allPowerUps)
            {
                if (_activePowerUps.TryGetValue(pu, out int stacks))
                {
                    if (!pu.Stackable || stacks >= pu.MaxStacks)
                        continue;
                }
                eligible.Add(pu);
            }

            // Shuffle and pick
            int count = Mathf.Min(_choicesPerSelection, eligible.Count);
            var choices = new PowerUpData[count];

            for (int i = 0; i < count; i++)
            {
                int idx = UnityEngine.Random.Range(i, eligible.Count);
                (eligible[i], eligible[idx]) = (eligible[idx], eligible[i]);
                choices[i] = eligible[i];
            }

            return choices;
        }

        private void RecalculateModifiers()
        {
            var mods = new PlayerStatModifiers();

            foreach (var kvp in _activePowerUps)
            {
                PowerUpData pu = kvp.Key;
                int stacks = kvp.Value;

                for (int i = 0; i < stacks; i++)
                {
                    mods.DamageMultiplier *= pu.DamageMultiplier;
                    mods.AttackSpeedMultiplier *= pu.AttackSpeedMultiplier;
                    mods.MoveSpeedMultiplier *= pu.MoveSpeedMultiplier;
                    mods.BonusHealth += pu.BonusHealth;
                    mods.CritChanceBonus += pu.CritChanceBonus;
                    mods.GoldMultiplier *= pu.GoldMultiplier;
                }
            }

            // Check synergies
            foreach (var kvp in _activePowerUps)
            {
                if (kvp.Key.SynergyWith == null) continue;
                foreach (var synergy in kvp.Key.SynergyWith)
                {
                    if (synergy != null && _activePowerUps.ContainsKey(synergy))
                    {
                        mods.SynergyBonus += 0.15f;
                    }
                }
            }

            _currentModifiers = mods;
        }
    }

    [Serializable]
    public class PlayerStatModifiers
    {
        public float DamageMultiplier = 1f;
        public float AttackSpeedMultiplier = 1f;
        public float MoveSpeedMultiplier = 1f;
        public int BonusHealth = 0;
        public float CritChanceBonus = 0f;
        public float GoldMultiplier = 1f;
        public float SynergyBonus = 0f;
    }
}
