using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Configuration")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> bossPrefabs = new List<GameObject>();
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnRadius = 15f;
    
    [Header("Wave Configuration")]
    [SerializeField] private int baseEnemiesPerWave = 5;
    [SerializeField] private float waveScaling = 1.2f;
    [SerializeField] private float timeBetweenWaves = 10f;
    [SerializeField] private float timeBetweenSpawns = 1f;
    
    [Header("Boss Configuration")]
    [SerializeField] private int wavesBetweenBosses = 5;
    [SerializeField] private float bossSpawnDelay = 3f;
    
    // Current state
    private int currentWave = 0;
    private int enemiesAlive = 0;
    private int enemiesSpawned = 0;
    private int enemiesToSpawn = 0;
    private bool isSpawningWave = false;
    private bool isBossActive = false;
    private float nextSpawnTime = 0f;
    private float nextWaveTime = 0f;
    
    // Components
    private Transform player;
    private SessionManager sessionManager;
    
    // Events
    public System.Action<int> OnWaveStarted;
    public System.Action<int> OnWaveCompleted;
    public System.Action OnBossSpawned;
    public System.Action OnBossDefeated;
    
    private void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        sessionManager = FindObjectOfType<SessionManager>();
        
        // Start first wave after a delay
        nextWaveTime = Time.time + 3f;
    }
    
    private void Update()
    {
        if (sessionManager != null && sessionManager.IsGameOver())
            return;
            
        HandleWaveSpawning();
        HandleEnemySpawning();
    }
    
    private void HandleWaveSpawning()
    {
        // Check if it's time to start a new wave
        if (!isSpawningWave && enemiesAlive == 0 && Time.time >= nextWaveTime)
        {
            StartNewWave();
        }
        
        // Check if wave is complete
        if (isSpawningWave && enemiesSpawned >= enemiesToSpawn && enemiesAlive == 0)
        {
            CompleteWave();
        }
    }
    
    private void HandleEnemySpawning()
    {
        if (!isSpawningWave || enemiesSpawned >= enemiesToSpawn)
            return;
            
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + timeBetweenSpawns;
        }
    }
    
    private void StartNewWave()
    {
        currentWave++;
        isSpawningWave = true;
        enemiesSpawned = 0;
        
        // Calculate enemies for this wave
        enemiesToSpawn = Mathf.RoundToInt(baseEnemiesPerWave * Mathf.Pow(waveScaling, currentWave - 1));
        enemiesToSpawn = Mathf.Min(enemiesToSpawn, 30); // Cap at 30 enemies
        
        OnWaveStarted?.Invoke(currentWave);
        
        Debug.Log($"Wave {currentWave} started! Spawning {enemiesToSpawn} enemies");
        
        // Check if boss wave
        if (currentWave % wavesBetweenBosses == 0 && !isBossActive)
        {
            Invoke(nameof(SpawnBoss), bossSpawnDelay);
        }
    }
    
    private void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0 || player == null)
            return;
            
        Vector3 spawnPosition = GetSpawnPosition();
        GameObject enemyPrefab = GetRandomEnemyPrefab();
        
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        // Subscribe to enemy death
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.OnEnemyDied += OnEnemyDied;
        }
        
        enemiesSpawned++;
        enemiesAlive++;
    }
    
    private void SpawnBoss()
    {
        if (bossPrefabs.Count == 0 || player == null)
            return;
            
        Vector3 spawnPosition = GetSpawnPosition(true);
        GameObject bossPrefab = GetRandomBossPrefab();
        
        GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        
        // Subscribe to boss death
        BossController bossController = boss.GetComponent<BossController>();
        if (bossController != null)
        {
            bossController.OnBossDied += OnBossDied;
        }
        else
        {
            // If no boss controller, treat as regular enemy
            EnemyController enemyController = boss.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.OnEnemyDied += OnEnemyDied;
            }
        }
        
        isBossActive = true;
        enemiesAlive++;
        OnBossSpawned?.Invoke();
        
        Debug.Log("Boss spawned!");
    }
    
    private Vector3 GetSpawnPosition(bool isBoss = false)
    {
        Vector3 spawnPosition;
        float radius = isBoss ? spawnRadius * 1.5f : spawnRadius;
        
        // Use spawn points if available, otherwise spawn in circle around player
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnPosition = spawnPoint.position;
        }
        else if (player != null)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            spawnPosition = player.position + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );
        }
        else
        {
            spawnPosition = Vector3.zero;
        }
        
        return spawnPosition;
    }
    
    private GameObject GetRandomEnemyPrefab()
    {
        // Simple random selection - could be enhanced with wave-based difficulty
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
    }
    
    private GameObject GetRandomBossPrefab()
    {
        return bossPrefabs[Random.Range(0, bossPrefabs.Count)];
    }
    
    private void OnEnemyDied(EnemyController enemy)
    {
        if (enemy != null)
        {
            enemy.OnEnemyDied -= OnEnemyDied;
        }
        
        enemiesAlive--;
        enemiesAlive = Mathf.Max(enemiesAlive, 0);
    }
    
    private void OnBossDied()
    {
        enemiesAlive--;
        enemiesAlive = Mathf.Max(enemiesAlive, 0);
        isBossActive = false;
        OnBossDefeated?.Invoke();
        
        Debug.Log("Boss defeated!");
    }
    
    private void CompleteWave()
    {
        isSpawningWave = false;
        nextWaveTime = Time.time + timeBetweenWaves;
        
        OnWaveCompleted?.Invoke(currentWave);
        
        Debug.Log($"Wave {currentWave} completed!");
    }
    
    public int GetCurrentWave()
    {
        return currentWave;
    }
    
    public int GetEnemiesAlive()
    {
        return enemiesAlive;
    }
    
    public bool IsSpawningWave()
    {
        return isSpawningWave;
    }
    
    public bool IsBossActive()
    {
        return isBossActive;
    }
    
    public float GetTimeUntilNextWave()
    {
        return Mathf.Max(0f, nextWaveTime - Time.time);
    }
    
    public void ForceStopSpawning()
    {
        isSpawningWave = false;
        isBossActive = false;
    }
}
