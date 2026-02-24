using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Door Visuals to Lock")]
    [SerializeField] private GameObject[] lockVisuals;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool isCleared = false;
    private bool isActive = false;

    public void StartBattle()
    {
        if (isCleared || isActive) return;
        isActive = true;

        foreach (var point in spawnPoints)
        {
            var enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], point.position, Quaternion.identity);
            enemy.transform.parent = transform;
            spawnedEnemies.Add(enemy);
        }

        if (spawnedEnemies.Count > 0) ToggleDoors(true);
        else isCleared = true;
    }

    private void Update()
    {
        if (!isActive || isCleared) return;

        spawnedEnemies.RemoveAll(e => e == null);

        if (spawnedEnemies.Count == 0)
        {
            isCleared = true;
            ToggleDoors(false);
            Debug.Log("Комната зачищена!");
        }
    }

    private void ToggleDoors(bool lockState)
    {
        foreach (var door in lockVisuals) door.SetActive(lockState);
    }
}
