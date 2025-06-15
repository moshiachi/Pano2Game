using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

// Helper classes for JSON deserialization
[System.Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToUnityVector3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public class ColliderInfoData
{
    public string type; // "box", "sphere", "capsule", "mesh"
    public bool is_trigger;
    public Vector3Data center;
    public Vector3Data size; // For BoxCollider
    public float radius; // For SphereCollider, CapsuleCollider
    public float height; // For CapsuleCollider
    public int direction; // For CapsuleCollider (0=X, 1=Y, 2=Z)
}

[System.Serializable]
public class RigidbodyInfoData
{
    public float mass;
    public bool use_gravity;
    public bool is_kinematic;
    public float drag;
    public float angular_drag;
}

[System.Serializable]
public class SceneObjectData
{
    public string asset_path;
    public string object_name_in_hierarchy;
    public Vector3Data position;
    public Vector3Data rotation_euler; // Euler angles
    public Vector3Data scale;
    public string material_path_override;
    public bool is_generic_asset;
    public ColliderInfoData collider_info;
    public RigidbodyInfoData rigidbody_info;
}

[System.Serializable]
public class SceneDataWrapper
{
    // JsonUtility cannot directly deserialize a JSON array.
    // It requires a root object with a public field of type List<T> or T[].
    public List<SceneObjectData> items;
}

public static class LLMGeneratedSceneBuilder
{
    [MenuItem("AI Tools/Construct Scene from LLM-Generated Code")]
    public static void ConstructScene()
    {
        // Find or create a root GameObject named "LLM_Generated_Scene_Root"
        GameObject sceneRoot = GameObject.Find("LLM_Generated_Scene_Root");
        if (sceneRoot == null)
        {
            sceneRoot = new GameObject("LLM_Generated_Scene_Root");
        }

        // Define a multiline string variable jsonInput containing the provided JSON data
        string jsonInput = @"[
    {
        ""asset_path"": ""Assets/Objects/GeniusCrate_Games/Kitchen_Set/Prefab/Kitchen_Dining_Set/Kitchen_Set_props.prefab"",
        ""object_name_in_hierarchy"": ""kitchen_countertop_surface_0"",
        ""position"": {
            ""x"": 0.0,
            ""y"": 0.89,
            ""z"": -0.2
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 200.0,
            ""use_gravity"": true,
            ""is_kinematic"": true,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/GeniusCrate_Games/Kitchen_Set/Prefab/Kitchen_Dining_Set/Kitchen_Set_props.prefab"",
        ""object_name_in_hierarchy"": ""large_mixing_bowl_1"",
        ""position"": {
            ""x"": 0.0,
            ""y"": 0.9,
            ""z"": 0.0
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""sphere"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 3.0,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/GeniusCrate_Games/Kitchen_Set/Prefab/Kitchen_Dining_Set/Kitchen_Set_props.prefab"",
        ""object_name_in_hierarchy"": ""small_metal_bowl_with_onions_2"",
        ""position"": {
            ""x"": 0.0,
            ""y"": 0.9,
            ""z"": 0.3
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""sphere"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 1.5,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/GeniusCrate_Games/Kitchen_Set/Prefab/Kitchen_Dining_Set/Kitchen_Set_props.prefab"",
        ""object_name_in_hierarchy"": ""pile_of_onions_3"",
        ""position"": {
            ""x"": -0.5,
            ""y"": 0.9,
            ""z"": 0.0
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""mesh"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.8,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/GeniusCrate_Games/Kitchen_Set/Prefab/Kitchen_Dining_Set/Kitchen_Set_props.prefab"",
        ""object_name_in_hierarchy"": ""wooden_rolling_pin_4"",
        ""position"": {
            ""x"": -1.0,
            ""y"": 0.9,
            ""z"": 0.3
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 45.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""capsule"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.5,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::capsule"",
        ""object_name_in_hierarchy"": ""white_pillar_candle_5"",
        ""position"": {
            ""x"": -0.8,
            ""y"": 0.9,
            ""z"": -0.2
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""capsule"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.2,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""water_filter_pitcher_6"",
        ""position"": {
            ""x"": -0.5,
            ""y"": 0.9,
            ""z"": -0.1
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 1.5,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/SandoraKa/Stylized Potted Plants/Built-In/Prefabs/Pot.prefab"",
        ""object_name_in_hierarchy"": ""potted_houseplant_7"",
        ""position"": {
            ""x"": -2.0,
            ""y"": 0.9,
            ""z"": -0.7
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""capsule"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 2.0,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/GeniusCrate_Games/Kitchen_Set/Prefab/Kitchen_Dining_Set/Kitchen_Set_props.prefab"",
        ""object_name_in_hierarchy"": ""food_processor_base_8"",
        ""position"": {
            ""x"": -2.5,
            ""y"": 0.9,
            ""z"": -0.7
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 3.0,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""food_processor_lid_9"",
        ""position"": {
            ""x"": -1.5,
            ""y"": 0.9,
            ""z"": -0.2
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.3,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""egg_carton_10"",
        ""position"": {
            ""x"": 4.0,
            ""y"": 0.9,
            ""z"": 0.5
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.5,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""baking_sheet_11"",
        ""position"": {
            ""x"": -3.5,
            ""y"": 0.9,
            ""z"": 0.2
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 90.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.7,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::capsule"",
        ""object_name_in_hierarchy"": ""food_jar_12"",
        ""position"": {
            ""x"": -3.0,
            ""y"": 0.9,
            ""z"": 0.2
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""capsule"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.4,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/GeniusCrate_Games/Kitchen_Set/Prefab/Kitchen_Dining_Set/Kitchen_Set_props.prefab"",
        ""object_name_in_hierarchy"": ""silicone_oven_mitts_13"",
        ""position"": {
            ""x"": -2.5,
            ""y"": 0.9,
            ""z"": 0.5
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.2,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::capsule"",
        ""object_name_in_hierarchy"": ""red_food_canister_14"",
        ""position"": {
            ""x"": -2.0,
            ""y"": 0.9,
            ""z"": 0.6
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""capsule"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.3,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""brown_paper_bag_15"",
        ""position"": {
            ""x"": -1.5,
            ""y"": 0.9,
            ""z"": 0.8
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": ""Assets/Materials/subtle-grained-wood_albedo.mat"",
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""mesh"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.1,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/Brick Project Studio/Apartment Kit/_Prefabs/Props/Misc/Vase_Apt_01.prefab"",
        ""object_name_in_hierarchy"": ""small_plastic_cup_16"",
        ""position"": {
            ""x"": 0.2,
            ""y"": 0.9,
            ""z"": 0.2
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""capsule"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.1,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/GeniusCrate_Games/Kitchen_Set/Prefab/Kitchen_Dining_Set/Kitchen_Set_props.prefab"",
        ""object_name_in_hierarchy"": ""wooden_serving_bowl_17"",
        ""position"": {
            ""x"": 1.5,
            ""y"": 0.9,
            ""z"": -0.7
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""sphere"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.5,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""stainless_steel_refrigerator_18"",
        ""position"": {
            ""x"": -10.0,
            ""y"": 0.0,
            ""z"": -1.5
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 50.0,
            ""use_gravity"": true,
            ""is_kinematic"": true,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/GeniusCrate_Games/Kitchen_Set/Prefab/Kitchen_Dining_Set/Kitchen_Set_props.prefab"",
        ""object_name_in_hierarchy"": ""kitchen_range_19"",
        ""position"": {
            ""x"": 10.0,
            ""y"": 0.0,
            ""z"": -1.5
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 70.0,
            ""use_gravity"": true,
            ""is_kinematic"": true,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::capsule"",
        ""object_name_in_hierarchy"": ""water_filter_pitcher_20"",
        ""position"": {
            ""x"": -0.5,
            ""y"": 0.9,
            ""z"": -0.1
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""capsule"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 2.0,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/SandoraKa/Stylized Potted Plants/Built-In/Prefabs/Pot.prefab"",
        ""object_name_in_hierarchy"": ""wooden_fruit_bowl_21"",
        ""position"": {
            ""x"": 0.5,
            ""y"": 0.9,
            ""z"": -0.1
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""mesh"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.5,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""bag_of_pasta_22"",
        ""position"": {
            ""x"": -0.7,
            ""y"": 0.9,
            ""z"": -0.4
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.5,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""bag_of_bread_23"",
        ""position"": {
            ""x"": 1.0,
            ""y"": 0.9,
            ""z"": -0.5
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.7,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/SandoraKa/Stylized Potted Plants/Built-In/Prefabs/Pot.prefab"",
        ""object_name_in_hierarchy"": ""potted_plant_24"",
        ""position"": {
            ""x"": 0.5,
            ""y"": 0.9,
            ""z"": -0.7
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""capsule"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 3.0,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/GeniusCrate_Games/Kitchen_Set/Prefab/Kitchen_Dining_Set/Kitchen_Set_props.prefab"",
        ""object_name_in_hierarchy"": ""dish_drainer_25"",
        ""position"": {
            ""x"": -4.5,
            ""y"": 0.9,
            ""z"": 0.0
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 1.0,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""kitchen_base_cabinets_26"",
        ""position"": {
            ""x"": 0.0,
            ""y"": 0.0,
            ""z"": -0.75
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": ""Assets/Materials/subtle-grained-wood_albedo.mat"",
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 50.0,
            ""use_gravity"": true,
            ""is_kinematic"": true,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""kitchen_upper_cabinets_27"",
        ""position"": {
            ""x"": 8.0,
            ""y"": 1.5,
            ""z"": -1.5
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {

            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": ""Assets/Materials/subtle-grained-wood_albedo.mat"",
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 40.0,
            ""use_gravity"": true,
            ""is_kinematic"": true,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""kitchen_window_28"",
        ""position"": {
            ""x"": 0.0,
            ""y"": 1.5,
            ""z"": -2.5
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": ""Assets/Materials/subtle-grained-wood_albedo.mat"",
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 10.0,
            ""use_gravity"": true,
            ""is_kinematic"": true,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""refrigerator_29"",
        ""position"": {
            ""x"": 12.0,
            ""y"": 0.0,
            ""z"": -1.5
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 80.0,
            ""use_gravity"": true,
            ""is_kinematic"": true,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::capsule"",
        ""object_name_in_hierarchy"": ""bicycle_pump_30"",
        ""position"": {
            ""x"": 0.0,
            ""y"": 0.0,
            ""z"": -4.0
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""capsule"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 1.0,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""bread_loaf_31"",
        ""position"": {
            ""x"": 0.0,
            ""y"": 0.9,
            ""z"": -0.3
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.8,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/prefab/Wooden_Plane.prefab"",
        ""object_name_in_hierarchy"": ""wooden_cutting_board_32"",
        ""position"": {
            ""x"": 0.0,
            ""y"": 0.89,
            ""z"": -0.3
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": ""Assets/Materials/subtle-grained-wood_albedo.mat"",
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.5,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""bag_of_pasta_33"",
        ""position"": {
            ""x"": -0.7,
            ""y"": 0.9,
            ""z"": -0.4
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.5,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""canned_food_34"",
        ""position"": {
            ""x"": -0.2,
            ""y"": 0.9,
            ""z"": -0.6
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.4,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/GeniusCrate_Games/Kitchen_Set/Prefab/Kitchen_Dining_Set/Kitchen_Set_props.prefab"",
        ""object_name_in_hierarchy"": ""pringles_cans_35"",
        ""position"": {
            ""x"": 7.0,
            ""y"": 0.9,
            ""z"": -1.0
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.2,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""egg_carton_36"",
        ""position"": {
            ""x"": 4.0,
            ""y"": 0.9,
            ""z"": 0.5
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.8,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/GeniusCrate_Games/Kitchen_Set/Prefab/Kitchen_Dining_Set/Kitchen_Set_props.prefab"",
        ""object_name_in_hierarchy"": ""baking_tray_37"",
        ""position"": {
            ""x"": 4.0,
            ""y"": 0.9,
            ""z"": 0.0
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 90.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.7,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""Assets/Objects/SandoraKa/Stylized Potted Plants/Built-In/Prefabs/Pot.prefab"",
        ""object_name_in_hierarchy"": ""blender_38"",
        ""position"": {
            ""x"": 1.0,
            ""y"": 0.9,
            ""z"": -0.4
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": false,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 3.0,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""cooking_oil_bottle_39"",
        ""position"": {
            ""x"": -8.0,
            ""y"": 0.9,
            ""z"": -1.0
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": ""Assets/Materials/subtle-grained-wood_albedo.mat"",
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 1.0,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""wall_sockets_40"",
        ""position"": {
            ""x"": 0.0,
            ""y"": 0.5,
            ""z"": 0.5
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 180.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.1,
            ""use_gravity"": true,
            ""is_kinematic"": true,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    },
    {
        ""asset_path"": ""primitive::box"",
        ""object_name_in_hierarchy"": ""silicone_oven_mitt_41"",
        ""position"": {
            ""x"": 5.5,
            ""y"": 0.9,
            ""z"": 0.2
        },
        ""rotation_euler"": {
            ""x"": 0,
            ""y"": 0.0,
            ""z"": 0
        },
        ""scale"": {
            ""x"": 1.0,
            ""y"": 1.0,
            ""z"": 1.0
        },
        ""material_path_override"": null,
        ""is_generic_asset"": true,
        ""collider_info"": {
            ""type"": ""box"",
            ""is_trigger"": false,
            ""center"": {
                ""x"": 0,
                ""y"": 0,
                ""z"": 0
            },
            ""size"": {
                ""x"": 1,
                ""y"": 1,
                ""z"": 1
            },
            ""radius"": 0.5,
            ""height"": 2.0,
            ""direction"": 1
        },
        ""rigidbody_info"": {
            ""mass"": 0.1,
            ""use_gravity"": true,
            ""is_kinematic"": false,
            ""drag"": 0.1,
            ""angular_drag"": 0.05
        }
    }
]";

        // Parse the JSON using JsonUtility and a wrapper object
        // JsonUtility cannot directly deserialize a JSON array, so we wrap it.
        string wrappedJsonInput = "{\"items\":" + jsonInput + "}";
        SceneDataWrapper wrapper = JsonUtility.FromJson<SceneDataWrapper>(wrappedJsonInput);

        if (wrapper == null || wrapper.items == null)
        {
            Debug.LogError("Failed to parse JSON or JSON contains no items.");
            return;
        }

        // Loop through the list of objects
        foreach (SceneObjectData objectData in wrapper.items)
        {
            GameObject instance = null;

            // If the asset_path is a primitive, create it using GameObject.CreatePrimitive()
            if (objectData.asset_path.StartsWith("primitive::"))
            {
                PrimitiveType primitiveType;
                string primitiveTypeName = objectData.asset_path.Substring(11).ToLower();
                switch (primitiveTypeName)
                {
                    case "box": primitiveType = PrimitiveType.Cube; break;
                    case "sphere": primitiveType = PrimitiveType.Sphere; break;
                    case "capsule": primitiveType = PrimitiveType.Capsule; break;
                    case "cylinder": primitiveType = PrimitiveType.Cylinder; break;
                    case "plane": primitiveType = PrimitiveType.Plane; break;
                    case "quad": primitiveType = PrimitiveType.Quad; break;
                    default:
                        Debug.LogWarning($"Unknown primitive type '{primitiveTypeName}' specified for object '{objectData.object_name_in_hierarchy}'. Creating Cube instead.");
                        primitiveType = PrimitiveType.Cube;
                        break;
                }
                instance = GameObject.CreatePrimitive(primitiveType);
                // Immediately destroy its default collider as per instruction
                Collider defaultCollider = instance.GetComponent<Collider>();
                if (defaultCollider != null)
                {
                    Object.DestroyImmediate(defaultCollider);
                }
            }
            // If it's a prefab path, load it with AssetDatabase.LoadAssetAtPath<GameObject>()
            else
            {
                GameObject loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(objectData.asset_path);
                if (loadedPrefab == null)
                {
                    Debug.LogWarning($"Failed to load prefab from path: {objectData.asset_path} for object '{objectData.object_name_in_hierarchy}'. Skipping this object.");
                    continue; // Skip to the next object if prefab cannot be loaded
                }
                // If successful, instantiate it using (GameObject)PrefabUtility.InstantiatePrefab(loadedPrefab)
                instance = (GameObject)PrefabUtility.InstantiatePrefab(loadedPrefab);
            }

            // After the if/else block, if instance is not null:
            if (instance != null)
            {
                // Set its parent
                instance.transform.SetParent(sceneRoot.transform);

                // Configure its name
                instance.name = objectData.object_name_in_hierarchy;

                // Configure its transform
                if (objectData.position != null)
                    instance.transform.position = objectData.position.ToUnityVector3();
                if (objectData.rotation_euler != null)
                    instance.transform.rotation = Quaternion.Euler(objectData.rotation_euler.ToUnityVector3());
                if (objectData.scale != null)
                    instance.transform.localScale = objectData.scale.ToUnityVector3();

                // Configure its material (if generic and material_path_override is provided)
                if (objectData.is_generic_asset && !string.IsNullOrEmpty(objectData.material_path_override))
                {
                    Material material = AssetDatabase.LoadAssetAtPath<Material>(objectData.material_path_override);
                    if (material != null)
                    {
                        Renderer renderer = instance.GetComponent<Renderer>();
                        if (renderer == null) // Primitives usually have one, but if not, add it.
                        {
                            renderer = instance.AddComponent<MeshRenderer>();
                            instance.AddComponent<MeshFilter>(); // A renderer often requires a MeshFilter
                        }
                        if (renderer != null)
                        {
                            renderer.material = material;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to load material from path: {objectData.material_path_override} for object '{instance.name}'.");
                    }
                }

                // Add and configure colliders
                if (objectData.collider_info != null)
                {
                    // Ensure no existing collider remains before adding the specified one
                    Collider existingCollider = instance.GetComponent<Collider>();
                    if (existingCollider != null)
                    {
                        Object.DestroyImmediate(existingCollider);
                    }

                    switch (objectData.collider_info.type.ToLower())
                    {
                        case "box":
                            BoxCollider boxCollider = instance.AddComponent<BoxCollider>();
                            if (objectData.collider_info.center != null)
                                boxCollider.center = objectData.collider_info.center.ToUnityVector3();
                            if (objectData.collider_info.size != null)
                                boxCollider.size = objectData.collider_info.size.ToUnityVector3();
                            boxCollider.isTrigger = objectData.collider_info.is_trigger;
                            break;
                        case "sphere":
                            SphereCollider sphereCollider = instance.AddComponent<SphereCollider>();
                            if (objectData.collider_info.center != null)
                                sphereCollider.center = objectData.collider_info.center.ToUnityVector3();
                            sphereCollider.radius = objectData.collider_info.radius;
                            sphereCollider.isTrigger = objectData.collider_info.is_trigger;
                            break;
                        case "capsule":
                            CapsuleCollider capsuleCollider = instance.AddComponent<CapsuleCollider>();
                            if (objectData.collider_info.center != null)
                                capsuleCollider.center = objectData.collider_info.center.ToUnityVector3();
                            capsuleCollider.radius = objectData.collider_info.radius;
                            capsuleCollider.height = objectData.collider_info.height;
                            capsuleCollider.direction = objectData.collider_info.direction;
                            capsuleCollider.isTrigger = objectData.collider_info.is_trigger;
                            break;
                        case "mesh":
                            MeshCollider meshCollider = instance.AddComponent<MeshCollider>();
                            // Mesh colliders need a mesh from a MeshFilter component
                            MeshFilter meshFilter = instance.GetComponent<MeshFilter>();
                            if (meshFilter != null && meshFilter.sharedMesh != null)
                            {
                                meshCollider.sharedMesh = meshFilter.sharedMesh;
                            }
                            else
                            {
                                Debug.LogWarning($"MeshCollider for object '{instance.name}' could not find a MeshFilter or sharedMesh. Collider may be incomplete.");
                            }
                            // MeshCollider 'convex' property is often set to true when used with Rigidbody for non-static objects.
                            // The instruction implies it's added *after* rigidbody, but for proper setup order (convex depends on kinematic), it's good to consider both.
                            // Assuming typical use where non-kinematic mesh colliders are convex.
                            if (objectData.rigidbody_info != null && !objectData.rigidbody_info.is_kinematic)
                            {
                                meshCollider.convex = true;
                            }
                            meshCollider.isTrigger = objectData.collider_info.is_trigger;
                            break;
                        default:
                            Debug.LogWarning($"Unsupported collider type '{objectData.collider_info.type}' for object '{instance.name}'. No collider added.");
                            break;
                    }
                }

                // Add and configure rigidbodies
                if (objectData.rigidbody_info != null)
                {
                    Rigidbody rb = instance.AddComponent<Rigidbody>();
                    rb.mass = objectData.rigidbody_info.mass;
                    rb.useGravity = objectData.rigidbody_info.use_gravity;
                    rb.isKinematic = objectData.rigidbody_info.is_kinematic;
                    rb.drag = objectData.rigidbody_info.drag;
                    rb.angularDrag = objectData.rigidbody_info.angular_drag;
                }
            }
        }

        Debug.Log("Scene construction from LLM-generated code completed under 'LLM_Generated_Scene_Root'.");
    }
}