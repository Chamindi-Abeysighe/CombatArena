using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrainGenerator: MonoBehaviour
{

    [Header("Grid Settings")]
    public int width = 70;
    public int height = 70;
    public float tileSize = 1f;

    [Header("Noise Settings")]
    public float scale = 10f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;

    [Header("Height & Tiles")]
    public float maxHeight = 5f;
    public GameObject tilePrefab;

    [Header("Terrain Colors")]
    public Color waterColor = Color.blue;
    public Color sandColor = new Color(0.94f, 0.87f, 0.73f); // sand
    public Color grassColor = Color.green;
    public Color rockColor = Color.gray;
    public Color snowColor = Color.white;

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float noiseValue = 0f;
                float frequency = 1f;
                float amplitude = 1f;
                float totalAmplitude = 0f;

                // Fractal Perlin Noise
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x / scale) * frequency;
                    float sampleZ = (z / scale) * frequency;

                    float perlin = Mathf.PerlinNoise(sampleX, sampleZ) * 2f - 1f;
                    noiseValue += perlin * amplitude;

                    totalAmplitude += amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                noiseValue /= totalAmplitude;

                // Position and height
                float tileHeight = Mathf.Max(0.1f, noiseValue * maxHeight);
                Vector3 position = new Vector3(x * tileSize, tileHeight / 2f, z * tileSize); // Center at half-height
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);

                // Scale tile
                tile.transform.localScale = new Vector3(tileSize, tileHeight, tileSize);

                // Assign color
                Renderer rend = tile.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = new Material(Shader.Find("Standard"));
                    rend.material.color = GetColorByHeight(noiseValue);
                }

                // Adjust collider for capsule player
                BoxCollider col = tile.GetComponent<BoxCollider>();
                if (col != null)
                {
                    col.size = new Vector3(tileSize, tileHeight, tileSize);
                    col.center = new Vector3(0f, tileHeight / 2f, 0f);
                }
            }
        }
    }

    Color GetColorByHeight(float height)
    {
        float h = Mathf.InverseLerp(-1f, 1f, height);

        if (h < 0.2f)
            return waterColor;
        else if (h < 0.35f)
            return sandColor;
        else if (h < 0.6f)
            return grassColor;
        else if (h < 0.8f)
            return rockColor;
        else
            return snowColor;
    }
}

