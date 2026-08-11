using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BackgroundData 
{
    public string key;
    public Sprite image;
}
[CreateAssetMenu(menuName = "Background/BackgroundDatabaseSO")]
public class BackgroundDatabaseSO : ScriptableObject
{
    public List<BackgroundData> backgroundDatas;

    Dictionary<string, BackgroundData> backgroundDataDict = new Dictionary<string, BackgroundData>();

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

}
