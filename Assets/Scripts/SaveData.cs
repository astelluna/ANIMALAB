using System.Collections.Generic;
using System;
using System.Diagnostics;


[Serializable]
public class SaveItem
{
    public string key;
    public int value;
}
[Serializable]
public class SaveFlag
{
    public string key;
    public int value;
}
[Serializable]
public class AffinityData
{
    public string id;
    public int amount;
    public int maxAmount;
    public string characterForm;

    public eCharacterForm CharacterForm
    {
        get
        {
            if (Enum.TryParse<eCharacterForm>(characterForm, true, out var result))
            {
                return result;
            }
            return eCharacterForm.none;
        }
    }
    public void AddAmount(int value)
    {
        amount += value;
        if (amount > maxAmount) { amount = maxAmount; }
        if (amount < 0) { amount = 0; }
    }
}
[Serializable]
public class SaveData
{
    public int curChapterId;
    public int nodeId;

    //스킵기능을 위해 읽었던 노드id 기록
    public List<int> readNodeIds;

    public List<AffinityData> saveAffinities = new List<AffinityData>();
    public List<SaveItem> saveItems = new List<SaveItem>();
    public List<SaveData> saveDatas = new List<SaveData>();

    //호감도 수치(검색용. 직렬화 안함)
    [NonSerialized]
    private Dictionary<string, AffinityData> affinityDict = new Dictionary<string, AffinityData>(); 
    public IReadOnlyDictionary<string, AffinityData> AffinityDict => affinityDict;

    //Dictionary<string, int> affinityDict = new Dictionary<string, int>(); //캐릭터id, 호감도수치
    //Dictionary<string, int> itemDict = new Dictionary<string, int>(); //아이템id, 갯수
    //Dictionary<string, int> flagDict = new Dictionary<string, int>(); //뭔가 특별한 이벤트가 진행해야할때? 아직안쓸듯
    public void SetAffinityData(CharacterDataSO[] dataSOArray)
    {
        affinityDict.Clear();
        saveAffinities.Clear();
        foreach (CharacterDataSO characterData in dataSOArray)
        {
            if(affinityDict.ContainsKey(characterData.CharacterId))
            {
                continue;
            }
            AffinityData data = new AffinityData();
            data.id = characterData.CharacterId;
            data.amount = characterData.InitialAffinity;
            data.maxAmount = characterData.MaxAffinity;
            data.characterForm = eCharacterForm.beast1.ToString();
            affinityDict.Add(characterData.CharacterId, data);
            saveAffinities.Add(data);
        }
    }
    public AffinityData GetAffinityData(string key)
    {
        if (!affinityDict.ContainsKey(key))
        {
            return null;
        }
        return affinityDict[key];
    }
}

