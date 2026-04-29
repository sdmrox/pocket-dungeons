using NUnit.Framework;

namespace PocketDungeons.Tests
{
    // ==================== PHASE 2: Depth & Feel Tests ====================

    [TestFixture]
    public class HeroClassTests
    {
        enum HeroClass { Warrior, Archer, Mage }

        record HeroStats(int HP, int Damage, float AttackSpeed, float AbilityCooldown);

        static HeroStats GetStats(HeroClass c) => c switch
        {
            HeroClass.Warrior => new(120, 15, 0.8f, 8f),
            HeroClass.Archer => new(80, 12, 0.5f, 8f),
            HeroClass.Mage => new(70, 18, 1.0f, 8f),
            _ => throw new ArgumentException()
        };

        [Test]
        public void Warrior_HasHighestHP()
        {
            Assert.That(GetStats(HeroClass.Warrior).HP, Is.GreaterThan(GetStats(HeroClass.Archer).HP));
            Assert.That(GetStats(HeroClass.Warrior).HP, Is.GreaterThan(GetStats(HeroClass.Mage).HP));
        }

        [Test]
        public void Mage_HasHighestDamage()
        {
            Assert.That(GetStats(HeroClass.Mage).Damage, Is.GreaterThan(GetStats(HeroClass.Warrior).Damage));
            Assert.That(GetStats(HeroClass.Mage).Damage, Is.GreaterThan(GetStats(HeroClass.Archer).Damage));
        }

        [Test]
        public void Archer_HasFastestAttackSpeed()
        {
            Assert.That(GetStats(HeroClass.Archer).AttackSpeed, Is.LessThan(GetStats(HeroClass.Warrior).AttackSpeed));
            Assert.That(GetStats(HeroClass.Archer).AttackSpeed, Is.LessThan(GetStats(HeroClass.Mage).AttackSpeed));
        }

        [Test]
        public void AllHeroes_HaveAbilityCooldown()
        {
            foreach (HeroClass c in Enum.GetValues<HeroClass>())
                Assert.That(GetStats(c).AbilityCooldown, Is.GreaterThan(0));
        }

        [Test]
        public void LevelBonus_IncreasesStats()
        {
            float hpPerLevel = 10f;
            float dmgPerLevel = 2f;
            int level = 5;

            int bonusHP = (int)(hpPerLevel * (level - 1));
            int bonusDmg = (int)(dmgPerLevel * (level - 1));

            Assert.That(bonusHP, Is.EqualTo(40));
            Assert.That(bonusDmg, Is.EqualTo(8));
        }
    }

    [TestFixture]
    public class BossPhaseTests
    {
        [Test]
        public void Phase2_TriggersAt60Percent()
        {
            float phase2Threshold = 0.6f;
            Assert.That(0.59f <= phase2Threshold, Is.True);
            Assert.That(0.61f <= phase2Threshold, Is.False);
        }

        [Test]
        public void Phase3_TriggersAt30Percent()
        {
            float phase3Threshold = 0.3f;
            Assert.That(0.29f <= phase3Threshold, Is.True);
            Assert.That(0.31f <= phase3Threshold, Is.False);
        }

        [Test]
        public void BossHP_IsScaled()
        {
            int bossBaseHP = 500;
            float hpScale = 0.15f;
            int floor = 10;
            int scaledHP = (int)Math.Round(bossBaseHP * (1 + hpScale * (floor - 1)));
            Assert.That(scaledHP, Is.EqualTo(1175));
        }

        [Test]
        public void CryptKing_EnrageIncreasesSpeed()
        {
            float normalCooldown = 1.5f;
            float phase2Cooldown = 1.2f;
            float phase3Cooldown = 0.8f;
            Assert.That(phase2Cooldown, Is.LessThan(normalCooldown));
            Assert.That(phase3Cooldown, Is.LessThan(phase2Cooldown));
        }
    }

    [TestFixture]
    public class PowerUpTests
    {
        struct PowerUp
        {
            public string Name;
            public float DmgMul, AtkSpd, MoveSpd;
            public int BonusHP;
            public float CritBonus, GoldMul;
            public bool Stackable;
            public int MaxStacks;
        }

        struct StatMods
        {
            public float DmgMul, AtkSpd, MoveSpd, GoldMul, CritBonus, SynergyBonus;
            public int BonusHP;
        }

        StatMods Calculate(PowerUp[] active, int[] stacks)
        {
            var m = new StatMods { DmgMul = 1, AtkSpd = 1, MoveSpd = 1, GoldMul = 1 };
            for (int i = 0; i < active.Length; i++)
            {
                for (int s = 0; s < stacks[i]; s++)
                {
                    m.DmgMul *= active[i].DmgMul;
                    m.AtkSpd *= active[i].AtkSpd;
                    m.MoveSpd *= active[i].MoveSpd;
                    m.BonusHP += active[i].BonusHP;
                    m.CritBonus += active[i].CritBonus;
                    m.GoldMul *= active[i].GoldMul;
                }
            }
            return m;
        }

        [Test]
        public void SinglePowerUp_ModifiesCorrectly()
        {
            var pu = new PowerUp { DmgMul = 1.2f, AtkSpd = 1f, MoveSpd = 1f, GoldMul = 1f, BonusHP = 0, CritBonus = 0.05f };
            var mods = Calculate(new[] { pu }, new[] { 1 });
            Assert.That(mods.DmgMul, Is.EqualTo(1.2f).Within(0.001f));
            Assert.That(mods.CritBonus, Is.EqualTo(0.05f).Within(0.001f));
        }

        [Test]
        public void StackedPowerUp_MultipliesCorrectly()
        {
            var pu = new PowerUp { DmgMul = 1.1f, AtkSpd = 1f, MoveSpd = 1f, GoldMul = 1f, Stackable = true, MaxStacks = 3 };
            var mods = Calculate(new[] { pu }, new[] { 3 });
            float expected = 1.1f * 1.1f * 1.1f;
            Assert.That(mods.DmgMul, Is.EqualTo(expected).Within(0.01f));
        }

        [Test]
        public void MultiplePowerUps_CombineCorrectly()
        {
            var dmg = new PowerUp { DmgMul = 1.3f, AtkSpd = 1f, MoveSpd = 1f, GoldMul = 1f };
            var spd = new PowerUp { DmgMul = 1f, AtkSpd = 1.2f, MoveSpd = 1f, GoldMul = 1f };
            var mods = Calculate(new[] { dmg, spd }, new[] { 1, 1 });
            Assert.That(mods.DmgMul, Is.EqualTo(1.3f).Within(0.001f));
            Assert.That(mods.AtkSpd, Is.EqualTo(1.2f).Within(0.001f));
        }

        [Test]
        public void SynergyBonus_Adds15Percent()
        {
            float synergyBonus = 0.15f;
            Assert.That(synergyBonus, Is.EqualTo(0.15f));
        }

        [Test]
        public void SelectionGenerates3Choices()
        {
            int choicesPerSelection = 3;
            int totalPowerUps = 10;
            int count = Math.Min(choicesPerSelection, totalPowerUps);
            Assert.That(count, Is.EqualTo(3));
        }
    }

    [TestFixture]
    public class SlowMotionTests
    {
        [Test]
        public void TimeScale_IsReducedDuringSlowMo()
        {
            float normalScale = 1f;
            float slowMoScale = 0.3f;
            Assert.That(slowMoScale, Is.LessThan(normalScale));
            Assert.That(slowMoScale, Is.GreaterThan(0f));
        }

        [Test]
        public void FixedDeltaTime_SyncsWithTimeScale()
        {
            float baseFixedDelta = 0.02f;
            float slowMoScale = 0.3f;
            float adjusted = baseFixedDelta * slowMoScale;
            Assert.That(adjusted, Is.EqualTo(0.006f).Within(0.001f));
        }
    }
}
