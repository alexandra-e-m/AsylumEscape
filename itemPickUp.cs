using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemPickUp : MonoBehaviour
{
    public Item Item;
    private AudioManager audioManager;

    public GameObject Cat1;
    public GameObject Cat2;
    public bool catpickedup = false;

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();

    }
    void PickUp()
    {
        if (gameObject != null)
        {
            if(Item.isKey)
            {
                inventorymanager.Instance.Add(Item);
                Destroy(gameObject);
                audioManager.Play("whoosh");
            }
            if(Item.isCat)
            {
                audioManager.Play("Purr");
                Cat1.SetActive(false); 
                Cat2.SetActive(true); 
                catpickedup = true;

            }
            else
            {
                inventorymanager.Instance.Add(Item);
                Destroy(gameObject);
            }
        }

        
    }

    private void OnMouseDown()
    {
        PickUp();
    }
    
        
    
}
