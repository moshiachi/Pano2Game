import json
import os
import google.generativeai as genai
import numpy as np

# --- Configuration ---
try:
    genai.configure(api_key=os.environ["GEMINI_API_KEY"])
    print("Gemini API key configured successfully.")
except KeyError:
    print("CRITICAL ERROR: The 'GEMINI_API_KEY' environment variable is not set.")
    exit()

ASSET_CATALOG_PATH = '../UnityGaussianSplatting/projects/GaussianExample/Assets/StreamingAssets/AssetCatalog_processed.json'
EMBEDDINGS_OUTPUT_PATH = 'asset_embeddings.json'
EMBEDDING_MODEL = 'text-embedding-004' # The new, powerful embedding model

def create_text_description_for_asset(asset):
    """Creates a detailed text description for an asset to be used for embedding."""
    if asset.get('type') != 'Prefab':
        return None
    
    name = asset.get('asset_name_id', '').replace('_Prefab', '').replace('_', ' ')
    tags = ", ".join(asset.get('tags', []))
    description = asset.get('description', '')
    
    # Combine all information into a rich description
    full_description = f"Asset Name: {name}. Tags: {tags}. Description: {description}"
    return full_description

def generate_embeddings():
    """
    Loads the asset catalog, generates a text description for each asset,
    calls the Gemini API to get embeddings, and saves them to a file.
    """
    print(f"Loading asset catalog from: {ASSET_CATALOG_PATH}")
    try:
        with open(ASSET_CATALOG_PATH, 'r') as f:
            asset_catalog = json.load(f)
    except Exception as e:
        print(f"Error loading asset catalog: {e}")
        return

    embeddings_data = []
    
    # Filter for only prefabs
    prefabs = [asset for asset in asset_catalog if asset.get('type') == 'Prefab']
    if not prefabs:
        print("No prefabs found in the asset catalog.")
        return

    print(f"Found {len(prefabs)} prefabs. Generating descriptions and embeddings...")

    for asset in prefabs:
        description = create_text_description_for_asset(asset)
        if not description:
            continue

        try:
            # Get the embedding for the description
            result = genai.embed_content(
                model=EMBEDDING_MODEL,
                content=description,
                task_type="RETRIEVAL_DOCUMENT" # Important for RAG
            )
            
            # Store the asset path and its corresponding vector
            embeddings_data.append({
                'asset_path': asset['path'],
                'asset_info': asset, # Store all asset info for later use
                'embedding': result['embedding']
            })
            print(f"-> Generated embedding for: {asset['asset_name_id']}")

        except Exception as e:
            print(f"Error generating embedding for {asset['asset_name_id']}: {e}")

    print(f"\nGenerated a total of {len(embeddings_data)} embeddings.")

    # Save the embeddings to a file
    try:
        with open(EMBEDDINGS_OUTPUT_PATH, 'w') as f:
            json.dump(embeddings_data, f, indent=4)
        print(f"Successfully saved asset embeddings to: {EMBEDDINGS_OUTPUT_PATH}")
    except Exception as e:
        print(f"Error saving embeddings file: {e}")

if __name__ == '__main__':
    generate_embeddings() 