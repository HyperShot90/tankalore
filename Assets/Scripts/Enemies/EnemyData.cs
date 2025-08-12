using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Tankalore/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Stats")]
    public string enemyName = "Enemy";
    public float maxHealth = 50f;
    public float damage = 15f;
    public float movementSpeed = 3f;
    public float detectionRange = 8f;
    public float attackRange = 6f;
    
    [Header("Combat")]
    public float fireRate = 0.5f;
    public float bulletSpeed = 8f;
    public GameObject bulletPrefab;
    
    [Header("Behavior")]
    public EnemyType enemyType = EnemyType.Basic;
    public float aggroRange = 10f;
    public float retreatHealthPercentage = 0.2f;
    
    [Header("Rewards")]
    public int xpReward = 10;
    public int currencyReward = 5;
    public float dropChance = 0.1f;
    
    [Header("Visual")]
    public Sprite enemySprite;
    public Color enemyColor = Color.red;
    
    [Header("Audio")]
    public AudioClip deathSound;
    public AudioClip attackSound;
}

public enum EnemyType
{
    Basic,
    Fast,
    Heavy,
    Sniper,
    Explosive
}
