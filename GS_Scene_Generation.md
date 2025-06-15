# How to Generate or Import 3D Gaussian Splatting (3DGS) Scene Objects in Unity

This guide explains two ways to get 3DGS scene objects into your Unity project:

1. **Download a Pretrained 3DGS Model (Recommended for Quick Start)**
2. **Generate Your Own 3DGS Model from Video or Images**

---

## Option 1: Download a Pretrained 3DGS Model

1. **Find a Pretrained 3DGS Model**
   - The [Inria 3D Gaussian Splatting repository](https://github.com/graphdeco-inria/gaussian-splatting) provides links to several pretrained models (see their [official model download page](https://repo-sam.inria.fr/fungraph/3d-gaussian-splatting/)).
   - Download a `.ply` file (not just any PLY, but one specifically produced by the 3DGS pipeline).

2. **Import the 3DGS Model into Unity**
   - Open your Unity project at `UnityGaussianSplatting/projects/GaussianExample`.
   - In Unity, go to **Tools → Gaussian Splats → Create GaussianSplatAsset**.
   - In the dialog:
     - Set **Input PLY/SPZ File** to your downloaded `.ply` file.
     - Choose output options and folder.
     - Click **Create Asset**.
   - A new GS asset will appear in your Unity project.
   - Drag it into your scene and assign it to a `GaussianSplatRenderer` component.

3. **For more details, see the [UnityGaussianSplatting README](https://github.com/aras-p/UnityGaussianSplatting#usage).**

---

## Option 2: Generate Your Own 3DGS Model from Video or Images

1. **Prepare Your Input**
   - **360° Video:** Record a panoramic video of your scene.
   - **Image Set:** Take photos from multiple angles.

2. **Extract Frames from Video (if using video)**
   ```bash
   ffmpeg -i your_video.mp4 -qscale:v 2 frames/frame_%04d.jpg
   ```

3. **Reconstruct Camera Poses and Point Cloud with COLMAP**
   - Download and install [COLMAP](https://colmap.github.io/).
   - Import your images/frames.
   - Run feature extraction, matching, and structure-from-motion.
   - Export the sparse point cloud as a `.ply` file.

4. **Train a 3DGS Model**
   - Follow the [Inria 3DGS repo instructions](https://github.com/graphdeco-inria/gaussian-splatting) to convert your COLMAP output to a 3DGS `.ply` or `.spz` file.

5. **Import the 3DGS Model into Unity**
   - Follow the same steps as in Option 1, Step 2.

---

## Tips

- Only `.ply` or `.spz` files produced by the 3DGS pipeline are compatible with UnityGaussianSplatting.
- You can use multiple GS objects in your scene for complex environments.
- For best results, use high-quality images and ensure good coverage of your scene.

---

## References

- [UnityGaussianSplatting GitHub](https://github.com/aras-p/UnityGaussianSplatting)
- [Inria 3D Gaussian Splatting](https://github.com/graphdeco-inria/gaussian-splatting)
- [COLMAP](https://colmap.github.io/) 