using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArtefactManager : MonoBehaviour
{

    [Header("Artefact Settings")]
    public GameObject[] artefactPrefabs; // 6 unique artefacts
    public int minPerType = 3;           // Minimum per artefact type
    public float spawnRadius = 10f;

    [Header("Player Reference")]
    public Transform player;

    [Header("Path Visualization")]
    public bool showPath = true;
    public Material pathMaterial;

    void Start()
    {
        SpawnArtefacts();
    }

    void SpawnArtefacts()
    {
        if (artefactPrefabs.Length != 6)
        {
            Debug.LogError("Please assign exactly 6 artefact prefabs!");
            return;
        }

        for (int i = 0; i < artefactPrefabs.Length; i++)
        {
            for (int j = 0; j < minPerType; j++)
            {
                Vector3 candidatePos = transform.position + new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0f,
                    Random.Range(-spawnRadius, spawnRadius)
                );

                // Check if player can reach this position
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(player.position, candidatePos, NavMesh.AllAreas, path)
                    && path.status == NavMeshPathStatus.PathComplete)
                {
                    GameObject artefact = Instantiate(artefactPrefabs[i], candidatePos, Quaternion.identity);

                    // Optional path visualization
                    if (showPath && path.corners.Length > 1)
                        StartCoroutine(DrawPath(path.corners));
                }
                else
                {
                    j--; // Retry this artefact if path is invalid
                }
            }
        }
    }

    IEnumerator DrawPath(Vector3[] corners)
    {
        for (int i = 0; i < corners.Length - 1; i++)
        {
            GameObject lineObj = new GameObject("PathLine");
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, corners[i]);
            lr.SetPosition(1, corners[i + 1]);
            lr.material = pathMaterial;
            lr.widthMultiplier = 0.05f;

            yield return new WaitForSeconds(2f); // Display briefly
            Destroy(lineObj);
        }
    }
}
