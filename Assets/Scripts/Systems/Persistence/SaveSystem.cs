using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static SaveSystem instance;
    public static SaveSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject saveSystemObj = new GameObject("SaveSystem");
                instance = saveSystemObj.AddComponent<SaveSystem>();
                DontDestroyOnLoad(saveSystemObj);
            }
            return instance;
        }
    }
    
    private string GetSaveFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName + ".json");
    }
    
    public void SaveData<T>(T data, string fileName)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            string filePath = GetSaveFilePath(fileName);
            File.WriteAllText(filePath, json);
            Debug.Log($"Data saved to: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
    }
    
    public T LoadData<T>(string fileName) where T : new()
    {
        try
        {
            string filePath = GetSaveFilePath(fileName);
            
            if (!File.Exists(filePath))
            {
                Debug.Log($"Save file not found: {filePath}. Creating new data.");
                return new T();
            }
            
            string json = File.ReadAllText(filePath);
            T data = JsonUtility.FromJson<T>(json);
            Debug.Log($"Data loaded from: {filePath}");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message}");
            return new T();
        }
    }
    
    public bool SaveFileExists(string fileName)
    {
        return File.Exists(GetSaveFilePath(fileName));
    }
    
    public void DeleteSave(string fileName)
    {
        try
        {
            string filePath = GetSaveFilePath(fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Save file deleted: {filePath}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to delete save file: {e.Message}");
        }
    }
    
    // Game-specific save methods
    public void SaveGameData()
    {
        GameSaveData saveData = new GameSaveData
        {
            currency = FindObjectOfType<CurrencyManager>()?.GetTotalCurrency() ?? 0,
            metaProgression = GetMetaProgressionData(),
            settings = GetSettingsData(),
            statistics = GetStatisticsData()
        };
        
        SaveData(saveData, "gamedata");
    }
    
    public GameSaveData LoadGameData()
    {
        return LoadData<GameSaveData>("gamedata");
    }
    
    private MetaProgressionData GetMetaProgressionData()
    {
        var metaData = new MetaProgressionData
        {
            permanentUpgradeLevels = new System.Collections.Generic.Dictionary<string, int>(),
            unlockedTanks = new System.Collections.Generic.List<string> { "BasicTank" },
            totalGamesPlayed = PlayerPrefs.GetInt("TotalGamesPlayed", 0),
            bestSurvivalTime = PlayerPrefs.GetFloat("BestSurvivalTime", 0f)
        };
        
        // Load permanent upgrade levels from PlayerPrefs (for backwards compatibility)
        foreach (PermanentUpgradeType upgradeType in System.Enum.GetValues(typeof(PermanentUpgradeType)))
        {
            string key = $"PermanentUpgrade_{upgradeType}";
            int level = PlayerPrefs.GetInt(key, 0);
            if (level > 0)
            {
                metaData.permanentUpgradeLevels[upgradeType.ToString()] = level;
            }
        }
        
        return metaData;
    }
    
    private SettingsData GetSettingsData()
    {
        return new SettingsData
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f),
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f),
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f),
            fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1,
            resolution = new SerializableVector2Int(Screen.width, Screen.height),
            quality = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel())
        };
    }
    
    private StatisticsData GetStatisticsData()
    {
        return new StatisticsData
        {
            totalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime", 0f),
            totalEnemiesKilled = PlayerPrefs.GetInt("TotalEnemiesKilled", 0),
            totalBossesKilled = PlayerPrefs.GetInt("TotalBossesKilled", 0),
            totalDeaths = PlayerPrefs.GetInt("TotalDeaths", 0),
            highestWave = PlayerPrefs.GetInt("HighestWave", 0),
            highestLevel = PlayerPrefs.GetInt("HighestLevel", 1)
        };
    }
    
    public void SavePermanentUpgrades()
    {
        // For now, permanent upgrades are saved automatically via PlayerPrefs in PermanentUpgradeButton
        // This method is here for consistency and future expansion
        PlayerPrefs.Save();
        Debug.Log("Permanent upgrades saved");
    }
    
    public void ApplyLoadedData(GameSaveData data)
    {
        // Apply currency
        CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
        if (currencyManager != null && data.currency > 0)
        {
            currencyManager.SetTotalCurrency(data.currency);
        }
        
        // Apply permanent upgrades to PlayerPrefs (for compatibility with existing system)
        if (data.metaProgression?.permanentUpgradeLevels != null)
        {
            foreach (var upgrade in data.metaProgression.permanentUpgradeLevels)
            {
                string key = $"PermanentUpgrade_{upgrade.Key}";
                PlayerPrefs.SetInt(key, upgrade.Value);
            }
            PlayerPrefs.Save();
        }
        
        // Apply settings
        ApplySettings(data.settings);
        
        // Apply statistics to PlayerPrefs
        ApplyStatistics(data.statistics);
        
        Debug.Log($"Applied loaded data: Currency={data.currency}, Upgrades={data.metaProgression?.permanentUpgradeLevels?.Count ?? 0}");
    }
    
    private void ApplySettings(SettingsData settings)
    {
        PlayerPrefs.SetFloat("MasterVolume", settings.masterVolume);
        PlayerPrefs.SetFloat("SFXVolume", settings.sfxVolume);
        PlayerPrefs.SetFloat("MusicVolume", settings.musicVolume);
        PlayerPrefs.SetInt("Fullscreen", settings.fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("QualityLevel", settings.quality);
        
        // Apply immediately
        AudioListener.volume = settings.masterVolume;
        Screen.fullScreen = settings.fullscreen;
        QualitySettings.SetQualityLevel(settings.quality);
    }
    
    private void ApplyStatistics(StatisticsData stats)
    {
        PlayerPrefs.SetFloat("TotalPlayTime", stats.totalPlayTime);
        PlayerPrefs.SetInt("TotalEnemiesKilled", stats.totalEnemiesKilled);
        PlayerPrefs.SetInt("TotalBossesKilled", stats.totalBossesKilled);
        PlayerPrefs.SetInt("TotalDeaths", stats.totalDeaths);
        PlayerPrefs.SetInt("HighestWave", stats.highestWave);
        PlayerPrefs.SetInt("HighestLevel", stats.highestLevel);
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGameData();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveGameData();
        }
    }
    
    private void OnDestroy()
    {
        if (instance == this)
        {
            SaveGameData();
        }
    }
    
    private void OnApplicationQuit()
    {
        Debug.Log("Application quitting - saving game data...");
        SaveGameData();
    }
}

[System.Serializable]
public class GameSaveData
{
    public int currency;
    public MetaProgressionData metaProgression;
    public SettingsData settings;
    public StatisticsData statistics;
}

[System.Serializable]
public class MetaProgressionData : UnityEngine.ISerializationCallbackReceiver
{
    [SerializeField] private System.Collections.Generic.List<UpgradeDataEntry> upgradeEntries;
    public System.Collections.Generic.List<string> unlockedTanks;
    public int totalGamesPlayed;
    public float bestSurvivalTime;
    
    [System.NonSerialized]
    private System.Collections.Generic.Dictionary<string, int> _permanentUpgradeLevels;
    
    public System.Collections.Generic.Dictionary<string, int> permanentUpgradeLevels
    {
        get
        {
            if (_permanentUpgradeLevels == null)
            {
                _permanentUpgradeLevels = new System.Collections.Generic.Dictionary<string, int>();
                if (upgradeEntries != null)
                {
                    foreach (var entry in upgradeEntries)
                    {
                        _permanentUpgradeLevels[entry.upgradeType] = entry.level;
                    }
                }
            }
            return _permanentUpgradeLevels;
        }
        set
        {
            _permanentUpgradeLevels = value;
            upgradeEntries = new System.Collections.Generic.List<UpgradeDataEntry>();
            if (_permanentUpgradeLevels != null)
            {
                foreach (var kvp in _permanentUpgradeLevels)
                {
                    upgradeEntries.Add(new UpgradeDataEntry { upgradeType = kvp.Key, level = kvp.Value });
                }
            }
        }
    }
    
    public MetaProgressionData()
    {
        upgradeEntries = new System.Collections.Generic.List<UpgradeDataEntry>();
        unlockedTanks = new System.Collections.Generic.List<string>();
        _permanentUpgradeLevels = new System.Collections.Generic.Dictionary<string, int>();
    }
    
    public void OnBeforeSerialize()
    {
        if (_permanentUpgradeLevels != null)
        {
            upgradeEntries = new System.Collections.Generic.List<UpgradeDataEntry>();
            foreach (var kvp in _permanentUpgradeLevels)
            {
                upgradeEntries.Add(new UpgradeDataEntry { upgradeType = kvp.Key, level = kvp.Value });
            }
        }
    }
    
    public void OnAfterDeserialize()
    {
        _permanentUpgradeLevels = new System.Collections.Generic.Dictionary<string, int>();
        if (upgradeEntries != null)
        {
            foreach (var entry in upgradeEntries)
            {
                _permanentUpgradeLevels[entry.upgradeType] = entry.level;
            }
        }
    }
}

[System.Serializable]
public class UpgradeDataEntry
{
    public string upgradeType;
    public int level;
}

[System.Serializable]
public class SettingsData
{
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;
    public bool fullscreen;
    public SerializableVector2Int resolution;
    public int quality;
}

[System.Serializable]
public class StatisticsData
{
    public float totalPlayTime;
    public int totalEnemiesKilled;
    public int totalBossesKilled;
    public int totalDeaths;
    public int highestWave;
    public int highestLevel;
}

[System.Serializable]
public class SerializableVector2Int
{
    public int x, y;
    
    public SerializableVector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
