using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEnemy : MonoBehaviour
{   
    public bool hasInteractedWithEnemy;
    private AudioManager audioManager;
    public GameObject triggerzone;
    public GameObject TriggerZoneS; //game object with the spawned enemy script
    public GameObject Cat;
    SpawnEnemy spawnE; //Declare variable that will contain the script


    
    void Awake()
    {
        spawnE = TriggerZoneS.GetComponent<SpawnEnemy>();  //attach scrip from gameobjs TriggerZoneS to the variable
        audioManager = FindObjectOfType<AudioManager>();
    }
       


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && spawnE.hasInteractedWithEnemy)
        {
            // Player entered the trigger zone
            Destroy();
        }
    }

    private void Destroy()
    {
        if (spawnE.spawnedEnemy != null)
        {
            // Destroy the enemy when the player exits the trigger zone and spawns the cat
            spawnE.hasInteractedWithEnemy = false;
            Destroy(spawnE.spawnedEnemy);
            audioManager.Stop("Chase");
            spawnE.spawnedEnemy.SetActive(false);
            Cat.SetActive(true); 
        }
    }
}
