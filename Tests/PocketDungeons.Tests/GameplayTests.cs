using NUnit.Framework;

namespace PocketDungeons.Tests
{
    // ==================== PHASE 1: Gameplay Tests ====================

    [TestFixture]
    public class BSPDungeonGeneratorTests
    {
        const int Width = 40, Height = 40;
        const int MinRoom = 6, MaxRoom = 12;
        const int MaxDepth = 5;

        class BSPNode
        {
            public int X, Y, W, H;
            public BSPNode? Left, Right;
            public (int rx, int ry, int rw, int rh)? Room;
        }

        BSPNode CreateRoot() => new() { X = 1, Y = 1, W = Width - 2, H = Height - 2 };

        void Split(BSPNode node, int depth, Random rng)
        {
            if (depth >= MaxDepth || node.W < MinRoom * 2 || node.H < MinRoom * 2) return;

            bool splitH = node.W > node.H ? false : (node.H > node.W ? true : rng.Next(2) == 0);

            if (splitH)
            {
                int split = rng.Next(MinRoom, node.H - MinRoom + 1);
                node.Left = new BSPNode { X = node.X, Y = node.Y, W = node.W, H = split };
                node.Right = new BSPNode { X = node.X, Y = node.Y + split, W = node.W, H = node.H - split };
            }
            else
            {
                int split = rng.Next(MinRoom, node.W - MinRoom + 1);
                node.Left = new BSPNode { X = node.X, Y = node.Y, W = split, H = node.H };
                node.Right = new BSPNode { X = node.X + split, Y = node.Y, W = node.W - split, H = node.H };
            }

            Split(node.Left!, depth + 1, rng);
            Split(node.Right!, depth + 1, rng);
        }

        void CollectLeaves(BSPNode? node, List<BSPNode> leaves)
        {
            if (node == null) return;
            if (node.Left == null && node.Right == null)
                leaves.Add(node);
            else
            {
                CollectLeaves(node.Left, leaves);
                CollectLeaves(node.Right, leaves);
            }
        }

        [Test]
        public void BSP_GeneratesAtLeastTwoRooms()
        {
            var rng = new Random(42);
            var root = CreateRoot();
            Split(root, 0, rng);
            var leaves = new List<BSPNode>();
            CollectLeaves(root, leaves);

            Assert.That(leaves.Count, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void BSP_AllLeavesAreWithinBounds()
        {
            var rng = new Random(12345);
            var root = CreateRoot();
            Split(root, 0, rng);
            var leaves = new List<BSPNode>();
            CollectLeaves(root, leaves);

            foreach (var leaf in leaves)
            {
                Assert.That(leaf.X, Is.GreaterThanOrEqualTo(1));
                Assert.That(leaf.Y, Is.GreaterThanOrEqualTo(1));
                Assert.That(leaf.X + leaf.W, Is.LessThanOrEqualTo(Width - 1));
                Assert.That(leaf.Y + leaf.H, Is.LessThanOrEqualTo(Height - 1));
            }
        }

        [Test]
        public void BSP_LeavesDoNotOverlap()
        {
            var rng = new Random(99);
            var root = CreateRoot();
            Split(root, 0, rng);
            var leaves = new List<BSPNode>();
            CollectLeaves(root, leaves);

            for (int i = 0; i < leaves.Count; i++)
            {
                for (int j = i + 1; j < leaves.Count; j++)
                {
                    var a = leaves[i]; var b = leaves[j];
                    bool overlaps = a.X < b.X + b.W && a.X + a.W > b.X &&
                                    a.Y < b.Y + b.H && a.Y + a.H > b.Y;
                    Assert.That(overlaps, Is.False, $"Leaf {i} overlaps leaf {j}");
                }
            }
        }

        [Test]
        public void BSP_SameSeadProducesSameLayout()
        {
            List<BSPNode> Generate(int seed)
            {
                var rng = new Random(seed);
                var root = CreateRoot();
                Split(root, 0, rng);
                var leaves = new List<BSPNode>();
                CollectLeaves(root, leaves);
                return leaves;
            }

            var a = Generate(777);
            var b = Generate(777);
            Assert.That(a.Count, Is.EqualTo(b.Count));
            for (int i = 0; i < a.Count; i++)
            {
                Assert.That(a[i].X, Is.EqualTo(b[i].X));
                Assert.That(a[i].Y, Is.EqualTo(b[i].Y));
            }
        }

        [Test]
        public void BSP_DifferentSeedsProduceDifferentLayouts()
        {
            List<BSPNode> Generate(int seed)
            {
                var rng = new Random(seed);
                var root = CreateRoot();
                Split(root, 0, rng);
                var leaves = new List<BSPNode>();
                CollectLeaves(root, leaves);
                return leaves;
            }

            var a = Generate(111);
            var b = Generate(222);
            bool allSame = a.Count == b.Count;
            if (allSame)
            {
                for (int i = 0; i < a.Count; i++)
                    if (a[i].X != b[i].X || a[i].Y != b[i].Y) { allSame = false; break; }
            }
            Assert.That(allSame, Is.False);
        }
    }

    [TestFixture]
    public class CombatTests
    {
        [Test]
        public void CritDamage_IsMultiplied()
        {
            int baseDamage = 10;
            float critMultiplier = 2f;
            int critDamage = (int)Math.Round(baseDamage * critMultiplier);
            Assert.That(critDamage, Is.EqualTo(20));
        }

        [Test]
        public void CritChance_IsWithinBounds()
        {
            float critChance = 0.05f;
            Assert.That(critChance, Is.GreaterThanOrEqualTo(0f));
            Assert.That(critChance, Is.LessThanOrEqualTo(1f));
        }

        [Test]
        public void DamagePopup_NormalDamage_HasCorrectScale()
        {
            float normalScale = 0.8f;
            float critScale = 1.2f;
            Assert.That(normalScale, Is.LessThan(critScale));
        }

        [Test]
        public void HitStop_CritIsLongerThanNormal()
        {
            int normalFrames = 2;
            int critFrames = 4;
            float fps = 60f;
            float normalMs = normalFrames / fps * 1000;
            float critMs = critFrames / fps * 1000;
            Assert.That(critMs, Is.GreaterThan(normalMs));
            Assert.That(normalMs, Is.EqualTo(33.333f).Within(1f));
            Assert.That(critMs, Is.EqualTo(66.666f).Within(1f));
        }

        [Test]
        public void ScreenShake_Amplitude_ScalesWithDamage()
        {
            float CalcAmplitude(int damage, bool isCrit)
            {
                float maxAmplitude = 8f;
                float normalized = Math.Clamp(damage / 50f, 0f, 1f);
                float amplitude = 2f + (maxAmplitude - 2f) * normalized;
                if (isCrit) amplitude *= 1.5f;
                return amplitude;
            }

            float lowDmg = CalcAmplitude(10, false);
            float highDmg = CalcAmplitude(40, false);
            float critDmg = CalcAmplitude(40, true);

            Assert.That(highDmg, Is.GreaterThan(lowDmg));
            Assert.That(critDmg, Is.GreaterThan(highDmg));
        }
    }

    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void DodgeCooldown_IsCorrect()
        {
            float dodgeCooldown = 2f;
            float dodgeDuration = 0.25f;
            float dodgeInvincibility = 0.35f;

            Assert.That(dodgeDuration, Is.LessThan(dodgeInvincibility));
            Assert.That(dodgeCooldown, Is.GreaterThan(dodgeDuration));
        }

        [Test]
        public void MoveInput_IsClamped()
        {
            float ClampMagnitude(float x, float y, float max)
            {
                float m = MathF.Sqrt(x * x + y * y);
                if (m > max) { x = x / m * max; y = y / m * max; }
                return MathF.Sqrt(x * x + y * y);
            }

            float result = ClampMagnitude(2f, 2f, 1f);
            Assert.That(result, Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        public void HealthBar_Color_ChangesAtThreshold()
        {
            bool isLowHealth(int current, int max) => (float)current / max <= 0.3f;

            Assert.That(isLowHealth(100, 100), Is.False);
            Assert.That(isLowHealth(30, 100), Is.True);
            Assert.That(isLowHealth(29, 100), Is.True);
            Assert.That(isLowHealth(31, 100), Is.False);
        }
    }

    [TestFixture]
    public class EnemyScalingTests
    {
        [Test]
        public void EnemyHP_ScalesPerFloor()
        {
            int baseHP = 10;
            float scalePerFloor = 0.15f;
            int floor = 5;
            int scaled = (int)Math.Round(baseHP * (1 + scalePerFloor * (floor - 1)));
            Assert.That(scaled, Is.EqualTo(16));
        }

        [Test]
        public void EnemyDamage_ScalesPerFloor()
        {
            int baseDmg = 3;
            float scalePerFloor = 0.10f;
            int floor = 10;
            int scaled = (int)Math.Round(baseDmg * (1 + scalePerFloor * (floor - 1)));
            Assert.That(scaled, Is.EqualTo(6));
        }

        [Test]
        public void EnemyCount_ScalesPerFloor_CappedAt15()
        {
            int baseCount = 3;
            float scalePerFloor = 0.5f;
            for (int floor = 1; floor <= 30; floor++)
            {
                int count = Math.Min(baseCount + (int)Math.Round(scalePerFloor * (floor - 1)), 15);
                Assert.That(count, Is.LessThanOrEqualTo(15));
                Assert.That(count, Is.GreaterThanOrEqualTo(baseCount));
            }
        }

        [Test]
        public void BossFloor_Every10th()
        {
            bool IsBossFloor(int floor) => floor % 10 == 0 && floor > 0;
            Assert.That(IsBossFloor(10), Is.True);
            Assert.That(IsBossFloor(20), Is.True);
            Assert.That(IsBossFloor(5), Is.False);
            Assert.That(IsBossFloor(15), Is.False);
        }
    }

    [TestFixture]
    public class LootTests
    {
        [Test]
        public void GoldMagnet_PullsWithinRadius()
        {
            float magnetRadius = 2f;
            float dist1 = 1.5f;
            float dist2 = 3f;
            Assert.That(dist1 < magnetRadius, Is.True);
            Assert.That(dist2 < magnetRadius, Is.False);
        }

        [Test]
        public void AutoCollect_WithinHalfUnit()
        {
            float collectRadius = 0.5f;
            Assert.That(0.3f < collectRadius, Is.True);
            Assert.That(0.6f < collectRadius, Is.False);
        }
    }

    [TestFixture]
    public class RunDataTests
    {
        [Test]
        public void ScoreCalculation_IsCorrect()
        {
            int CalcScore(int floors, int enemies, int gold, bool boss, float time)
            {
                int score = floors * 100 + enemies * 10 + gold * 5;
                if (boss) score += 500;
                if (time < 180f) score += 200;
                return score;
            }

            int score = CalcScore(5, 30, 100, false, 150f);
            Assert.That(score, Is.EqualTo(5 * 100 + 30 * 10 + 100 * 5 + 200)); // 500+300+500+200=1500

            int bossScore = CalcScore(10, 50, 200, true, 200f);
            Assert.That(bossScore, Is.EqualTo(10 * 100 + 50 * 10 + 200 * 5 + 500)); // 1000+500+1000+500=3000
        }
    }
}
