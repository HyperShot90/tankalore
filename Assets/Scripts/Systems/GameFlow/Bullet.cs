using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private float damage;
    private float speed;
    private float maxDistance;
    private string ownerTag;
    
    private Vector3 startPosition;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void Initialize(float bulletDamage, float bulletSpeed, float range, string owner)
    {
        damage = bulletDamage;
        speed = bulletSpeed;
        maxDistance = range;
        ownerTag = owner;
        
        startPosition = transform.position;
        
        // Apply velocity
        rb.velocity = transform.up * speed;
        
        // Auto-destroy after maximum flight time
        Destroy(gameObject, range / speed + 1f);
    }
    
    private void Update()
    {
        // Check if bullet has traveled too far
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Don't hit the owner
        if (other.CompareTag(ownerTag))
            return;
            
        // Check for damageable objects
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            
            // Create impact effect here if needed
            CreateImpactEffect();
            
            Destroy(gameObject);
        }
        
        // Hit walls or other obstacles
        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            CreateImpactEffect();
            Destroy(gameObject);
        }
    }
    
    private void CreateImpactEffect()
    {
        // Placeholder for impact effects
        // You can instantiate particle effects here
    }
}

public interface IDamageable
{
    void TakeDamage(float damage);
}
