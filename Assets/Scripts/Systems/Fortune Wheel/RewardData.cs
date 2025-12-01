using UnityEngine;

namespace FortuneSpinWheel
{
    /// <summary>
    /// ScriptableObject that defines a reward for the fortune wheel.
    /// </summary>
    [CreateAssetMenu(fileName = "RewardData", menuName = "CustomObjects/RewardData", order = 1)]
    public class RewardData : ScriptableObject
    {
        public RewardType m_Type = RewardType.Energy;
        public Sprite m_Icon;
        public int m_Count = 1;
    }
}
