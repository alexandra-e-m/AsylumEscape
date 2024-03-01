using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound2 : MonoBehaviour
{
    bool wastriggeredScream = false;
    
    private AudioManager audioManager;
    // Start is called before the first frame update

    void Start()
    {  
        audioManager = FindObjectOfType<AudioManager>();
    }
    void OnTriggerEnter(Collider other)
    {   
        Debug.Log("Trigger entered!");
        
        if (other.CompareTag("Player") && !wastriggeredScream)
        {
            
            Debug.Log("Knock sound triggered!");
            audioManager.Play("Scream2");
            wastriggeredScream = true;
        }
        
       
        
    }

    // Update is called once per frame
    
}
