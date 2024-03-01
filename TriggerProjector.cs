using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerProjector : MonoBehaviour
{
    bool wastriggered = false;
    private AudioManager audioManager;
    public GameObject Screen;
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
            
            Debug.Log("Projector triggered!");
            Screen.SetActive(true); 
           // audioManager.Play("PlateBreaking");
            wastriggered= true;
        }
        
       
        
    }
}
