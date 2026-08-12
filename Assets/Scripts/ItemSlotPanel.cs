using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotPanel : MonoBehaviour
{
    [SerializeField] ItemSlotImageData itemImagePrefab;
    [SerializeField] Transform spawnPos;
    [SerializeField] ItemSlotText itemSlotText;
    [SerializeField] Button closeButton;
    List<ItemSlotImageData> itemSlotImageList = new List<ItemSlotImageData>();
    public bool IsPanelShow => gameObject.activeInHierarchy;
    private void Awake()
    {
        closeButton.onClick.AddListener(() => this.gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        itemSlotText.ClearText();
        foreach(var item in itemSlotImageList)
        {
            item.OnImageClick -= ItemDataShow;
        }
    }

    private void RegisterEvent()
    {
        foreach(ItemSlotImageData item in itemSlotImageList)
        {
            item.OnImageClick -= ItemDataShow;
            item.OnImageClick += ItemDataShow;
        }
    }

    public void ActiveItemSlotCanvas(List<SaveItem> saveItemList)
    {
        this.gameObject.SetActive(true);
        int count = saveItemList.Count;
        while(itemSlotImageList.Count < count)
        {
            itemSlotImageList.Add(Instantiate(itemImagePrefab, spawnPos));
        }
        //일단 전체 비활성화
        foreach (var item in itemSlotImageList)
        {
            item.gameObject.SetActive(false);
        }
        //이벤트 구독
        RegisterEvent();

        for (int index = 0; index< saveItemList.Count; index++)
        {
            ItemDataClass item = ResourceManager.Instance.GetItemData(saveItemList[index].key);
            itemSlotImageList[index].SetItemData(item, saveItemList[index].amount);
        }

    }

    public void ItemDataShow(ItemDataClass itemData, int amount)
    {
        itemSlotText.SetItemSlotText(itemData.ItemId, amount.ToString(), itemData.Description);
    }


}
