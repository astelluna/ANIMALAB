using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CGData 
{
    public string key;
    public Sprite image;
    public eCgCategory category;
}
[CreateAssetMenu(menuName = "CG/CGDatabaseSO")]
public class CGDatabaseSO : ScriptableObject
{
    public List<CGData> backgroundDatas;

    Dictionary<string, CGData> backgroundDataDict = new Dictionary<string, CGData>();

    public Sprite GetSprite(string key)
    {
        if(backgroundDataDict.Count == 0 && backgroundDatas.Count != 0)
        {
            foreach (var k in backgroundDatas)
            {
                if (!backgroundDataDict.ContainsKey(k.key))
                {
                    backgroundDataDict.Add(k.key, k);
                }
            }
        }
        //반환해준다
        if (backgroundDataDict.ContainsKey(key))
        {
            return backgroundDataDict[key].image;
        }

        Debug.LogError("요청한 배경에 맞는 SO 데이터가 없습니다");
        return null;

    }
    public CGData GetCGData(string key)
    {
        if (backgroundDataDict.Count == 0 && backgroundDatas.Count != 0)
        {
            foreach (var k in backgroundDatas)
            {
                if (!backgroundDataDict.ContainsKey(k.key))
                {
                    backgroundDataDict.Add(k.key, k);
                }
            }
        }
        //반환해준다
        if (backgroundDataDict.ContainsKey(key))
        {
            return backgroundDataDict[key];
        }

        Debug.LogError("요청한 배경에 맞는 SO 데이터가 없습니다");
        return null;
    }
    public List<CGData> GetCGDataByCategory(eCgCategory type)
    {
        List<CGData> cgDataList = new List<CGData>();
        foreach(var item in backgroundDatas)
        {
            if(item.category == type)
            {
                cgDataList.Add(item);
            }
        }
        return cgDataList;
    }
}
