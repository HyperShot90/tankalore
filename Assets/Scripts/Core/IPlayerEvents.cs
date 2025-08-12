using System;

namespace Tankalore.Contracts
{
    /// <summary>
    /// Interface for player-related events that other systems can subscribe to
    /// </summary>
    public interface IPlayerEvents
    {
        /// <summary>
        /// Event triggered when the player dies
        /// </summary>
        event Action OnPlayerDied;
        
        /// <summary>
        /// Event triggered when player health changes (currentHealth, maxHealth)
        /// </summary>
        event Action<float, float> OnHealthChanged;
    }
}
