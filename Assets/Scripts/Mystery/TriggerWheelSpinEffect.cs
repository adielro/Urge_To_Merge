using UnityEngine;

/// <summary>
/// Triggers the fortune wheel for an immediate bonus.
/// </summary>
public class TriggerWheelSpinEffect : IMysteryEffect
{
    public MysteryEffectType Type => MysteryEffectType.TriggerWheelSpin;

    public void Execute()
    {
        bool success = NumberSlotGenerator.Instance.TriggerWheelManually();
    }
}
