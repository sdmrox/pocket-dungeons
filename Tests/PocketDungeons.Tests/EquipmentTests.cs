using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class EquipmentSystemTests
    {
        enum EquipmentSlot { Weapon, Armor, Accessory }
        enum Rarity { Common, Uncommon, Rare, Epic, Legendary }
        enum SetBonus { None, Warrior, Shadow, Arcane, Guardian, Fortune }

        record Equipment(string Name, EquipmentSlot Slot, Rarity Rarity, int Damage, int Health,
            float CritChance, float MoveSpeed, float AttackSpeed, float Defense,
            SetBonus Set = SetBonus.None, int SetPieces = 2, int SellValue = 10, int CraftCost = 50);

        class EquipmentStats
        {
            public int Damage; public int Health; public float CritChance;
            public float MoveSpeed; public float AttackSpeed; public float Defense;
            public float AbilityDmgMul = 1f; public float CooldownReduce;
            public float GoldFind; public float LootQuality;
        }

        Dictionary<EquipmentSlot, Equipment> _equipped = new();
        List<Equipment> _inventory = new();

        bool Equip(Equipment item)
        {
            if (_equipped.TryGetValue(item.Slot, out var current))
                _inventory.Add(current);
            _equipped[item.Slot] = item;
            _inventory.Remove(item);
            return true;
        }

        bool Unequip(EquipmentSlot slot)
        {
            if (!_equipped.TryGetValue(slot, out var item)) return false;
            _equipped.Remove(slot);
            _inventory.Add(item);
            return true;
        }

        EquipmentStats GetTotalStats()
        {
            var stats = new EquipmentStats();
            foreach (var item in _equipped.Values)
            {
                stats.Damage += item.Damage;
                stats.Health += item.Health;
                stats.CritChance += item.CritChance;
                stats.MoveSpeed += item.MoveSpeed;
                stats.AttackSpeed += item.AttackSpeed;
                stats.Defense += item.Defense;
            }

            var setCounts = new Dictionary<SetBonus, int>();
            foreach (var item in _equipped.Values)
            {
                if (item.Set == SetBonus.None) continue;
                setCounts.TryGetValue(item.Set, out int c);
                setCounts[item.Set] = c + 1;
            }
            foreach (var kvp in setCounts)
            {
                if (kvp.Value < 2) continue;
                switch (kvp.Key)
                {
                    case SetBonus.Warrior:
                        stats.Health += (int)(stats.Health * 0.15f);
                        stats.Damage += (int)(stats.Damage * 0.10f);
                        break;
                    case SetBonus.Shadow:
                        stats.CritChance += 0.20f;
                        stats.MoveSpeed += 0.10f;
                        break;
                    case SetBonus.Fortune:
                        stats.GoldFind = 0.25f;
                        stats.LootQuality = 0.15f;
                        break;
                }
            }
            return stats;
        }

        [SetUp]
        public void SetUp()
        {
            _equipped.Clear();
            _inventory.Clear();
        }

        [Test]
        public void Equip_AddsToSlot()
        {
            var sword = new Equipment("Iron Sword", EquipmentSlot.Weapon, Rarity.Common, 10, 0, 0, 0, 0, 0);
            Assert.That(Equip(sword), Is.True);
            Assert.That(_equipped.ContainsKey(EquipmentSlot.Weapon), Is.True);
        }

        [Test]
        public void Equip_ReplacesExisting_MovesToInventory()
        {
            var sword1 = new Equipment("Iron Sword", EquipmentSlot.Weapon, Rarity.Common, 10, 0, 0, 0, 0, 0);
            var sword2 = new Equipment("Steel Sword", EquipmentSlot.Weapon, Rarity.Uncommon, 20, 0, 0, 0, 0, 0);
            Equip(sword1);
            Equip(sword2);
            Assert.That(_equipped[EquipmentSlot.Weapon], Is.EqualTo(sword2));
            Assert.That(_inventory.Contains(sword1), Is.True);
        }

        [Test]
        public void Unequip_MovesToInventory()
        {
            var armor = new Equipment("Leather", EquipmentSlot.Armor, Rarity.Common, 0, 20, 0, 0, 0, 0.1f);
            Equip(armor);
            Assert.That(Unequip(EquipmentSlot.Armor), Is.True);
            Assert.That(_equipped.ContainsKey(EquipmentSlot.Armor), Is.False);
            Assert.That(_inventory.Contains(armor), Is.True);
        }

        [Test]
        public void Unequip_EmptySlot_ReturnsFalse()
        {
            Assert.That(Unequip(EquipmentSlot.Accessory), Is.False);
        }

        [Test]
        public void TotalStats_SumsAllEquipped()
        {
            Equip(new Equipment("Sword", EquipmentSlot.Weapon, Rarity.Rare, 15, 0, 0.05f, 0, 0.1f, 0));
            Equip(new Equipment("Plate", EquipmentSlot.Armor, Rarity.Rare, 0, 50, 0, 0, 0, 0.2f));
            Equip(new Equipment("Ring", EquipmentSlot.Accessory, Rarity.Epic, 5, 10, 0.1f, 0.05f, 0, 0));
            var stats = GetTotalStats();
            Assert.That(stats.Damage, Is.EqualTo(20));
            Assert.That(stats.Health, Is.EqualTo(60));
            Assert.That(stats.CritChance, Is.EqualTo(0.15f).Within(0.001f));
            Assert.That(stats.Defense, Is.EqualTo(0.2f).Within(0.001f));
        }

        [Test]
        public void SetBonus_ActivatesWhenTwoPiecesEquipped()
        {
            Equip(new Equipment("Shadow Blade", EquipmentSlot.Weapon, Rarity.Epic, 10, 0, 0, 0, 0, 0, SetBonus.Shadow));
            Equip(new Equipment("Shadow Ring", EquipmentSlot.Accessory, Rarity.Epic, 0, 0, 0, 0, 0, 0, SetBonus.Shadow));
            var stats = GetTotalStats();
            Assert.That(stats.CritChance, Is.EqualTo(0.20f).Within(0.001f));
            Assert.That(stats.MoveSpeed, Is.EqualTo(0.10f).Within(0.001f));
        }

        [Test]
        public void SetBonus_DoesNotActivateWithOnePiece()
        {
            Equip(new Equipment("Shadow Blade", EquipmentSlot.Weapon, Rarity.Epic, 10, 0, 0, 0, 0, 0, SetBonus.Shadow));
            var stats = GetTotalStats();
            Assert.That(stats.CritChance, Is.EqualTo(0f));
        }

        [Test]
        public void RarityValues_AreOrdered()
        {
            Assert.That((int)Rarity.Common, Is.LessThan((int)Rarity.Uncommon));
            Assert.That((int)Rarity.Uncommon, Is.LessThan((int)Rarity.Rare));
            Assert.That((int)Rarity.Rare, Is.LessThan((int)Rarity.Epic));
            Assert.That((int)Rarity.Epic, Is.LessThan((int)Rarity.Legendary));
        }
    }
}
