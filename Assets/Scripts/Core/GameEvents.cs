using System;

/// <summary>
/// Central event system for game state changes.
/// </summary>
public static class GameEvents
{
    public static event Action<NumberTile> OnNumberMerged;
    public static event Action<NumberTile> OnGoalNumberReached;
    public static event Action OnTileGenerated;

    public static void RaiseNumberMerged(NumberTile tile)
    {
        OnNumberMerged?.Invoke(tile);
    }

    public static void RaiseGoalNumberReached(NumberTile tile)
    {
        OnGoalNumberReached?.Invoke(tile);
    }

    public static void RaiseTileGenerated()
    {
        OnTileGenerated?.Invoke();
    }
}