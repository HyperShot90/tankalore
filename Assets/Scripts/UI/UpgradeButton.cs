using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image borderImage;
    
    // Current upgrade data
    private UpgradeData currentUpgrade;
    
    // Events
    public System.Action<UpgradeData> OnUpgradeSelected;
    
    // Rarity colors
    private readonly Color commonColor = Color.white;
    private readonly Color uncommonColor = Color.green;
    private readonly Color rareColor = Color.blue;
    private readonly Color epicColor = Color.magenta;
    private readonly Color legendaryColor = Color.yellow;
    
    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
            
        if (button != null)
            button.onClick.AddListener(OnButtonClicked);
    }
    
    public void SetUpgrade(UpgradeData upgrade)
    {
        currentUpgrade = upgrade;
        
        if (upgrade == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        // Update visual elements
        UpdateIcon();
        UpdateText();
        UpdateColors();
        
        gameObject.SetActive(true);
    }
    
    private void UpdateIcon()
    {
        if (iconImage != null && currentUpgrade.icon != null)
        {
            iconImage.sprite = currentUpgrade.icon;
            iconImage.gameObject.SetActive(true);
        }
        else if (iconImage != null)
        {
            iconImage.gameObject.SetActive(false);
        }
    }
    
    private void UpdateText()
    {
        if (nameText != null)
        {
            nameText.text = currentUpgrade.upgradeName;
        }
        
        if (descriptionText != null)
        {
            string description = currentUpgrade.description;
            
            // Add effect value to description
            if (currentUpgrade.isPercentage)
            {
                float percentage = (currentUpgrade.effectValue - 1f) * 100f;
                description += $"\n+{percentage:F0}% {GetEffectTypeString()}";
            }
            else
            {
                description += $"\n+{currentUpgrade.effectValue:F0} {GetEffectTypeString()}";
            }
            
            // Show stacking info if applicable
            UpgradeManager upgradeManager = FindObjectOfType<UpgradeManager>();
            if (upgradeManager != null)
            {
                int currentStacks = upgradeManager.GetUpgradeStacks(currentUpgrade.upgradeType);
                if (currentStacks > 0)
                {
                    description += $"\nCurrent Stacks: {currentStacks}";
                }
            }
            
            descriptionText.text = description;
        }
    }
    
    private void UpdateColors()
    {
        Color rarityColor = GetRarityColor();
        
        if (borderImage != null)
        {
            borderImage.color = rarityColor;
        }
        
        if (backgroundImage != null)
        {
            // Slightly transparent background with rarity tint
            Color bgColor = rarityColor;
            bgColor.a = 0.3f;
            backgroundImage.color = bgColor;
        }
        
        if (nameText != null)
        {
            nameText.color = rarityColor;
        }
    }
    
    private Color GetRarityColor()
    {
        if (currentUpgrade == null)
            return commonColor;
            
        return currentUpgrade.rarity switch
        {
            UpgradeRarity.Common => commonColor,
            UpgradeRarity.Uncommon => uncommonColor,
            UpgradeRarity.Rare => rareColor,
            UpgradeRarity.Epic => epicColor,
            UpgradeRarity.Legendary => legendaryColor,
            _ => commonColor
        };
    }
    
    private string GetEffectTypeString()
    {
        return currentUpgrade.upgradeType switch
        {
            UpgradeType.HealthBoost => "Health",
            UpgradeType.ArmorBoost => "Armor",
            UpgradeType.FirepowerBoost => "Damage",
            UpgradeType.SpeedBoost => "Speed",
            UpgradeType.FireRateBoost => "Fire Rate",
            UpgradeType.BulletSpeedBoost => "Bullet Speed",
            UpgradeType.RangeBoost => "Range",
            UpgradeType.HealthRegen => "Health Regen",
            UpgradeType.CriticalChance => "Crit Chance",
            UpgradeType.CriticalDamage => "Crit Damage",
            UpgradeType.ExplosiveBullets => "Explosive",
            UpgradeType.PiercingShots => "Piercing",
            UpgradeType.DoubleShot => "Double Shot",
            UpgradeType.TripleShot => "Triple Shot",
            _ => "Effect"
        };
    }
    
    private void OnButtonClicked()
    {
        if (currentUpgrade != null)
        {
            OnUpgradeSelected?.Invoke(currentUpgrade);
        }
    }
    
    // Animation and feedback methods
    public void OnPointerEnter()
    {
        // Scale up slightly on hover
        transform.localScale = Vector3.one * 1.05f;
    }
    
    public void OnPointerExit()
    {
        // Return to normal scale
        transform.localScale = Vector3.one;
    }
    
    public void OnPointerDown()
    {
        // Scale down slightly on press
        transform.localScale = Vector3.one * 0.95f;
    }
    
    public void OnPointerUp()
    {
        // Return to hover scale
        transform.localScale = Vector3.one * 1.05f;
    }
}
