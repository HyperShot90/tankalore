using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour, IDamageable
{
    [Header("Boss Configuration")]
    [SerializeField] private EnemyData bossData;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private GameObject specialAttackPrefab;
    
    [Header("Boss Specific Settings")]
    [SerializeField] private float specialAttackCooldown = 10f;
    [SerializeField] private float phaseTransitionHealth = 0.5f;
    [SerializeField] private int maxPhases = 3;
    
    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    
    // Boss state
    private float currentHealth;
    private float maxHealth;
    private Transform player;
    private BossState currentState = BossState.Idle;
    private BossPhase currentPhase = BossPhase.Phase1;
    private float stateTimer = 0f;
    private float nextSpecialAttackTime = 0f;
    private float nextFireTime = 0f;
    
    // Movement patterns
    private Vector3 targetPosition;
    private bool isMovingToTarget = false;
    
    // Events
    public System.Action OnBossDied;
    public System.Action<BossPhase> OnPhaseChanged;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        InitializeBoss();
    }
    
    private void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        StartBehavior();
    }
    
    private void Update()
    {
        if (player == null) return;
        
        stateTimer += Time.deltaTime;
        
        UpdateBehavior();
        CheckPhaseTransition();
    }
    
    private void InitializeBoss()
    {
        if (bossData == null) return;
        
        maxHealth = bossData.maxHealth;
        currentHealth = maxHealth;
        
        if (spriteRenderer && bossData.enemySprite)
        {
            spriteRenderer.sprite = bossData.enemySprite;
            spriteRenderer.color = bossData.enemyColor;
        }
        
        // Scale up boss sprite
        transform.localScale = Vector3.one * 1.5f;
    }
    
    private void StartBehavior()
    {
        nextSpecialAttackTime = Time.time + specialAttackCooldown;
        ChangeState(BossState.Approach);
    }
    
    private void UpdateBehavior()
    {
        switch (currentState)
        {
            case BossState.Idle:
                HandleIdleState();
                break;
            case BossState.Approach:
                HandleApproachState();
                break;
            case BossState.Attack:
                HandleAttackState();
                break;
            case BossState.SpecialAttack:
                HandleSpecialAttackState();
                break;
            case BossState.Retreat:
                HandleRetreatState();
                break;
            case BossState.Circle:
                HandleCircleState();
                break;
        }
    }
    
    private void HandleIdleState()
    {
        if (stateTimer > 2f)
        {
            ChangeState(BossState.Approach);
        }
    }
    
    private void HandleApproachState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer > bossData.attackRange * 1.2f)
        {
            MoveTowards(player.position);
        }
        else
        {
            ChangeState(BossState.Attack);
        }
        
        LookTowards(player.position);
    }
    
    private void HandleAttackState()
    {
        // Stop moving and focus on attacking
        rb.velocity = Vector2.zero;
        LookTowards(player.position);
        
        // Regular attack
        if (Time.time >= nextFireTime)
        {
            PerformRegularAttack();
            nextFireTime = Time.time + (1f / GetCurrentFireRate());
        }
        
        // Check for special attack
        if (Time.time >= nextSpecialAttackTime)
        {
            ChangeState(BossState.SpecialAttack);
            return;
        }
        
        // State timeout - move to different position
        if (stateTimer > 5f)
        {
            ChangeState(Random.value > 0.5f ? BossState.Circle : BossState.Retreat);
        }
    }
    
    private void HandleSpecialAttackState()
    {
        // Stop moving during special attack
        rb.velocity = Vector2.zero;
        
        if (stateTimer > 0.5f) // Wind-up time
        {
            PerformSpecialAttack();
            nextSpecialAttackTime = Time.time + specialAttackCooldown;
            ChangeState(BossState.Retreat);
        }
    }
    
    private void HandleRetreatState()
    {
        // Move away from player
        Vector2 retreatDirection = (transform.position - player.position).normalized;
        Vector2 retreatTarget = (Vector2)transform.position + retreatDirection * 8f;
        
        MoveTowards(retreatTarget);
        LookTowards(player.position);
        
        if (stateTimer > 3f)
        {
            ChangeState(BossState.Circle);
        }
    }
    
    private void HandleCircleState()
    {
        // Circle around the player
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector2 perpendicular = new Vector2(-directionToPlayer.y, directionToPlayer.x);
        Vector2 circleTarget = (Vector2)player.position + perpendicular * bossData.attackRange;
        
        MoveTowards(circleTarget);
        LookTowards(player.position);
        
        // Attack while circling
        if (Time.time >= nextFireTime)
        {
            PerformRegularAttack();
            nextFireTime = Time.time + (1f / GetCurrentFireRate());
        }
        
        if (stateTimer > 4f)
        {
            ChangeState(BossState.Approach);
        }
    }
    
    private void PerformRegularAttack()
    {
        if (bossData.bulletPrefab == null || firePoints == null || firePoints.Length == 0)
            return;
        
        // Fire from all fire points
        foreach (Transform firePoint in firePoints)
        {
            if (firePoint == null) continue;
            
            GameObject bullet = Instantiate(bossData.bulletPrefab, firePoint.position, firePoint.rotation);
            
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(
                    GetCurrentDamage(),
                    bossData.bulletSpeed,
                    bossData.attackRange,
                    "Enemy"
                );
            }
        }
        
        // Play attack sound
        if (audioSource && bossData.attackSound)
        {
            audioSource.PlayOneShot(bossData.attackSound);
        }
    }
    
    private void PerformSpecialAttack()
    {
        switch (currentPhase)
        {
            case BossPhase.Phase1:
                PerformSpreadShot();
                break;
            case BossPhase.Phase2:
                PerformRapidFire();
                break;
            case BossPhase.Phase3:
                PerformExplosiveShot();
                break;
        }
    }
    
    private void PerformSpreadShot()
    {
        if (bossData.bulletPrefab == null) return;
        
        int bulletCount = 8;
        float angleStep = 360f / bulletCount;
        
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.up;
            Vector3 spawnPos = transform.position + direction * 1f;
            
            GameObject bullet = Instantiate(bossData.bulletPrefab, spawnPos, Quaternion.LookRotation(Vector3.forward, direction));
            
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(
                    GetCurrentDamage() * 0.8f, // Slightly less damage for spread shot
                    bossData.bulletSpeed,
                    bossData.attackRange,
                    "Enemy"
                );
            }
        }
    }
    
    private void PerformRapidFire()
    {
        StartCoroutine(RapidFireCoroutine());
    }
    
    private IEnumerator RapidFireCoroutine()
    {
        int shots = 10;
        float rapidFireRate = 0.1f;
        
        for (int i = 0; i < shots; i++)
        {
            if (player == null) break;
            
            // Aim at player for each shot
            LookTowards(player.position);
            PerformRegularAttack();
            
            yield return new WaitForSeconds(rapidFireRate);
        }
    }
    
    private void PerformExplosiveShot()
    {
        if (specialAttackPrefab != null)
        {
            Vector3 targetPos = player.position;
            GameObject explosive = Instantiate(specialAttackPrefab, targetPos, Quaternion.identity);
            
            // You would implement explosive behavior in the special attack prefab
        }
        else
        {
            // Fallback to enhanced spread shot
            PerformSpreadShot();
        }
    }
    
    private void CheckPhaseTransition()
    {
        float healthPercentage = GetHealthPercentage();
        
        if (currentPhase == BossPhase.Phase1 && healthPercentage <= 0.66f)
        {
            TransitionToPhase(BossPhase.Phase2);
        }
        else if (currentPhase == BossPhase.Phase2 && healthPercentage <= 0.33f)
        {
            TransitionToPhase(BossPhase.Phase3);
        }
    }
    
    private void TransitionToPhase(BossPhase newPhase)
    {
        currentPhase = newPhase;
        OnPhaseChanged?.Invoke(currentPhase);
        
        // Visual feedback for phase transition
        StartCoroutine(PhaseTransitionEffect());
        
        Debug.Log($"Boss transitioned to {currentPhase}");
    }
    
    private IEnumerator PhaseTransitionEffect()
    {
        // Flash effect and brief invulnerability
        Color originalColor = spriteRenderer.color;
        
        for (int i = 0; i < 6; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * GetCurrentMovementSpeed();
    }
    
    private void LookTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.AngleTowards(
                transform.rotation,
                Quaternion.Euler(0, 0, angle),
                180f * Time.deltaTime
            );
        }
    }
    
    private void ChangeState(BossState newState)
    {
        currentState = newState;
        stateTimer = 0f;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        
        // Flash effect on hit
        StartCoroutine(FlashEffect());
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    
    private IEnumerator FlashEffect()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
    
    private void Die()
    {
        // Play death sound
        if (audioSource && bossData.deathSound)
        {
            audioSource.PlayOneShot(bossData.deathSound);
        }
        
        // Award XP and currency (bosses give more)
        XPSystem xpSystem = FindObjectOfType<XPSystem>();
        CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
        
        if (xpSystem != null)
            xpSystem.AddXP(bossData.xpReward * 5); // 5x normal XP
            
        if (currencyManager != null)
            currencyManager.AddCurrency(bossData.currencyReward * 3); // 3x normal currency
        
        OnBossDied?.Invoke();
        
        Destroy(gameObject);
    }
    
    // Phase-based stat scaling
    private float GetCurrentDamage()
    {
        float multiplier = currentPhase switch
        {
            BossPhase.Phase1 => 1f,
            BossPhase.Phase2 => 1.2f,
            BossPhase.Phase3 => 1.5f,
            _ => 1f
        };
        return bossData.damage * multiplier;
    }
    
    private float GetCurrentFireRate()
    {
        float multiplier = currentPhase switch
        {
            BossPhase.Phase1 => 1f,
            BossPhase.Phase2 => 1.3f,
            BossPhase.Phase3 => 1.6f,
            _ => 1f
        };
        return bossData.fireRate * multiplier;
    }
    
    private float GetCurrentMovementSpeed()
    {
        float multiplier = currentPhase switch
        {
            BossPhase.Phase1 => 1f,
            BossPhase.Phase2 => 1.1f,
            BossPhase.Phase3 => 1.25f,
            _ => 1f
        };
        return bossData.movementSpeed * multiplier;
    }
    
    private float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    public BossPhase GetCurrentPhase()
    {
        return currentPhase;
    }
    
    public EnemyData GetBossData()
    {
        return bossData;
    }
}

public enum BossState
{
    Idle,
    Approach,
    Attack,
    SpecialAttack,
    Retreat,
    Circle
}

public enum BossPhase
{
    Phase1,
    Phase2,
    Phase3
}
