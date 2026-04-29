namespace PocketDungeons.Core.StateMachine
{
    public enum GameState
    {
        Boot,           // Initialize services, load config
        MainMenu,       // Title screen, hero select
        Loading,        // Generate dungeon, load assets
        Gameplay,       // Active dungeon run
        Paused,         // Pause menu overlay
        PowerUpSelect,  // Choose 1 of 3 power-ups
        BossIntro,      // Boss cutscene/animation
        Death,          // Death animation + results
        Results,        // Score screen, rewards
        Shop,           // IAP store
        Settings        // Options menu
    }
}
