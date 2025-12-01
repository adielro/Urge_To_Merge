using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneSpinWheel
{
    /// <summary>
    /// Manages the fortune spin wheel mechanics, including spinning, reward distribution, and UI updates.
    /// </summary>
    public class FortuneSpinWheel : MonoBehaviour
    {
        [SerializeField] private RewardData[] m_RewardData;
        [SerializeField] private Image m_CircleBase;
        [SerializeField] private Image[] m_RewardPictures;
        [SerializeField] private GameObject m_RewardPanel;
        [SerializeField] private Image m_RewardFinalImage;
        [SerializeField] private Image m_SpinButton;
        [SerializeField] private CanvasGroup m_CanvasGroup;

        private bool m_IsSpinning = false;
        private float m_SpinSpeed = 0;
        private float m_Rotation = 0;
        private int m_RewardNumber = -1;

        public bool IsSpinning => m_IsSpinning;

        /// <summary>
        /// Initialize wheel state and shuffle rewards when the object is enabled.
        /// </summary>
        private void OnEnable()
        {
            SoundManager.Instance?.PlayWheelStartSound();

            m_Rotation = 0;
            m_IsSpinning = false;
            m_RewardNumber = -1;

            ShuffleArray(m_RewardData);

            for (int i = 0; i < m_RewardData.Length; i++)
            {
                m_RewardPictures[i].sprite = m_RewardData[i].m_Icon;
                m_RewardPictures[i].transform.localScale = Vector3.one;
            }

            if (m_CanvasGroup == null) m_CanvasGroup = GetComponentInChildren<CanvasGroup>();
            StartCoroutine(AnimateOpen());
        }

        private IEnumerator AnimateOpen()
        {
            transform.localScale = Vector3.zero;
            if (m_CanvasGroup != null) m_CanvasGroup.alpha = 0;

            float duration = 0.4f;
            float elapsed = 0;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                
                // Ease Out Back
                float c1 = 1.70158f;
                float c3 = c1 + 1;
                float scale = 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
                
                transform.localScale = Vector3.one * scale;
                
                if (m_CanvasGroup != null) m_CanvasGroup.alpha = t;

                yield return null;
            }

            transform.localScale = Vector3.one;
            if (m_CanvasGroup != null) m_CanvasGroup.alpha = 1;
        }

        /// <summary>
        /// Shuffles the reward data array using Fisher-Yates algorithm.
        /// </summary>
        private void ShuffleArray(RewardData[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (array[randomIndex], array[i]) = (array[i], array[randomIndex]);
            }
        }

        private void Update()
        {
            if (m_IsSpinning)
            {
                m_RewardPanel.gameObject.SetActive(false);
                
                // Decelerate the spin
                if (m_SpinSpeed > 2)
                {
                    m_SpinSpeed -= 6 * Time.deltaTime;
                }
                else
                {
                    m_SpinSpeed -= .85f * Time.deltaTime;
                }
                
                m_Rotation += 100 * Time.deltaTime * m_SpinSpeed;
                m_CircleBase.transform.localRotation = Quaternion.Euler(0, 0, m_Rotation);
                
                // Keep reward pictures upright
                for (int i = 0; i < 6; i++)
                {
                    m_RewardPictures[i].transform.rotation = Quaternion.identity;
                }
                
                // Check if spin has stopped
                if (m_SpinSpeed <= 0)
                {
                    m_SpinSpeed = 0;
                    m_IsSpinning = false;
                    m_RewardNumber = (int)((m_Rotation % 360) / 60);

                    StartCoroutine(ShowRewardMenu(1));
                    HandleReward();
                }
            }
            else
            {
                // Animate the selected reward
                if (m_RewardNumber != -1)
                {
                    m_RewardPictures[m_RewardNumber].transform.localScale = (1 + 0.2f * Mathf.Sin(10 * Time.time)) * Vector3.one;
                }
            }
        }

        /// <summary>
        /// Handles the reward based on the landed segment.
        /// </summary>
        public void HandleReward()
        {
            RewardData reward = m_RewardData[m_RewardNumber];

            switch (reward.m_Type)
            {
                case RewardType.ElementalChaos:
                    BonusSystem.Instance.ApplyElementalChaos();
                    break;

                case RewardType.DoubleMerge:
                    BonusSystem.Instance.ActivateDoubleMerge();
                    break;

                case RewardType.Mystery:
                    BonusSystem.Instance.QueueMysteryTile(reward.m_Count);
                    break;

                case RewardType.Chaos:
                    BonusSystem.Instance.ApplyChaosEffect();
                    break;

                case RewardType.Energy:
                    EnergySystem.Instance.ChangeEnergy(reward.m_Count);
                    break;

                case RewardType.Nothing:
                    break;

                default:
                    Debug.LogWarning($"Wheel: Unhandled reward type {reward.m_Type}");
                    break;
            }
        }

        /// <summary>
        /// Displays the reward panel and waits for player input before resetting.
        /// </summary>
        private IEnumerator ShowRewardMenu(int seconds)
        {
            RewardData reward = m_RewardData[m_RewardNumber];
            yield return new WaitForSeconds(seconds);
            
            if (reward.m_Type != RewardType.Nothing)
            {
                SoundManager.Instance?.PlayRewardSound();
                m_RewardPanel.gameObject.SetActive(true);
                m_RewardFinalImage.sprite = reward.m_Icon;
                
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.touchCount > 0);
            }

            yield return new WaitForSeconds(.1f);
            Reset();
        }

        /// <summary>
        /// Starts the wheel spin with a random speed.
        /// </summary>
        public void StartSpin()
        {
            if (!m_IsSpinning)
            {
                m_SpinSpeed = Random.Range(4f, 14f);
                m_IsSpinning = true;
                m_RewardNumber = -1;
                m_SpinButton.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Resets the wheel to its initial state.
        /// </summary>
        public void Reset()
        {
            m_Rotation = 0;
            m_CircleBase.transform.localRotation = Quaternion.identity;
            m_IsSpinning = false;
            m_RewardNumber = -1;
            m_SpinButton.gameObject.SetActive(true);
            m_RewardPanel.gameObject.SetActive(false);
            
            for (int i = 0; i < m_RewardPictures.Length; i++)
            {
                m_RewardPictures[i].transform.localScale = Vector3.one;
            }
            
            gameObject.SetActive(false);
        }
    }
}
