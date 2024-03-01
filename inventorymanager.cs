using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventorymanager : MonoBehaviour
{
    public static inventorymanager Instance;
    public List<Item> Items = new List<Item>();
    private AudioManager audioManager;
   
    public GameObject basementdoortrigger;

    public Transform ItemContent;
    public GameObject InventoryItem;
    private float unlockRange = 3.0f;
    private float distanceToLockedDoor;
    bool itemsAlreadyListed = false;
    public bool doorunlocked = false;
    public GameObject player;
    public GameObject player2;
    public GameObject lockedDoor;
    public GameObject Catpickup;
    FirstPersonController firstpc;
    Rigidbody rb;
    itemPickUp cat;

    

   
    void Awake()
    {
        Instance = this;
        //player = GameObject.FindGameObjectWithTag("Player");
       // lockedDoor = GameObject.FindGameObjectWithTag("Locked");
        audioManager = FindObjectOfType<AudioManager>();
        rb = lockedDoor.GetComponent<Rigidbody>();
        firstpc = player.GetComponent<FirstPersonController>();
        cat = Catpickup.GetComponent<itemPickUp>(); 

    }

    void Update()
    {

        distanceToLockedDoor = Vector3.Distance(player.transform.position, lockedDoor.transform.position);
        if(cat.catpickedup){player = player2;}
            
    }

    public void Add(Item item)
    {
        if (!Items.Contains(item))
        {
            Debug.Log("Adding item: " + item.itemName);
            Items.Add(item);
            ListItems();
        }

        
        
    }
    public void Remove(Item item)
    {
        int index = Items.IndexOf(item);

        if (index >= 0 && index < Items.Count)
        {   
            Item itemToRemove = Items[index];
            DestroyItem(index);       
            Items.Remove(item);  
            ListItems();
        }
        
        
    }
    public void ListItems()
    {

        foreach (Transform child in ItemContent)
        {
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
       
       
        foreach (var item in Items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;
            
        }

        Debug.Log("UI elements updated");
        itemsAlreadyListed = true;
    }

    public void UseItem()
    {   
        Debug.Log("itemUsed!");

        if(firstpc.currentHealth <= 75)
        {
        
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];

                if(item.isMed) 
                {
                    firstpc.IncreaseHealth(item.healthvalue);
                    if (item != null)
                    {
                        Remove(item);
                    }
                    break;
                }
   

            }
        }

        if (distanceToLockedDoor <= unlockRange)
        {

            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                
                if (item.isKey)
                {
                    audioManager.Play("AngryCat");
                    rb.isKinematic = false; //door moves again
                    Debug.Log("door unlocked " + rb.isKinematic);
                    Destroy(basementdoortrigger);
                    if (item != null)
                    {
                        Remove(item);
                    }
                    break;
                }
        }   }
    }



    public void DestroyItem(int index)
    { 
        if (index >= 0 && index < ItemContent.childCount)
        {
            Transform child = ItemContent.GetChild(index);

            if (child != null)
            {
                Debug.Log("Destroying item:");
                Destroy(child.gameObject);
            } 
        }
    }
   

   
        

}

