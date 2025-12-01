using UnityEngine;

/// <summary>
/// Freezes 1-3 random tiles for 1-6 turns, preventing interaction.
/// </summary>
public class FreezeRandomTilesEffect : IMysteryEffect
{
    public MysteryEffectType Type => MysteryEffectType.FreezeRandomTiles;

    public void Execute()
    {
        var allTiles = NumberSlotGenerator.Instance.GetAllActiveTiles();
        
        if (allTiles.Count == 0)
        {
            return;
        }

        // Random number of tiles to freeze (1-3, or less if fewer tiles exist)
        int tilesToFreeze = Mathf.Min(Random.Range(1, 4), allTiles.Count);
        int freezeTurns = Random.Range(1, 7); // 1-3 turns

        for (int i = 0; i < tilesToFreeze; i++)
        {
            int randomIndex = Random.Range(0, allTiles.Count);
            NumberTile tile = allTiles[randomIndex];
            
            tile.Freeze(freezeTurns);
            
            // Remove from list so we don't freeze the same tile twice
            allTiles.RemoveAt(randomIndex);
        }
    }
}
