using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Batch asset optimizer for mobile devices. Optimizes textures, materials, models, and audio.
/// Run from menu: Assets > Optimize All Assets for Mobile
/// </summary>
public class AssetOptimizer
{
    private const string TEXTURES_PATH = "Assets/Textures";
    private const string CUSTOM_TEXTURES_PATH = "Assets/CustomTextures";
    private const string MATERIALS_PATH = "Assets/Materials";
    private const string MODELS_PATH = "Assets/Models";
    private const string SOUNDS_PATH = "Assets/sounds";
    
    private static int optimizedTextureCount = 0;
    private static int optimizedMaterialCount = 0;
    private static int optimizedModelCount = 0;
    private static int optimizedAudioCount = 0;

    [MenuItem("Assets/Optimize All Assets for Mobile")]
    public static void OptimizeAllAssets()
    {
        Debug.Log("[AssetOptimizer] Starting batch optimization for mobile devices...");
        
        optimizedTextureCount = 0;
        optimizedMaterialCount = 0;
        optimizedModelCount = 0;
        optimizedAudioCount = 0;

        // Optimize textures
        OptimizeTexturesInFolder(TEXTURES_PATH);
        OptimizeTexturesInFolder(CUSTOM_TEXTURES_PATH);
        OptimizeTexturesInFolder(MATERIALS_PATH);

        // Optimize models
        OptimizeModelsInFolder(MODELS_PATH);

        // Optimize audio
        OptimizeAudioInFolder(SOUNDS_PATH);

        // Optimize materials (reduce emission/complexity)
        OptimizeMaterialsInFolder(MATERIALS_PATH);
        OptimizeMaterialsInFolder(CUSTOM_TEXTURES_PATH);

        Debug.Log($"[AssetOptimizer] Optimization complete!\n" +
                  $"- Textures optimized: {optimizedTextureCount}\n" +
                  $"- Models optimized: {optimizedModelCount}\n" +
                  $"- Audio files optimized: {optimizedAudioCount}\n" +
                  $"- Materials optimized: {optimizedMaterialCount}");

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Asset Optimization Complete", 
            $"Optimized {optimizedTextureCount} textures, {optimizedModelCount} models, " +
            $"{optimizedAudioCount} audio files, and {optimizedMaterialCount} materials.", "OK");
    }

    private static void OptimizeTexturesInFolder(string folderPath)
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
            return;

        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
        
        foreach (string guid in textureGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            OptimizeTexture(assetPath);
        }
    }

    private static void OptimizeTexture(string assetPath)
    {
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (textureImporter == null)
            return;

        bool changed = false;

        // Detect texture type by name
        string fileName = Path.GetFileNameWithoutExtension(assetPath).ToLower();
        bool isNormalMap = fileName.Contains("normal") || fileName.Contains("nml");
        bool isEmissive = fileName.Contains("emit") || fileName.Contains("glow");

        // Configure Android settings
        TextureImporterPlatformSettings androidSettings = textureImporter.GetPlatformTextureSettings("Android");
        androidSettings.overridden = true;
        androidSettings.maxTextureSize = 1024; // Reduce resolution
        androidSettings.format = TextureImporterFormat.ETC2;
        androidSettings.compressionQuality = 50;

        if (isNormalMap)
            androidSettings.format = TextureImporterFormat.ETC2;

        textureImporter.SetPlatformTextureSettings(androidSettings);

        // Configure iOS settings (PVRTC)
        TextureImporterPlatformSettings iosSettings = textureImporter.GetPlatformTextureSettings("iPhone");
        iosSettings.overridden = true;
        iosSettings.maxTextureSize = 1024;
        iosSettings.format = TextureImporterFormat.PVRTC_RGB4;
        iosSettings.compressionQuality = 50;

        if (isNormalMap)
            iosSettings.format = TextureImporterFormat.PVRTC_RGBA4;

        textureImporter.SetPlatformTextureSettings(iosSettings);

        // General settings
        textureImporter.mipmapEnabled = true;
        textureImporter.mipMapBias = 0.5f; // Bias towards lower mips for performance
        textureImporter.anisoLevel = 2; // Reduce aniso filtering
        
        if (!isNormalMap && !isEmissive)
        {
            textureImporter.sRGBTexture = true;
        }

        EditorUtility.SetDirty(textureImporter);
        textureImporter.SaveAndReimport();
        optimizedTextureCount++;
    }

    private static void OptimizeModelsInFolder(string folderPath)
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
            return;

        string[] modelGuids = AssetDatabase.FindAssets("t:Model", new[] { folderPath });
        
        foreach (string guid in modelGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            OptimizeModel(assetPath);
        }
    }

    private static void OptimizeModel(string assetPath)
    {
        ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        if (modelImporter == null)
            return;

        bool changed = false;

        // Enable mesh compression
        if (!modelImporter.meshCompression.HasFlag(ModelImporterMeshCompression.Off))
            changed = true;
        
        modelImporter.meshCompression = ModelImporterMeshCompression.High;

        // Reduce blend shapes for mobile
        modelImporter.importBlendShapes = false;

        // Optimize bones/rigging
        if (modelImporter.optimizeGameObjects)
            changed = true;
        modelImporter.optimizeGameObjects = true;

        if (changed)
        {
            EditorUtility.SetDirty(modelImporter);
            modelImporter.SaveAndReimport();
            optimizedModelCount++;
        }
    }

    private static void OptimizeAudioInFolder(string folderPath)
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
            return;

        string[] audioGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { folderPath });
        
        foreach (string guid in audioGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            OptimizeAudio(assetPath);
        }
    }

    private static void OptimizeAudio(string assetPath)
    {
        AudioImporter audioImporter = AssetImporter.GetAtPath(assetPath) as AudioImporter;
        if (audioImporter == null)
            return;

        bool changed = false;

        // Configure Android settings
        AudioImporterPlatformSettings androidSettings = audioImporter.GetPlatformSettings("Android");
        androidSettings["loadType"] = (int)AudioClipLoadType.CompressedInMemory;
        androidSettings["sampleRateSetting"] = (int)AudioSampleRateSetting.OptimizeSampleRate;
        androidSettings["sampleRate"] = 24000; // Reduce sample rate for non-critical audio
        audioImporter.SetPlatformSettings("Android", androidSettings);
        changed = true;

        // Configure iOS settings
        AudioImporterPlatformSettings iosSettings = audioImporter.GetPlatformSettings("iPhone");
        iosSettings["loadType"] = (int)AudioClipLoadType.CompressedInMemory;
        iosSettings["sampleRateSetting"] = (int)AudioSampleRateSetting.OptimizeSampleRate;
        iosSettings["sampleRate"] = 24000;
        audioImporter.SetPlatformSettings("iPhone", iosSettings);

        // General settings - compress with Vorbis
        if (audioImporter.defaultSampleSettings.loadType != AudioClipLoadType.CompressedInMemory)
            changed = true;

        AudioImporterSampleSettings sampleSettings = audioImporter.defaultSampleSettings;
        sampleSettings.loadType = AudioClipLoadType.CompressedInMemory;
        sampleSettings.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
        sampleSettings.sampleRate = 24000;
        audioImporter.defaultSampleSettings = sampleSettings;

        if (changed)
        {
            EditorUtility.SetDirty(audioImporter);
            audioImporter.SaveAndReimport();
            optimizedAudioCount++;
        }
    }

    private static void OptimizeMaterialsInFolder(string folderPath)
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
            return;

        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });
        
        foreach (string guid in materialGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            
            if (material != null)
            {
                OptimizeMaterial(material);
                optimizedMaterialCount++;
            }
        }
    }

    private static void OptimizeMaterial(Material material)
    {
        // Reduce emission for glow effects (preserve visibility, reduce overdraw)
        if (material.HasProperty("_EmissionColor"))
        {
            Color emission = material.GetColor("_EmissionColor");
            emission *= 0.7f; // Reduce emission intensity by 30%
            material.SetColor("_EmissionColor", emission);
        }

        // Disable expensive features for secondary objects
        if (material.HasProperty("_Metallic"))
        {
            material.SetFloat("_Metallic", 0.5f); // Reduce metallic intensity
        }

        if (material.HasProperty("_Glossiness"))
        {
            material.SetFloat("_Glossiness", 0.5f); // Reduce shine
        }

        EditorUtility.SetDirty(material);
    }

    /// <summary>
    /// Analyze current asset usage and suggest further optimizations
    /// </summary>
    [MenuItem("Assets/Analyze Asset Performance")]
    public static void AnalyzeAssets()
    {
        long textureMemory = CalculateTextureMemoryUsage();
        long audioMemory = CalculateAudioMemoryUsage();
        
        string report = $"Asset Performance Analysis:\n\n" +
                       $"Texture Memory: {textureMemory / (1024 * 1024)} MB\n" +
                       $"Audio Memory: {audioMemory / (1024 * 1024)} MB\n\n" +
                       $"Recommendation: If total > 500MB, further reduce texture resolution or use streaming.";
        
        EditorUtility.DisplayDialog("Asset Analysis", report, "OK");
        Debug.Log(report);
    }

    private static long CalculateTextureMemoryUsage()
    {
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D");
        long totalMemory = 0;
        
        foreach (string guid in textureGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture != null)
            {
                totalMemory += texture.width * texture.height * 4; // Rough estimate
            }
        }
        
        return totalMemory;
    }

    private static long CalculateAudioMemoryUsage()
    {
        string[] audioGuids = AssetDatabase.FindAssets("t:AudioClip");
        long totalMemory = 0;
        
        foreach (string guid in audioGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
            if (clip != null)
            {
                totalMemory += clip.samples * clip.channels * 2; // 16-bit per sample rough estimate
            }
        }
        
        return totalMemory;
    }
}
