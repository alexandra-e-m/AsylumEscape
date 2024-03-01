using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteEnemyDoor : MonoBehaviour
{
    public Transform DoorL; //left door
    public Transform DoorR; //right door
    public GameObject Enemy;
    private Vector3 previousPositionL;
    private Vector3 previousPositionR;
    // Start is called before the first frame update
    void Start()
    {
        previousPositionL = DoorL.position;
        previousPositionR = DoorR.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(previousPositionL != DoorL.position ||  previousPositionR != DoorR.position)
        {
            Enemy.SetActive(false);
        }
    }
}
