# Pano2Game: 360° Video to Interactive 3D Scene Converter

##### ref: Collaborate with Gemini 2.5 pro

A Unity-based pipeline that converts 360° panoramic videos into interactive 3D scenes using Gaussian Splatting and AI-driven scene understanding. This project extends the [UnityGaussianSplatting](https://github.com/aras-p/UnityGaussianSplatting) repository with AI-powered scene analysis and reconstruction capabilities.

---

## Comprehensive Workflow & Code Guide

### 1. Folder Structure

Your workspace should look like this (side-by-side folders):

```
YourWorkspace/
├── UnityGaussianSplatting/          # Original Gaussian Splatting project (from aras-p)
│   └── projects/
│       └── GaussianExample/
│           └── Assets/
│               ├── Scripts/
│               ├── Editor/
│               └── StreamingAssets/
│               └── GSTestScene.unity
├── Pano2Game/
│   ├── Assets/
│   ├── LICENSE
│   └── README.md
└── AIPipeline/                      # AI processing pipeline (from Pano2Game)
    ├── ai_pipeline.py
    ├── generate_asset_embeddings.py
    ├── prompts/
    └── environment.yml
```

---

### 2. Environment & Repository Setup

**a. Clone Both Repositories**
```bash
git clone https://github.com/aras-p/UnityGaussianSplatting.git
git clone https://github.com/YOUR_USERNAME/Pano2Game.git
# Move AIPipeline out of Pano2Game if needed
mv Pano2Game/AIPipeline ./AIPipeline
```
> All three folders must be at the same directory level.

**b. Copy Required Files (Do NOT overwrite scene files)**
```bash
cp -r Pano2Game/Assets/Scripts UnityGaussianSplatting/projects/GaussianExample/Assets/
cp -r Pano2Game/Assets/Editor UnityGaussianSplatting/projects/GaussianExample/Assets/
cp -r Pano2Game/Assets/StreamingAssets UnityGaussianSplatting/projects/GaussianExample/Assets/
```
> **Note:** Do not overwrite or delete `GSTestScene.unity` or any `.unity` scene files.

**c. Set Up the AI Pipeline Environment**
```bash
cd ../AIPipeline
conda env create -f environment.yml
conda activate EV-ai-pipeline
```

---

### 3. Asset Catalog Preparation

- Go to `UnityGaussianSplatting/projects/GaussianExample/Assets/StreamingAssets/`
- **Create your own `AssetCatalog_processed.json`** based on your available assets.
- Use the provided template as a reference for the required format.

---

### 4. Asset Embedding Generation

**Script:** `generate_asset_embeddings.py`  
**Purpose:** Generates semantic embeddings for all prefabs in your asset catalog using the Gemini API.  
**How to run:**
```bash
cd ../AIPipeline
conda activate EV-ai-pipeline
python generate_asset_embeddings.py
```
- This will create `asset_embeddings.json` for use in the AI pipeline.

---

### How to Add a 3DGS Object to Your Unity Scene

Before running the AI pipeline, you need a 3D Gaussian Splatting (3DGS) object in your Unity scene.  
You have two options:

1. **Download a Pretrained 3DGS Model:**  
   - Download a `.ply` file from the [official 3DGS model repository](https://repo-sam.inria.fr/fungraph/3d-gaussian-splatting/).
   - Import it into Unity using **Tools → Gaussian Splats → Create GaussianSplatAsset**.

2. **Generate Your Own 3DGS Model:**  
   - Use your own 360° video or image set and follow the steps to create a 3DGS `.ply` file.

For detailed, step-by-step instructions on both methods, **see [`GS_Scene_Generation.md`](./GS_Scene_Generation.md)**.

Once you have created a Gaussian Splat asset in Unity, drag it into your scene and assign it to a `GaussianSplatRenderer` component.  
You can now proceed with the AI-driven scene analysis and reconstruction!

---

### 5. Unity Scene Setup & Capture

**a. Open Unity**
- Open the project at `UnityGaussianSplatting/projects/GaussianExample/` in Unity.

**b. Scene Preparation**
- Use the default `GSTestScene.unity` or create your own scene.
- In the Hierarchy, create an empty GameObject (e.g., `SceneCaptureRoot`).
- Add the `SceneCaptureController` component to this GameObject.

**c. Configure Capture Settings**
- In the Inspector, set your desired capture strategy (Global or Local Orbital).
- Adjust parameters such as number of viewpoints, radius, and angles.

**d. Run the Capture**
- **Right-click** the `SceneCaptureController` component header in the Inspector.
- Select **"START Capture & Analysis"** from the context menu.
- The script will:
  - Move the camera to each viewpoint
  - Capture images
  - Send them to the Gemini VLM for analysis
  - Save JSON results in the specified output directory

> **To stop the capture early:**  
> Right-click the component and select **"STOP Capture & Analysis"**.

---

### 6. AI Pipeline: Scene Understanding & Script Generation

**Script:** `ai_pipeline.py`  
**Purpose:**  
- Reads all VLM JSON outputs
- Consolidates detected objects
- Matches objects to your asset catalog using the generated embeddings (RAG)
- Determines plausible object placements
- **Generates a Unity C# script** for scene reconstruction

**How to run:**
```bash
cd ../AIPipeline
conda activate EV-ai-pipeline
python ai_pipeline.py
```
- Output: `GeneratedUnityScript.cs.txt`

---

### 7. Scene Generation in Unity (NO manual copying needed!)

**Tool:**  
- In Unity, go to the top menu:  
  **AI Tools → Update and Build Scene from AI**

**What happens:**  
- The tool automatically reads the latest generated C# script and builds the scene in Unity, placing all objects as determined by the AI pipeline.

---

### 8. Play, Refine, and Evaluate

- Enter Play mode in Unity to explore your interactive, AI-reconstructed scene.
- You can further tweak object placements, materials, or add your own logic as needed.

---

### What Each Code Does

| Code/File                      | Purpose                                                                                   |
|------------------------------- |------------------------------------------------------------------------------------------|
| `SceneCaptureController.cs`     | Unity component for automated camera capture and VLM analysis                            |
| `GeminiVLMIntegration.cs`       | Handles communication with the Gemini Vision Language Model API                           |
| `ai_pipeline.py`                | Main Python pipeline: consolidates VLM results, matches assets, generates Unity script   |
| `generate_asset_embeddings.py`  | Generates semantic embeddings for all prefabs in your asset catalog                      |
| `LLMGeneratedSceneBuilder.cs`   | (Auto-generated) Instantiates and places objects in Unity based on AI pipeline output    |
| `AI Tools/Update and Build Scene from AI` | Unity editor tool to automatically build the scene from the generated script      |

---

### Troubleshooting & Tips

- **Missing Assets:** If objects are not appearing, check your `AssetCatalog_processed.json` for correct paths and asset availability.
- **API Keys:** Ensure your Gemini API key is set as an environment variable (`GEMINI_API_KEY`) before running Python scripts.
- **Python Errors:** Make sure your Conda environment is activated and all dependencies are installed.
- **Scene Overwrite:** Never overwrite or delete Unity scene files when copying assets.

---

### Summary Table

| Step | Tool/Script                        | Description                                      |
|------|------------------------------------|--------------------------------------------------|
| 1    | Git, Conda                         | Clone repos, set up environment                  |
| 2    | Unity, AssetCatalog                | Scene setup, asset catalog creation              |
| 3    | generate_asset_embeddings.py       | Generate semantic embeddings for asset matching   |
| 4    | Unity (SceneCaptureController)     | Capture and VLM analysis                         |
| 5    | ai_pipeline.py                     | AI-driven scene understanding & script generation|
| 6    | Unity Editor Tool                  | Scene instantiation (no manual copying needed)   |
| 7    | Unity                              | Play, refine, evaluate                           |

---

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- [UnityGaussianSplatting](https://github.com/aras-p/UnityGaussianSplatting) for the base Gaussian Splatting implementation
- Google Gemini Vision Language Model
- COLMAP for Structure from Motion