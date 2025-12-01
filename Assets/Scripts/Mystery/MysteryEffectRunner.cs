using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages and executes random mystery effects when transform tiles are merged.
/// </summary>
public class MysteryEffectRunner : SingletonInstance<MysteryEffectRunner>
{
    private readonly List<IMysteryEffect> _effects = new List<IMysteryEffect>();

    protected override void Awake()
    {
        base.Awake();
        RegisterEffects();
    }

    private void RegisterEffects()
    {
        _effects.Add(new DeleteRandomTileEffect());
        _effects.Add(new SpawnRandomTilesEffect());
        _effects.Add(new SpawnMysteryTileEffect());
        _effects.Add(new TriggerWheelSpinEffect());
        _effects.Add(new RerollRandomTilesEffect());
        _effects.Add(new FreezeRandomTilesEffect());
        _effects.Add(new BurnRandomTilesEffect());
    }

    /// <summary>
    /// Executes a randomly selected mystery effect.
    /// </summary>
    public void ExecuteRandomEffect()
    {
        if (_effects.Count == 0) return;

        int index = Random.Range(0, _effects.Count);
        IMysteryEffect chosen = _effects[index];

        chosen.Execute();
    }

    /// <summary>
    /// Executes a specific effect by type.
    /// </summary>
    public void ExecuteEffect(MysteryEffectType effectType)
    {
        IMysteryEffect effect = _effects.Find(e => e.Type == effectType);
        effect?.Execute();
    }
}
