using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GuyInventory : MonoBehaviour
{
    public List<Item> inventory = new List<Item>();
    public List<Weapon> weapons = new List<Weapon>();
    public List<Image> inventoryImages;
    public List<Image> weaponImages;
    public Sprite defaultSprite;
    public Sprite newSprite;
    public void AddItem(Item item)
    {
        inventory.Add(item);
        Debug.Log("Item added to inventory: " + item.name);
    }

    public void AddWeapon(string itemName)
    {
        Weapon newWeapon = ScriptableObject.CreateInstance<Weapon>();

        newWeapon.CheckNewWeapon(itemName);

        weapons.Add(newWeapon);
        UpdateInventoryImage(weapons.Count - 1, newWeapon.icon, weaponImages);
        Debug.Log("Weapon added to inventory: " + newWeapon.itemName);
    }

    public Weapon GetWeapon(int index)
    {
        if (index >= 0 && index < weapons.Count)
        {
            return weapons[index];
        }
        return null;
    }

    public void UpdateInventoryImage(int index, Sprite newSprite, List<Image> images)
    {
        if (index >= 0 && index < images.Count && images[index] != null)
        {
            images[index].sprite = newSprite;
        }
        else
        {
            Debug.LogError("Invalid index or Image is not assigned.");
        }
    }

}
