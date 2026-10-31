using System;
using System.Collections.Generic;
using UnityEngine;

public enum eEndingType
{
    Bad,Normal,True,Hidden
}

[Serializable]
public class EndingData
{
    public string key;
    public string title; //유니티 로컬라이제이션에 대응하는 타이틀 키
    public string description; //엔딩 설명 키
    public eEndingType endingType;
}
[CreateAssetMenu(menuName = "Ending/EndingDatabase")]
public class EndingDatabaseSO : ScriptableObject
{
    [field: SerializeField] public List<EndingData> endingDataList { get; private set; }
    private Dictionary<string, EndingData> endingDataDict;

    public EndingData GetEndingData(string key)
    {
        if(endingDataDict == null)
        {
            endingDataDict = new Dictionary<string, EndingData>();
            foreach(EndingData item in endingDataList)
            {
                if(endingDataDict.ContainsKey(item.key))
                {
                    Debug.LogError($"{item.key}값이 중복됩니다 엔딩SO확인해주세요");
                    continue;
                }
                endingDataDict.Add(item.key, item);
            }
        }

        if(!endingDataDict.ContainsKey(key))
        {
            Debug.LogError($"{key}값이 엔딩SO에 존재하지않음");
            return null;
        }
        return endingDataDict[key];
    }


}
