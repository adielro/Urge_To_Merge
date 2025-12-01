using UnityEngine;

/// <summary>
/// Board slot that can hold one number tile.
/// </summary>
public class SlotTile : MonoBehaviour
{
    private NumberTile numberTile;

    public bool IsOccupied => numberTile != null;
    public NumberTile Tile => numberTile;

    /// <summary>
    /// Registers this slot with the NumberSlotGenerator on awake.
    /// </summary>
    private void Awake()
    {
        NumberSlotGenerator.Instance.RegisterSlot(this);
    }
    
    /// <summary>
    /// Marks this slot as occupied by the given tile.
    /// </summary>
    public void OccupySlot(NumberTile tile)
    {
        numberTile = tile;
    }

    /// <summary>
    /// Clears the slot, marking it as available.
    /// </summary>
    public void ClearSlot()
    {
        numberTile = null;
    }
}
