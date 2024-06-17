using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public int attackPower;
    public GameObject prefab;
    public override void Use()
    {
        base.Use();

        EquipWeapon();
    }

    private void EquipWeapon()
    {
        GuyAction playerWeaponManager = FindObjectOfType<GuyAction>();
        if (playerWeaponManager != null)
        {
            playerWeaponManager.EquipWeapon(this);
        }
    }
    public void CheckNewWeapon(string name)
    {
        switch (name)
        {
            case "dark_sword":
                this.prefab = Resources.Load<GameObject>("Weapons/Prefabs/dark_sword");
                this.icon = Resources.Load<Sprite>("Inventory_Icon/Weapon/dark_sword_icon");
                this.attackPower = 20;
                break;
            case "axe":
                this.prefab = Resources.Load<GameObject>("Weapons/Prefabs/axe");
                this.icon = Resources.Load<Sprite>("Inventory_Icon/Weapon/axe_icon");
                this.attackPower = 10;
                break;
            case "hammer":
                this.prefab = Resources.Load<GameObject>("Weapons/Prefabs/hammer");
                this.icon = Resources.Load<Sprite>("Inventory_Icon/Weapon/hammer_icon");
                this.attackPower = 15;
                break;
            case "mace":
                this.prefab = Resources.Load<GameObject>("Weapons/Prefabs/mace");
                this.icon = Resources.Load<Sprite>("Inventory_Icon/Weapon/mace_icon");
                this.attackPower = 25;
                break;
            case "skull_axe":
                this.prefab = Resources.Load<GameObject>("Weapons/Prefabs/skull_axe");
                this.icon = Resources.Load<Sprite>("Inventory_Icon/Weapon/skull_axe_icon");
                this.attackPower = 20;
                break;
        }
    }
}

