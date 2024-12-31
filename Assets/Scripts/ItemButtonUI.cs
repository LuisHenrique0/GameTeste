using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemButtonUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text amountText;
    public TMP_Text descriptionText;
    public Button button; 

 
    public void UpdateButton(Item item)
    {
        iconImage.sprite = item.icon;
        nameText.text = item.name;
        amountText.text = $"x{item.amount}";
        descriptionText.text = item.description;

        
        button.interactable = item.amount > 0;
    }
}
