using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{

    public GameObject[] objectPrefabs; // 6 prefabs
    public int totalObjectsToSpawn = 18;
    public float spawnRadius = 10f;

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        if (objectPrefabs.Length != 6)
        {
            Debug.LogError("Assign exactly 6 prefabs in Inspector!");
            return;
        }

        for (int i = 0; i < totalObjectsToSpawn; i++)
        {
            int randomIndex = Random.Range(0, objectPrefabs.Length);
            GameObject selectedPrefab = objectPrefabs[randomIndex];

            Vector3 randomSpawnPosition = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius),50f, // start high above terrain
                                                                                                                        //
            Random.Range(-spawnRadius, spawnRadius)
            );

            // Raycast downward to find terrain
            if (Physics.Raycast(randomSpawnPosition, Vector3.down, out RaycastHit hit, 100f))
            {
                randomSpawnPosition.y = hit.point.y + 1f; // 1 unit above terrain
            }

            Instantiate(selectedPrefab, randomSpawnPosition, Quaternion.identity);


        }
    }
}

