using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerTrigger : MonoBehaviour
{
    public GameObject Catpickup;
    public GameObject player1;
    public GameObject player2;
    public GameObject enemy;
    itemPickUp cat;
    void Awake()
    {
        cat = Catpickup.GetComponent<itemPickUp>(); 
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && cat.catpickedup)
        {
            player1.SetActive(false);
            player2.SetActive(true); 
            enemy.SetActive(true);
        }
        
    }
}
