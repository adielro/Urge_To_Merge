using UnityEngine;

/// <summary>
/// Spawns 1-4 new random tiles on the board.
/// </summary>
public class SpawnRandomTilesEffect : IMysteryEffect
{
    private readonly int _minTiles;
    private readonly int _maxTiles;

    public MysteryEffectType Type => MysteryEffectType.SpawnRandomTiles;

    public SpawnRandomTilesEffect(int minTiles = 1, int maxTiles = 4)
    {
        _minTiles = minTiles;
        _maxTiles = maxTiles;
    }

    public void Execute()
    {
        int tilesToSpawn = Random.Range(_minTiles, _maxTiles + 1);

        for (int i = 0; i < tilesToSpawn; i++)
        {
            NumberTile tile = NumberSlotGenerator.Instance.SpawnTileInRandomSlot(isMysteryTile: false);
            if (tile == null)
                break;
        }
    }
}
