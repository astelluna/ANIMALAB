using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class ItemSlotImageData : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image itemImage;

    ItemDataClass itemData;
    int itemAmount; 
    public event Action<ItemDataClass,int> OnImageClick;

    public void SetItemData(ItemDataClass itemDataClass, int amount)
    {
        this.gameObject.SetActive(true);
        itemData = itemDataClass;
        itemImage.sprite = itemData.Icon;
        itemAmount = amount;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("이미지 클릭");
        OnImageClick?.Invoke(itemData, itemAmount);
    }
}
