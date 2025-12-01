using UnityEngine;

public class EnergySystem : SingletonInstance<EnergySystem>
{
    [SerializeField] private int maxEnergy = 10;
    [SerializeField] private float regenIntervalSeconds = 180f; // 3 minutes per energy
    [SerializeField] private int startEnergy = 10;
    [SerializeField] private int energyRewardOnGoal = 5;

    private int currentEnergy;
    private float regenTimer;
    private float uiUpdateTimer;
    private const float UI_UPDATE_INTERVAL = 1f; // Update UI every second

    public int CurrentEnergy => currentEnergy;
    public int MaxEnergy => maxEnergy;
    public float RegenTimer => regenTimer;

    protected override void Awake()
    {
        base.Awake();
        currentEnergy = Mathf.Clamp(startEnergy, 0, maxEnergy);
        GameEvents.OnGoalNumberReached += OnGoalReached;
    }

    private void Start()
    {
        UIManager.Instance?.UpdateEnergy(currentEnergy, maxEnergy);
    }

    private void OnDestroy()
    {
        GameEvents.OnGoalNumberReached -= OnGoalReached;
    }

    /// <summary>
    /// Rewards player with energy when goal is reached.
    /// </summary>
    private void OnGoalReached(NumberTile tile)
    {
        ChangeEnergy(energyRewardOnGoal);
    }

    /// <summary>
    /// Sets the current energy to a specific value (clamped to max).
    /// </summary>
    public void SetEnergy(int value)
    {
        int previousEnergy = currentEnergy;
        currentEnergy = Mathf.Clamp(value, 0, maxEnergy);
        
        // Reset timer if we reached max energy from below max
        if (previousEnergy < maxEnergy && currentEnergy >= maxEnergy)
        {
            regenTimer = 0;
        }
        
        UIManager.Instance?.UpdateEnergy(currentEnergy, maxEnergy);
    }

    /// <summary>
    /// Restores energy based on time passed while offline.
    /// </summary>
    public void RestoreEnergyFromOfflineTime(int savedEnergy, float savedRegenTimer, long secondsPassed)
    {
        // Start with saved timer progress
        float totalTimePassed = savedRegenTimer + secondsPassed;
        
        // Calculate how much energy should have regenerated
        int energyToAdd = (int)(totalTimePassed / regenIntervalSeconds);
        float remainingTime = totalTimePassed % regenIntervalSeconds;
        
        int restoredEnergy = savedEnergy + energyToAdd;
        SetEnergy(restoredEnergy);
        
        // Set the timer to the remaining partial progress
        regenTimer = remainingTime;
    }

    private void Update()
    {
        // Simple in-session regen (offline regen can be added later)
        if (currentEnergy >= maxEnergy)
        {
            if (uiUpdateTimer != UI_UPDATE_INTERVAL)
            {
                UIManager.Instance?.UpdateEnergyTimer(0);
                uiUpdateTimer = UI_UPDATE_INTERVAL; // Prevent further updates until timer resets
            }
            return;
        }

        regenTimer += Time.deltaTime;
        uiUpdateTimer += Time.deltaTime;
        
        // Update timer UI only once per second
        if (uiUpdateTimer >= UI_UPDATE_INTERVAL)
        {
            float timeRemaining = regenIntervalSeconds - regenTimer;
            UIManager.Instance?.UpdateEnergyTimer(timeRemaining);
            uiUpdateTimer = 0;
        }
        
        if (regenTimer >= regenIntervalSeconds)
        {
            regenTimer -= regenIntervalSeconds;
            ChangeEnergy(1);
        }
    }

    public bool HasEnergy(int amount = 1)
    {
        return currentEnergy >= amount;
    }

    public bool TrySpendEnergy(int amount = 1)
    {
        if (!HasEnergy(amount))
            return false;

        ChangeEnergy(-amount);
        return true;
    }

    public void ChangeEnergy(int delta)
    {
        int previousEnergy = currentEnergy;
        currentEnergy += delta;
        
        // Only clamp to prevent negative energy
        if (currentEnergy < 0) currentEnergy = 0;
        
        // Don't clamp to max - allow over-max energy (e.g., from rewards)
        
        // Reset timer if we reached max energy from below max
        if (previousEnergy < maxEnergy && currentEnergy >= maxEnergy)
        {
            regenTimer = 0;
            uiUpdateTimer = 0; // Reset UI timer to update immediately
        }
        
        UIManager.Instance?.UpdateEnergy(currentEnergy, maxEnergy);
    }
}
