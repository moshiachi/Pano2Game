using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;

// Namespace to group these AI-related classes
namespace EmbodiedVision.AI
{
    [Serializable]
    public class EstimatedTransformHints
    {
        [JsonProperty("size_category")]
        public string SizeCategory;

        [JsonProperty("typical_placement_description")]
        public string TypicalPlacementDescription;
    }

    [Serializable]
    public class EstimatedColliderInfo
    {
        [JsonProperty("suggested_shape")]
        public string SuggestedShape;

        [JsonProperty("justification")]
        public string Justification;
    }

    [Serializable]
    public class EstimatedRigidbodyInfo
    {
        [JsonProperty("estimated_mass_kg")]
        public float EstimatedMassKg;

        [JsonProperty("is_likely_dynamic")]
        public bool IsLikelyDynamic;
    }

    [Serializable]
    public class SceneObjectVLMDetails
    {
        [JsonProperty("identified_object_name")]
        public string IdentifiedObjectName;

        [JsonProperty("estimated_position")]
        public EstimatedPosition EstimatedPosition;

        [JsonProperty("alternative_names")]
        public string[] AlternativeNames;

        [JsonProperty("suggested_materials")]
        public string[] SuggestedMaterials;

        [JsonProperty("general_description")]
        public string GeneralDescription;

        [JsonProperty("transform_hints")]
        public EstimatedTransformHints TransformHints;

        [JsonProperty("collider_suggestion")]
        public EstimatedColliderInfo ColliderSuggestion;

        [JsonProperty("rigidbody_suggestion")]
        public EstimatedRigidbodyInfo RigidbodySuggestion;

        [JsonProperty("overall_confidence")]
        public float OverallConfidence;

        [JsonProperty("processing_failure_reason")]
        public string ProcessingFailureReason; // Populated by our code if Gemini fails
    }

    // New class for estimated position
    [Serializable]
    public class EstimatedPosition
    {
        [JsonProperty("x")]
        public float X;
        [JsonProperty("y")]
        public float Y;
        [JsonProperty("z")]
        public float Z;
    }

    // New wrapper class to hold a list of scene objects from a single analysis
    [Serializable]
    public class VLMSceneAnalysis
    {
        [JsonProperty("scene_objects")]
        public List<SceneObjectVLMDetails> SceneObjects;

        // Add a constructor to initialize the list
        public VLMSceneAnalysis()
        {
            SceneObjects = new List<SceneObjectVLMDetails>();
        }
    }

    // Helper classes for parsing the outer Gemini response structure
    [Serializable]
    public class GeminiPart
    {
        public string text;
    }

    [Serializable]
    public class GeminiContent
    {
        public GeminiPart[] parts;
        public string role;
    }

    [Serializable]
    public class GeminiCandidate
    {
        public GeminiContent content;
        public string finishReason;
        public int index;
    }

    [Serializable]
    public class GeminiRootResponse
    {
        public GeminiCandidate[] candidates;
        public object usageMetadata;
    }


    public class GeminiAnalyzer
    {
        private const string DefaultModelId = "gemini-2.5-flash-preview-05-20";
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/";

        private string apiKey;
        private string modelId;
        public bool IsBusy { get; private set; } = false;

        public GeminiAnalyzer(string apiKey, string modelId = DefaultModelId)
        {
            if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_API_KEY_HERE")
            {
                throw new ArgumentException("API key must be provided and valid.", nameof(apiKey));
            }
            this.apiKey = apiKey;
            this.modelId = string.IsNullOrEmpty(modelId) ? DefaultModelId : modelId;
        }

        private JObject CreateResponseSchema()
        {
            // The main schema for a single object remains the same.
            JObject singleObjectSchema = JObject.Parse(@"
            {
                ""type"": ""object"",
                ""properties"": {
                    ""identified_object_name"": { ""type"": ""string"", ""description"": ""Common name of the object (e.g., 'park bench', 'oak tree')."" },
                    ""estimated_position"": {
                        ""type"": ""object"",
                        ""description"": ""A rough estimate of the object's 3D position in the scene relative to a potential origin (0,0,0) based on the camera's view. This is for de-duplication, not final placement."",
                        ""properties"": {
                            ""x"": { ""type"": ""number"" },
                            ""y"": { ""type"": ""number"" },
                            ""z"": { ""type"": ""number"" }
                        },
                        ""required"": [""x"", ""y"", ""z""]
                    },
                    ""alternative_names"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } },
                    ""suggested_materials"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } },
                    ""general_description"": { ""type"": ""string"", ""description"": ""A brief description of the object's appearance and state. IMPORTANT: Include its relationship to other objects (e.g., 'A bench located on the grass, facing a flowerbed.')."" },
                    ""transform_hints"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""size_category"": { ""type"": ""string"", ""description"": ""Estimated size category relative to a person (e.g., small, medium, large, very large)."" },
                            ""typical_placement_description"": { ""type"": ""string"", ""description"": ""How this object is typically placed (e.g., 'on the ground', 'growing from the ground', 'on a path')."" }
                        }, ""required"": [""size_category"", ""typical_placement_description""]
                    },
                    ""collider_suggestion"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""suggested_shape"": { ""type"": ""string"", ""enum"": [""box"", ""sphere"", ""capsule"", ""mesh"", ""none""] },
                            ""justification"": { ""type"": ""string"", ""description"": ""Reasoning for the suggested collider shape."" }
                        }, ""required"": [""suggested_shape"", ""justification""]
                    },
                    ""rigidbody_suggestion"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""estimated_mass_kg"": { ""type"": ""number"" },
                            ""is_likely_dynamic"": { ""type"": ""boolean"", ""description"": ""True if the object is likely movable."" }
                        }, ""required"": [""estimated_mass_kg"", ""is_likely_dynamic""]
                    },
                    ""overall_confidence"": { ""type"": ""number"", ""format"": ""float"", ""description"": ""Confidence in this specific object's analysis (0.0-1.0)."" }
                },
                ""required"": [
                    ""identified_object_name"", 
                    ""estimated_position"",
                    ""general_description"", 
                    ""transform_hints"",
                    ""collider_suggestion"",
                    ""rigidbody_suggestion"",
                    ""overall_confidence""
                ]
            }");

            // We now wrap this single object schema inside a parent schema that expects a list.
            var rootSchema = new JObject
            {
                ["type"] = "object",
                ["properties"] = new JObject
                {
                    ["scene_objects"] = new JObject
                    {
                        ["type"] = "array",
                        ["description"] = "An array of all key objects identified in the scene.",
                        ["items"] = singleObjectSchema
                    }
                },
                ["required"] = new JArray { "scene_objects" }
            };

            return rootSchema;
        }

        public async Task<VLMSceneAnalysis> AnalyzeImageAsync(Texture2D imageTexture, CancellationToken cancellationToken, string customPrompt = null, Vector3? cameraPosition = null, Quaternion? cameraRotation = null)
        {
            if (imageTexture == null)
            {
                Debug.LogError("[GeminiAnalyzer] Image texture cannot be null.");
                return new VLMSceneAnalysis { SceneObjects = new List<SceneObjectVLMDetails> { new SceneObjectVLMDetails { ProcessingFailureReason = "Input image texture was null." } } };
            }

            string base64Png = Convert.ToBase64String(imageTexture.EncodeToPNG());
            return await SendToGeminiAsync(base64Png, cancellationToken, customPrompt, cameraPosition, cameraRotation);
        }

        private async Task<VLMSceneAnalysis> SendToGeminiAsync(string base64Png, CancellationToken cancellationToken, string customPrompt = null, Vector3? cameraPosition = null, Quaternion? cameraRotation = null)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.LogWarning("[GeminiAnalyzer] Analysis was cancelled before sending the request.");
                return new VLMSceneAnalysis { SceneObjects = new List<SceneObjectVLMDetails> { new SceneObjectVLMDetails { ProcessingFailureReason = "Operation was cancelled." } } };
            }

            IsBusy = true;
            
            // --- Camera Context Injection ---
            string cameraContext = "The camera's position and orientation were not provided.";
            if (cameraPosition.HasValue && cameraRotation.HasValue)
            {
                Vector3 pos = cameraPosition.Value;
                Vector3 fwd = cameraRotation.Value * Vector3.forward;
                cameraContext = $"The image was captured from camera position ({pos.x:F2}, {pos.y:F2}, {pos.z:F2}) with the camera looking in the direction ({fwd.x:F2}, {fwd.y:F2}, {fwd.z:F2}).";
            }

            string promptText = customPrompt ?? 
                $@"You are an expert 3D Scene Analyst and Environmental Storyteller. Your goal is to generate a structured JSON description of a scene from an image, which will be used to reconstruct it in a game engine. This image may be one of several views of the same scene.

**CAMERA CONTEXT:**
{cameraContext}

**PRINCIPLES FOR YOUR ANALYSIS (Follow these strictly):**
1.  **Identify Key Objects & Landmarks:** List the most prominent features.
2.  **Estimate Relative Position:** For each object, provide a rough estimate of its (x, y, z) position. Imagine the most central object is at (0,0,0) and estimate others relative to it. This is a crucial hint for de-duplication.
3.  **Describe Spatial Relationships:** For each object's 'general_description', you MUST describe its position relative to other key objects using the camera context and your estimated positions.
4.  **Use Relative Scale:** In the description, compare the size of objects (e.g., 'A bench that is about half the height of the nearby bush.').
5.  **Incorporate Environmental Storytelling:** Note the state or implied history of objects (e.g., 'well-maintained', 'neglected', 'worn').
6.  **Define Functional Zones:** If objects form a logical group, mention it (e.g., 'A seating area').
7.  **Describe Gaps and Suggest Fillers:** Identify empty areas and suggest how to fill them.

Your final output must be a JSON object containing a 'scene_objects' array, where each element is a detailed analysis of one key object, following the provided schema.";

            JObject schema = CreateResponseSchema();

            var requestBody = new
            {
                contents = new[] {
                    new {
                        role  = "USER",
                        parts = new object[] {
                            new { inlineData = new { mimeType = "image/png", data = base64Png } },
                            new { text = promptText }
                        }
                    }
                },
                generationConfig = new { 
                    responseMimeType = "application/json",
                    responseSchema   = schema 
                }
            };

            string requestJson = JsonConvert.SerializeObject(requestBody, Formatting.Indented);

            string url = $"{BaseUrl}{this.modelId}:generateContent?key={this.apiKey}";
            using var webRequest = new UnityWebRequest(url, "POST")
            {
                uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestJson)),
                downloadHandler = new DownloadHandlerBuffer()
            };
            webRequest.SetRequestHeader("Content-Type", "application/json");

            try
            {
                await webRequest.SendWebRequest();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GeminiAnalyzer] Exception during web request: {ex.Message}");
                IsBusy = false;
                return new VLMSceneAnalysis { SceneObjects = new List<SceneObjectVLMDetails> { new SceneObjectVLMDetails { ProcessingFailureReason = $"Web request exception: {ex.Message}" } } };
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string rawResponse = webRequest.downloadHandler.text;
                try
                {
                    var rootResponse = JsonConvert.DeserializeObject<GeminiRootResponse>(rawResponse);
                    string innerJson = rootResponse?.candidates?[0]?.content?.parts?[0]?.text;
                    if (string.IsNullOrWhiteSpace(innerJson))
                    {
                        Debug.LogError("[GeminiAnalyzer] Invalid or empty response from Gemini: " + rawResponse);
                        IsBusy = false;
                        return new VLMSceneAnalysis { SceneObjects = new List<SceneObjectVLMDetails> { new SceneObjectVLMDetails { ProcessingFailureReason = "Invalid or empty inner JSON from Gemini." } } };
                    }

                    VLMSceneAnalysis analysis = JsonConvert.DeserializeObject<VLMSceneAnalysis>(innerJson);
                    if (analysis == null || analysis.SceneObjects == null)
                    {
                        Debug.LogError("[GeminiAnalyzer] Failed to deserialize inner JSON to VLMSceneAnalysis or SceneObjects list is null. Inner JSON: " + innerJson);
                        IsBusy = false;
                        return new VLMSceneAnalysis { SceneObjects = new List<SceneObjectVLMDetails> { new SceneObjectVLMDetails { ProcessingFailureReason = "Failed to deserialize inner JSON." } } };
                    }
                    IsBusy = false;
                    return analysis;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[GeminiAnalyzer] JSON Deserialization failed: {ex.Message}\nRaw response: {rawResponse}");
                    IsBusy = false;
                    return new VLMSceneAnalysis { SceneObjects = new List<SceneObjectVLMDetails> { new SceneObjectVLMDetails { ProcessingFailureReason = $"JSON Deserialization error: {ex.Message}" } } };
                }
            }
            else
            {
                string errorBody = webRequest.downloadHandler?.text ?? "N/A";
                Debug.LogError($"[GeminiAnalyzer] Gemini API Error (HTTP {webRequest.responseCode}): {webRequest.error}\nBody: {errorBody}");
                IsBusy = false;
                return new VLMSceneAnalysis { SceneObjects = new List<SceneObjectVLMDetails> { new SceneObjectVLMDetails { ProcessingFailureReason = $"Gemini API Error (HTTP {webRequest.responseCode}): {webRequest.error}" } } };
            }
        }
    }
} 