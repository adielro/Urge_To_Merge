using UnityEngine;
using TMPro;

/// <summary>
/// Manages UI element updates.
/// </summary>
public class UIManager : SingletonInstance<UIManager>
{
    [SerializeField] private TMP_Text goalNumberText;
    [SerializeField] private SpriteRenderer borderSpriteRenderer;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text energyTimerText;
    [SerializeField] private Color plusColor;
    [SerializeField] private Color minusColor;

    protected override void Awake()
    {
        base.Awake();
        GameEvents.OnGoalNumberReached += GoalReachedHandler;
    }
    
    /// <summary>
    /// Updates the goal number display.
    /// </summary>
    public void UpdateGoalNumber(int goalNumber)
    {
        goalNumberText.text = "Goal: " + goalNumber.ToString();
    }

    private void GoalReachedHandler(NumberTile tile)
    {
    }

    /// <summary>
    /// Updates the energy display.
    /// </summary>
    public void UpdateEnergy(int current, int max)
    {
        energyText.text = $"ENERGY {current}/{max}";
    }

    /// <summary>
    /// Updates the energy regeneration timer display.
    /// </summary>
    public void UpdateEnergyTimer(float secondsRemaining)
    {
        if (secondsRemaining <= 0)
        {
            energyTimerText.text = "Next in --:--";
            return;
        }
        
        int minutes = Mathf.FloorToInt(secondsRemaining / 60);
        int seconds = Mathf.FloorToInt(secondsRemaining % 60);
        energyTimerText.text = $"Next in {minutes:00}:{seconds:00}";
    }

    private void OnDestroy()
    {
        GameEvents.OnGoalNumberReached -= GoalReachedHandler;
    }

    /// <summary>
    /// Sets the border color to indicate addition mode.
    /// </summary>
    public void SetBorderToPlus()
    {
        borderSpriteRenderer.color = plusColor;
    }

    /// <summary>
    /// Sets the border color to indicate subtraction mode.
    /// </summary>
    public void SetBorderToMinus()
    {
        borderSpriteRenderer.color = minusColor;
    }
}
