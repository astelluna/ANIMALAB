using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardIconEntry
{
    public eDialogItemType type;
    public string key;
    public Sprite icon;
}
[CreateAssetMenu(fileName = "RewardIconSO", menuName = "Dialogue/RewardIconSO")]
public class RewardIconSO : ScriptableObject
{
    public List<RewardIconEntry> rewardDataList;
    private Dictionary<string, Sprite> iconSpriteDict;

    public Sprite GetIcon(eDialogItemType type, string key)
    {
        if (iconSpriteDict == null) { Init(); }
        string dictKey = MakeKey(type, key);
        if (!iconSpriteDict.ContainsKey(dictKey)) { return null; }
        return iconSpriteDict[dictKey];
    }
    void Init()
    {
        iconSpriteDict = new Dictionary<string, Sprite>();
        foreach(var item in rewardDataList)
        {
            string key = MakeKey(item.type, item.key);
            iconSpriteDict.Add(key, item.icon);
        }
    }
    private string MakeKey(eDialogItemType type, string key)
    {
        return type + "_" + key;
    }
}
