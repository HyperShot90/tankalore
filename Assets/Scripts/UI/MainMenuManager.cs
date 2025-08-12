using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button upgradesButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingsButton;
    
    [Header("UI Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Settings UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button backButton;
    
    [Header("Currency Display")]
    [SerializeField] private TextMeshProUGUI currencyText;
    
    // Components
    private CurrencyManager currencyManager;
    
    private void Awake()
    {
        // Find components
        currencyManager = FindObjectOfType<CurrencyManager>();
        
        // Setup button listeners
        SetupButtonListeners();
    }
    
    private void Start()
    {
        // Initialize UI
        InitializeUI();
        
        // Update currency display
        UpdateCurrencyDisplay();
    }
    
    private void SetupButtonListeners()
    {
        // Main menu buttons
        if (playButton != null)
            playButton.onClick.AddListener(LoadGameScene);
            
        if (upgradesButton != null)
            upgradesButton.onClick.AddListener(LoadUpgradeShop);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);
            
        // Settings panel buttons
        if (backButton != null)
            backButton.onClick.AddListener(ShowMainMenu);
            
        // Volume slider
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            volumeSlider.value = AudioListener.volume;
        }
    }
    
    private void InitializeUI()
    {
        // Show main panel, hide settings
        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        // Platform-specific UI adjustments
        #if UNITY_WEBGL || UNITY_WEBPLAYER
        if (quitButton != null)
            quitButton.gameObject.SetActive(false);
        #endif
    }
    
    private void UpdateCurrencyDisplay()
    {
        if (currencyText != null && currencyManager != null)
        {
            currencyText.text = $"Credits: {currencyManager.GetTotalCurrency()}";
        }
    }
    
    // Button event handlers
    private void LoadGameScene()
    {
        Debug.Log("Loading Game scene...");
        SceneManager.LoadScene("Game");
    }
    
    private void LoadUpgradeShop()
    {
        Debug.Log("Loading UpgradeShop scene...");
        SceneManager.LoadScene("UpgradeShop");
    }
    
    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    private void ShowSettings()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }
    
    private void ShowMainMenu()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }
    
    private void OnVolumeChanged(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
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
    }
    
    // Animation methods for button feedback
    public void OnPlayButtonHover()
    {
        // Can be used for button animations
        if (playButton != null)
        {
            playButton.transform.localScale = Vector3.one * 1.05f;
        }
    }
    
    public void OnPlayButtonExit()
    {
        if (playButton != null)
        {
            playButton.transform.localScale = Vector3.one;
        }
    }
    
    public void OnUpgradesButtonHover()
    {
        if (upgradesButton != null)
        {
            upgradesButton.transform.localScale = Vector3.one * 1.05f;
        }
    }
    
    public void OnUpgradesButtonExit()
    {
        if (upgradesButton != null)
        {
            upgradesButton.transform.localScale = Vector3.one;
        }
    }
    
    public void OnQuitButtonHover()
    {
        if (quitButton != null)
        {
            quitButton.transform.localScale = Vector3.one * 1.05f;
        }
    }
    
    public void OnQuitButtonExit()
    {
        if (quitButton != null)
        {
            quitButton.transform.localScale = Vector3.one;
        }
    }
}
