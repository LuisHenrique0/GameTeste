using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Item
{
    public enum ItemType
    {
        HealthPotion,
        ManaPotion,
    }

    public ItemType itemType;
    public int amount;
    public string name;
    public string description;
    public int effectValue;
    public Sprite icon; 

    public Item(ItemType type, int amount)
    {
        this.itemType = type;
        this.amount = amount;
        
        
        string iconPath = $"Icons/{type}"; 
        this.icon = Resources.Load<Sprite>(iconPath);
        
        switch (type)
        {
            case ItemType.HealthPotion:
                name = "Poção de Vida";
                description = "Recupera 30 pontos de vida";
                effectValue = 30;
                break;
            case ItemType.ManaPotion:
                name = "Poção de Mana";
                description = "Recupera 20 pontos de mana";
                effectValue = 20;
                break;
        }
    }
}