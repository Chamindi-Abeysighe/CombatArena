using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ArenaGenerator : MonoBehaviour
{
    [Header("Arena Settings")]
    public int gridSizeX = 70;
    public int gridSizeY = 70;
    public float tileSize = 2f; // Size of each cube tile
    public GameObject tilePrefab; // Assign a cube prefab

   

    /// <summary>
    /// Generate arena in Play Mode
    /// </summary>
    private void Start()
    {
        if (Application.isPlaying)
        {
            ClearArena();
            GenerateArena();
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Allow manual arena generation via button in Inspector
    /// </summary>
    [CustomEditor(typeof(ArenaGenerator))]
    public class ArenaGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ArenaGenerator generator = (ArenaGenerator)target;
            if (GUILayout.Button("Generate Arena"))
            {
                if (generator.tilePrefab == null)
                {
                    Debug.LogWarning("Assign a Tile Prefab first!");
                    return;
                }

                generator.ClearArena();
                generator.GenerateArena();
            }
        }
    }

    /// <summary>
    /// Optional: automatically regenerate arena in editor when something changes
    /// Uses delayCall to avoid OnValidate SendMessage errors
    /// </summary>
    private void OnValidate()
    {
        if (tilePrefab == null) return;

        if (!Application.isPlaying)
        {
            EditorApplication.delayCall += () =>
            {
                if (this != null)
                {
                    ClearArena();
                    GenerateArena();
                }
            };
        }
    }
#endif

    /// <summary>
    /// Clear existing children tiles
    /// </summary>
    private void ClearArena()
    {
        // Loop backwards to safely delete children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
#if UNITY_EDITOR
            else
                DestroyImmediate(child);
#endif
        }
    }

    /// <summary>
    /// Generate grid-based arena
    /// </summary>
    private void GenerateArena()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeY; z++)
            {
                Vector3 pos = new Vector3(x * tileSize, 0, z * tileSize);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                tile.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);

                // Ensure tile has collider
                if (tile.GetComponent<Collider>() == null)
                    tile.AddComponent<BoxCollider>();
            }
        }

        
    }
}
