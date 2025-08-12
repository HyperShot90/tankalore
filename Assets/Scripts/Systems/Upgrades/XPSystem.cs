using UnityEngine;

public class XPSystem : MonoBehaviour
{
    [Header("XP Configuration")]
    [SerializeField] private int baseXPRequired = 80;
    [SerializeField] private float xpScaling = 1.15f;
    [SerializeField] private int maxLevel = 50;
    
    // Current state
    private int currentLevel = 1;
    private int currentXP = 0;
    private int xpRequiredForNextLevel;
    
    // Events
    public System.Action<int> OnLevelUp; // new level
    public System.Action<int, int, int> OnXPChanged; // currentXP, xpRequired, level
    
    private void Awake()
    {
        CalculateXPRequiredForNextLevel();
    }
    
    private void Start()
    {
        OnXPChanged?.Invoke(currentXP, xpRequiredForNextLevel, currentLevel);
    }
    
    public void AddXP(int amount)
    {
        if (currentLevel >= maxLevel) return;
        
        currentXP += amount;
        
        while (currentXP >= xpRequiredForNextLevel && currentLevel < maxLevel)
        {
            LevelUp();
        }
        
        OnXPChanged?.Invoke(currentXP, xpRequiredForNextLevel, currentLevel);
    }
    
    private void LevelUp()
    {
        currentXP -= xpRequiredForNextLevel;
        currentLevel++;
        
        CalculateXPRequiredForNextLevel();
        
        OnLevelUp?.Invoke(currentLevel);
        
        Debug.Log($"Level Up! Now level {currentLevel}");
    }
    
    private void CalculateXPRequiredForNextLevel()
    {
        xpRequiredForNextLevel = Mathf.RoundToInt(baseXPRequired * Mathf.Pow(xpScaling, currentLevel - 1));
    }
    
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    public int GetCurrentXP()
    {
        return currentXP;
    }
    
    public int GetXPRequiredForNextLevel()
    {
        return xpRequiredForNextLevel;
    }
    
    public float GetXPProgress()
    {
        return (float)currentXP / xpRequiredForNextLevel;
    }
    
    public bool IsMaxLevel()
    {
        return currentLevel >= maxLevel;
    }
    
    // For save/load system
    public XPSaveData GetSaveData()
    {
        return new XPSaveData
        {
            level = currentLevel,
            xp = currentXP
        };
    }
    
    public void LoadSaveData(XPSaveData data)
    {
        currentLevel = data.level;
        currentXP = data.xp;
        CalculateXPRequiredForNextLevel();
        OnXPChanged?.Invoke(currentXP, xpRequiredForNextLevel, currentLevel);
    }
    
    public void ResetForNewSession()
    {
        // For rogue-like, reset level but keep meta progression
        currentLevel = 1;
        currentXP = 0;
        CalculateXPRequiredForNextLevel();
        OnXPChanged?.Invoke(currentXP, xpRequiredForNextLevel, currentLevel);
    }
}

[System.Serializable]
public class XPSaveData
{
    public int level;
    public int xp;
}
