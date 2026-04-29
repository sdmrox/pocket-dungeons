using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Meta.Onboarding
{
    /// <summary>
    /// Progressive disclosure onboarding per the GDD schedule:
    /// Run 1: Movement, attack, dodge, loot pickup
    /// Run 2: Power-up selection, floor progression
    /// Run 3: Gold spending, first town upgrade
    /// Run 4: Daily quests introduction
    /// Run 5: Battle Pass introduction
    /// Run 7: Hero switching
    /// Run 10: Social features
    /// </summary>
    public class OnboardingManager : MonoBehaviour
    {
        public static OnboardingManager Instance { get; private set; }

        [Serializable]
        public struct OnboardingStep
        {
            public int TriggerRunNumber;
            public string StepId;
            public string Title;
            [TextArea(1, 3)]
            public string Message;
            public Sprite Image;
            public string HighlightElementId;
        }

        [SerializeField] private OnboardingStep[] _steps;

        private readonly HashSet<string> _completedSteps = new();
        private int _totalRunsPlayed;

        public event Action<OnboardingStep> OnStepTriggered;

        private void Awake()
        {
            Instance = this;
        }

        public void OnRunStarted(int runNumber)
        {
            _totalRunsPlayed = runNumber;

            foreach (var step in _steps)
            {
                if (step.TriggerRunNumber == runNumber && !_completedSteps.Contains(step.StepId))
                {
                    OnStepTriggered?.Invoke(step);
                }
            }
        }

        public void CompleteStep(string stepId)
        {
            _completedSteps.Add(stepId);
        }

        public bool IsStepCompleted(string stepId)
        {
            return _completedSteps.Contains(stepId);
        }

        public bool ShouldShowFeature(string featureId)
        {
            return featureId switch
            {
                "power_ups" => _totalRunsPlayed >= 2,
                "town_upgrades" => _totalRunsPlayed >= 3,
                "daily_quests" => _totalRunsPlayed >= 4,
                "battle_pass" => _totalRunsPlayed >= 5,
                "hero_switching" => _totalRunsPlayed >= 7,
                "social" => _totalRunsPlayed >= 10,
                _ => true
            };
        }

        public void LoadState(HashSet<string> completedSteps, int totalRuns)
        {
            _totalRunsPlayed = totalRuns;
            _completedSteps.Clear();
            foreach (var step in completedSteps)
                _completedSteps.Add(step);
        }
    }
}
