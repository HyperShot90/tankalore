using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Configuration")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Transform firePoint;
    
    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    
    // Runtime state
    private float currentHealth;
    private Transform player;
    private EnemyState currentState = EnemyState.Patrol;
    private Vector2 patrolTarget;
    private float nextFireTime;
    private float stateTimer;
    
    // Events
    public System.Action<EnemyController> OnEnemyDied;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        InitializeEnemy();
    }
    
    private void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        SetRandomPatrolTarget();
    }
    
    private void Update()
    {
        if (player == null) return;
        
        UpdateState();
        HandleBehavior();
        
        stateTimer += Time.deltaTime;
    }
    
    private void InitializeEnemy()
    {
        if (enemyData == null) return;
        
        currentHealth = enemyData.maxHealth;
        
        if (spriteRenderer && enemyData.enemySprite)
        {
            spriteRenderer.sprite = enemyData.enemySprite;
            spriteRenderer.color = enemyData.enemyColor;
        }
    }
    
    private void UpdateState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (distanceToPlayer <= enemyData.detectionRange)
                {
                    ChangeState(EnemyState.Pursue);
                }
                break;
                
            case EnemyState.Pursue:
                if (distanceToPlayer > enemyData.aggroRange)
                {
                    ChangeState(EnemyState.Patrol);
                }
                else if (distanceToPlayer <= enemyData.attackRange)
                {
                    ChangeState(EnemyState.Attack);
                }
                break;
                
            case EnemyState.Attack:
                if (distanceToPlayer > enemyData.attackRange)
                {
                    ChangeState(EnemyState.Pursue);
                }
                else if (GetHealthPercentage() <= enemyData.retreatHealthPercentage)
                {
                    ChangeState(EnemyState.Retreat);
                }
                break;
                
            case EnemyState.Retreat:
                if (stateTimer > 3f) // Retreat for 3 seconds
                {
                    ChangeState(EnemyState.Pursue);
                }
                break;
        }
    }
    
    private void HandleBehavior()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Pursue:
                Pursue();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Retreat:
                Retreat();
                break;
        }
    }
    
    private void Patrol()
    {
        MoveTowards(patrolTarget);
        
        // Check if reached patrol target
        if (Vector2.Distance(transform.position, patrolTarget) < 1f)
        {
            SetRandomPatrolTarget();
        }
        
        // Look around occasionally
        if (stateTimer > 2f)
        {
            LookTowards(player.position);
        }
    }
    
    private void Pursue()
    {
        MoveTowards(player.position);
        LookTowards(player.position);
    }
    
    private void Attack()
    {
        // Stop moving and focus on shooting
        rb.velocity = Vector2.zero;
        LookTowards(player.position);
        
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (1f / enemyData.fireRate);
        }
    }
    
    private void Retreat()
    {
        // Move away from player
        Vector2 retreatDirection = (transform.position - player.position).normalized;
        Vector2 retreatTarget = (Vector2)transform.position + retreatDirection * 5f;
        
        MoveTowards(retreatTarget);
        LookTowards(player.position); // Still face the player while retreating
    }
    
    private void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * enemyData.movementSpeed;
    }
    
    private void LookTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0, 0, angle),
                180f * Time.deltaTime
            );
        }
    }
    
    private void Shoot()
    {
        if (enemyData.bulletPrefab == null || firePoint == null) return;
        
        GameObject bullet = Instantiate(enemyData.bulletPrefab, firePoint.position, firePoint.rotation);
        
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(
                enemyData.damage,
                enemyData.bulletSpeed,
                enemyData.attackRange,
                "Enemy"
            );
        }
        
        // Play attack sound
        if (audioSource && enemyData.attackSound)
        {
            audioSource.PlayOneShot(enemyData.attackSound);
        }
    }
    
    private void SetRandomPatrolTarget()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randomDistance = Random.Range(3f, 8f);
        
        patrolTarget = (Vector2)transform.position + new Vector2(
            Mathf.Cos(randomAngle) * randomDistance,
            Mathf.Sin(randomAngle) * randomDistance
        );
    }
    
    private void ChangeState(EnemyState newState)
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
        else
        {
            // Aggro on damage
            if (currentState == EnemyState.Patrol)
            {
                ChangeState(EnemyState.Pursue);
            }
        }
    }
    
    private System.Collections.IEnumerator FlashEffect()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
    
    private void Die()
    {
        // Play death sound
        if (audioSource && enemyData.deathSound)
        {
            audioSource.PlayOneShot(enemyData.deathSound);
        }
        
        // Notify systems
        OnEnemyDied?.Invoke(this);
        
        // Award XP and currency
        XPSystem xpSystem = FindObjectOfType<XPSystem>();
        CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
        
        if (xpSystem != null)
            xpSystem.AddXP(enemyData.xpReward);
            
        if (currencyManager != null)
            currencyManager.AddCurrency(enemyData.currencyReward);
        
        Destroy(gameObject);
    }
    
    private float GetHealthPercentage()
    {
        return currentHealth / enemyData.maxHealth;
    }
    
    public EnemyData GetEnemyData()
    {
        return enemyData;
    }
}

public enum EnemyState
{
    Patrol,
    Pursue,
    Attack,
    Retreat
}
