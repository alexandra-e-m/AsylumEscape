using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject spawnedEnemy;
    public Transform spawnPoint;
    public bool hasInteractedWithEnemy = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasInteractedWithEnemy)
        {
            // Player entered the trigger zone
            SpawnE();
            hasInteractedWithEnemy = true;
        }
    }

    private void SpawnE()
    {    
        Debug.Log("Player entered the trigger zone");
        spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        spawnedEnemy.SetActive(true);
    }
}

