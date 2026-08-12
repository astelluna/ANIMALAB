using System.Collections.Generic;
using System;
using System.Diagnostics;
using static UnityEditor.Progress;
using UnityEditor.Overlays;


[Serializable]
public class SaveItem
{
    public string key;
    public int amount;
    //public string explain;
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
public class ChapterSaveData
{
    //챕터 아이디, 호감도, 아이템, 이벤트 목록
    public int chapterId;
    public List<AffinityData> saveAffinities;
    public List<SaveItem> saveItemList;

}

[Serializable]
public class SaveData
{
    public int curChapterId;
    public int nodeId;

    //스킵기능을 위해 읽었던 노드id 기록
    public List<int> readNodeIds;

    public List<AffinityData> saveAffinitieList = new List<AffinityData>();
    public List<SaveItem> saveItemList = new List<SaveItem>();

    //챕터 데이터 (n번째 챕터에서부터 다시하기를 위해서 사용한다)
    public List<ChapterSaveData> ChapterSaveDataList = new List<ChapterSaveData>();
    private Dictionary<int, ChapterSaveData> chapterSaveDataDict = new Dictionary<int, ChapterSaveData>();
    public IReadOnlyDictionary<int, ChapterSaveData> ChapterSaveDataDict => chapterSaveDataDict;

    //호감도 수치(검색용. 직렬화 안함)
    [NonSerialized]
    private Dictionary<string, AffinityData> affinityDict = new Dictionary<string, AffinityData>(); 
    public IReadOnlyDictionary<string, AffinityData> AffinityDict => affinityDict; 
    private Dictionary<string, SaveItem> saveItemDict = new Dictionary<string, SaveItem>();
    public IReadOnlyDictionary<string, SaveItem> SaveItemDict => saveItemDict;

    //Dictionary<string, int> flagDict = new Dictionary<string, int>(); //뭔가 특별한 이벤트가 진행해야할때? 아직안쓸듯

    public void ChapterDataSave(ChapterSaveData data)
    {
        //챕터 시작 시에 저장기능이 활성화되므로 딕셔너리로 중복 체크를 해야함. (그냥 계속 저장되게 하자 상관없게 만들면됨)
        if(chapterSaveDataDict.ContainsKey(data.chapterId))
        {
            //UnityEngine.Debug.Log($"{data.chapterId}가 이미 있었습니다. 덮어쓰기함");
            chapterSaveDataDict[data.chapterId] = data;
            ChapterSaveData savedata = ChapterSaveDataList.Find(x => x.chapterId == data.chapterId);
            if (savedata != null) { ChapterSaveDataList.Remove(savedata); }
            ChapterSaveDataList.Add(data);
        }
        else
        {
            chapterSaveDataDict.Add(data.chapterId, data);
            ChapterSaveDataList.Add(data);
        }
            
    }
    public void ChapterDataDelete(ChapterSaveData data)
    {
        if (chapterSaveDataDict.ContainsKey(data.chapterId))
        {
            chapterSaveDataDict.Remove(data.chapterId);
            return;
        }
        UnityEngine.Debug.LogError($"{data.chapterId}가 없어서 못지움. ");
    }

    public void SetChpaterData()
    {
        chapterSaveDataDict.Clear();
        foreach (ChapterSaveData item in ChapterSaveDataList)
        {
            if (chapterSaveDataDict.ContainsKey(item.chapterId))
            {
                UnityEngine.Debug.LogError($"{item.chapterId}가 중복된 키값이 있음. ");
                continue;
            }
            chapterSaveDataDict.Add(item.chapterId, item);
        }
    }

    public void SetAffinityData(CharacterDataSO[] dataSOArray)
    {
        affinityDict.Clear();
        saveAffinitieList.Clear();
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
            saveAffinitieList.Add(data);
        }
    }
    public void SetAffinityDict()
    {
        affinityDict.Clear();
        foreach(AffinityData item in saveAffinitieList)
        {
            if(affinityDict.ContainsKey(item.id))
            {
                UnityEngine.Debug.LogError($"{item.id}가 중복된 키값이 있음. ");
                continue;
            }
            affinityDict.Add(item.id, item);
        }
    }
    public void AddItemData(SaveItem item)
    {
        if (SaveItemDict.ContainsKey(item.key))
        {
            //넣어줄때 딕셔너리랑 리스트랑 같은 주소값으로 넣어주기때문에 딕셔너리만 수정하면 된다
            saveItemDict[item.key].amount += item.amount;
        }
        else
        {
            saveItemDict.Add(item.key, item);
            saveItemList.Add(item);
        }
    }
}

