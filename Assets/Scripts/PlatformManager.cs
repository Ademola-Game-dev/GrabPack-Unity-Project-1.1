using UnityEngine;

/// <summary>
/// Global platform detection manager to cache platform checks and avoid redundant checks throughout the codebase.
/// </summary>
public class PlatformManager : MonoBehaviour
{
    private static PlatformManager instance;
    
    /// <summary>
    /// Cached mobile platform flag - checked once at startup
    /// </summary>
    public static bool IsMobile { get; private set; }
    
    void Awake()
    {
        // Ensure only one instance exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Cache the platform check once at startup
        IsMobile = Application.isMobilePlatform;
        
        // Auto-select quality settings based on device capabilities
        AutoSelectQualitySettings();
    }
    
    /// <summary>
    /// Automatically select quality settings based on device RAM and GPU capabilities.
    /// This helps older devices run the game smoothly.
    /// </summary>
    private void AutoSelectQualitySettings()
    {
        if (!IsMobile)
            return;
        
        // For mobile devices, use Lower quality tiers based on device specs
        // Android and iOS default to Medium (2) which is already configured
        // This can be enhanced with more granular checks using SystemInfo
        
        // Check available system RAM to select appropriate quality
        int systemMemoryMB = SystemInfo.systemMemorySize;
        
        if (systemMemoryMB < 2048) // Less than 2GB RAM
        {
            // Very Low quality (0) for very old phones
            QualitySettings.SetQualityLevel(0, true);
        }
        else if (systemMemoryMB < 4096) // 2-4GB RAM
        {
            // Low quality (1)
            QualitySettings.SetQualityLevel(1, true);
        }
        // Otherwise keep the platform default (Medium for mobile)
    }
}
