using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject 
{ 
    public int id;
    public string itemName;
    public int healthvalue;
    public Sprite icon;

    public bool isKey = false;
    public bool isMed = false;
    public bool isCat = false;
    
}
