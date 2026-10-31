using System;
using System.Collections.Generic;

[Serializable]
public class ItemClassJson
{
    public string key;
    public string explain;
}

[Serializable]
public class ItemDataJson 
{
    public List<ItemClassJson> itemClassList;

    private Dictionary<string, ItemClassJson> itemClassDict = new Dictionary<string, ItemClassJson>();
    //public IReadOnlyDictionary<string, ItemClass> ItemClassDict => itemClassDict;

    public ItemClassJson GetItemInfo(string id)
    {
        //초기 1회 캐싱
        if(itemClassList != null && itemClassList.Count > 0 && itemClassDict.Count == 0)
        {
            foreach(ItemClassJson item in itemClassList)
            {
                if(itemClassDict.ContainsKey(item.key))
                {
                    UnityEngine.Debug.LogError($"{item.key} 아이템키값이 중복됨 csv 확인해주세요");
                    continue;
                }
                itemClassDict.Add(item.key, item);
            }
        }
        //딕셔너리에서 검색을 해서 반환
        if(itemClassDict.ContainsKey(id))
        {
            return itemClassDict[id];
        }
        UnityEngine.Debug.LogError($"{id}값이 csv테이블에 존재하지않습니다 csv확인해주세요");
        return null;
    }

}
