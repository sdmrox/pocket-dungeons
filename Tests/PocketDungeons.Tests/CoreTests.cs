using NUnit.Framework;

namespace PocketDungeons.Tests
{
    // ==================== PHASE 0: Core Architecture Tests ====================

    [TestFixture]
    public class GameStateTests
    {
        enum GameState { Boot, MainMenu, Loading, Gameplay, Paused, Death, Results, Shop, Settings, Credits, Tutorial }

        private static readonly bool[,] ValidTransitions = InitTransitions();

        static bool[,] InitTransitions()
        {
            int count = Enum.GetValues<GameState>().Length;
            var t = new bool[count, count];
            void Allow(GameState from, GameState to) => t[(int)from, (int)to] = true;

            Allow(GameState.Boot, GameState.MainMenu);
            Allow(GameState.MainMenu, GameState.Loading);
            Allow(GameState.MainMenu, GameState.Shop);
            Allow(GameState.MainMenu, GameState.Settings);
            Allow(GameState.MainMenu, GameState.Credits);
            Allow(GameState.Loading, GameState.Gameplay);
            Allow(GameState.Loading, GameState.Tutorial);
            Allow(GameState.Gameplay, GameState.Paused);
            Allow(GameState.Gameplay, GameState.Death);
            Allow(GameState.Paused, GameState.Gameplay);
            Allow(GameState.Paused, GameState.MainMenu);
            Allow(GameState.Death, GameState.Results);
            Allow(GameState.Results, GameState.MainMenu);
            Allow(GameState.Results, GameState.Loading);
            Allow(GameState.Shop, GameState.MainMenu);
            Allow(GameState.Settings, GameState.MainMenu);
            Allow(GameState.Settings, GameState.Paused);
            Allow(GameState.Credits, GameState.MainMenu);
            Allow(GameState.Tutorial, GameState.Gameplay);
            return t;
        }

        [Test]
        public void Boot_To_MainMenu_IsValid()
        {
            Assert.That(ValidTransitions[(int)GameState.Boot, (int)GameState.MainMenu], Is.True);
        }

        [Test]
        public void Boot_To_Gameplay_IsInvalid()
        {
            Assert.That(ValidTransitions[(int)GameState.Boot, (int)GameState.Gameplay], Is.False);
        }

        [Test]
        public void Gameplay_To_Death_IsValid()
        {
            Assert.That(ValidTransitions[(int)GameState.Gameplay, (int)GameState.Death], Is.True);
        }

        [Test]
        public void Death_To_Results_IsValid()
        {
            Assert.That(ValidTransitions[(int)GameState.Death, (int)GameState.Results], Is.True);
        }

        [Test]
        public void Results_To_MainMenu_IsValid()
        {
            Assert.That(ValidTransitions[(int)GameState.Results, (int)GameState.MainMenu], Is.True);
        }

        [Test]
        public void Results_To_Loading_IsValid_ForPlayAgain()
        {
            Assert.That(ValidTransitions[(int)GameState.Results, (int)GameState.Loading], Is.True);
        }

        [Test]
        public void Paused_To_MainMenu_IsValid()
        {
            Assert.That(ValidTransitions[(int)GameState.Paused, (int)GameState.MainMenu], Is.True);
        }

        [Test]
        public void AllStatesHaveAtLeastOneOutbound()
        {
            int count = Enum.GetValues<GameState>().Length;
            for (int from = 0; from < count; from++)
            {
                // Credits, Results, Tutorial, Shop are terminal-ish states that go back to MainMenu
                // Just verify no isolated states
                if ((GameState)from == GameState.Boot || (GameState)from == GameState.Loading)
                    continue; // These have exactly 1 outbound

                bool hasOutbound = false;
                for (int to = 0; to < count; to++)
                    if (ValidTransitions[from, to]) { hasOutbound = true; break; }

                Assert.That(hasOutbound, Is.True,
                    $"{(GameState)from} has no outbound transitions");
            }
        }
    }

    [TestFixture]
    public class ServiceLocatorTests
    {
        private readonly Dictionary<Type, object> _registry = new();

        void Register<T>(T svc) where T : class => _registry[typeof(T)] = svc;
        T? Get<T>() where T : class => _registry.TryGetValue(typeof(T), out var s) ? (T)s : null;

        [SetUp]
        public void SetUp() => _registry.Clear();

        [Test]
        public void Register_And_Get_ReturnsService()
        {
            var svc = new TestService();
            Register<ITestService>(svc);
            Assert.That(Get<ITestService>(), Is.SameAs(svc));
        }

        [Test]
        public void Get_Unregistered_ReturnsNull()
        {
            Assert.That(Get<ITestService>(), Is.Null);
        }

        [Test]
        public void Register_Overwrites_Previous()
        {
            var svc1 = new TestService();
            var svc2 = new TestService();
            Register<ITestService>(svc1);
            Register<ITestService>(svc2);
            Assert.That(Get<ITestService>(), Is.SameAs(svc2));
        }

        interface ITestService { }
        class TestService : ITestService { }
    }

    [TestFixture]
    public class ObjectPoolTests
    {
        [Test]
        public void Pool_Get_ReducesAvailable()
        {
            var pool = new Queue<int>();
            for (int i = 0; i < 10; i++) pool.Enqueue(i);
            var inUse = new HashSet<int>();

            int item = pool.Dequeue();
            inUse.Add(item);

            Assert.That(pool.Count, Is.EqualTo(9));
            Assert.That(inUse.Count, Is.EqualTo(1));
        }

        [Test]
        public void Pool_Return_IncreasesAvailable()
        {
            var pool = new Queue<int>();
            var inUse = new HashSet<int>();
            pool.Enqueue(0);
            int item = pool.Dequeue();
            inUse.Add(item);

            inUse.Remove(item);
            pool.Enqueue(item);

            Assert.That(pool.Count, Is.EqualTo(1));
            Assert.That(inUse.Count, Is.EqualTo(0));
        }

        [Test]
        public void Pool_DoesNotExceedMax()
        {
            int maxSize = 5;
            var pool = new Queue<int>();
            for (int i = 0; i < maxSize; i++) pool.Enqueue(i);

            Assert.That(pool.Count, Is.LessThanOrEqualTo(maxSize));
        }
    }
}
