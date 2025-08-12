using UnityEngine;

/// <summary>
/// Test component to verify SaveSystem functionality
/// Can be attached to any GameObject for testing purposes
/// </summary>
public class SaveSystemTest : MonoBehaviour
{
    [Header("Test Controls")]
    [SerializeField] private bool testOnStart = false;
    [SerializeField] private int testCurrency = 500;
    [SerializeField] private bool logSaveData = true;
    
    private void Start()
    {
        if (testOnStart)
        {
            TestSaveSystem();
        }
    }
    
    [ContextMenu("Test Save System")]
    public void TestSaveSystem()
    {
        Debug.Log("=== SaveSystem Test Started ===");
        
        var saveSystem = SaveSystem.Instance;
        
        // Test saving currency
        var currencyManager = FindObjectOfType<CurrencyManager>();
        if (currencyManager != null)
        {
            Debug.Log($"Current currency: {currencyManager.GetTotalCurrency()}");
            currencyManager.AddTotalCurrency(testCurrency);
            Debug.Log($"Added {testCurrency} currency. New total: {currencyManager.GetTotalCurrency()}");
        }
        
        // Force save
        saveSystem.SaveGameData();
        Debug.Log("Game data saved");
        
        // Test loading
        var loadedData = saveSystem.LoadGameData();
        if (logSaveData)
        {
            Debug.Log($"Loaded data - Currency: {loadedData.currency}");
            Debug.Log($"Loaded data - Upgrades count: {loadedData.metaProgression?.permanentUpgradeLevels?.Count ?? 0}");
            if (loadedData.metaProgression?.permanentUpgradeLevels != null)
            {
                foreach (var upgrade in loadedData.metaProgression.permanentUpgradeLevels)
                {
                    Debug.Log($"  Upgrade {upgrade.Key}: Level {upgrade.Value}");
                }
            }
        }
        
        // Test save file existence
        bool saveExists = saveSystem.SaveFileExists("gamedata");
        Debug.Log($"Save file exists: {saveExists}");
        
        if (saveExists)
        {
            string saveFilePath = System.IO.Path.Combine(Application.persistentDataPath, "gamedata.json");
            Debug.Log($"Save file location: {saveFilePath}");
        }
        
        Debug.Log("=== SaveSystem Test Completed ===");
    }
    
    [ContextMenu("Apply Loaded Data")]
    public void TestApplyLoadedData()
    {
        var saveSystem = SaveSystem.Instance;
        var loadedData = saveSystem.LoadGameData();
        saveSystem.ApplyLoadedData(loadedData);
        Debug.Log("Applied loaded data to game systems");
    }
    
    [ContextMenu("Delete Save Data")]
    public void DeleteSaveData()
    {
        var saveSystem = SaveSystem.Instance;
        saveSystem.DeleteSave("gamedata");
        Debug.Log("Save data deleted");
    }
}
