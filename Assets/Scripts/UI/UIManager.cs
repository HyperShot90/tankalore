using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Tankalore.Contracts;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider xpBar;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI enemiesAliveText;
    
    [Header("Upgrade UI")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();
    
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverReasonText;
    [SerializeField] private TextMeshProUGUI survivalTimeText;
    [SerializeField] private TextMeshProUGUI finalLevelText;
    [SerializeField] private TextMeshProUGUI finalWaveText;
    [SerializeField] private TextMeshProUGUI currencyEarnedText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseMainMenuButton;
    
    // Component references
    private PlayerController playerController;
    private XPSystem xpSystem;
    private UpgradeManager upgradeManager;
    private SessionManager sessionManager;
    private EnemySpawner enemySpawner;
    private CurrencyManager currencyManager;
    
    private void Awake()
    {
        // Find components
        playerController = FindObjectOfType<PlayerController>();
        xpSystem = FindObjectOfType<XPSystem>();
        upgradeManager = FindObjectOfType<UpgradeManager>();
        sessionManager = FindObjectOfType<SessionManager>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        currencyManager = FindObjectOfType<CurrencyManager>();
        
        // Setup button listeners
        SetupButtonListeners();
    }
    
    private void Start()
    {
        // Subscribe to events
        SubscribeToEvents();
        
        // Initialize UI
        InitializeUI();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    private void SetupButtonListeners()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(() => sessionManager?.RestartSession());
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(() => sessionManager?.ReturnToMainMenu());
            
        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => sessionManager?.ResumeGame());
            
        if (pauseMainMenuButton != null)
            pauseMainMenuButton.onClick.AddListener(() => sessionManager?.ReturnToMainMenu());
            
        // Setup upgrade button listeners
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            int buttonIndex = i; // Capture for closure
            if (upgradeButtons[i] != null)
            {
                upgradeButtons[i].OnUpgradeSelected += (upgrade) => SelectUpgrade(upgrade);
            }
        }
    }
    
    private void SubscribeToEvents()
    {
        if (playerController != null)
            playerController.OnHealthChanged += UpdateHealthBar;
            
        if (xpSystem != null)
        {
            xpSystem.OnXPChanged += UpdateXPBar;
            xpSystem.OnLevelUp += UpdateLevel;
        }
        
        if (upgradeManager != null)
            upgradeManager.OnUpgradeChoicesReady += ShowUpgradeChoices;
            
        if (sessionManager != null)
        {
            sessionManager.OnTimeChanged += UpdateTimer;
            sessionManager.OnGameOver += ShowGameOver;
            sessionManager.OnGamePaused += ShowPauseMenu;
            sessionManager.OnGameResumed += HidePauseMenu;
        }
        
        if (enemySpawner != null)
            enemySpawner.OnWaveStarted += UpdateWave;
            
        if (currencyManager != null)
            currencyManager.OnSessionCurrencyChanged += UpdateCurrency;
    }
    
    private void UnsubscribeFromEvents()
    {
        if (playerController != null)
            playerController.OnHealthChanged -= UpdateHealthBar;
            
        if (xpSystem != null)
        {
            xpSystem.OnXPChanged -= UpdateXPBar;
            xpSystem.OnLevelUp -= UpdateLevel;
        }
        
        if (upgradeManager != null)
            upgradeManager.OnUpgradeChoicesReady -= ShowUpgradeChoices;
            
        if (sessionManager != null)
        {
            sessionManager.OnTimeChanged -= UpdateTimer;
            sessionManager.OnGameOver -= ShowGameOver;
            sessionManager.OnGamePaused -= ShowPauseMenu;
            sessionManager.OnGameResumed -= HidePauseMenu;
        }
        
        if (enemySpawner != null)
            enemySpawner.OnWaveStarted -= UpdateWave;
            
        if (currencyManager != null)
            currencyManager.OnSessionCurrencyChanged -= UpdateCurrency;
    }
    
    private void InitializeUI()
    {
        // Hide panels initially
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        
        // Initialize HUD values
        if (levelText != null && xpSystem != null)
            levelText.text = $"Level {xpSystem.GetCurrentLevel()}";
            
        if (waveText != null)
            waveText.text = "Wave 0";
            
        if (currencyText != null && currencyManager != null)
            currencyText.text = currencyManager.GetSessionCurrency().ToString();
    }
    
    private void Update()
    {
        // Update enemies alive count
        if (enemiesAliveText != null && enemySpawner != null)
        {
            enemiesAliveText.text = $"Enemies: {enemySpawner.GetEnemiesAlive()}";
        }
    }
    
    // Event handlers
    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }
    
    private void UpdateXPBar(int currentXP, int requiredXP, int level)
    {
        if (xpBar != null)
        {
            xpBar.value = (float)currentXP / requiredXP;
        }
    }
    
    private void UpdateLevel(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {newLevel}";
        }
    }
    
    private void UpdateWave(int waveNumber)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave {waveNumber}";
        }
    }
    
    private void UpdateTimer(float currentTime, float totalTime)
    {
        if (timerText != null && sessionManager != null)
        {
            timerText.text = sessionManager.GetTimeString();
        }
    }
    
    private void UpdateCurrency(int currency)
    {
        if (currencyText != null)
        {
            currencyText.text = currency.ToString();
        }
    }
    
    private void ShowUpgradeChoices(List<UpgradeData> upgrades)
    {
        if (upgradePanel == null) return;
        
        upgradePanel.SetActive(true);
        
        // Update upgrade buttons
        for (int i = 0; i < upgradeButtons.Count && i < upgrades.Count; i++)
        {
            upgradeButtons[i].SetUpgrade(upgrades[i]);
            upgradeButtons[i].gameObject.SetActive(true);
        }
        
        // Hide unused buttons
        for (int i = upgrades.Count; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].gameObject.SetActive(false);
        }
    }
    
    private void SelectUpgrade(UpgradeData upgrade)
    {
        if (upgradeManager != null)
        {
            upgradeManager.ApplyUpgrade(upgrade);
        }
        
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
    }
    
    private void ShowGameOver(GameOverReason reason, float survivalTime)
    {
        if (gameOverPanel == null) return;
        
        gameOverPanel.SetActive(true);
        
        // Get session stats
        SessionStats stats = sessionManager?.GetSessionStats();
        if (stats == null) return;
        
        // Update game over UI
        if (gameOverReasonText != null)
        {
            string reasonText = reason == GameOverReason.PlayerDeath ? "Defeated!" : "Time's Up!";
            gameOverReasonText.text = reasonText;
        }
        
        if (survivalTimeText != null)
        {
            int minutes = Mathf.FloorToInt(stats.survivalTime / 60f);
            int seconds = Mathf.FloorToInt(stats.survivalTime % 60f);
            survivalTimeText.text = $"Survived: {minutes:00}:{seconds:00}";
        }
        
        if (finalLevelText != null)
            finalLevelText.text = $"Final Level: {stats.finalLevel}";
            
        if (finalWaveText != null)
            finalWaveText.text = $"Final Wave: {stats.finalWave}";
            
        if (currencyEarnedText != null)
            currencyEarnedText.text = $"Currency Earned: {stats.currencyEarned}";
        
        // End session for currency manager
        if (currencyManager != null)
        {
            currencyManager.EndSession();
        }
    }
    
    private void ShowPauseMenu()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }
    
    private void HidePauseMenu()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }
}
