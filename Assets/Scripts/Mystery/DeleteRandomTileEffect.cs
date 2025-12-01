using UnityEngine;

/// <summary>
/// Removes one random tile from the board.
/// </summary>
public class DeleteRandomTileEffect : IMysteryEffect
{
    public MysteryEffectType Type => MysteryEffectType.DeleteRandomTile;

    public void Execute()
    {
        var activeTiles = NumberSlotGenerator.Instance.GetAllActiveTiles();
        if (activeTiles.Count == 0)
        {
            return;
        }

        var randomTile = activeTiles[Random.Range(0, activeTiles.Count)];

        randomTile.AssignedSlot?.ClearSlot();
        ObjectPool.Instance.ReturnNumberTile(randomTile.gameObject);
    }
}
