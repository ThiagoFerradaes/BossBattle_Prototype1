#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Editor script that automatically collects all FurnitureFeaturesSo assets from the project
/// and populates the StoreSystem's furniture list before build or on manual request.
/// Implements IPreprocessBuildWithReport to ensure the list is updated before each build.
/// </summary>
public class FurnitureAutoCollector : IPreprocessBuildWithReport
{
    /// <summary>
    /// Callback execution order. Lower values execute first.
    /// Priority 0 is appropriate for this collector.
    /// </summary>
    public int callbackOrder => 0;
    
    /// <summary>
    /// Called automatically before the build process starts.
    /// Updates the StoreSystem with all available furniture ScriptableObjects.
    /// </summary>
    /// <param name="report">Build report containing build information</param>
    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Updating FurnitureFeaturesSo before build...");
        UpdateStoreSystemData();
    }
    
    /// <summary>
    /// Scans the project for all FurnitureFeaturesSo assets and updates the StoreSystem
    /// in the Tavern_Scene with the complete furniture list.
    /// Opens the scene, finds all furniture assets, removes duplicates, and saves the updated list.
    /// </summary>
    public static void UpdateStoreSystemData()
    {
        string scenePath = "Assets/PROJECT/Scenes/Tavern&Menu_Scenes/Tavern_Scene.unity";

        // Open the tavern scene
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!scene.isLoaded)
        {
            Debug.LogError("Unable to open Tavern_Scene.");
            return;
        }

        // Find the StoreSystem component in the scene
        var storeSystem = GameObject.FindObjectOfType<StoreSystem>();
        if (storeSystem == null)
        {
            Debug.LogError("StoreSystem not found in the scene.");
            return;
        }

        // Find all FurnitureFeaturesSo ScriptableObjects in the project
        var guids = AssetDatabase.FindAssets("t:FurnitureFeaturesSo");
        List<FurnitureFeaturesSo> allSO = new();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var so = AssetDatabase.LoadAssetAtPath<FurnitureFeaturesSo>(path);
            if (so != null)
                allSO.Add(so);
        }

        // Remove duplicate entries
        allSO = allSO.Distinct().ToList();

        // Update the StoreSystem with the collected furniture list
        Undo.RecordObject(storeSystem, "Update Furniture List");
        storeSystem.allFurnitureFormProject = allSO;

        // Save changes to the scene
        EditorUtility.SetDirty(storeSystem);
        EditorSceneManager.SaveScene(scene);

        Debug.Log($"Updated {allSO.Count} FurnitureFeaturesSo assets.");
    }
    
    /// <summary>
    /// Manual menu item to trigger the furniture collection without building.
    /// Accessible via Tools > Store System > Update Furniture in the Unity Editor menu.
    /// </summary>
    [MenuItem("Tools/Store System/Update Furniture")]
    public static void ManualUpdate()
    {
        UpdateStoreSystemData();
    }
}
#endif