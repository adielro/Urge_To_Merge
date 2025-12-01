using UnityEngine;

/// <summary>
/// Types of mystery effects available in the game.
/// </summary>
public enum MysteryEffectType
{
    DeleteRandomTile,
    SpawnRandomTiles,
    SpawnMysteryTile,
    TriggerWheelSpin,
    RerollRandomTiles,
    FreezeRandomTiles,
    BurnRandomTiles
}

/// <summary>
/// Interface for mystery tile effects that trigger random gameplay events.
/// </summary>
public interface IMysteryEffect
{
    MysteryEffectType Type { get; }
    void Execute();
}
