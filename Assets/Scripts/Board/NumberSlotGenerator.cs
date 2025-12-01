using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Spawns number tiles into available slots on the board.
/// </summary>
public class NumberSlotGenerator : SingletonInstance<NumberSlotGenerator>
{
    [SerializeField] private GameObject generateButton;
    [SerializeField] private readonly int energyCostPerTile = 1;
    [SerializeField, Range(0f, 1f)] private float wheelTriggerChance = 0.07f;
    [SerializeField, Range(0f, 1f)] private float mysteryTileChance = 0.05f;
    [SerializeField] private FortuneSpinWheel.FortuneSpinWheel fortuneWheel;
    private List<SlotTile> slotTiles = new List<SlotTile>();

    /// <summary>
    /// Registers a slot tile to track availability.
    /// </summary>
    public void RegisterSlot(SlotTile slot)
    {
        slotTiles.Add(slot);
    }

    /// <summary>
    /// Generates a new number tile in a random free slot.
    /// </summary>
    public void GenerateNumberTile()
    {
        if (GetRandomFreeSlot() == null || !EnergySystem.Instance.TrySpendEnergy(energyCostPerTile))
            return;

        if (TryTriggerWheel()) return;

        // Check if we have a pending mystery/transform tile from the wheel or random chance
        bool isMysteryTile = BonusSystem.Instance?.TryConsumeMysteryTile() == true || Random.value <= mysteryTileChance;

        SpawnTileInRandomSlot(isMysteryTile);
        
        // Raise event for turn-based effects
        GameEvents.RaiseTileGenerated();
    }

    /// <summary>
    /// Spawns a tile in a random free slot without energy cost or wheel trigger.
    /// </summary>
    /// <param name="isMysteryTile">Whether to activate transform mode on this tile.</param>
    /// <returns>The spawned NumberTile, or null if no free slots.</returns>
    public NumberTile SpawnTileInRandomSlot(bool isMysteryTile = false)
    {
        SlotTile slot = GetRandomFreeSlot();
        if (slot == null) return null; // No free slots available

        GameObject numberTileObj = ObjectPool.Instance.GetNumberTile();
        NumberTile numberTile = numberTileObj.GetComponent<NumberTile>();

        if (isMysteryTile)
        {
            numberTile.ActivateTransformMode();
        }

        // Normal number assignment
        numberTile.NumberValue = NumberManager.Instance.GetRandomNumber();
        numberTile.AssignSlot(slot);
        numberTileObj.SetActive(true);

        // Animate from button
        if (generateButton != null)
        {
            numberTile.StartFlyAnimation(generateButton.transform.position, slot.transform.position);
        }

        slot.OccupySlot(numberTile);

        return numberTile;
    }

    /// <summary>
    /// Returns a random unoccupied slot, or null if none available.
    /// </summary>
    private SlotTile GetRandomFreeSlot()
    {
        var freeSlots = slotTiles.Where(slot => !slot.IsOccupied).ToList();
        if (freeSlots.Count == 0) return null; // No free slots available
        return freeSlots[Random.Range(0, freeSlots.Count)];
    }

    /// <summary>
    /// Gets all active number tiles on the board.
    /// </summary>
    public List<NumberTile> GetAllActiveTiles()
    {
        return slotTiles
            .Where(slot => slot.IsOccupied)
            .Select(slot => slot.Tile)
            .Where(tile => tile != null)
            .ToList();
    }

    /// <summary>
    /// Gets all slot tiles on the board.
    /// </summary>
    public List<SlotTile> GetAllSlots()
    {
        return slotTiles;
    }

    /// <summary>
    /// Spawns a tile in a specific slot (used for loading saves).
    /// </summary>
    public NumberTile SpawnTileInSlot(SlotTile slot, bool isMysteryTile = false)
    {
        if (slot == null || slot.IsOccupied) return null;

        GameObject numberTileObj = ObjectPool.Instance.GetNumberTile();
        NumberTile numberTile = numberTileObj.GetComponent<NumberTile>();

        if (isMysteryTile)
        {
            numberTile.ActivateTransformMode();
        }

        numberTile.NumberValue = NumberManager.Instance.GetRandomNumber();
        numberTile.AssignSlot(slot);
        numberTileObj.SetActive(true);
        slot.OccupySlot(numberTile);

        return numberTile;
    }

    /// <summary>
    /// Attempts to trigger the fortune wheel based on chance.
    /// </summary>
    private bool TryTriggerWheel()
    {
        float roll = Random.value;
        if (roll <= wheelTriggerChance)
        {
            return TriggerWheelManually();
        }
        return false;
    }

    /// <summary>
    /// Manually triggers the fortune wheel (for mystery effects).
    /// </summary>
    public bool TriggerWheelManually()
    {
        if (fortuneWheel == null || fortuneWheel.IsSpinning)
            return false;

        fortuneWheel.gameObject.SetActive(true);
        return true;
    }
}
