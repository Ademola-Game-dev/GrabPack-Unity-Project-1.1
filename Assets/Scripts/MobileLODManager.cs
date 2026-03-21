using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Mobile-optimized LOD manager for complex meshes and animations.
/// Automatically reduces quality based on device capabilities and frame rate.
/// </summary>
public class MobileLODManager : MonoBehaviour
{
    /// <summary>
    /// LOD bias - higher means more aggressive LOD reduction (faster at cost of quality)
    /// </summary>
    public static float lodBias = 1.0f;

    /// <summary>
    /// Target FPS - if frame time exceeds this, increase LOD aggressiveness
    /// </summary>
    private static float targetFrameTime = 33.3f; // ~30 FPS

    private static float frameTimeMovingAverage = 33.3f;
    private static int frameCounter = 0;
    private const int FRAME_AVERAGE_WINDOW = 30;

    private static List<LODGroup> managedLODGroups = new List<LODGroup>();
    private static bool isInitialized = false;

    void Awake()
    {
        if (!isInitialized)
        {
            InitializeLODManager();
            isInitialized = true;
        }
    }

    private static void InitializeLODManager()
    {
        // Set default LOD bias based on device RAM
        long systemMemory = SystemInfo.systemMemorySize;
        
        if (systemMemory < 2048) // < 2GB
        {
            lodBias = 1.5f; // Aggressive LOD reduction
            targetFrameTime = 40f; // Target 25 FPS for low-end devices
        }
        else if (systemMemory < 3072) // < 3GB
        {
            lodBias = 1.2f; // Moderate LOD reduction
            targetFrameTime = 33.3f; // Target 30 FPS
        }
        else
        {
            lodBias = 1.0f; // Normal LOD
            targetFrameTime = 16.6f; // Target 60 FPS
        }

        Debug.Log($"[MobileLODManager] Initialized with lodBias={lodBias}, targetFrameTime={targetFrameTime}ms (SystemMemory: {systemMemory}MB)");
    }

    void Update()
    {
        UpdateLODBias();
    }

    private void UpdateLODBias()
    {
        // Calculate frame time moving average
        float currentFrameTime = Time.deltaTime * 1000f; // Convert to ms
        frameTimeMovingAverage = Mathf.Lerp(frameTimeMovingAverage, currentFrameTime, 1f / FRAME_AVERAGE_WINDOW);
        frameCounter++;

        // Adjust LOD bias if frame time is too high
        if (frameCounter % 30 == 0) // Check every 30 frames
        {
            if (frameTimeMovingAverage > targetFrameTime * 1.1f) // 10% buffer
            {
                lodBias = Mathf.Min(lodBias + 0.05f, 2.0f); // Increase LOD reduction
                Debug.Log($"[MobileLODManager] Frame time too high ({frameTimeMovingAverage:F1}ms), increasing LOD bias to {lodBias:F2}");
            }
            else if (frameTimeMovingAverage < targetFrameTime * 0.8f && lodBias > 0.8f)
            {
                lodBias = Mathf.Max(lodBias - 0.05f, 0.8f); // Ease off LOD reduction
                Debug.Log($"[MobileLODManager] Frame time good ({frameTimeMovingAverage:F1}ms), decreasing LOD bias to {lodBias:F2}");
            }
        }

        // Apply LOD bias to QualitySettings
        QualitySettings.lodBias = lodBias;
    }

    /// <summary>
    /// Register a LOD group for optimization (called automatically on Start)
    /// </summary>
    public static void RegisterLODGroup(LODGroup lodGroup)
    {
        if (!managedLODGroups.Contains(lodGroup))
        {
            managedLODGroups.Add(lodGroup);
        }
    }

    /// <summary>
    /// Unregister a LOD group
    /// </summary>
    public static void UnregisterLODGroup(LODGroup lodGroup)
    {
        managedLODGroups.Remove(lodGroup);
    }

    /// <summary>
    /// Get current frame time average in milliseconds
    /// </summary>
    public static float GetAverageFrameTime()
    {
        return frameTimeMovingAverage;
    }

    /// <summary>
    /// Get current FPS based on moving average
    /// </summary>
    public static float GetAverageFPS()
    {
        return 1000f / frameTimeMovingAverage;
    }
}

/// <summary>
/// Auto-register LOD groups for optimization
/// </summary>
public class LODGroupOptimizer : MonoBehaviour
{
    private LODGroup lodGroup;

    void Start()
    {
        lodGroup = GetComponent<LODGroup>();
        if (lodGroup != null)
        {
            MobileLODManager.RegisterLODGroup(lodGroup);
            
            // Ensure LOD setup is valid
            if (lodGroup.GetLODs().Length == 0)
            {
                Debug.LogWarning($"[LODGroupOptimizer] {gameObject.name} has no LOD levels defined!", gameObject);
            }
        }
    }

    void OnDestroy()
    {
        if (lodGroup != null)
        {
            MobileLODManager.UnregisterLODGroup(lodGroup);
        }
    }
}
