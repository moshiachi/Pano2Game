using UnityEngine;
using UnityEditor;
using System.IO;

public class SceneBuilderTool : EditorWindow
{
    private string pythonGeneratedScriptPath = "C:/Users/USER/Documents/Lab/113-2/Embodied Vision/final/AIPipeline/GeneratedUnityScript.cs.txt";
    private string targetUnityScriptPath = "Assets/Scripts/AI/LLMGeneratedSceneBuilder.cs";

    [MenuItem("AI Tools/Update and Build Scene from AI")]
    public static void ShowWindow()
    {
        GetWindow<SceneBuilderTool>("AI Scene Builder");
    }

    void OnGUI()
    {
        GUILayout.Label("AI Scene Construction", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This tool will overwrite the LLMGeneratedSceneBuilder.cs script with the latest output from the Python pipeline and then construct the scene.", MessageType.Info);

        EditorGUILayout.Space();

        pythonGeneratedScriptPath = EditorGUILayout.TextField("Python Output Path", pythonGeneratedScriptPath);
        targetUnityScriptPath = EditorGUILayout.TextField("Target Unity Script Path", targetUnityScriptPath);

        EditorGUILayout.Space();
        
        if (GUILayout.Button("1. Overwrite LLM Builder Script"))
        {
            OverwriteBuilderScript();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("2. Construct Scene via Updated Script"))
        {
            // This invokes the menu item command from the script we just overwrote.
            EditorApplication.ExecuteMenuItem("AI Tools/Construct Scene from LLM-Generated Code");
        }
    }

    private void OverwriteBuilderScript()
    {
        string projectRoot = Application.dataPath.Replace("/Assets", "");
        string fullPythonScriptPath = Path.Combine(projectRoot, pythonGeneratedScriptPath);
        string fullTargetUnityScriptPath = Path.Combine(projectRoot, targetUnityScriptPath);

        if (!File.Exists(fullPythonScriptPath))
        {
            Debug.LogError($"[AI Scene Builder] Python-generated script not found at: {fullPythonScriptPath}");
            this.ShowNotification(new GUIContent("Python script not found!"));
            return;
        }

        try
        {
            string newScriptContent = File.ReadAllText(fullPythonScriptPath);

            if (string.IsNullOrEmpty(newScriptContent))
            {
                Debug.LogError($"[AI Scene Builder] The generated script at {fullPythonScriptPath} is empty. Aborting.");
                this.ShowNotification(new GUIContent("Generated script is empty."));
                return;
            }

            // Simple, robust overwrite. No more complex parsing.
            File.WriteAllText(fullTargetUnityScriptPath, newScriptContent);
            
            AssetDatabase.Refresh();
            Debug.Log($"[AI Scene Builder] Successfully overwrote '{targetUnityScriptPath}' and triggered recompile. Ready to construct scene.");
            this.ShowNotification(new GUIContent("Builder Script Updated!"));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AI Scene Builder] Error updating script: {e.Message}");
            this.ShowNotification(new GUIContent("Error updating script."));
        }
    }
} 