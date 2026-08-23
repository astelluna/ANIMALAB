using System.Collections.Generic;
using System;
using UnityEngine;


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
public class CGSaveData
{
    public string id;

}

[Serializable]
public class ReadDataSet
{
    [SerializeField] List<string> readList = new List<string>();
    HashSet<string> readHash;

    public HashSet<string> GetReadData()
    {
        Init();
        return readHash;
    }
    public bool FindExist(string key)
    {
        Init();
        if (readHash.Contains(key))
        {
            return true;
        }
        return false;

    }
    public void Add(string key)
    {
        Init();
        if (readHash.Contains(key))
        {
            return; // 이미 있으면 중복 추가 안 함
        }

        readList.Add(key);
        readHash.Add(key);
    }
    private void Init()
    {
        if (readHash == null)
        {
            readHash = new HashSet<string>();
            foreach (var item in readList)
            {
                if (!readHash.Contains(item))
                {
                    readHash.Add(item);
                }
            }
        }
    }
}
[Serializable]
public class GallerySaveData
{
    [SerializeField] ReadDataSet cg = new ReadDataSet();
    [SerializeField] ReadDataSet ending = new ReadDataSet();

    public HashSet<string> GetReadEndingData() => ending.GetReadData();
    public bool FindEndingExist(string key) => ending.FindExist(key);
    public void AddEnding(string key) => ending.Add(key);

    public HashSet<string> GetReadCgData() => cg.GetReadData();
    public bool FindCgExist(string key) => cg.FindExist(key);
    public void AddCg(string key) => cg.Add(key);

}

[Serializable]
public class SaveData
{
    public int curChapterId;
    public int nodeId;

    [field: SerializeField] public string curBackgroundKey { get; private set; }

    //스킵기능을 위해 읽었던 노드id 기록
    public List<int> readNodeIds;

    
    

    //챕터 데이터 (n번째 챕터에서부터 다시하기를 위해서 사용한다)
    public List<ChapterSaveData> ChapterSaveDataList = new List<ChapterSaveData>();
    private Dictionary<int, ChapterSaveData> chapterSaveDataDict = new Dictionary<int, ChapterSaveData>();
    public IReadOnlyDictionary<int, ChapterSaveData> ChapterSaveDataDict => chapterSaveDataDict;

    //호감도 수치
    public List<AffinityData> saveAffinitieList = new List<AffinityData>();
    [NonSerialized]
    private Dictionary<string, AffinityData> affinityDict = new Dictionary<string, AffinityData>(); 
    public IReadOnlyDictionary<string, AffinityData> AffinityDict => affinityDict;
    //얻은 아이템 리스트
    public List<SaveItem> saveItemList = new List<SaveItem>();
    private Dictionary<string, SaveItem> saveItemDict = new Dictionary<string, SaveItem>();
    public IReadOnlyDictionary<string, SaveItem> SaveItemDict => saveItemDict;

    //보상을 받으면 보상을 받은 노드에 대해 중복 받지 않도록 해당 노드 ID 기록한다.
    public List<int> getRewardNodeList = new List<int>();
    private HashSet<int> getRewardNodeHash = new HashSet<int>();

    public void Init()
    {
        if (chapterSaveDataDict.Count == 0 && ChapterSaveDataList.Count != 0)
        {
            chapterSaveDataDict = new Dictionary<int, ChapterSaveData>();
            foreach(var item in ChapterSaveDataList)
            {
                if(!chapterSaveDataDict.ContainsKey(item.chapterId))
                {
                    chapterSaveDataDict.Add(item.chapterId, item);
                }
            }
        }
        if (affinityDict.Count == 0 && saveAffinitieList.Count != 0)
        {
            affinityDict = new Dictionary<string, AffinityData>();
            foreach (var item in saveAffinitieList)
            {
                if (!affinityDict.ContainsKey(item.id))
                {
                    affinityDict.Add(item.id, item);
                }
            }
        }
        if (saveItemDict.Count == 0 && saveItemList.Count != 0)
        {
            saveItemDict = new Dictionary<string, SaveItem>();
            foreach (var item in saveItemList)
            {
                if (!saveItemDict.ContainsKey(item.key))
                {
                    saveItemDict.Add(item.key, item);
                }
            }
        }
        if(getRewardNodeHash.Count == 0 && getRewardNodeList.Count != 0)
        {
            getRewardNodeHash = new HashSet<int>();
            foreach (var item in getRewardNodeList)
            {
                if (!getRewardNodeHash.Contains(item))
                {
                    getRewardNodeHash.Add(item);
                }
            }
        }
    }

    public bool TryAddRewardNode(int node)
    {
        //만약에 이미 노드가 있으면? 추가를 안함.
        //if(getRewardNodeList.Contains(node))
        if(getRewardNodeHash.Contains(node))
        {
            return false;
        }
        getRewardNodeList.Add(node);
        getRewardNodeHash.Add(node);
        return true;
    }

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
    //public void SetAffinityDict()
    //{
    //    affinityDict.Clear();
    //    foreach(AffinityData item in saveAffinitieList)
    //    {
    //        if(affinityDict.ContainsKey(item.id))
    //        {
    //            UnityEngine.Debug.LogError($"{item.id}가 중복된 키값이 있음. ");
    //            continue;
    //        }
    //        affinityDict.Add(item.id, item);
    //    }
    //}
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

    public void SetCurBackgroundKey(string key)
    {
        curBackgroundKey = key;
    }

    public Sprite GetBackground()
    {
        //키값으로 마지막 상태의 백그라운드 아이디를 넣는다.
        if(string.IsNullOrEmpty(curBackgroundKey))
        {
            return null;
        }
        return ResourceManager.Instance.GetCGOrBackgroundData(curBackgroundKey).image;
    }
}

