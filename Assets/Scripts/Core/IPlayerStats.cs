namespace Tankalore.Contracts
{
    /// <summary>
    /// Interface for player stats that can be upgraded by systems
    /// </summary>
    public interface IPlayerStats
    {
        /// <summary>
        /// Gets the current health percentage (0.0 to 1.0)
        /// </summary>
        float GetHealthPercentage();
        
        /// <summary>
        /// Heals the player by the specified amount
        /// </summary>
        /// <param name="amount">Amount to heal</param>
        void Heal(float amount);
        
        /// <summary>
        /// Gets the current effective max health
        /// </summary>
        float GetMaxHealth();
        
        /// <summary>
        /// Gets the upgradeable stats interface for modifying multipliers
        /// </summary>
        IUpgradeableStats GetUpgradeableStats();
    }
}
