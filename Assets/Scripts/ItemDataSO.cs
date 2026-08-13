using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDataClass
{
    [field: SerializeField] public string ItemId { get; private set; }
    //[field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }

    public void SetItemData(string id, string msg)
    {
        ItemId = id;
        Description = msg;
    }
}
[CreateAssetMenu(menuName = "Item/Item Data", fileName = "ItemData")]
public class ItemDataSO : ScriptableObject
{
    [field: SerializeField] public List<ItemDataClass> ItemDataClassList { get; private set; } = new List<ItemDataClass>();

    Dictionary<string, ItemDataClass> itemDataclassDict;

    public ItemDataClass GetItemData(string id)
    {
        //초기 1회 캐싱
        if(itemDataclassDict == null)
        {
            itemDataclassDict = new Dictionary<string, ItemDataClass>();
            foreach(var item in ItemDataClassList)
            {
                if(itemDataclassDict.ContainsKey(item.ItemId))
                {
                    UnityEngine.Debug.LogError($"{item.ItemId}의 아이템이 중복되었습니다 확인해주세요");
                    continue;
                }
                itemDataclassDict.Add(item.ItemId, item);
            }
        }
        //데이터를 반환해주자.
        if(itemDataclassDict.ContainsKey(id))
        {
            return itemDataclassDict[id];
        }
        UnityEngine.Debug.LogError($"{id}의 아이템이 없습니다 CSV를 확인해주세요");
        return null;
    }

}