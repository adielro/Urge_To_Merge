using System;

/// <summary>
/// Returns absolute difference between two numbers.
/// </summary>
public class SubstractionMerge : IMergeStrategy
{
    public int Merge(int value1, int value2)
    {
        return Math.Abs(value2 - value1);
    }
}
