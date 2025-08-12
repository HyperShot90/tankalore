using UnityEngine;

/// <summary>
/// Simple component to mark GameObjects as DontDestroyOnLoad
/// </summary>
public class DontDestroyOnLoadMarker : MonoBehaviour
{
    private void Awake()
    {
        // Check if this is a duplicate instance
        DontDestroyOnLoadMarker[] existing = FindObjectsOfType<DontDestroyOnLoadMarker>();
        
        if (existing.Length > 1)
        {
            // Destroy this duplicate instance
            Destroy(gameObject);
            return;
        }
        
        // Mark as don't destroy on load
        DontDestroyOnLoad(gameObject);
    }
}
