using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PermanentUpgradeButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image backgroundImage;
    
    [Header("Upgrade Configuration")]
    [SerializeField] private PermanentUpgradeType upgradeType;
    [SerializeField] private string upgradeName = "Upgrade Name";
    [SerializeField] private string baseDescription = "Upgrade description";
    [SerializeField] private Sprite upgradeIcon;
    [SerializeField] private int baseCost = 100;
    [SerializeField] private float costMultiplier = 1.5f;
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private float effectPerLevel = 0.1f;
    [SerializeField] private bool isPercentageUpgrade = true;
    
    // Current state
    private int currentLevel = 0;
    private bool isMaxLevel = false;
    
    // Components
    private CurrencyManager currencyManager;
    private SaveSystem saveSystem;
    
    // Events
    public System.Action<PermanentUpgradeType, int> OnUpgradePurchased;
    
    private void Awake()
    {
        // Find components
        currencyManager = FindObjectOfType<CurrencyManager>();
        saveSystem = FindObjectOfType<SaveSystem>();
        
        // Setup button
        if (button == null)
            button = GetComponent<Button>();
            
        if (button != null)
            button.onClick.AddListener(OnButtonClicked);
    }
    
    private void Start()
    {
        // Load current level from save data
        LoadUpgradeLevel();
        
        // Initialize display
        UpdateDisplay();
    }
    
    public void UpdateDisplay()
    {
        // Update icon
        if (iconImage != null && upgradeIcon != null)
        {
            iconImage.sprite = upgradeIcon;
            iconImage.gameObject.SetActive(true);
        }
        else if (iconImage != null)
        {
            iconImage.gameObject.SetActive(false);
        }
        
        // Update name
        if (nameText != null)
        {
            nameText.text = upgradeName;
        }
        
        // Update description with effect
        if (descriptionText != null)
        {
            string description = baseDescription;
            float totalEffect = GetTotalEffect();
            
            if (currentLevel > 0)
            {
                if (isPercentageUpgrade)
                {
                    description += $"\nCurrent: +{totalEffect * 100f:F0}%";
                }
                else
                {
                    description += $"\nCurrent: +{totalEffect:F1}";
                }
            }
            
            if (!isMaxLevel)
            {
                float nextLevelEffect = GetEffectAtLevel(currentLevel + 1);
                if (isPercentageUpgrade)
                {
                    description += $"\nNext: +{nextLevelEffect * 100f:F0}%";
                }
                else
                {
                    description += $"\nNext: +{nextLevelEffect:F1}";
                }
            }
            
            descriptionText.text = description;
        }
        
        // Update level
        if (levelText != null)
        {
            if (isMaxLevel)
            {
                levelText.text = "MAX";
                levelText.color = Color.yellow;
            }
            else
            {
                levelText.text = $"Level {currentLevel}/{maxLevel}";
                levelText.color = Color.white;
            }
        }
        
        // Update cost and button state
        if (costText != null && button != null)
        {
            if (isMaxLevel)
            {
                costText.text = "MAXED";
                costText.color = Color.yellow;
                button.interactable = false;
            }
            else
            {
                int cost = GetCurrentCost();
                bool canAfford = currencyManager != null && currencyManager.GetTotalCurrency() >= cost;
                
                costText.text = $"{cost} Credits";
                costText.color = canAfford ? Color.white : Color.red;
                button.interactable = canAfford;
            }
        }
        
        // Update background color based on affordability
        if (backgroundImage != null)
        {
            if (isMaxLevel)
            {
                backgroundImage.color = new Color(1f, 1f, 0f, 0.3f); // Yellow tint for maxed
            }
            else if (button != null && button.interactable)
            {
                backgroundImage.color = new Color(0f, 1f, 0f, 0.2f); // Green tint for affordable
            }
            else
            {
                backgroundImage.color = new Color(0.5f, 0.5f, 0.5f, 0.3f); // Gray tint for unaffordable
            }
        }
    }
    
    private void OnButtonClicked()
    {
        if (isMaxLevel || currencyManager == null) return;
        
        int cost = GetCurrentCost();
        if (currencyManager.GetTotalCurrency() >= cost)
        {
            // Spend currency
            currencyManager.SpendCurrency(cost);
            
            // Increase level
            currentLevel++;
            isMaxLevel = currentLevel >= maxLevel;
            
            // Save progress
            SaveUpgradeLevel();
            
            // Update display
            UpdateDisplay();
            
            // Notify listeners
            OnUpgradePurchased?.Invoke(upgradeType, currentLevel);
            
            Debug.Log($"Purchased {upgradeName} level {currentLevel} for {cost} credits");
        }
    }
    
    private int GetCurrentCost()
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel));
    }
    
    private float GetTotalEffect()
    {
        return GetEffectAtLevel(currentLevel);
    }
    
    private float GetEffectAtLevel(int level)
    {
        return effectPerLevel * level;
    }
    
    private void LoadUpgradeLevel()
    {
        if (saveSystem != null)
        {
            // This would load from the save system
            // For now, use PlayerPrefs as a simple implementation
            string key = $"PermanentUpgrade_{upgradeType}";
            currentLevel = PlayerPrefs.GetInt(key, 0);
            isMaxLevel = currentLevel >= maxLevel;
        }
    }
    
    private void SaveUpgradeLevel()
    {
        if (saveSystem != null)
        {
            // This would save to the save system
            // For now, use PlayerPrefs as a simple implementation
            string key = $"PermanentUpgrade_{upgradeType}";
            PlayerPrefs.SetInt(key, currentLevel);
            PlayerPrefs.Save();
        }
    }
    
    // Public methods for external access
    public PermanentUpgradeType GetUpgradeType()
    {
        return upgradeType;
    }
    
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    public float GetCurrentEffect()
    {
        return GetTotalEffect();
    }
    
    public bool IsMaxed()
    {
        return isMaxLevel;
    }
    
    public int GetNextCost()
    {
        if (isMaxLevel) return 0;
        return GetCurrentCost();
    }
    
    // Animation methods for button feedback
    public void OnPointerEnter()
    {
        if (button != null && button.interactable)
        {
            transform.localScale = Vector3.one * 1.02f;
        }
    }
    
    public void OnPointerExit()
    {
        transform.localScale = Vector3.one;
    }
    
    public void OnPointerDown()
    {
        if (button != null && button.interactable)
        {
            transform.localScale = Vector3.one * 0.98f;
        }
    }
    
    public void OnPointerUp()
    {
        if (button != null && button.interactable)
        {
            transform.localScale = Vector3.one * 1.02f;
        }
    }
}
