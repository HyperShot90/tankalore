namespace Tankalore.Contracts
{
    /// <summary>
    /// Interface for upgradeable tank statistics that systems can modify
    /// </summary>
    public interface IUpgradeableStats
    {
        /// <summary>
        /// Health multiplier for upgrades
        /// </summary>
        float HealthMultiplier { get; set; }
        
        /// <summary>
        /// Armor multiplier for upgrades
        /// </summary>
        float ArmorMultiplier { get; set; }
        
        /// <summary>
        /// Firepower multiplier for upgrades
        /// </summary>
        float FirepowerMultiplier { get; set; }
        
        /// <summary>
        /// Speed multiplier for upgrades
        /// </summary>
        float SpeedMultiplier { get; set; }
        
        /// <summary>
        /// Fire rate multiplier for upgrades
        /// </summary>
        float FireRateMultiplier { get; set; }
        
        /// <summary>
        /// Gets the effective health considering multipliers
        /// </summary>
        float GetEffectiveHealth();
        
        /// <summary>
        /// Resets all multipliers to their default values
        /// </summary>
        void ResetMultipliers();
    }
}
