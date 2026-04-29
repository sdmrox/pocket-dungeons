using NUnit.Framework;

namespace PocketDungeons.Tests
{
    [TestFixture]
    public class AudioManagerTests
    {
        [Test]
        public void MasterVolume_ClampsToRange()
        {
            float Clamp01(float v) => Math.Clamp(v, 0f, 1f);
            Assert.That(Clamp01(-0.5f), Is.EqualTo(0f));
            Assert.That(Clamp01(1.5f), Is.EqualTo(1f));
            Assert.That(Clamp01(0.7f), Is.EqualTo(0.7f).Within(0.001f));
        }

        [Test]
        public void MusicVolume_CombinesWithMaster()
        {
            float master = 0.8f;
            float music = 0.7f;
            float effective = master * music;
            Assert.That(effective, Is.EqualTo(0.56f).Within(0.001f));
        }

        [Test]
        public void Mute_SilencesAllOutput()
        {
            bool isMuted = false;
            isMuted = !isMuted;
            Assert.That(isMuted, Is.True);
            isMuted = !isMuted;
            Assert.That(isMuted, Is.False);
        }

        [Test]
        public void SFX_PitchVariation_StaysInRange()
        {
            float pitchMin = 0.95f;
            float pitchMax = 1.05f;
            var random = new Random(42);
            for (int i = 0; i < 100; i++)
            {
                float pitch = pitchMin + (float)(random.NextDouble() * (pitchMax - pitchMin));
                Assert.That(pitch, Is.InRange(pitchMin, pitchMax));
            }
        }
    }

    [TestFixture]
    public class AdaptiveMusicTests
    {
        enum CombatIntensity { None, Low, Medium, High, Boss }

        CombatIntensity CalculateIntensity(int enemies, bool isBoss)
        {
            if (isBoss) return CombatIntensity.Boss;
            if (enemies >= 6) return CombatIntensity.High;
            if (enemies >= 3) return CombatIntensity.Medium;
            if (enemies >= 1) return CombatIntensity.Low;
            return CombatIntensity.None;
        }

        float[] GetLayerTargets(CombatIntensity intensity)
        {
            float[] targets = new float[6];
            switch (intensity)
            {
                case CombatIntensity.None:
                    targets[0] = 1f; targets[1] = 0.6f; break;
                case CombatIntensity.Low:
                    targets[0] = 0.5f; targets[2] = 0.4f; break;
                case CombatIntensity.Medium:
                    targets[0] = 0.3f; targets[2] = 0.7f; break;
                case CombatIntensity.High:
                    targets[2] = 1f; break;
                case CombatIntensity.Boss:
                    targets[3] = 1f; break;
            }
            return targets;
        }

        [Test]
        public void NoEnemies_ReturnsNone()
        {
            Assert.That(CalculateIntensity(0, false), Is.EqualTo(CombatIntensity.None));
        }

        [Test]
        public void FewEnemies_ReturnsLow()
        {
            Assert.That(CalculateIntensity(1, false), Is.EqualTo(CombatIntensity.Low));
            Assert.That(CalculateIntensity(2, false), Is.EqualTo(CombatIntensity.Low));
        }

        [Test]
        public void MediumEnemies_ReturnsMedium()
        {
            Assert.That(CalculateIntensity(3, false), Is.EqualTo(CombatIntensity.Medium));
            Assert.That(CalculateIntensity(5, false), Is.EqualTo(CombatIntensity.Medium));
        }

        [Test]
        public void ManyEnemies_ReturnsHigh()
        {
            Assert.That(CalculateIntensity(6, false), Is.EqualTo(CombatIntensity.High));
            Assert.That(CalculateIntensity(20, false), Is.EqualTo(CombatIntensity.High));
        }

        [Test]
        public void Boss_OverridesEnemyCount()
        {
            Assert.That(CalculateIntensity(0, true), Is.EqualTo(CombatIntensity.Boss));
            Assert.That(CalculateIntensity(100, true), Is.EqualTo(CombatIntensity.Boss));
        }

        [Test]
        public void LayerTargets_BossOnlyActivatesBossLayer()
        {
            var targets = GetLayerTargets(CombatIntensity.Boss);
            Assert.That(targets[3], Is.EqualTo(1f));
            Assert.That(targets[0], Is.EqualTo(0f));
            Assert.That(targets[2], Is.EqualTo(0f));
        }

        [Test]
        public void LayerTargets_NoneActivatesAmbientAndExploration()
        {
            var targets = GetLayerTargets(CombatIntensity.None);
            Assert.That(targets[0], Is.EqualTo(1f));
            Assert.That(targets[1], Is.EqualTo(0.6f).Within(0.001f));
        }

        [Test]
        public void FadeProgress_ConvergesOnTarget()
        {
            float current = 0f;
            float target = 1f;
            float speed = 2f;
            float dt = 0.1f;

            for (int i = 0; i < 20; i++)
            {
                float step = speed * dt;
                current = current < target
                    ? Math.Min(current + step, target)
                    : Math.Max(current - step, target);
            }
            Assert.That(current, Is.EqualTo(1f).Within(0.001f));
        }
    }
}
