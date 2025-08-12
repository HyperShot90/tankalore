using UnityEngine;
using Tankalore.Contracts;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IDamageable, IPlayerEvents, IPlayerStats
{
    [Header("Tank Configuration")]
    [SerializeField] private TankStats tankStats;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    
    [Header("Audio")]
    [SerializeField] private AudioClip shootSound;
    
    // Components
    private Rigidbody2D rb;
    private AudioSource audioSource;
    
    // Input
    private Vector2 moveInput;
    private Vector2 aimDirection;
    
    // Combat
    private float nextFireTime = 0f;
    private float currentHealth;
    
    // Events
    public System.Action<float, float> OnHealthChanged; // currentHealth, maxHealth
    public System.Action OnPlayerDied;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        
        if (tankStats != null)
        {
            currentHealth = tankStats.GetEffectiveHealth();
        }
    }
    
    private void Update()
    {
        HandleInput();
        HandleAiming();
        HandleShooting();
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    private void HandleInput()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        moveInput = moveInput.normalized;
        
        // Mouse aiming
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        aimDirection = (mousePos - transform.position).normalized;
    }
    
    private void HandleAiming()
    {
        if (aimDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.AngleAxis(angle, Vector3.forward),
                tankStats.rotationSpeed * Time.deltaTime
            );
        }
    }
    
    private void HandleMovement()
    {
        Vector2 force = moveInput * tankStats.GetEffectiveMovementSpeed();
        rb.AddForce(force, ForceMode2D.Force);
        
        // Apply drag to prevent sliding - tuned for better control
        rb.drag = 8f;
    }
    
    private void HandleShooting()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (1f / tankStats.GetEffectiveFireRate());
        }
    }
    
    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;
        
        // Create bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // Set bullet properties
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(
                tankStats.GetEffectiveFirepower(),
                tankStats.bulletSpeed,
                tankStats.range,
                "Player"
            );
        }
        
        // Play sound
        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
    
    public void TakeDamage(float damage)
    {
        float effectiveArmor = tankStats.GetEffectiveArmor();
        float actualDamage = Mathf.Max(damage - effectiveArmor, damage * 0.1f); // At least 10% damage gets through
        
        currentHealth -= actualDamage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        
        OnHealthChanged?.Invoke(currentHealth, tankStats.GetEffectiveHealth());
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, tankStats.GetEffectiveHealth());
        OnHealthChanged?.Invoke(currentHealth, tankStats.GetEffectiveHealth());
    }
    
    private void Die()
    {
        OnPlayerDied?.Invoke();
        // Don't destroy immediately - let GameManager handle this
    }
    
    public void SetTankStats(TankStats newStats)
    {
        if (newStats != null)
        {
            float healthPercentage = currentHealth / tankStats.GetEffectiveHealth();
            tankStats = newStats;
            currentHealth = tankStats.GetEffectiveHealth() * healthPercentage;
            OnHealthChanged?.Invoke(currentHealth, tankStats.GetEffectiveHealth());
        }
    }
    
    public float GetHealthPercentage()
    {
        return currentHealth / tankStats.GetEffectiveHealth();
    }
    
    public float GetMaxHealth()
    {
        return tankStats.GetEffectiveHealth();
    }
    
    public IUpgradeableStats GetUpgradeableStats()
    {
        return tankStats;
    }
}
