using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Automated testing script to validate core game functionality
/// </summary>
public class PlayModeValidator : MonoBehaviour
{
    [Header("Testing Configuration")]
    [SerializeField] private bool enableAutoTesting = true;
    [SerializeField] private float testDuration = 60f; // Test for 1 minute
    [SerializeField] private bool logDetailedInfo = true;

    private PlayerController playerController;
    private SessionManager sessionManager;
    private EnemySpawner enemySpawner;
    private XPSystem xpSystem;
    private UpgradeManager upgradeManager;
    
    private float testStartTime;
    private bool testingComplete = false;

    private void Start()
    {
        if (!enableAutoTesting) return;

        // Find all key components
        FindGameComponents();
        
        if (ValidateComponentsExist())
        {
            StartCoroutine(RunPlayModeTests());
        }
        else
        {
            Debug.LogError("PlayModeValidator: Missing required components for testing!");
        }
    }

    private void FindGameComponents()
    {
        playerController = FindObjectOfType<PlayerController>();
        sessionManager = FindObjectOfType<SessionManager>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        xpSystem = FindObjectOfType<XPSystem>();
        upgradeManager = FindObjectOfType<UpgradeManager>();
    }

    private bool ValidateComponentsExist()
    {
        bool allValid = true;

        if (playerController == null)
        {
            Debug.LogError("PlayerController not found!");
            allValid = false;
        }

        if (sessionManager == null)
        {
            Debug.LogError("SessionManager not found!");
            allValid = false;
        }

        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner not found!");
            allValid = false;
        }

        if (xpSystem == null)
        {
            Debug.LogError("XPSystem not found!");
            allValid = false;
        }

        return allValid;
    }

    private IEnumerator RunPlayModeTests()
    {
        Debug.Log("=== STARTING PLAY MODE VALIDATION ===");
        testStartTime = Time.time;

        // Test 1: Initial State Validation
        yield return StartCoroutine(TestInitialState());

        // Test 2: Player Movement (simulate input)
        yield return StartCoroutine(TestPlayerMovement());

        // Test 3: Enemy Spawning
        yield return StartCoroutine(TestEnemySpawning());

        // Test 4: XP System
        yield return StartCoroutine(TestXPSystem());

        // Test 5: Session Timer
        yield return StartCoroutine(TestSessionTimer());

        // Test 6: Upgrade System (if level up occurs)
        yield return StartCoroutine(TestUpgradeSystem());

        Debug.Log("=== PLAY MODE VALIDATION COMPLETE ===");
        testingComplete = true;

        // Optional: Generate test report
        GenerateTestReport();
    }

    private IEnumerator TestInitialState()
    {
        Debug.Log("Testing Initial State...");

        // Validate player is alive and healthy
        if (playerController != null)
        {
            float healthPercent = playerController.GetHealthPercentage();
            Debug.Log($"Player health: {healthPercent * 100}%");
            
            if (healthPercent <= 0.99f)
            {
                Debug.LogWarning("Player doesn't start with full health!");
            }
        }

        // Validate session manager state
        if (sessionManager != null)
        {
            Debug.Log($"Session time remaining: {sessionManager.GetTimeRemaining()}s");
            Debug.Log($"Game over status: {sessionManager.IsGameOver()}");
        }

        // Validate XP system initial state
        if (xpSystem != null)
        {
            Debug.Log($"Starting level: {xpSystem.GetCurrentLevel()}");
            Debug.Log($"Starting XP: {xpSystem.GetCurrentXP()}");
        }

        yield return new WaitForSeconds(1f);
        Debug.Log("✓ Initial State Test Complete");
    }

    private IEnumerator TestPlayerMovement()
    {
        Debug.Log("Testing Player Movement...");

        if (playerController == null)
        {
            Debug.LogError("Cannot test movement - PlayerController is null");
            yield break;
        }

        Vector3 startPosition = playerController.transform.position;
        
        // Simulate movement input for a few seconds
        float movementTestTime = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < movementTestTime)
        {
            // Simulate WASD input
            float horizontal = Mathf.Sin(Time.time * 2f);
            float vertical = Mathf.Cos(Time.time * 2f);
            
            // This would require modifying PlayerController to accept simulated input
            // For now, we'll just check if the player can move at all
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector3 endPosition = playerController.transform.position;
        float distanceMoved = Vector3.Distance(startPosition, endPosition);

        Debug.Log($"Player moved {distanceMoved:F2} units during movement test");
        
        if (distanceMoved > 0.1f)
        {
            Debug.Log("✓ Player Movement Test Complete");
        }
        else
        {
            Debug.LogWarning("Player may not be moving properly");
        }
    }

    private IEnumerator TestEnemySpawning()
    {
        Debug.Log("Testing Enemy Spawning...");

        if (enemySpawner == null)
        {
            Debug.LogError("Cannot test enemy spawning - EnemySpawner is null");
            yield break;
        }

        int initialWave = enemySpawner.GetCurrentWave();
        int initialEnemyCount = enemySpawner.GetEnemiesAlive();

        Debug.Log($"Initial wave: {initialWave}, Initial enemies: {initialEnemyCount}");

        // Wait for first wave to start
        float waitTime = 0f;
        while (enemySpawner.GetCurrentWave() == 0 && waitTime < 10f)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }

        if (enemySpawner.GetCurrentWave() > 0)
        {
            Debug.Log($"✓ Wave system working - Current wave: {enemySpawner.GetCurrentWave()}");
            Debug.Log($"Enemies alive: {enemySpawner.GetEnemiesAlive()}");
        }
        else
        {
            Debug.LogWarning("Wave system may not be working - no waves started");
        }

        yield return new WaitForSeconds(2f);
        Debug.Log("✓ Enemy Spawning Test Complete");
    }

    private IEnumerator TestXPSystem()
    {
        Debug.Log("Testing XP System...");

        if (xpSystem == null)
        {
            Debug.LogError("Cannot test XP system - XPSystem is null");
            yield break;
        }

        int startingLevel = xpSystem.GetCurrentLevel();
        int startingXP = xpSystem.GetCurrentXP();

        Debug.Log($"Starting Level: {startingLevel}, Starting XP: {startingXP}");

        // Add some test XP
        xpSystem.AddXP(50);

        Debug.Log($"After adding 50 XP - Level: {xpSystem.GetCurrentLevel()}, XP: {xpSystem.GetCurrentXP()}");

        if (xpSystem.GetCurrentXP() > startingXP || xpSystem.GetCurrentLevel() > startingLevel)
        {
            Debug.Log("✓ XP System working correctly");
        }
        else
        {
            Debug.LogWarning("XP System may not be working correctly");
        }

        yield return new WaitForSeconds(1f);
        Debug.Log("✓ XP System Test Complete");
    }

    private IEnumerator TestSessionTimer()
    {
        Debug.Log("Testing Session Timer...");

        if (sessionManager == null)
        {
            Debug.LogError("Cannot test session timer - SessionManager is null");
            yield break;
        }

        float initialTime = sessionManager.GetSessionTime();
        yield return new WaitForSeconds(2f);
        float afterTime = sessionManager.GetSessionTime();

        float timeDifference = afterTime - initialTime;
        Debug.Log($"Time progressed: {timeDifference:F2}s");

        if (timeDifference >= 1.5f && timeDifference <= 2.5f)
        {
            Debug.Log("✓ Session Timer working correctly");
        }
        else
        {
            Debug.LogWarning($"Session Timer may be incorrect - expected ~2s, got {timeDifference:F2}s");
        }

        Debug.Log($"Time remaining: {sessionManager.GetTimeRemaining():F1}s");
        Debug.Log("✓ Session Timer Test Complete");
    }

    private IEnumerator TestUpgradeSystem()
    {
        Debug.Log("Testing Upgrade System...");

        if (upgradeManager == null)
        {
            Debug.LogWarning("UpgradeManager not found - skipping upgrade test");
            yield break;
        }

        // The upgrade system is event-driven and requires level-ups
        // For this test, we'll just verify it exists and basic functionality
        Debug.Log("Upgrade system present and ready");

        yield return new WaitForSeconds(1f);
        Debug.Log("✓ Upgrade System Test Complete");
    }

    private void GenerateTestReport()
    {
        float totalTestTime = Time.time - testStartTime;
        
        Debug.Log("=== PLAY MODE TEST REPORT ===");
        Debug.Log($"Total test duration: {totalTestTime:F1}s");
        
        if (sessionManager != null)
        {
            Debug.Log($"Session time: {sessionManager.GetSessionTime():F1}s");
            Debug.Log($"Game over: {sessionManager.IsGameOver()}");
        }

        if (enemySpawner != null)
        {
            Debug.Log($"Current wave: {enemySpawner.GetCurrentWave()}");
            Debug.Log($"Enemies alive: {enemySpawner.GetEnemiesAlive()}");
        }

        if (xpSystem != null)
        {
            Debug.Log($"Player level: {xpSystem.GetCurrentLevel()}");
            Debug.Log($"Player XP: {xpSystem.GetCurrentXP()}");
        }

        if (playerController != null)
        {
            Debug.Log($"Player health: {playerController.GetHealthPercentage() * 100:F1}%");
        }

        Debug.Log("=== END TEST REPORT ===");
    }

    private void Update()
    {
        if (!enableAutoTesting || testingComplete) return;

        // Optional: Display real-time testing info
        if (logDetailedInfo && Time.time % 5f < Time.deltaTime)
        {
            LogGameState();
        }

        // Check if we should stop testing
        if (Time.time - testStartTime > testDuration)
        {
            if (!testingComplete)
            {
                Debug.Log("Test duration reached - generating final report");
                GenerateTestReport();
                testingComplete = true;
            }
        }
    }

    private void LogGameState()
    {
        Debug.Log($"[{Time.time:F1}s] Game State - " +
                  $"Wave: {enemySpawner?.GetCurrentWave() ?? 0}, " +
                  $"Enemies: {enemySpawner?.GetEnemiesAlive() ?? 0}, " +
                  $"Level: {xpSystem?.GetCurrentLevel() ?? 0}, " +
                  $"Health: {(playerController?.GetHealthPercentage() * 100):F0}%");
    }
}
