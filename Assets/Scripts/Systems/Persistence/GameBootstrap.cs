using UnityEngine;

/// <summary>
/// Bootstrap script that initializes core game systems early in the application lifecycle
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeGame()
    {
        Debug.Log("GameBootstrap: Initializing core systems...");
        
        // Ensure SaveSystem is created and initialized early
        var saveSystem = SaveSystem.Instance;
        Debug.Log("GameBootstrap: SaveSystem initialized");
        
        // Load game data at startup
        var gameData = saveSystem.LoadGameData();
        saveSystem.ApplyLoadedData(gameData);
        
        Debug.Log($"GameBootstrap: Game data loaded - Currency: {gameData.currency}");
    }
}
