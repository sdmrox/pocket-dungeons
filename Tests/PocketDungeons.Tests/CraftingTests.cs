using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class CraftingSystemTests
    {
        record Ingredient(string ItemId, int Quantity);
        record Recipe(string Id, string ResultId, int GoldCost, Ingredient[] Ingredients);

        Dictionary<string, Recipe> _recipes = new();
        Dictionary<string, int> _materials = new();

        [SetUp]
        public void SetUp()
        {
            _recipes = new()
            {
                { "iron_sword", new("iron_sword", "sword_common", 50,
                    new[] { new Ingredient("iron_ore", 3), new Ingredient("wood", 1) }) },
                { "steel_armor", new("steel_armor", "armor_uncommon", 150,
                    new[] { new Ingredient("steel_ingot", 5), new Ingredient("leather", 2) }) },
                { "magic_ring", new("magic_ring", "ring_rare", 500,
                    new[] { new Ingredient("gold_bar", 2), new Ingredient("gem", 1) }) }
            };
            _materials = new()
            {
                { "iron_ore", 10 },
                { "wood", 5 },
                { "steel_ingot", 3 },
                { "leather", 2 },
                { "gold_bar", 1 },
                { "gem", 0 }
            };
        }

        bool CanCraft(string recipeId, int gold)
        {
            if (!_recipes.TryGetValue(recipeId, out var recipe)) return false;
            if (gold < recipe.GoldCost) return false;
            foreach (var ing in recipe.Ingredients)
            {
                _materials.TryGetValue(ing.ItemId, out int count);
                if (count < ing.Quantity) return false;
            }
            return true;
        }

        bool Craft(string recipeId, ref int gold)
        {
            if (!CanCraft(recipeId, gold)) return false;
            var recipe = _recipes[recipeId];
            gold -= recipe.GoldCost;
            foreach (var ing in recipe.Ingredients)
            {
                _materials[ing.ItemId] -= ing.Quantity;
                if (_materials[ing.ItemId] <= 0) _materials.Remove(ing.ItemId);
            }
            return true;
        }

        [Test]
        public void CanCraft_WithSufficientMaterials_ReturnsTrue()
        {
            Assert.That(CanCraft("iron_sword", 100), Is.True);
        }

        [Test]
        public void CanCraft_InsufficientGold_ReturnsFalse()
        {
            Assert.That(CanCraft("iron_sword", 10), Is.False);
        }

        [Test]
        public void CanCraft_InsufficientMaterials_ReturnsFalse()
        {
            Assert.That(CanCraft("magic_ring", 1000), Is.False); // 0 gems
        }

        [Test]
        public void CanCraft_UnknownRecipe_ReturnsFalse()
        {
            Assert.That(CanCraft("dragon_slayer", 9999), Is.False);
        }

        [Test]
        public void Craft_ConsumesGoldAndMaterials()
        {
            int gold = 200;
            Assert.That(Craft("iron_sword", ref gold), Is.True);
            Assert.That(gold, Is.EqualTo(150));
            Assert.That(_materials["iron_ore"], Is.EqualTo(7));
            Assert.That(_materials["wood"], Is.EqualTo(4));
        }

        [Test]
        public void Craft_RemovesEmptyMaterials()
        {
            _materials["iron_ore"] = 3;
            int gold = 100;
            Craft("iron_sword", ref gold);
            Assert.That(_materials.ContainsKey("iron_ore"), Is.False);
        }

        [Test]
        public void Craft_FailsWhenCannotCraft()
        {
            int gold = 5;
            Assert.That(Craft("iron_sword", ref gold), Is.False);
            Assert.That(gold, Is.EqualTo(5));
        }

        [Test]
        public void AddMaterial_StacksQuantity()
        {
            int existing = _materials["iron_ore"];
            _materials["iron_ore"] += 5;
            Assert.That(_materials["iron_ore"], Is.EqualTo(existing + 5));
        }
    }
}
