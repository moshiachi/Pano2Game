using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks; // Required for Task
using EmbodiedVision.AI; // Required for GeminiAnalyzer and SceneObjectVLMDetails
using Newtonsoft.Json; // Required for JsonConvert
using System.Threading;

public class SceneCaptureController : MonoBehaviour
{
    public Camera captureCamera;
    public Transform target3DGSSceneRoot; // Assign the root of your imported 3DGS scene
    public int imageWidth = 1920;
    public int imageHeight = 1080;
    public string outputDirectory = "SceneCaptures"; // Relative to Application.dataPath
    public float fieldOfView = 60f;

    public enum CaptureStrategy
    {
        GlobalOrbital,
        LocalOrbitalFromPoints
    }

    [Header("Capture Strategy")]
    [Tooltip("Choose how to position the capture cameras.")]
    public CaptureStrategy captureStrategy = CaptureStrategy.GlobalOrbital;

    [Header("Global Orbital Strategy Settings")]
    [Tooltip("The height of the camera relative to the scene's center.")]
    public float captureHeight = 1.5f;
    [Tooltip("The distance (radius) of the camera from the scene's center.")]
    public float captureRadius = 8.0f;
    [Tooltip("How many pictures to take in a full 360-degree circle. 12 = every 30 degrees.")]
    public int numberOfCapturePoints = 12;
    [Tooltip("Adjusts the vertical point the camera looks at. Positive looks up, negative looks down.")]
    public float lookAtCenterYOffset = 0.5f;

    [Header("Local Orbital Strategy Settings")]
    [Tooltip("Create empty GameObjects for scan centers. From each, a local orbit will be performed.")]
    public List<Transform> localScanCenters;
    [Tooltip("The radius of the small orbit around each manual point.")]
    public float localOrbitRadius = 2.5f;
    [Tooltip("How many pictures to take in the 360-degree orbit around each point.")]
    public int localNumberOfCapturePoints = 8;
    [Tooltip("The vertical angles (in degrees) to capture from. 0 is level, 30 is looking down, -20 is looking up.")]
    public List<float> localOrbitAngles = new List<float> { 20f };
    [Tooltip("A small height offset to prevent the camera from being exactly on the ground.")]
    public float verticalOffset = 0.2f;

    [Header("Gemini VLM Settings")]
    public string geminiApiKey = "YOUR_API_KEY_HERE"; // PASTE YOUR KEY IN INSPECTOR
    public string vlmOutputDir = "VLM_Analysis_Outputs"; // Subdirectory for VLM JSONs

    private int captureIndex = 0;
    private GeminiAnalyzer vlmAnalyzer;
    private CancellationTokenSource cancellationTokenSource;
    private bool isRunning = false;

    private struct CaptureRequest
    {
        public Vector3 CameraPosition;
        public Vector3 LookAtTarget;
    }

    void Start()
    {
        // Start() is primarily for runtime. The main logic is now initiated from the context menu.
        // We can ensure the FoV is set if the user enters play mode.
        if (captureCamera != null)
        {
            captureCamera.fieldOfView = fieldOfView;
        }
    }

    /// <summary>
    /// Checks that all required components are assigned and initializes the VLM analyzer.
    /// This is called before any capture operations to ensure readiness, especially for editor calls.
    /// </summary>
    /// <returns>True if ready, false otherwise.</returns>
    private bool EnsureDependenciesAreReady()
    {
        if (captureCamera == null)
        {
            Debug.LogError("[SceneCaptureController] The 'Capture Camera' has not been assigned in the Inspector. Aborting.");
            return false;
        }

        if (target3DGSSceneRoot == null)
        {
            Debug.LogError("[SceneCaptureController] The 'Target 3DGS Scene Root' has not been assigned in the Inspector. Aborting.");
            return false;
        }

        // Create output directories if they don't exist
        try
        {
            string fullOutputDir = Path.Combine(Application.dataPath, outputDirectory);
            Directory.CreateDirectory(fullOutputDir);
            Directory.CreateDirectory(Path.Combine(fullOutputDir, vlmOutputDir));
        }
        catch (Exception e)
        {
            Debug.LogError($"[SceneCaptureController] Failed to create output directories. Error: {e.Message}");
            return false;
        }

        // Initialize VLM Analyzer if it hasn't been already
        if (vlmAnalyzer == null)
        {
            if (string.IsNullOrEmpty(geminiApiKey) || geminiApiKey == "YOUR_API_KEY_HERE")
            {
                Debug.LogWarning("[SceneCaptureController] Gemini API Key is not set. VLM analysis will be skipped.");
            }
            else
            {
                vlmAnalyzer = new GeminiAnalyzer(geminiApiKey);
                Debug.Log("[SceneCaptureController] GeminiAnalyzer has been initialized.");
            }
        }
        return true;
    }

    [ContextMenu("START Capture & Analysis")]
    public void StartCapture()
    {
        if (isRunning)
        {
            Debug.LogWarning("[SceneCaptureController] A capture process is already running.");
            return;
        }
        _ = CaptureAndAnalyzeViewsAsync(); 
    }

    [ContextMenu("STOP Capture & Analysis")]
    public void StopCapture()
    {
        if (!isRunning)
        {
            Debug.LogWarning("[SceneCaptureController] No capture process is currently running.");
            return;
        }
        if (cancellationTokenSource != null)
        {
            Debug.LogWarning("[SceneCaptureController] STOP request received. Cancellation requested.");
            cancellationTokenSource.Cancel();
        }
    }

    private async Task CaptureAndAnalyzeViewsAsync()
    {
        isRunning = true;
        cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        if (!EnsureDependenciesAreReady())
        {
            Debug.LogError("[SceneCaptureController] Dependency check failed. Aborting capture process.");
            return;
        }

        if (target3DGSSceneRoot == null)
        {
            Debug.LogError("[SceneCaptureController] Target 3DGS Scene Root is null. Aborting capture.");
            return;
        }

        Bounds sceneBounds = CalculateSceneBounds();
        if (sceneBounds.size == Vector3.zero)
        {
            Debug.LogWarning("[SceneCaptureController] Could not determine scene bounds. Using target root as center.");
        }
        
        Vector3 sceneCenter = sceneBounds.center;
        Debug.Log($"[SceneCaptureController] Using scene center: {sceneCenter}. Capture Radius: {captureRadius}, Height: {captureHeight}");

        List<CaptureRequest> captureRequests = new List<CaptureRequest>();

        if (captureStrategy == CaptureStrategy.GlobalOrbital)
        {
            Debug.Log($"[SceneCaptureController] Using Global Orbital strategy. Center: {sceneCenter}, Radius: {captureRadius}, Height: {captureHeight}");
            float angleStep = 360.0f / numberOfCapturePoints;
            Vector3 lookAtTarget = sceneCenter + new Vector3(0, lookAtCenterYOffset, 0);

            for (int i = 0; i < numberOfCapturePoints; i++)
            {
                float currentAngle = i * angleStep;
                float x = sceneCenter.x + captureRadius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
                float z = sceneCenter.z + captureRadius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
                float y = sceneCenter.y + captureHeight;
                
                captureRequests.Add(new CaptureRequest { CameraPosition = new Vector3(x, y, z), LookAtTarget = lookAtTarget });
            }
        }
        else // LocalOrbitalFromPoints
        {
            if (localScanCenters == null || localScanCenters.Count == 0)
            {
                Debug.LogError("[SceneCaptureController] 'Local Orbital' strategy is selected, but the 'Local Scan Centers' list is empty. Please assign at least one Transform.");
                return;
            }

            Debug.Log($"[SceneCaptureController] Using Local Orbital strategy with {localScanCenters.Count} scan centers.");
            float angleStep = 360.0f / localNumberOfCapturePoints;

            foreach (var centerTransform in localScanCenters)
            {
                if (centerTransform == null)
                {
                    Debug.LogWarning("[SceneCaptureController] A null Transform was found in the 'Local Scan Centers' list and will be skipped.");
                    continue;
                }

                Vector3 scanCenter = centerTransform.position;
                Debug.Log($"-- Generating orbits around point: {centerTransform.name} at {scanCenter}");

                // Loop through each defined vertical angle
                foreach (float verticalAngle in localOrbitAngles)
                {
                    Debug.Log($"---- Capturing from vertical angle: {verticalAngle} degrees");
                    for (int i = 0; i < localNumberOfCapturePoints; i++)
                    {
                        float horizontalAngle = i * angleStep;
                        
                        // Calculate position on a horizontal circle first
                        float x = localOrbitRadius * Mathf.Cos(horizontalAngle * Mathf.Deg2Rad);
                        float z = localOrbitRadius * Mathf.Sin(horizontalAngle * Mathf.Deg2Rad);
                        
                        // Calculate height based on vertical angle and radius
                        float height = localOrbitRadius * Mathf.Sin(verticalAngle * Mathf.Deg2Rad);
                        
                        // Scale the horizontal position based on the vertical angle
                        float horizontalScale = Mathf.Cos(verticalAngle * Mathf.Deg2Rad);
                        x *= horizontalScale;
                        z *= horizontalScale;

                        // Add the scan center position and the vertical offset to the final camera position
                        Vector3 cameraPosition = scanCenter + new Vector3(x, height + verticalOffset, z);
                        
                        captureRequests.Add(new CaptureRequest { CameraPosition = cameraPosition, LookAtTarget = scanCenter });
                    }
                }
            }
        }

        Debug.Log($"[SceneCaptureController] Starting to loop through {captureRequests.Count} generated viewpoints...");
        captureIndex = 0; // Reset index for each run

        foreach (var request in captureRequests)
        {
            // Check for cancellation at the start of each loop iteration
            if (token.IsCancellationRequested)
            {
                Debug.LogWarning("[SceneCaptureController] Capture process was cancelled by the user.");
                break; // Exit the loop
            }

            try
            {
                Debug.Log($"[SceneCaptureController] Processing viewpoint at world position: {request.CameraPosition}");

                captureCamera.transform.position = request.CameraPosition;
                captureCamera.transform.LookAt(request.LookAtTarget);
                Debug.Log($"[SceneCaptureController] Camera positioned at {request.CameraPosition}, looking at {request.LookAtTarget}");

                string imageFileName = $"capture_{captureIndex++}.png";
                string imageFilePath = Path.Combine(Application.dataPath, outputDirectory, imageFileName);
                
                Debug.Log($"[SceneCaptureController] About to capture view for: {imageFileName}");
                Texture2D capturedTexture = CaptureSingleView(imageFilePath);
                Debug.Log($"[SceneCaptureController] Finished capturing view for: {imageFileName}. Texture is {(capturedTexture == null ? "null" : "not null")}");

                if (capturedTexture != null && vlmAnalyzer != null)
                {
                    Debug.Log($"[SceneCaptureController] Starting VLM analysis for {imageFileName}...");
                    VLMSceneAnalysis analysisResult = await vlmAnalyzer.AnalyzeImageAsync(capturedTexture, token, null, captureCamera.transform.position, captureCamera.transform.rotation);
                    ProcessVLMResult(analysisResult, imageFileName);
                    // Use DestroyImmediate for editor-time operations
                    DestroyImmediate(capturedTexture);
                }
                else if (vlmAnalyzer == null && !(string.IsNullOrEmpty(geminiApiKey) || geminiApiKey == "YOUR_API_KEY_HERE"))
                {
                    Debug.LogWarning("[SceneCaptureController] vlmAnalyzer is null, but API key seems set. Check initialization.");
                }
                else if (capturedTexture == null)
                {
                    Debug.LogWarning($"[SceneCaptureController] Captured texture was null for {imageFileName}. Skipping VLM.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SceneCaptureController] An exception occurred inside the capture loop! Stopping. Error: {e.Message}\nStackTrace: {e.StackTrace}");
                // We break here because one error often means subsequent ones will also fail.
                break; 
            }
        }
        Debug.Log($"[SceneCaptureController] Finished looping through viewpoints.");
        isRunning = false;
        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;
    }
    
    Bounds CalculateSceneBounds()
    {
        if (target3DGSSceneRoot == null) return new Bounds();

        Renderer[] renderers = target3DGSSceneRoot.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            // Fallback if no renderers (e.g. if 3DGS is a point cloud not using standard Renderers)
            // You might need a specific way to get bounds from your 3DGS representation.
            // For now, let's assume a default small area if no renderers found.
            // Or, use the target3DGSSceneRoot's direct colliders if any.
            Collider col = target3DGSSceneRoot.GetComponent<Collider>();
            if (col != null) return col.bounds;
            Debug.LogWarning("No renderers found in target3DGSSceneRoot hierarchy to calculate bounds. Provide a Collider or custom bounds logic.");
            return new Bounds(target3DGSSceneRoot.position, Vector3.one * 2); // Default small bounds
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return bounds;
    }

    public Texture2D CaptureSingleView(string filePath)
    {
        RenderTexture rt = new RenderTexture(imageWidth, imageHeight, 24);
        captureCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
        captureCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        screenShot.Apply(); // Apply the changes to the Texture2D
        
        // Save the image file
        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllText(filePath, ""); // Clear file first in case of error
        File.WriteAllBytes(filePath, bytes);
        Debug.Log($"Captured view to {filePath}");

        // Detach render texture
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt); 
        
        // Return the Texture2D so it can be used for VLM
        // The caller will be responsible for Destroying this texture if it's no longer needed
        // However, if we return it, we need a new instance because screenShot might be reused/destroyed
        // Let's create a new texture and copy to it to be safe for async operations
        Texture2D persistentTexture = new Texture2D(screenShot.width, screenShot.height, screenShot.format, false);
        persistentTexture.LoadRawTextureData(screenShot.GetRawTextureData()); // More efficient than Get/SetPixels
        persistentTexture.Apply();
        
        // This is the fix: explicitly destroy the temporary screenshot texture to prevent a memory leak.
        DestroyImmediate(screenShot);

        return persistentTexture; 
    }

    private void ProcessVLMResult(VLMSceneAnalysis analysisResult, string sourceImageName)
    {
        if (analysisResult == null || analysisResult.SceneObjects == null || analysisResult.SceneObjects.Count == 0) 
        {
            Debug.LogError($"[SceneCaptureController] VLM analysis result was null or contained no objects for {sourceImageName}.");
            return;
        }

        // Check if the analysis itself failed
        if (analysisResult.SceneObjects.Count == 1 && !string.IsNullOrEmpty(analysisResult.SceneObjects[0].ProcessingFailureReason))
        {
            Debug.LogError($"[SceneCaptureController] VLM Analysis Failed for {sourceImageName}: {analysisResult.SceneObjects[0].ProcessingFailureReason}");
        }
        else
        {
            Debug.Log($"[SceneCaptureController] VLM Analysis Success for {sourceImageName}! Found {analysisResult.SceneObjects.Count} objects.");
            // Serialize and save the result
            try
            {
                string jsonOutput = JsonConvert.SerializeObject(analysisResult, Formatting.Indented);
                string outputJsonName = Path.GetFileNameWithoutExtension(sourceImageName) + "_vlm_scene.json"; // new name
                string filePath = Path.Combine(Application.dataPath, outputDirectory, vlmOutputDir, outputJsonName);
                
                File.WriteAllText(filePath, jsonOutput);
                Debug.Log($"[SceneCaptureController] VLM scene analysis for {sourceImageName} saved to: {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SceneCaptureController] Failed to serialize or save VLM output for {sourceImageName}: {e.Message}");
            }
        }
    }

} 