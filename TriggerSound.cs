using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerSound : MonoBehaviour
{
    bool wastriggeredKnock = false;
    private AudioManager audioManager;
    // Start is called before the first frame update

    void Start()
    {  
        audioManager = FindObjectOfType<AudioManager>();
    }
    void OnTriggerEnter(Collider other)
    {   
        Debug.Log("Trigger entered!");
        
        if (other.CompareTag("Player") && !wastriggeredKnock)
        {
            
            Debug.Log("Knock sound triggered!");
            audioManager.Play("door_knock");
            wastriggeredKnock = true;
        }
        
       
        
    }

    // Update is called once per frame
    
}
