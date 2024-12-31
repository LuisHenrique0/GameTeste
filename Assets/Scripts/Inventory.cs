using UnityEngine;
using System.Collections.Generic;

public class Inventory
{
    private List<Item> itemList;
    public List<Item> Items => itemList;

    public Inventory()
    {
        itemList = new List<Item>();
        AddItem(new Item(Item.ItemType.HealthPotion, 3));
        AddItem(new Item(Item.ItemType.ManaPotion, 2));
    }

    public void AddItem(Item item)
    {
        var existingItem = itemList.Find(i => i.itemType == item.itemType);
        if (existingItem != null)
        {
            existingItem.amount += item.amount;
        }
        else
        {
            itemList.Add(item);
        }
    }

    public bool UseItem(Item.ItemType itemType)
    {
        var item = itemList.Find(i => i.itemType == itemType);
        if (item != null && item.amount > 0)
        {
            item.amount--;
            if (item.amount <= 0)
            {
                
            }
            return true;
        }
        return false;
    }

    public Item GetItem(Item.ItemType itemType)
    {
        return itemList.Find(i => i.itemType == itemType);
    }
}
