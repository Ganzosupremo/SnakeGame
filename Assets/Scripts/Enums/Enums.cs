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