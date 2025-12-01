using UnityEngine;
using TMPro;

/// <summary>
/// Manages freeze and burn status effects on tiles.
/// </summary>
public class TileStatusEffects : MonoBehaviour
{
    [SerializeField] private EffectController frostController;
    [SerializeField] private EffectController fireController;
    [SerializeField] private TMP_Text freezeCounterText;
    [SerializeField] private TMP_Text burnCounterText;
    
    private NumberTile tile;
    private int freezeTurnsRemaining;
    private int burnTurnsRemaining;

    public bool IsFrozen => freezeTurnsRemaining > 0;
    public bool IsBurning => burnTurnsRemaining > 0;
    public int FreezeTurnsRemaining => freezeTurnsRemaining;
    public int BurnTurnsRemaining => burnTurnsRemaining;

    private void Awake()
    {
        tile = GetComponent<NumberTile>();
    }

    /// <summary>
    /// Freezes this tile for a specified number of turns.
    /// </summary>
    public void Freeze(int turns)
    {
        freezeTurnsRemaining = turns;
        frostController?.Activate();

        if (freezeCounterText != null)
        {
            freezeCounterText.gameObject.SetActive(true);
            freezeCounterText.text = turns.ToString();
        }

        // Subscribe to merge events
        GameEvents.OnNumberMerged -= DecrementFreeze;
        GameEvents.OnNumberMerged += DecrementFreeze;
    }

    /// <summary>
    /// Burns this tile for a specified number of turns, reducing value by 25% each turn.
    /// </summary>
    public void Burn(int turns)
    {
        burnTurnsRemaining = turns;
        fireController?.Activate();

        if (burnCounterText != null)
        {
            burnCounterText.gameObject.SetActive(true);
            burnCounterText.text = turns.ToString();
        }

        // Subscribe to merge events
        GameEvents.OnNumberMerged -= DecrementBurn;
        GameEvents.OnNumberMerged += DecrementBurn;
    }

    /// <summary>
    /// Decrements freeze turns. Called when a merge happens.
    /// </summary>
    private void DecrementFreeze(NumberTile mergedTile)
    {
        if (freezeTurnsRemaining > 0)
        {
            freezeTurnsRemaining--;

            if (freezeCounterText != null)
            {
                freezeCounterText.text = freezeTurnsRemaining.ToString();
            }

            if (freezeTurnsRemaining == 0)
            {
                frostController?.Deactivate();
                if (freezeCounterText != null) freezeCounterText.gameObject.SetActive(false);

                // Unsubscribe when no longer frozen
                GameEvents.OnNumberMerged -= DecrementFreeze;
            }
        }
    }

    /// <summary>
    /// Clears freeze state immediately.
    /// </summary>
    public void ClearFreeze()
    {
        freezeTurnsRemaining = 0;
        frostController?.Deactivate();
        if (freezeCounterText != null) freezeCounterText.gameObject.SetActive(false);

        // Unsubscribe from merge events
        GameEvents.OnNumberMerged -= DecrementFreeze;
    }

    /// <summary>
    /// Decrements burn turns and reduces tile value by 25%.
    /// </summary>
    private void DecrementBurn(NumberTile mergedTile)
    {
        if (burnTurnsRemaining > 0)
        {
            // Reduce value by 25%, minimum 1 damage, minimum final value 1
            int damage = Mathf.Max(1, Mathf.RoundToInt(tile.NumberValue * 0.25f));
            tile.NumberValue = Mathf.Max(1, tile.NumberValue - damage);

            burnTurnsRemaining--;

            if (burnCounterText != null)
            {
                burnCounterText.text = burnTurnsRemaining.ToString();
            }

            if (burnTurnsRemaining == 0) ClearBurn();
        }
    }

    /// <summary>
    /// Clears burn state immediately.
    /// </summary>
    public void ClearBurn()
    {
        burnTurnsRemaining = 0;

        if (fireController != null)
            fireController.Deactivate();

        if (burnCounterText != null) burnCounterText.gameObject.SetActive(false);

        // Unsubscribe from merge events
        GameEvents.OnNumberMerged -= DecrementBurn;
    }
}
