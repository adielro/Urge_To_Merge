/// <summary>
/// Adds two numbers together.
/// </summary>
public class AdditionMerge : IMergeStrategy
{
    public int Merge(int value1, int value2)
    {
        return value1 + value2;
    }
}
