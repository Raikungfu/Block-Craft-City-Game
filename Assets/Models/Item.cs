using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    HealthPotion,
    ManaPotion,
    Weapon,
    Armor,
    Block,
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    public int amount; 
    public virtual void Use()
    {
        Debug.Log("Using item: " + itemName);
    }
}

