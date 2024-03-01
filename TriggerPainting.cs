using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPainting : MonoBehaviour
{
    public bool change = false;
    public bool wastriggered = false;

    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !wastriggered)
        {
            change = true;
            wastriggered = true;
        }
    }
}
