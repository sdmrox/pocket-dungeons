using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PocketDungeons.Data;

namespace PocketDungeons.Gameplay.Crafting
{
    [Serializable]
    public class CraftingRecipe
    {
        public string RecipeId;
        public string ResultItemId;
        public Rarity ResultRarity;
        public CraftingIngredient[] Ingredients;
        public int GoldCost;
    }

    [Serializable]
    public class CraftingIngredient
    {
        public string ItemId;
        public int Quantity;
    }

    [Serializable]
    public class CraftingMaterial
    {
        public string MaterialId;
        public string DisplayName;
        public Rarity Rarity;
        public int Quantity;
    }

    public class CraftingManager : MonoBehaviour
    {
        public static CraftingManager Instance { get; private set; }

        [SerializeField] private CraftingRecipe[] _recipes;

        private readonly Dictionary<string, CraftingRecipe> _recipeMap = new();
        private readonly Dictionary<string, CraftingMaterial> _materials = new();

        public event Action<string> OnItemCrafted;
        public event Action<string, int> OnMaterialAdded;

        private void Awake()
        {
            Instance = this;

            if (_recipes != null)
            {
                foreach (var recipe in _recipes)
                    _recipeMap[recipe.RecipeId] = recipe;
            }
        }

        public void Initialize(CraftingRecipe[] recipes)
        {
            _recipeMap.Clear();
            if (recipes != null)
            {
                foreach (var recipe in recipes)
                    _recipeMap[recipe.RecipeId] = recipe;
            }
        }

        public void AddMaterial(string materialId, string displayName, Rarity rarity, int quantity)
        {
            if (_materials.TryGetValue(materialId, out var existing))
            {
                existing.Quantity += quantity;
            }
            else
            {
                _materials[materialId] = new CraftingMaterial
                {
                    MaterialId = materialId,
                    DisplayName = displayName,
                    Rarity = rarity,
                    Quantity = quantity
                };
            }
            OnMaterialAdded?.Invoke(materialId, GetMaterialCount(materialId));
        }

        public bool CanCraft(string recipeId, int playerGold)
        {
            if (!_recipeMap.TryGetValue(recipeId, out var recipe)) return false;
            if (playerGold < recipe.GoldCost) return false;

            return recipe.Ingredients.All(ing =>
                GetMaterialCount(ing.ItemId) >= ing.Quantity);
        }

        public bool Craft(string recipeId, ref int playerGold)
        {
            if (!CanCraft(recipeId, playerGold)) return false;

            var recipe = _recipeMap[recipeId];
            playerGold -= recipe.GoldCost;

            foreach (var ing in recipe.Ingredients)
                ConsumeMaterial(ing.ItemId, ing.Quantity);

            OnItemCrafted?.Invoke(recipe.ResultItemId);
            return true;
        }

        public int GetMaterialCount(string materialId)
        {
            return _materials.TryGetValue(materialId, out var mat) ? mat.Quantity : 0;
        }

        public CraftingRecipe GetRecipe(string recipeId)
        {
            return _recipeMap.TryGetValue(recipeId, out var recipe) ? recipe : null;
        }

        public int RecipeCount => _recipeMap.Count;
        public int MaterialTypeCount => _materials.Count;

        public List<string> GetAvailableRecipes(int playerGold)
        {
            return _recipeMap
                .Where(kvp => CanCraft(kvp.Key, playerGold))
                .Select(kvp => kvp.Key)
                .ToList();
        }

        private void ConsumeMaterial(string materialId, int quantity)
        {
            if (!_materials.TryGetValue(materialId, out var mat)) return;
            mat.Quantity = Mathf.Max(0, mat.Quantity - quantity);
            if (mat.Quantity == 0)
                _materials.Remove(materialId);
        }

        public void ClearAll()
        {
            _materials.Clear();
        }
    }
}
