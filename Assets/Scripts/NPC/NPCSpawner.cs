using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public float spawnInterval = 5f;
    public List<GameObject> npcPrefabs;
    public List<Transform> spawnPoints;

    [Header("Общие точки маршрута для NPC")]
    public Transform entryPoint;
    public Transform insidePoint;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRandomNPC();
        }
    }

    private void SpawnRandomNPC()
    {
        if (npcPrefabs.Count == 0 || spawnPoints.Count == 0)
        {
            Debug.LogWarning("NPCSpawner: нет доступных префабов или точек спавна!");
            return;
        }

        GameObject npcPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Count)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        GameObject npc = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);

        NPCController controller = npc.GetComponent<NPCController>();
        if (controller != null)
        {
            controller.entryPoint = entryPoint;
            controller.insidePoint = insidePoint;
        }
        else
        {
            Debug.LogWarning("NPCSpawner: Префаб не содержит NPCController.");
        }
    }
}
