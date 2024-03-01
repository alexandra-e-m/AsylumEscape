using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TReadPaper : MonoBehaviour
{
    public bool read = false;
    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {   
        Debug.Log("Trigger entered!");
        
        if (other.CompareTag("Player") )
        {
            read = true;

        }
    }
        
}
