using UnityEngine;
using System;

/// <summary>
/// Manages bonus inventory and special effect activation.
/// </summary>
public class BonusSystem : SingletonInstance<BonusSystem>
{
    [Header("Inventory")]
    [SerializeField] private bool nextMergeDouble;
    [SerializeField] private int pendingMysteryTiles;

    public static event Action OnInventoryChanged;

    public bool HasPendingMystery => pendingMysteryTiles > 0;
    public bool HasDoubleMerge => nextMergeDouble;

    public void ActivateDoubleMerge()
    {
        nextMergeDouble = true;
        OnInventoryChanged?.Invoke();
    }

    public bool TryConsumeDoubleMerge()
    {
        if (!nextMergeDouble) return false;
        nextMergeDouble = false;
        OnInventoryChanged?.Invoke();
        return true;
    }
    
    public bool IsDoubleMergeActive()
    {
        return nextMergeDouble;
    }

    public void QueueMysteryTile(int amount)
    {
        pendingMysteryTiles += Mathf.Max(0, amount);
        OnInventoryChanged?.Invoke();
    }

    public bool TryConsumeMysteryTile()
    {
        if (pendingMysteryTiles <= 0) return false;
        pendingMysteryTiles--;
        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Randomizes all active number tiles on the board.
    /// </summary>
    public void ApplyChaosEffect()
    {
        var allTiles = NumberSlotGenerator.Instance.GetAllActiveTiles();
        
        foreach (var tile in allTiles)
        {
            tile.NumberValue = NumberManager.Instance.GetRandomNumber();
        }
    }

    /// <summary>
    /// Applies burn and freeze effects to random tiles.
    /// </summary>
    public void ApplyElementalChaos()
    {
        MysteryEffectRunner.Instance.ExecuteEffect(MysteryEffectType.BurnRandomTiles);
        MysteryEffectRunner.Instance.ExecuteEffect(MysteryEffectType.FreezeRandomTiles);
    }
}
