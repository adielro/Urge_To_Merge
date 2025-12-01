using UnityEngine;

/// <summary>
/// Rerolls the values of 1 to all tiles on the board.
/// </summary>
public class RerollRandomTilesEffect : IMysteryEffect
{
    public MysteryEffectType Type => MysteryEffectType.RerollRandomTiles;

    public void Execute()
    {
        var allTiles = NumberSlotGenerator.Instance.GetAllActiveTiles();
        
        if (allTiles.Count == 0) return;

        // Random number of tiles to reroll (1 to all tiles on board)
        int tilesToReroll = Random.Range(1, allTiles.Count + 1);

        // Shuffle and take random tiles
        for (int i = 0; i < tilesToReroll; i++)
        {
            int randomIndex = Random.Range(0, allTiles.Count);
            NumberTile tile = allTiles[randomIndex];
            
            // Reroll the number
            tile.NumberValue = NumberManager.Instance.GetRandomNumber();
            
            // Remove from list so we don't reroll the same tile twice
            allTiles.RemoveAt(randomIndex);
        }
    }
}
