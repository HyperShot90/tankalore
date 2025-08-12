using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Tankalore/Upgrade")]
public class UpgradeData : ScriptableObject
{
    [Header("Basic Info")]
    public string upgradeName = "Upgrade";
    public string description = "Upgrade description";
    public Sprite icon;
    
    [Header("Effects")]
    public UpgradeType upgradeType;
    public float effectValue = 1.1f; // Multiplier or additive value
    public bool isPercentage = true; // True for multipliers, false for flat values
    
    [Header("Stacking")]
    public bool canStack = true;
    public int maxStacks = 5;
    public float stackingEfficiency = 0.8f; // Each stack is 80% as effective
    
    [Header("Requirements")]
    public int levelRequirement = 1;
    public UpgradeRarity rarity = UpgradeRarity.Common;
    
    [Header("Visual")]
    public Color rarityColor = Color.white;
}

public enum UpgradeType
{
    HealthBoost,
    ArmorBoost,
    FirepowerBoost,
    SpeedBoost,
    FireRateBoost,
    BulletSpeedBoost,
    RangeBoost,
    HealthRegen,
    CriticalChance,
    CriticalDamage,
    ExplosiveBullets,
    PiercingShots,
    DoubleShot,
    TripleShot
}

public enum UpgradeRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
