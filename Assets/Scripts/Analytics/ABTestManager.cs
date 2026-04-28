using System;
using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Analytics
{
    /// <summary>
    /// A/B testing framework. Assigns users to test groups,
    /// serves variant values, and tracks exposure events.
    /// Integrates with Firebase Remote Config or similar.
    /// </summary>
    public class ABTestManager : MonoBehaviour
    {
        public static ABTestManager Instance { get; private set; }

        [Serializable]
        public struct ABTest
        {
            public string TestName;
            public string[] Variants;
            public float[] Weights;
        }

        [SerializeField] private ABTest[] _activeTests;

        private readonly Dictionary<string, string> _assignments = new();

        public event Action<string, string> OnTestExposure; // testName, variant

        private void Awake()
        {
            Instance = this;
            AssignGroups();
        }

        private void AssignGroups()
        {
            foreach (var test in _activeTests)
            {
                if (_assignments.ContainsKey(test.TestName)) continue;

                string variant = SelectVariant(test);
                _assignments[test.TestName] = variant;
            }
        }

        public string GetVariant(string testName)
        {
            if (_assignments.TryGetValue(testName, out string variant))
            {
                OnTestExposure?.Invoke(testName, variant);
                return variant;
            }

            return "control";
        }

        public T GetValue<T>(string testName, Dictionary<string, T> variantValues, T defaultValue)
        {
            string variant = GetVariant(testName);
            return variantValues.TryGetValue(variant, out T value) ? value : defaultValue;
        }

        private string SelectVariant(ABTest test)
        {
            if (test.Variants == null || test.Variants.Length == 0)
                return "control";

            float totalWeight = 0f;
            foreach (float w in test.Weights)
                totalWeight += w;

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < test.Variants.Length; i++)
            {
                cumulative += i < test.Weights.Length ? test.Weights[i] : 1f;
                if (roll <= cumulative)
                    return test.Variants[i];
            }

            return test.Variants[0];
        }

        public void LoadAssignments(Dictionary<string, string> savedAssignments)
        {
            foreach (var kvp in savedAssignments)
                _assignments[kvp.Key] = kvp.Value;
        }
    }
}
