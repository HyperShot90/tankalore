using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [Header("Currency Configuration")]
    [SerializeField] private int startingCurrency = 0;
    
    // Currency state
    private int sessionCurrency = 0; // Currency earned this session
    private int totalCurrency = 0; // Persistent currency
    
    // Events
    public System.Action<int> OnSessionCurrencyChanged;
    public System.Action<int> OnTotalCurrencyChanged;
    public System.Action<int> OnCurrencySpent; // amount spent
    
    private void Awake()
    {
        LoadCurrency();
    }
    
    private void Start()
    {
        sessionCurrency = startingCurrency;
        OnSessionCurrencyChanged?.Invoke(sessionCurrency);
        OnTotalCurrencyChanged?.Invoke(totalCurrency);
    }
    
    public void AddCurrency(int amount)
    {
        if (amount <= 0) return;
        
        sessionCurrency += amount;
        OnSessionCurrencyChanged?.Invoke(sessionCurrency);
        
        Debug.Log($"Gained {amount} currency. Session total: {sessionCurrency}");
    }
    
    public bool CanAfford(int cost)
    {
        return totalCurrency >= cost;
    }
    
    public bool SpendCurrency(int amount)
    {
        if (amount <= 0) return false;
        
        if (!CanAfford(amount))
        {
            Debug.Log($"Cannot afford {amount} currency. Have: {totalCurrency}");
            return false;
        }
        
        totalCurrency -= amount;
        OnTotalCurrencyChanged?.Invoke(totalCurrency);
        OnCurrencySpent?.Invoke(amount);
        
        SaveCurrency();
        
        Debug.Log($"Spent {amount} currency. Remaining: {totalCurrency}");
        return true;
    }
    
    public void EndSession()
    {
        // Add session currency to total currency
        totalCurrency += sessionCurrency;
        OnTotalCurrencyChanged?.Invoke(totalCurrency);
        
        SaveCurrency();
        
        Debug.Log($"Session ended. Added {sessionCurrency} to total currency. New total: {totalCurrency}");
    }
    
    public void ResetSession()
    {
        sessionCurrency = 0;
        OnSessionCurrencyChanged?.Invoke(sessionCurrency);
    }
    
    private void LoadCurrency()
    {
        totalCurrency = PlayerPrefs.GetInt("TotalCurrency", 0);
    }
    
    private void SaveCurrency()
    {
        PlayerPrefs.SetInt("TotalCurrency", totalCurrency);
        PlayerPrefs.Save();
    }
    
    public int GetSessionCurrency()
    {
        return sessionCurrency;
    }
    
    public int GetTotalCurrency()
    {
        return totalCurrency;
    }
    
    // For debugging/testing
    public void AddTotalCurrency(int amount)
    {
        totalCurrency += amount;
        OnTotalCurrencyChanged?.Invoke(totalCurrency);
        SaveCurrency();
    }
    
    public void ResetAllCurrency()
    {
        sessionCurrency = 0;
        totalCurrency = 0;
        OnSessionCurrencyChanged?.Invoke(sessionCurrency);
        OnTotalCurrencyChanged?.Invoke(totalCurrency);
        SaveCurrency();
        
        Debug.Log("All currency reset!");
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveCurrency();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveCurrency();
        }
    }
    
    private void OnDestroy()
    {
        SaveCurrency();
    }
}
