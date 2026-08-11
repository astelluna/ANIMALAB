using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    List<SaveItem> saveItemList = new List<SaveItem>();
    Dictionary<string, SaveItem> saveItemDict = new Dictionary<string, SaveItem>();

    public void SetInitItemInfo(SaveData saveData)
    {
        List<SaveItem> saveItemData = saveData.saveItemList;
        saveItemList = new List<SaveItem>(saveItemData);
        foreach(var item in saveItemList)
        {
            saveItemDict.Add(item.key, item);
        }
    }
    public void AddItemInfo(SaveItem item)
    {
        if(saveItemDict.ContainsKey(item.key))
        {
            saveItemDict[item.key].value += item.value;
        }
        else
        {
            saveItemList.Add(item);
            saveItemDict.Add(item.key, item);
        }
    }
    public List<SaveItem> GetItemInfo()
    {
        return saveItemList;
    }

}
