using UnityEngine;

/// <summary>
/// Spawns a new mystery/transform tile on the board.
/// </summary>
public class SpawnMysteryTileEffect : IMysteryEffect
{
    public MysteryEffectType Type => MysteryEffectType.SpawnMysteryTile;

    public void Execute()
    {
        NumberTile tile = NumberSlotGenerator.Instance.SpawnTileInRandomSlot(isMysteryTile: true);
    }
}
