using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool isCleared = false;
    private bool isActive = false;

    public bool AreExitsBlocked => isActive && !isCleared;

    public void StartBattle()
    {
        if (isCleared || isActive) return;

        GameObject[] validPrefabs = GetValidPrefabs();
        if (validPrefabs.Length == 0)
        {
            Debug.LogWarning("EnemySpawnPoint: массив enemyPrefabs пуст или все слоты null — бой пропущен.");
            isCleared = true;
            return;
        }

        isActive = true;

        int spawned = 0;
        if (spawnPoints != null)
        {
            foreach (var point in spawnPoints)
            {
                if (point == null)
                    continue;

                GameObject prefab = validPrefabs[Random.Range(0, validPrefabs.Length)];
                GameObject enemy = Instantiate(prefab, point.position, Quaternion.identity);
                enemy.transform.SetParent(transform, worldPositionStays: true);
                spawnedEnemies.Add(enemy);
                spawned++;
            }
        }

        if (spawned > 0)
        {
            Room room = GetRoomForDoors();
            if (room != null)
                room.SetExitsLocked(true);
        }
        else
        {
            isCleared = true;
            isActive = false;
            Debug.LogWarning("EnemySpawnPoint: нет точек спавна (spawnPoints) — двери не блокируются.");
        }
    }

    GameObject[] GetValidPrefabs()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            return System.Array.Empty<GameObject>();

        int n = 0;
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemyPrefabs[i] != null)
                n++;
        }

        if (n == 0)
            return System.Array.Empty<GameObject>();

        var list = new GameObject[n];
        int w = 0;
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemyPrefabs[i] != null)
                list[w++] = enemyPrefabs[i];
        }

        return list;
    }

    private void Update()
    {
        if (!isActive || isCleared) return;

        spawnedEnemies.RemoveAll(e => e == null);

        if (spawnedEnemies.Count == 0)
        {
            isCleared = true;
            Room room = GetRoomForDoors();
            if (room != null)
                room.SetExitsLocked(false);
            Debug.Log("������� ��������!");
        }
    }

    private Room GetRoomForDoors()
    {
        Room onSelf = GetComponent<Room>();
        return onSelf != null ? onSelf : GetComponentInParent<Room>();
    }
}
