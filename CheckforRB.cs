using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckforRB : MonoBehaviour
{
    private float stepOffset;
    [SerializeField] private CharacterController CC;
    void Start()
    {
        stepOffset = CC.stepOffset;
    }

    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Rigidbody>() != null)
        {
            CC.stepOffset = 0.0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Rigidbody>() != null)
        {
            CC.stepOffset = stepOffset;
        }
    }
}
