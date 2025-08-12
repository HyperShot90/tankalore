using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class SceneSetupEditor : EditorWindow
{
    [MenuItem("Tankalore/Setup Game Scene")]
    public static void SetupGameScene()
    {
        // Main Camera setup with URP 2D Renderer
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
            var cameraData = mainCamera.GetUniversalAdditionalCameraData();
            if (cameraData != null)
            {
                cameraData.SetRenderer(0); // Use first renderer (should be 2D renderer)
            }
        }

        // EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }

        // Systems GameObject with all required components
        GameObject systemsGO = GameObject.Find("Systems");
        if (systemsGO == null)
        {
            systemsGO = new GameObject("Systems");
        }
        
        // Add all system components
        if (systemsGO.GetComponent<SessionManager>() == null)
            systemsGO.AddComponent<SessionManager>();
            
        if (systemsGO.GetComponent<EnemySpawner>() == null)
            systemsGO.AddComponent<EnemySpawner>();
            
        if (systemsGO.GetComponent<XPSystem>() == null)
            systemsGO.AddComponent<XPSystem>();
            
        if (systemsGO.GetComponent<CurrencyManager>() == null)
            systemsGO.AddComponent<CurrencyManager>();
            
        if (systemsGO.GetComponent<UpgradeManager>() == null)
            systemsGO.AddComponent<UpgradeManager>();
            
        if (systemsGO.GetComponent<UIManager>() == null)
            systemsGO.AddComponent<UIManager>();
            
        if (systemsGO.GetComponent<SaveSystem>() == null)
            systemsGO.AddComponent<SaveSystem>();
            
        if (systemsGO.GetComponent<DontDestroyOnLoadMarker>() == null)
            systemsGO.AddComponent<DontDestroyOnLoadMarker>();

        // Canvas prefab instance
        GameObject canvasPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/GameCanvas.prefab");
        if (canvasPrefab != null && GameObject.Find("GameCanvas") == null)
        {
            GameObject canvasInstance = (GameObject)PrefabUtility.InstantiatePrefab(canvasPrefab);
            Canvas canvas = canvasInstance.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = mainCamera;
            }
        }

        // URP 2D Global Light
        if (GameObject.Find("Global Light 2D") == null)
        {
            GameObject lightGO = new GameObject("Global Light 2D");
            Light2D light2D = lightGO.AddComponent<Light2D>();
            light2D.lightType = Light2D.LightType.Global;
            light2D.intensity = 1.0f;
            light2D.color = Color.white;
        }
        
        // Set up URP asset for project if not already configured
        SetupURPAsset();
        
        Debug.Log("Game Scene setup completed with all core objects!");
    }
    
    private static void SetupURPAsset()
    {
        // Load and assign URP asset to Graphics Settings
        UniversalRenderPipelineAsset urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/UniversalRP-HighQuality.asset");
        if (urpAsset != null)
        {
            GraphicsSettings.renderPipelineAsset = urpAsset;
            QualitySettings.renderPipeline = urpAsset;
            Debug.Log("URP Asset assigned to project settings.");
        }
        else
        {
            Debug.LogWarning("URP Asset not found. Please ensure it exists at Assets/Settings/UniversalRP-HighQuality.asset");
        }
    }
}

