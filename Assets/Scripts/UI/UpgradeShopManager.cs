using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UpgradeShopManager : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Button backToMainMenuButton;
    [SerializeField] private Button playButton;
    
    [Header("Currency Display")]
    [SerializeField] private TextMeshProUGUI currencyText;
    
    [Header("Upgrade Categories")]
    [SerializeField] private GameObject healthUpgradesPanel;
    [SerializeField] private GameObject damageUpgradesPanel;
    [SerializeField] private GameObject speedUpgradesPanel;
    [SerializeField] private GameObject specialUpgradesPanel;
    
    [Header("Category Buttons")]
    [SerializeField] private Button healthCategoryButton;
    [SerializeField] private Button damageCategoryButton;
    [SerializeField] private Button speedCategoryButton;
    [SerializeField] private Button specialCategoryButton;
    
    [Header("Permanent Upgrades")]
    [SerializeField] private List<PermanentUpgradeButton> permanentUpgradeButtons = new List<PermanentUpgradeButton>();
    
    [Header("Reset Progress")]
    [SerializeField] private Button resetProgressButton;
    [SerializeField] private GameObject confirmResetPanel;
    [SerializeField] private Button confirmResetButton;
    [SerializeField] private Button cancelResetButton;
    
    // Current selected category
    private UpgradeCategory currentCategory = UpgradeCategory.Health;
    
    // Components
    private CurrencyManager currencyManager;
    private SaveSystem saveSystem;
    
    private void Awake()
    {
        // Find components
        currencyManager = FindObjectOfType<CurrencyManager>();
        saveSystem = FindObjectOfType<SaveSystem>();
        
        // Setup button listeners
        SetupButtonListeners();
    }
    
    private void Start()
    {
        // Initialize UI
        InitializeUI();
        
        // Update displays
        UpdateCurrencyDisplay();
        UpdateUpgradeButtons();
        
        // Show default category
        ShowCategory(UpgradeCategory.Health);
    }
    
    private void SetupButtonListeners()
    {
        // Navigation buttons
        if (backToMainMenuButton != null)
            backToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
            
        if (playButton != null)
            playButton.onClick.AddListener(StartGame);
            
        // Category buttons
        if (healthCategoryButton != null)
            healthCategoryButton.onClick.AddListener(() => ShowCategory(UpgradeCategory.Health));
            
        if (damageCategoryButton != null)
            damageCategoryButton.onClick.AddListener(() => ShowCategory(UpgradeCategory.Damage));
            
        if (speedCategoryButton != null)
            speedCategoryButton.onClick.AddListener(() => ShowCategory(UpgradeCategory.Speed));
            
        if (specialCategoryButton != null)
            specialCategoryButton.onClick.AddListener(() => ShowCategory(UpgradeCategory.Special));
            
        // Reset progress buttons
        if (resetProgressButton != null)
            resetProgressButton.onClick.AddListener(ShowResetConfirmation);
            
        if (confirmResetButton != null)
            confirmResetButton.onClick.AddListener(ConfirmResetProgress);
            
        if (cancelResetButton != null)
            cancelResetButton.onClick.AddListener(CancelResetProgress);
            
        // Setup permanent upgrade button listeners
        foreach (var upgradeButton in permanentUpgradeButtons)
        {
            if (upgradeButton != null)
            {
                upgradeButton.OnUpgradePurchased += OnPermanentUpgradePurchased;
            }
        }
    }
    
    private void InitializeUI()
    {
        // Hide all panels initially
        if (healthUpgradesPanel != null) healthUpgradesPanel.SetActive(false);
        if (damageUpgradesPanel != null) damageUpgradesPanel.SetActive(false);
        if (speedUpgradesPanel != null) speedUpgradesPanel.SetActive(false);
        if (specialUpgradesPanel != null) specialUpgradesPanel.SetActive(false);
        
        // Hide reset confirmation
        if (confirmResetPanel != null) confirmResetPanel.SetActive(false);
    }
    
    private void UpdateCurrencyDisplay()
    {
        if (currencyText != null && currencyManager != null)
        {
            currencyText.text = $"Credits: {currencyManager.GetTotalCurrency()}";
        }
    }
    
    private void UpdateUpgradeButtons()
    {
        foreach (var upgradeButton in permanentUpgradeButtons)
        {
            if (upgradeButton != null)
            {
                upgradeButton.UpdateDisplay();
            }
        }
    }
    
    // Category management
    private void ShowCategory(UpgradeCategory category)
    {
        currentCategory = category;
        
        // Hide all panels
        if (healthUpgradesPanel != null) healthUpgradesPanel.SetActive(false);
        if (damageUpgradesPanel != null) damageUpgradesPanel.SetActive(false);
        if (speedUpgradesPanel != null) speedUpgradesPanel.SetActive(false);
        if (specialUpgradesPanel != null) specialUpgradesPanel.SetActive(false);
        
        // Show selected panel
        switch (category)
        {
            case UpgradeCategory.Health:
                if (healthUpgradesPanel != null) healthUpgradesPanel.SetActive(true);
                break;
            case UpgradeCategory.Damage:
                if (damageUpgradesPanel != null) damageUpgradesPanel.SetActive(true);
                break;
            case UpgradeCategory.Speed:
                if (speedUpgradesPanel != null) speedUpgradesPanel.SetActive(true);
                break;
            case UpgradeCategory.Special:
                if (specialUpgradesPanel != null) specialUpgradesPanel.SetActive(true);
                break;
        }
        
        // Update category button visuals
        UpdateCategoryButtons();
    }
    
    private void UpdateCategoryButtons()
    {
        // Reset all button states
        SetCategoryButtonState(healthCategoryButton, currentCategory == UpgradeCategory.Health);
        SetCategoryButtonState(damageCategoryButton, currentCategory == UpgradeCategory.Damage);
        SetCategoryButtonState(speedCategoryButton, currentCategory == UpgradeCategory.Speed);
        SetCategoryButtonState(specialCategoryButton, currentCategory == UpgradeCategory.Special);
    }
    
    private void SetCategoryButtonState(Button button, bool isSelected)
    {
        if (button == null) return;
        
        var colors = button.colors;
        if (isSelected)
        {
            colors.normalColor = Color.yellow;
            colors.highlightedColor = Color.yellow * 0.9f;
        }
        else
        {
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.gray;
        }
        button.colors = colors;
    }
    
    // Event handlers
    private void OnPermanentUpgradePurchased(PermanentUpgradeType upgradeType, int newLevel)
    {
        Debug.Log($"Purchased permanent upgrade: {upgradeType} to level {newLevel}");
        
        // Update currency display
        UpdateCurrencyDisplay();
        
        // Update all upgrade buttons
        UpdateUpgradeButtons();
        
        // Save progress
        if (saveSystem != null)
        {
            saveSystem.SavePermanentUpgrades();
        }
    }
    
    // Navigation
    private void ReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu...");
        SceneManager.LoadScene("MainMenu");
    }
    
    private void StartGame()
    {
        Debug.Log("Starting game from Upgrade Shop...");
        SceneManager.LoadScene("Game");
    }
    
    // Reset progress functionality
    private void ShowResetConfirmation()
    {
        if (confirmResetPanel != null)
        {
            confirmResetPanel.SetActive(true);
        }
    }
    
    private void CancelResetProgress()
    {
        if (confirmResetPanel != null)
        {
            confirmResetPanel.SetActive(false);
        }
    }
    
    private void ConfirmResetProgress()
    {
        // Reset all permanent upgrades
        if (currencyManager != null)
        {
            currencyManager.ResetPermanentUpgrades();
        }
        
        // Update UI
        UpdateCurrencyDisplay();
        UpdateUpgradeButtons();
        
        // Save the reset state
        if (saveSystem != null)
        {
            saveSystem.SavePermanentUpgrades();
        }
        
        // Hide confirmation panel
        CancelResetProgress();
        
        Debug.Log("Progress reset successfully!");
    }
    
    private void OnEnable()
    {
        // Subscribe to currency changes
        if (currencyManager != null)
        {
            currencyManager.OnTotalCurrencyChanged += UpdateCurrencyDisplay;
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from currency changes
        if (currencyManager != null)
        {
            currencyManager.OnTotalCurrencyChanged -= UpdateCurrencyDisplay;
        }
        
        // Unsubscribe from upgrade buttons
        foreach (var upgradeButton in permanentUpgradeButtons)
        {
            if (upgradeButton != null)
            {
                upgradeButton.OnUpgradePurchased -= OnPermanentUpgradePurchased;
            }
        }
    }
}

public enum UpgradeCategory
{
    Health,
    Damage,
    Speed,
    Special
}

public enum PermanentUpgradeType
{
    BaseHealth,
    BaseArmor,
    BaseDamage,
    BaseFireRate,
    BaseSpeed,
    XPMultiplier,
    CurrencyMultiplier,
    StartingLevel,
    BulletPenetration,
    CriticalChance,
    HealthRegeneration
}
