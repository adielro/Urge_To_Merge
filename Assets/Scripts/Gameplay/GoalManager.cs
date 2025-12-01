using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the goal number and tracks completion.
/// </summary>
public class GoalManager : SingletonInstance<GoalManager>
{
    [SerializeField] private int goalNumber;

    protected override void Awake()
    {
        base.Awake();
        GameEvents.OnNumberMerged += CheckGoalNumber;
    }

    public int GoalNumber
    {
        get => goalNumber;
        set
        {
            goalNumber = value;
            UIManager.Instance.UpdateGoalNumber(goalNumber);
        }
    }

    private void Start()
    {
        SetGoalNumber();
    }

    /// <summary>
    /// Generates a new goal number.
    /// </summary>
    public void SetGoalNumber()
    {
        int newGoalNumber;
        do
        {
            newGoalNumber = NumberManager.Instance.GetGoalNumber();
        } while (newGoalNumber == goalNumber);

        GoalNumber = newGoalNumber;
        UIManager.Instance.UpdateGoalNumber(goalNumber);
    }

    /// <summary>
    /// Sets the goal number to a specific value (used for loading saves).
    /// </summary>
    public void SetGoalNumber(int value)
    {
        GoalNumber = value;
    }

    /// <summary>
    /// Checks if a merged tile matches the goal number.
    /// </summary>
    private void CheckGoalNumber(NumberTile tile)
    {
        if (tile.NumberValue == goalNumber)
        {
            OnGoalCompleted(tile);
            
            // Check if any existing tiles already match the new goal after a delay
            StartCoroutine(CheckExistingTilesAfterDelay(1f));
        }
    }

    /// <summary>
    /// Handles goal completion: advances difficulty, sets new goal, triggers events.
    /// </summary>
    private void OnGoalCompleted(NumberTile tile)
    {
        NumberManager.Instance.AdvanceDifficulty();
        SetGoalNumber();
        GameEvents.RaiseGoalNumberReached(tile);
        tile.CelebrateGoal();
    }

    /// <summary>
    /// Waits before checking existing tiles for the goal.
    /// </summary>
    private IEnumerator CheckExistingTilesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CheckExistingTilesForGoal();
    }

    /// <summary>
    /// Checks all tiles on the board for the current goal number.
    /// </summary>
    private void CheckExistingTilesForGoal()
    {
        var allTiles = NumberSlotGenerator.Instance.GetAllActiveTiles();
        foreach (var tile in allTiles)
        {
            if (tile.NumberValue == goalNumber)
            {
                OnGoalCompleted(tile);
                
                // Recursively check again in case the new goal also exists
                CheckExistingTilesForGoal();
                break;
            }
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnNumberMerged -= CheckGoalNumber;
    }
}
