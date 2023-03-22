namespace SnakeGame
{
    /// <summary>
    /// Used to know the orientation of a room
    /// </summary>
    public enum Orientation
    {
        North,
        East,
        South,
        West,
        None
    }

    /// <summary>
    /// This enum is used by the <see cref="GameManager"/> to manage the states of the game
    /// </summary>
    public enum GameState
    {
        Started,
        Playing,
        EngagingEnemies,
        BossStage,
        EngagingBoss,
        LevelCompleted,
        GameWon,
        GameLost,
        Paused,
        OverviewMap,
        Restarted
    }

    /// <summary>
    /// Used to determine the direction the player is aiming
    /// </summary>
    public enum AimDirection
    {
        Up,
        UpLeft,
        UpRight,
        Right,
        Left,
        Down
    }

    /// <summary>
    /// To know when to spawn a chest
    /// </summary>
    public enum ChestSpawnEvent
    {
        onRoomEntry,
        onEnemiesDefeated
    }

    /// <summary>
    /// The position the chest will spawn
    /// </summary>
    public enum ChestSpawnPosition
    {
        SpawnerPosition,
        PlayerPosition
    }

    /// <summary>
    /// Define the state of a chest
    /// </summary>
    public enum ChestState
    {
        Closed,
        HealthItem,
        AmmoItem,
        WeaponItem,
        Empty
    }

    public enum Difficulty
    {
        Noob = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3,
        DarkSouls = 4,
        EmotionalDamage = 5
    }

    public enum Abilities
    {
        None,
        /// <summary>
        /// No need to explain what this does
        /// </summary>
        SlowDownTime,
        /// <summary>
        /// The current active weapon fires multiple bullets at the cost of one bullet.
        /// </summary>
        MultipleBullets,
        /// <summary>
        /// GOTTA GO FAST!
        /// </summary>
        Flash,
        /// <summary>
        /// GOTTA FIRE FAST!
        /// </summary>
        QuickFire
    }

    public enum SceneIndex
    {
        MainMenu = 0,
        MainGame = 1,
        Settings = 2,
        HighScores = 3,
    }
}

namespace SnakeGame.PlayerSystem.AbilitySystem
{
    public enum AbilityState
    {
        Ready,
        Active,
        Cooldown
    }
}