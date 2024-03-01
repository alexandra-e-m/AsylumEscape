using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FLight : MonoBehaviour
{
    // Start is called before the first frame update
    Light Flight;
    float minT = 0.3f;
    float maxT =2f;
    float minint = 0.5f;
    float maxint = 2f;
    void Start()
    {

        Flight = GetComponent<Light>();
        StartCoroutine(Flashing());
        
    }
    IEnumerator Flashing ()
        {
            while(true)
            {
                yield return new WaitForSeconds(Random.Range(minT, maxT));
                Flight.intensity = Random.Range(minint, maxint);

            }
        }
}
