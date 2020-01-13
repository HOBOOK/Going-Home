using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemSystem
{
    public static List<Item> items = new List<Item>();
    public static void LoadItem()
    {
        ItemDatabase id = ItemDatabase.Load();
        foreach (Item item in id.items)
        {
            items.Add(item);
        }
        Debugging.LogSystem("ItemDatabase is loaded Succesfully.");
    }

    public static Item GetItem(int id)
    {
        return items.Find(item => item.id == id || item.id.Equals(id));
    }

    public static Item GetItem(string name)
    {
        return items.Find(item => item.name.Equals(name));
    }

    public static int GetWeaponType(int id)
    {
        return items.Find(item => item.id == id).weapontype;
    }
}
