using UnityEngine;
using Tankalore.Contracts;

[CreateAssetMenu(fileName = "New Tank Stats", menuName = "Tankalore/Tank Stats")]
public class TankStats : ScriptableObject, IUpgradeableStats
{
    [Header("Base Stats")]
    public float maxHealth = 100f;
    public float armor = 0f;
    public float firepower = 25f;
    public float movementSpeed = 5f;
    public float rotationSpeed = 180f;
    
    [Header("Combat Stats")]
    public float fireRate = 1f; // Shots per second
    public float bulletSpeed = 10f;
    public float range = 10f;
    
    [Header("Visual")]
    public Sprite tankSprite;
    public string tankName = "Tank";
    
    [Header("Upgrade Multipliers")]
    public float healthMultiplier = 1f;
    public float armorMultiplier = 1f;
    public float firepowerMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float fireRateMultiplier = 1f;
    
    // Interface properties
    public float HealthMultiplier { get => healthMultiplier; set => healthMultiplier = value; }
    public float ArmorMultiplier { get => armorMultiplier; set => armorMultiplier = value; }
    public float FirepowerMultiplier { get => firepowerMultiplier; set => firepowerMultiplier = value; }
    public float SpeedMultiplier { get => speedMultiplier; set => speedMultiplier = value; }
    public float FireRateMultiplier { get => fireRateMultiplier; set => fireRateMultiplier = value; }
    
    // Runtime stats calculation
    public float GetEffectiveHealth() => maxHealth * healthMultiplier;
    public float GetEffectiveArmor() => armor * armorMultiplier;
    public float GetEffectiveFirepower() => firepower * firepowerMultiplier;
    public float GetEffectiveMovementSpeed() => movementSpeed * speedMultiplier;
    public float GetEffectiveFireRate() => fireRate * fireRateMultiplier;
    
    public void ResetMultipliers()
    {
        healthMultiplier = 1f;
        armorMultiplier = 1f;
        firepowerMultiplier = 1f;
        speedMultiplier = 1f;
        fireRateMultiplier = 1f;
    }
}
