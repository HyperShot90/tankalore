using UnityEngine;
using Tankalore.Contracts;

public class SessionManager : MonoBehaviour
{
    [Header("Session Configuration")]
    [SerializeField] private float sessionDuration = 900f; // 15 minutes in seconds
    [SerializeField] private bool enableTimeLimit = true;
    
    // Session state
    private float sessionStartTime;
    private bool isGameOver = false;
    private bool isPaused = false;
    private GameOverReason gameOverReason;
    
    // Components
    private IPlayerEvents playerEvents;
    private EnemySpawner enemySpawner;
    
    // Events
    public System.Action<float, float> OnTimeChanged; // currentTime, totalTime
    public System.Action<GameOverReason, float> OnGameOver; // reason, survivalTime
    public System.Action OnGamePaused;
    public System.Action OnGameResumed;
    
    private void Awake()
    {
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerEvents = playerController;
        }
        
        enemySpawner = FindObjectOfType<EnemySpawner>();
    }
    
    private void Start()
    {
        StartSession();
        
        // Subscribe to player death
        if (playerEvents != null)
        {
            playerEvents.OnPlayerDied += HandlePlayerDeath;
        }
    }
    
    private void OnDestroy()
    {
        if (playerEvents != null)
        {
            playerEvents.OnPlayerDied -= HandlePlayerDeath;
        }
    }
    
    private void Update()
    {
        if (isGameOver || isPaused) return;
        
        float currentTime = GetSessionTime();
        float timeRemaining = GetTimeRemaining();
        
        // Update time display
        OnTimeChanged?.Invoke(currentTime, sessionDuration);
        
        // Check if time limit reached
        if (enableTimeLimit && timeRemaining <= 0f)
        {
            EndSession(GameOverReason.TimeUp);
        }
        
        // Handle pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    private void StartSession()
    {
        sessionStartTime = Time.time;
        isGameOver = false;
        isPaused = false;
        
        Debug.Log("Session started!");
    }
    
    public void EndSession(GameOverReason reason)
    {
        if (isGameOver) return;
        
        isGameOver = true;
        gameOverReason = reason;
        float survivalTime = GetSessionTime();
        
        // Stop spawning
        if (enemySpawner != null)
        {
            enemySpawner.ForceStopSpawning();
        }
        
        OnGameOver?.Invoke(reason, survivalTime);
        
        Debug.Log($"Session ended! Reason: {reason}, Survival time: {survivalTime:F1}s");
    }
    
    private void HandlePlayerDeath()
    {
        EndSession(GameOverReason.PlayerDeath);
    }
    
    public void TogglePause()
    {
        if (isGameOver) return;
        
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    public void PauseGame()
    {
        if (isGameOver || isPaused) return;
        
        isPaused = true;
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();
    }
    
    public void ResumeGame()
    {
        if (isGameOver || !isPaused) return;
        
        isPaused = false;
        Time.timeScale = 1f;
        OnGameResumed?.Invoke();
    }
    
    public void RestartSession()
    {
        // Reset all systems
        Time.timeScale = 1f;
        
        // Reset upgrade system
        UpgradeManager upgradeManager = FindObjectOfType<UpgradeManager>();
        if (upgradeManager != null)
        {
            upgradeManager.ResetSessionUpgrades();
        }
        
        // Reset XP system
        XPSystem xpSystem = FindObjectOfType<XPSystem>();
        if (xpSystem != null)
        {
            xpSystem.ResetForNewSession();
        }
        
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    public float GetSessionTime()
    {
        return Time.time - sessionStartTime;
    }
    
    public float GetTimeRemaining()
    {
        if (!enableTimeLimit) return float.MaxValue;
        return Mathf.Max(0f, sessionDuration - GetSessionTime());
    }
    
    public float GetSessionProgress()
    {
        if (!enableTimeLimit) return 0f;
        return GetSessionTime() / sessionDuration;
    }
    
    public bool IsGameOver()
    {
        return isGameOver;
    }
    
    public bool IsPaused()
    {
        return isPaused;
    }
    
    public GameOverReason GetGameOverReason()
    {
        return gameOverReason;
    }
    
    public string GetTimeString()
    {
        float time = enableTimeLimit ? GetTimeRemaining() : GetSessionTime();
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
    
    // Session statistics for end screen
    public SessionStats GetSessionStats()
    {
        return new SessionStats
        {
            survivalTime = GetSessionTime(),
            finalLevel = FindObjectOfType<XPSystem>()?.GetCurrentLevel() ?? 1,
            finalWave = FindObjectOfType<EnemySpawner>()?.GetCurrentWave() ?? 0,
            currencyEarned = FindObjectOfType<CurrencyManager>()?.GetSessionCurrency() ?? 0,
            gameOverReason = gameOverReason
        };
    }
}

public enum GameOverReason
{
    PlayerDeath,
    TimeUp,
    Manual
}

[System.Serializable]
public class SessionStats
{
    public float survivalTime;
    public int finalLevel;
    public int finalWave;
    public int currencyEarned;
    public GameOverReason gameOverReason;
}
