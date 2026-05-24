using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;


public class ArtifactSpawner : MonoBehaviour
{
    public GameObject[] artifactPrefabs; // 6 prefabs
    public int minPerType = 3;
    public Transform player;
    public Terrain terrain;
    public bool showPath = true;

    private List<Vector3> placedPositions = new List<Vector3>();

    void Start()
    {
        NavMeshSurface surface = terrain.gameObject.GetComponent<NavMeshSurface>();
        if (surface != null) surface.BuildNavMesh();

        SpawnArtifacts();
    }

    void SpawnArtifacts()
    {
        foreach (GameObject prefab in artifactPrefabs)
        {
            for (int i = 0; i < minPerType; i++)
            {
                Vector3 pos = GetValidPosition();
                if (pos != Vector3.zero)
                {
                    GameObject obj = Instantiate(prefab, pos, Quaternion.Euler(0, Random.Range(0, 360), 0));
                    placedPositions.Add(pos);

                    if (showPath)
                        Debug.DrawLine(player.position, pos, Color.cyan, 30f); // Visualise path
                }
            }
        }
    }

    Vector3 GetValidPosition()
    {
        int maxAttempts = 50;
        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(0, terrain.terrainData.size.x);
            float z = Random.Range(0, terrain.terrainData.size.z);
            float y = terrain.SampleHeight(new Vector3(x, 0, z));

            Vector3 candidate = new Vector3(x, y, z);

            if (Vector3.Distance(candidate, player.position) < 5f) continue;

            bool tooClose = false;
            foreach (Vector3 pos in placedPositions)
            {
                if (Vector3.Distance(candidate, pos) < 5f) { tooClose = true; break; }
            }
            if (tooClose) continue;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, 1f, NavMesh.AllAreas))
                return hit.position;
        }
        return Vector3.zero; // fallback
    }
}
