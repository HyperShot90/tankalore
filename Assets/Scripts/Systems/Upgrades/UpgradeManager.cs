using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("Upgrade Configuration")]
    [SerializeField] private List<UpgradeData> availableUpgrades = new List<UpgradeData>();
    [SerializeField] private int upgradesPerLevelUp = 3;
    
    // Current session upgrades
    private Dictionary<UpgradeType, int> appliedUpgrades = new Dictionary<UpgradeType, int>();
    private List<UpgradeData> sessionUpgrades = new List<UpgradeData>();
    
    // Components
    private PlayerController playerController;
    private TankStats playerTankStats;
    private XPSystem xpSystem;
    
    // Events
    public System.Action<List<UpgradeData>> OnUpgradeChoicesReady;
    public System.Action<UpgradeData> OnUpgradeApplied;
    
    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        xpSystem = FindObjectOfType<XPSystem>();
    }
    
    private void Start()
    {
        if (xpSystem != null)
        {
            xpSystem.OnLevelUp += HandleLevelUp;
        }
    }
    
    private void OnDestroy()
    {
        if (xpSystem != null)
        {
            xpSystem.OnLevelUp -= HandleLevelUp;
        }
    }
    
    private void HandleLevelUp(int newLevel)
    {
        List<UpgradeData> choices = GenerateUpgradeChoices(newLevel);
        OnUpgradeChoicesReady?.Invoke(choices);
        
        // Pause the game for upgrade selection
        Time.timeScale = 0f;
    }
    
    public List<UpgradeData> GenerateUpgradeChoices(int playerLevel)
    {
        List<UpgradeData> choices = new List<UpgradeData>();
        List<UpgradeData> eligibleUpgrades = GetEligibleUpgrades(playerLevel);
        
        if (eligibleUpgrades.Count == 0)
        {
            Debug.LogWarning("No eligible upgrades found!");
            return choices;
        }
        
        // Select random upgrades based on rarity weights
        for (int i = 0; i < upgradesPerLevelUp && eligibleUpgrades.Count > 0; i++)
        {
            UpgradeData selectedUpgrade = SelectWeightedRandomUpgrade(eligibleUpgrades);
            choices.Add(selectedUpgrade);
            
            // Remove from eligible list if it can't stack or reached max stacks
            if (!selectedUpgrade.canStack || GetUpgradeStacks(selectedUpgrade.upgradeType) >= selectedUpgrade.maxStacks - 1)
            {
                eligibleUpgrades.Remove(selectedUpgrade);
            }
        }
        
        return choices;
    }
    
    private List<UpgradeData> GetEligibleUpgrades(int playerLevel)
    {
        List<UpgradeData> eligible = new List<UpgradeData>();
        
        foreach (UpgradeData upgrade in availableUpgrades)
        {
            // Check level requirement
            if (upgrade.levelRequirement > playerLevel)
                continue;
                
            // Check if already at max stacks
            if (!upgrade.canStack && appliedUpgrades.ContainsKey(upgrade.upgradeType))
                continue;
                
            if (upgrade.canStack && GetUpgradeStacks(upgrade.upgradeType) >= upgrade.maxStacks)
                continue;
                
            eligible.Add(upgrade);
        }
        
        return eligible;
    }
    
    private UpgradeData SelectWeightedRandomUpgrade(List<UpgradeData> upgrades)
    {
        Dictionary<UpgradeRarity, float> rarityWeights = new Dictionary<UpgradeRarity, float>
        {
            { UpgradeRarity.Common, 50f },
            { UpgradeRarity.Uncommon, 30f },
            { UpgradeRarity.Rare, 15f },
            { UpgradeRarity.Epic, 4f },
            { UpgradeRarity.Legendary, 1f }
        };
        
        float totalWeight = 0f;
        List<float> weights = new List<float>();
        
        foreach (UpgradeData upgrade in upgrades)
        {
            float weight = rarityWeights[upgrade.rarity];
            weights.Add(weight);
            totalWeight += weight;
        }
        
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        for (int i = 0; i < upgrades.Count; i++)
        {
            currentWeight += weights[i];
            if (randomValue <= currentWeight)
            {
                return upgrades[i];
            }
        }
        
        return upgrades[upgrades.Count - 1]; // Fallback
    }
    
    public void ApplyUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null) return;
        
        // Track the upgrade
        if (!appliedUpgrades.ContainsKey(upgrade.upgradeType))
        {
            appliedUpgrades[upgrade.upgradeType] = 0;
        }
        appliedUpgrades[upgrade.upgradeType]++;
        
        sessionUpgrades.Add(upgrade);
        
        // Apply the effect to the player
        ApplyUpgradeEffect(upgrade);
        
        OnUpgradeApplied?.Invoke(upgrade);
        
        // Resume the game
        Time.timeScale = 1f;
        
        Debug.Log($"Applied upgrade: {upgrade.upgradeName}");
    }
    
    private void ApplyUpgradeEffect(UpgradeData upgrade)
    {
        if (playerTankStats == null)
        {
            Debug.LogError("Player tank stats not found!");
            return;
        }
        
        int stacks = GetUpgradeStacks(upgrade.upgradeType);
        float effectValue = upgrade.effectValue;
        
        // Apply stacking efficiency
        if (stacks > 1)
        {
            effectValue = 1f + (upgrade.effectValue - 1f) * Mathf.Pow(upgrade.stackingEfficiency, stacks - 1);
        }
        
        switch (upgrade.upgradeType)
        {
            case UpgradeType.HealthBoost:
                playerTankStats.healthMultiplier *= effectValue;
                break;
            case UpgradeType.ArmorBoost:
                playerTankStats.armorMultiplier *= effectValue;
                break;
            case UpgradeType.FirepowerBoost:
                playerTankStats.firepowerMultiplier *= effectValue;
                break;
            case UpgradeType.SpeedBoost:
                playerTankStats.speedMultiplier *= effectValue;
                break;
            case UpgradeType.FireRateBoost:
                playerTankStats.fireRateMultiplier *= effectValue;
                break;
            // Add more upgrade types as needed
        }
        
        // If it's a health boost, heal the player to the new max health
        if (upgrade.upgradeType == UpgradeType.HealthBoost && playerController != null)
        {
            float newMaxHealth = playerTankStats.GetEffectiveHealth();
            float currentHealthPercentage = playerController.GetHealthPercentage();
            float newCurrentHealth = newMaxHealth * currentHealthPercentage;
            
            // Set health to maintain the same percentage, but with new max
            playerController.Heal(newCurrentHealth - (newMaxHealth * currentHealthPercentage));
        }
    }
    
    public int GetUpgradeStacks(UpgradeType upgradeType)
    {
        return appliedUpgrades.ContainsKey(upgradeType) ? appliedUpgrades[upgradeType] : 0;
    }
    
    public List<UpgradeData> GetSessionUpgrades()
    {
        return new List<UpgradeData>(sessionUpgrades);
    }
    
    public void ResetSessionUpgrades()
    {
        appliedUpgrades.Clear();
        sessionUpgrades.Clear();
        
        // Reset player stats multipliers
        if (playerTankStats != null)
        {
            playerTankStats.ResetMultipliers();
        }
    }
    
    public void SetPlayerTankStats(TankStats tankStats)
    {
        playerTankStats = tankStats;
    }
}
