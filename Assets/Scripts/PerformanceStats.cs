using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime performance stats display for mobile optimization debugging.
/// Shows FPS, frame time, memory, and LOD bias in real-time.
/// </summary>
public class PerformanceStats : MonoBehaviour
{
    public bool enableStatsDisplay = true;
    public bool enableDetailedStats = false;
    
    private Text statsText;
    private float updateInterval = 0.5f; // Update every 0.5 seconds
    private float timeSinceLastUpdate = 0f;
    
    private float fps;
    private int frameCount = 0;
    private float frameDeltaTime = 0f;

    void Awake()
    {
        // Create UI if not already present
        if (statsText == null && enableStatsDisplay)
        {
            CreateStatsUI();
        }
    }

    void Update()
    {
        if (!enableStatsDisplay)
            return;

        frameCount++;
        frameDeltaTime += Time.deltaTime;

        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            fps = frameCount / frameDeltaTime;
            UpdateStatsDisplay();
            
            frameCount = 0;
            frameDeltaTime = 0f;
            timeSinceLastUpdate = 0f;
        }

        // Toggle stats with ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            enableDetailedStats = !enableDetailedStats;
        }
    }

    private void CreateStatsUI()
    {
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("StatsCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        }

        // Create Text UI
        GameObject textGO = new GameObject("PerformanceStats");
        textGO.transform.SetParent(canvas.transform, false);

        statsText = textGO.AddComponent<Text>();
        statsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        statsText.fontSize = 20;
        statsText.fontStyle = FontStyle.Normal;
        statsText.alignment = TextAnchor.UpperLeft;
        statsText.text = "Stats";

        // Position
        RectTransform rectTransform = textGO.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(10, -10);
        rectTransform.sizeDelta = new Vector2(300, 300);

        // Shadow for readability
        Shadow shadow = textGO.AddComponent<Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(1, -1);
    }

    private void UpdateStatsDisplay()
    {
        if (statsText == null)
            return;

        string stats = "";
        
        // Basic stats
        stats += $"<color=cyan>FPS: {fps:F1}\n</color>";
        stats += $"<color=yellow>Frame Time: {1000f / fps:F1}ms\n</color>";
        
        // LOD stats
        if (MobileLODManager.lodBias > 0)
        {
            stats += $"<color=lime>LOD Bias: {MobileLODManager.lodBias:F2}\n</color>";
            stats += $"Avg Frame: {MobileLODManager.GetAverageFrameTime():F1}ms\n";
        }

        // Device info
        stats += $"\n<color=magenta>Device:\n</color>";
        stats += $"RAM: {SystemInfo.systemMemorySize}MB\n";
        stats += $"GPU: {SystemInfo.graphicsDeviceName}\n";
        stats += $"Quality: {QualitySettings.names[QualitySettings.GetQualityLevel()]}\n";

        // Detailed stats
        if (enableDetailedStats)
        {
            stats += $"\n<color=white>Detailed:\n</color>";
            stats += $"Draw Calls: {UnityStats.drawCalls}\n";
            stats += $"Triangles: {UnityStats.triangles}\n";
            stats += $"Vertices: {UnityStats.vertices}\n";
            stats += $"Batches: {UnityStats.batches}\n";
            stats += $"Timer SinceStartup: {Time.realtimeSinceStartup:F1}s\n";
            stats += $"\n<color=gray>Press ESC to toggle\n</color>";
        }
        else
        {
            stats += $"\n<color=gray>(Press ESC for details)\n</color>";
        }

        statsText.text = stats;
    }

    /// <summary>
    /// Manually enable/disable stats display
    /// </summary>
    public void SetStatsVisible(bool visible)
    {
        enableStatsDisplay = visible;
        if (statsText != null)
        {
            statsText.enabled = visible;
        }
    }

    /// <summary>
    /// Log current performance snapshot to console
    /// </summary>
    public void LogPerformanceSnapshot()
    {
        Debug.Log($"[Performance Snapshot]\n" +
                  $"FPS: {fps:F1}\n" +
                  $"Frame Time: {1000f / fps:F1}ms\n" +
                  $"LOD Bias: {MobileLODManager.lodBias:F2}\n" +
                  $"Device: {SystemInfo.graphicsDeviceName} ({SystemInfo.systemMemorySize}MB)\n" +
                  $"Quality: {QualitySettings.names[QualitySettings.GetQualityLevel()]}\n" +
                  $"Draw Calls: {UnityStats.drawCalls}\n" +
                  $"Triangles: {UnityStats.triangles}\n" +
                  $"Batches: {UnityStats.batches}");
    }
}
