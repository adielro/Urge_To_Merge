using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int goalNumber;
    public int goalNumberRange;
    public bool doubleMerge;
    public int mysteryTiles;
    public int energy;
    public float energyRegenTimer;
    public long lastEnergyTimestamp;
    public bool musicEnabled = true;
    public bool sfxEnabled = true;
    public List<TileData> tiles = new List<TileData>();
}

[Serializable]
public class TileData
{
    public int slotIndex;
    public int value;
    public bool isTransform;
    public int freezeTurns;
    public int burnTurns;
}

public class SaveSystem : SingletonInstance<SaveSystem>
{
    private const string SAVE_KEY = "GameSaveData";
    [SerializeField] private float autoSaveInterval = 30f; // Auto-save every 30 seconds
    
    private float autoSaveTimer;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        
        // Subscribe to game events for immediate saves
        GameEvents.OnNumberMerged += OnGameStateChanged;
        GameEvents.OnGoalNumberReached += OnGameStateChanged;
        GameEvents.OnTileGenerated += SaveGame;
        BonusSystem.OnInventoryChanged += SaveGame;
    }

    private void OnDestroy()
    {
        GameEvents.OnNumberMerged -= OnGameStateChanged;
        GameEvents.OnGoalNumberReached -= OnGameStateChanged;
        GameEvents.OnTileGenerated -= SaveGame;
        BonusSystem.OnInventoryChanged -= SaveGame;
    }

    private void Update()
    {
        // Periodic auto-save as backup
        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= autoSaveInterval)
        {
            SaveGame();
            autoSaveTimer = 0f;
        }
    }

    private void OnGameStateChanged(NumberTile tile)
    {
        SaveGame();
    }

    /// <summary>
    /// Saves the current game state to PlayerPrefs.
    /// </summary>
    public void SaveGame()
    {
        SaveData data = new SaveData();

        // Save goal number
        if (GoalManager.Instance != null)
        {
            data.goalNumber = GoalManager.Instance.GoalNumber;
        }

        // Save number manager progression
        if (NumberManager.Instance != null)
        {
            data.goalNumberRange = NumberManager.Instance.GoalNumberRange;
        }

        // Save bonus state
        if (BonusSystem.Instance != null)
        {
            data.doubleMerge = BonusSystem.Instance.HasDoubleMerge;
            data.mysteryTiles = BonusSystem.Instance.HasPendingMystery ? 1 : 0; // Simplified
        }

        // Save energy
        if (EnergySystem.Instance != null)
        {
            data.energy = EnergySystem.Instance.CurrentEnergy;
            data.energyRegenTimer = EnergySystem.Instance.RegenTimer;
            data.lastEnergyTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        // Preserve audio settings if they exist
        if (HasSaveData())
        {
            string existingJson = PlayerPrefs.GetString(SAVE_KEY);
            SaveData existingData = JsonUtility.FromJson<SaveData>(existingJson);
            data.musicEnabled = existingData.musicEnabled;
            data.sfxEnabled = existingData.sfxEnabled;
        }

        // Save all active tiles
        if (NumberSlotGenerator.Instance != null)
        {
            var allSlots = NumberSlotGenerator.Instance.GetAllSlots();
            for (int i = 0; i < allSlots.Count; i++)
            {
                var slot = allSlots[i];
                if (slot.IsOccupied && slot.Tile != null)
                {
                    var tile = slot.Tile;
                    data.tiles.Add(new TileData
                    {
                        slotIndex = i,
                        value = tile.NumberValue,
                        isTransform = tile.IsTransformTile,
                        freezeTurns = tile.FreezeTurnsRemaining,
                        burnTurns = tile.BurnTurnsRemaining
                    });
                }
            }
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the saved game state from PlayerPrefs.
    /// </summary>
    public bool LoadGame()
    {
        if (!HasSaveData())
        {
            return false;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Load goal number
        if (GoalManager.Instance != null)
        {
            GoalManager.Instance.SetGoalNumber(data.goalNumber);
        }

        // Load number manager progression
        if (NumberManager.Instance != null)
        {
            NumberManager.Instance.SetGoalNumberRange(data.goalNumberRange);
        }

        // Load bonus state
        if (BonusSystem.Instance != null)
        {
            if (data.doubleMerge)
            {
                BonusSystem.Instance.ActivateDoubleMerge();
            }
            if (data.mysteryTiles > 0)
            {
                BonusSystem.Instance.QueueMysteryTile(data.mysteryTiles);
            }
        }

        // Load energy
        if (EnergySystem.Instance != null)
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long timePassed = currentTime - data.lastEnergyTimestamp;
            EnergySystem.Instance.RestoreEnergyFromOfflineTime(data.energy, data.energyRegenTimer, timePassed);
        }

        // Load tiles
        if (NumberSlotGenerator.Instance != null && data.tiles.Count > 0)
        {
            var allSlots = NumberSlotGenerator.Instance.GetAllSlots();
            
            foreach (var tileData in data.tiles)
            {
                if (tileData.slotIndex >= 0 && tileData.slotIndex < allSlots.Count)
                {
                    var slot = allSlots[tileData.slotIndex];
                    var tile = NumberSlotGenerator.Instance.SpawnTileInSlot(slot, tileData.isTransform);
                    
                    if (tile != null)
                    {
                        tile.NumberValue = tileData.value;
                        
                        if (tileData.freezeTurns > 0)
                        {
                            tile.Freeze(tileData.freezeTurns);
                        }
                        if (tileData.burnTurns > 0)
                        {
                            tile.Burn(tileData.burnTurns);
                        }
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if save data exists.
    /// </summary>
    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    /// <summary>
    /// Deletes all save data.
    /// </summary>
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Saves audio settings.
    /// </summary>
    public void SaveSettings(bool musicEnabled, bool sfxEnabled)
    {
        if (!HasSaveData())
        {
            // Create new save data if none exists
            SaveData data = new SaveData();
            data.musicEnabled = musicEnabled;
            data.sfxEnabled = sfxEnabled;
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SAVE_KEY, json);
        }
        else
        {
            // Update existing save data
            string json = PlayerPrefs.GetString(SAVE_KEY);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            data.musicEnabled = musicEnabled;
            data.sfxEnabled = sfxEnabled;
            json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SAVE_KEY, json);
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads audio settings. Returns (musicEnabled, sfxEnabled).
    /// </summary>
    public (bool musicEnabled, bool sfxEnabled) LoadSettings()
    {
        if (!HasSaveData())
        {
            return (true, true);
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return (data.musicEnabled, data.sfxEnabled);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
