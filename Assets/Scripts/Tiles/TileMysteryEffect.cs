using UnityEngine;

/// <summary>
/// Manages mystery/transform tile special effects.
/// </summary>
public class TileMysteryEffect : MonoBehaviour
{
    [SerializeField] private EffectController magicController;
    private bool isTransformTile = false;

    public bool IsTransformTile => isTransformTile;

    /// <summary>
    /// Activates the transform mode for mystery tiles.
    /// </summary>
    public void ActivateTransformMode()
    {
        isTransformTile = true;
        magicController?.Activate();
    }

    /// <summary>
    /// Consumes the transform effect after use.
    /// </summary>
    public void ConsumeTransformEffect()
    {
        isTransformTile = false;
        if (magicController != null)
            magicController.Deactivate();
    }

    /// <summary>
    /// Applies the special effect when a transform tile is merged.
    /// </summary>
    public void ApplyTransformMergeEffect()
    {
        MysteryEffectRunner.Instance?.ExecuteRandomEffect();
        ConsumeTransformEffect();
    }
}
