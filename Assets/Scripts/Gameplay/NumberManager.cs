using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages number generation, difficulty progression, and merge strategies.
/// </summary>
public class NumberManager : SingletonInstance<NumberManager>
{
    [SerializeField] private int goalNumberRange;
    [SerializeField] private int goalNumberExpander;
    [SerializeField] private int baseDifficultyDivisor = 2;
    [SerializeField] private int difficultyIncreaseInterval = 100;
    private Dictionary<MergeType, IMergeStrategy> mergeStrategies = new Dictionary<MergeType, IMergeStrategy>();
    private IMergeStrategy currentMergeStrategy;

    public int GoalNumberRange => goalNumberRange;

    /// <summary>
    /// Sets the goal number range (used for loading saves).
    /// </summary>
    public void SetGoalNumberRange(int value)
    {
        goalNumberRange = value;
    }

    /// <summary>
    /// Generates a random number for new tiles.
    /// Tiles grow slower than goal to create increasing difficulty.
    /// </summary>
    public int GetRandomNumber()
    {
        int goalNumber = GoalManager.Instance.GoalNumber;
        
        // Calculate current divisor based on progression
        int progressionBonus = Mathf.Min(3, goalNumberRange / difficultyIncreaseInterval);
        int currentDivisor = baseDifficultyDivisor + progressionBonus;
        
        int maxTile = Mathf.Max(1, goalNumber / currentDivisor);
        return Random.Range(1, maxTile + 1);
    }

    /// <summary>
    /// Generates a new goal number with progressive difficulty.
    /// </summary>
    public int GetGoalNumber()
    {
        int goalNumber = Random.Range(goalNumberRange / 2, goalNumberRange);
        return goalNumber;
    }

    /// <summary>
    /// Increases difficulty by expanding the goal number range.
    /// </summary>
    public void AdvanceDifficulty()
    {
        goalNumberRange += goalNumberExpander;
    }

    /// <summary>
    /// Merges two numbers using the current merge strategy.
    /// </summary>
    public int MergeNumbers(int value1, int value2)
    {
        currentMergeStrategy ??= GetMergeStrategy(MergeType.Addition); // Default merge strategy

        int res = currentMergeStrategy.Merge(value1, value2);

        if (BonusSystem.Instance != null && BonusSystem.Instance.TryConsumeDoubleMerge())
        {
            res *= 2;
        }

        return res;
    }

    /// <summary>
    /// Previews the result of merging two numbers without applying it.
    /// </summary>
    public int GetMergeResultPreview(int value1, int value2)
    {
        currentMergeStrategy ??= GetMergeStrategy(MergeType.Addition); // Default merge strategy

        int res = currentMergeStrategy.Merge(value1, value2);

        if (BonusSystem.Instance != null && BonusSystem.Instance.IsDoubleMergeActive())
        {
            res *= 2;
        }
        return res;
    }

    /// <summary>
    /// Gets or creates a merge strategy of the specified type.
    /// </summary>
    public IMergeStrategy GetMergeStrategy(MergeType mergeType)
    {
        if (!mergeStrategies.ContainsKey(mergeType))
        {
            switch (mergeType)
            {
                case MergeType.Addition:
                    mergeStrategies.Add(mergeType, new AdditionMerge());
                    break;
                case MergeType.Substraction:
                    mergeStrategies.Add(mergeType, new SubstractionMerge());
                    break;
            }
        }

        return mergeStrategies[mergeType];
    }

    /// <summary>
    /// Sets the active merge strategy by type index.
    /// </summary>
    public void SetCurrentMergeStrategy(int mergeType)
    {
        currentMergeStrategy = GetMergeStrategy((MergeType)mergeType);
    }

    /// <summary>
    /// Returns the currently active merge strategy type.
    /// </summary>
    public MergeType GetCurrentMergeStrategy()
    {
        if (currentMergeStrategy is AdditionMerge)
            return MergeType.Addition;
        else if (currentMergeStrategy is SubstractionMerge)
            return MergeType.Substraction;

        else
            currentMergeStrategy = GetMergeStrategy(MergeType.Addition); // Default merge strategy

        return MergeType.Addition;
    }

    public enum MergeType
    {
        Addition,
        Substraction
    }
}
