using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound3 : MonoBehaviour
{
    bool wastriggered = false;
    private AudioManager audioManager;
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {   
        Debug.Log("Trigger entered!");
        
        if (other.CompareTag("Player") && !wastriggered)
        {
            
            Debug.Log("PlateBreaking triggered!");
            audioManager.Play("PlateBreaking");
            wastriggered= true;
        }
        
       
        
    }
}
