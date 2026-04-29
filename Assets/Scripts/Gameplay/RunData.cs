namespace PocketDungeons.Gameplay
{
    /// <summary>
    /// Snapshot of a completed (or failed) run for the results screen.
    /// </summary>
    public class RunData
    {
        public int FloorsCleared;
        public int EnemiesKilled;
        public int GoldCollected;
        public float TotalTimeSeconds;
        public bool ReachedBoss;
        public bool DefeatedBoss;
        public string HeroName;
        public int DamageTaken;
        public int HealthPotionsUsed;
        public int PowerUpsCollected;

        public int ScoreTotal => CalculateScore();

        private int CalculateScore()
        {
            int score = 0;
            score += FloorsCleared * 100;
            score += EnemiesKilled * 10;
            score += GoldCollected * 5;
            if (DefeatedBoss) score += 500;

            // Time bonus: faster = more points
            float minutesTaken = TotalTimeSeconds / 60f;
            if (minutesTaken < 3f)
                score += 200;
            else if (minutesTaken < 5f)
                score += 100;

            return score;
        }
    }
}
