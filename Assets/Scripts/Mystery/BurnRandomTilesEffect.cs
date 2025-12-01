using UnityEngine;

/// <summary>
/// Burns 1-3 random tiles for 2-6 turns, causing gradual damage.
/// </summary>
public class BurnRandomTilesEffect : IMysteryEffect
{
    public MysteryEffectType Type => MysteryEffectType.BurnRandomTiles;

    public void Execute()
    {
        var allTiles = NumberSlotGenerator.Instance.GetAllActiveTiles();
        if (allTiles.Count == 0) return;

        int tilesToBurn = Random.Range(1, 4);
        tilesToBurn = Mathf.Min(tilesToBurn, allTiles.Count);

        for (int i = 0; i < tilesToBurn; i++)
        {
            int randomIndex = Random.Range(0, allTiles.Count);
            NumberTile tile = allTiles[randomIndex];
            allTiles.RemoveAt(randomIndex);

            int burnDuration = Random.Range(2, 7);
            tile.Burn(burnDuration);
        }
    }
}