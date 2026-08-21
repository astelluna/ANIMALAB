using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    string savePath;
    string endingSavePath;

    public SaveData CurrentSaveData { get; private set; }
    public EndingSaveData CurrentEndingSaveData { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        //다음 원하는 코드 작성
        savePath = Application.persistentDataPath + PathManager.saveFileName;
        endingSavePath = Application.persistentDataPath + PathManager.endingSaveFileName;

        LoadSaveDataToJsonFile();
        LoadEndingSaveDataToJsonFile();

    }
    #region saveData전용
    public SaveData CreateNewSaveFile()
    {
        SaveData saveData = new SaveData();
        saveData.curChapterId = 1;
        saveData.nodeId = 1;
        //예외: 스킵을 위해 읽었던 노드목록은 초기화하면 안됨
        SaveData prevData = LoadSaveFile(); //기존 세이브파일 읽어서 반환해야함.
        saveData.readNodeIds = (prevData != null) ? prevData.readNodeIds : new List<int>();

        CheckAndFixAffinityData(saveData);

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        CurrentSaveData = saveData;
        return saveData;
    }
    public void CheckAndFixAffinityData(SaveData saveData)
    {
        //Affinity가 save파일에 정보가 있으면 딕셔너리를 채워줌. 없다면 리소스 매니저에서 리스트와 딕셔너리 채운다
        if(saveData.saveAffinitieList.Count != 0 && saveData.AffinityDict.Count == 0)
        {
            saveData.SetAffinityDict();
        }
        else if(saveData.AffinityDict.Count == 0)
        {
            saveData.SetAffinityData(ResourceManager.Instance.CharacterDataSOArray);
        }
        //item
        saveData.SetChpaterData();
    }

    public void CreateSaveData(SaveData data, bool isChapterEnd = false)
    {
        //TODO,   호감도 및 아이템은 아직 모르겠다. 
        //챕터가 끝났을때 해당 챕터에 대해서 어떤 정보들을 가지고있었는지 저장
        if(isChapterEnd)
        {
            int chapterNo = data.curChapterId - 1; //챕터정보 저장하는 시점은 다음챕터로 넘어갔을때 기준이므로 -1해줌
            if(chapterNo > 0)
            {
                ChapterSaveData chapterSaveData = new ChapterSaveData();
                chapterSaveData.chapterId = chapterNo; //챕터정보 저장하는 시점은 다음챕터로 넘어갔을때 기준이므로 -1해줌
                chapterSaveData.saveItemList = new List<SaveItem>(data.saveItemList);
                chapterSaveData.saveAffinities = new List<AffinityData>(data.saveAffinitieList);
                data.ChapterDataSave(chapterSaveData);
            }
        }
        //현재 가지고있는 아이템도 넣어줘야함
        data.saveItemList = new List<SaveItem>(data.saveItemList);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        CurrentSaveData = data;
        CheckAndFixAffinityData(CurrentSaveData);
    }

    public void LoadSaveDataToJsonFile()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("SaveFile이 없어서 저장정보 설정은 안합니다");
            return;
        }
        string json = File.ReadAllText(savePath);
        CurrentSaveData = JsonUtility.FromJson<SaveData>(json);
        CheckAndFixAffinityData(CurrentSaveData);
        //return CurrentSaveData;
    }

    public SaveData LoadSaveFile()
    {
        if(CurrentSaveData ==null)
        {
            Debug.LogError("SaveData null 반환될거임 처리 필요합니다");
        }

        return CurrentSaveData;
    }

    public void AddAffinityData(string id, int amount)
    {
        if (CurrentSaveData.AffinityDict.ContainsKey(id))
        {
            AffinityData affinityData = CurrentSaveData.AffinityDict[id];
            affinityData.AddAmount(amount);
        }
    }
    public void AddItemData(SaveItem item)
    {
        CurrentSaveData.AddItemData(item);
    }
    public List<SaveItem> GetSaveItemList()
    {
        return CurrentSaveData.saveItemList;
    }
    #endregion
    #region EndingSave전용
    public void LoadEndingSaveDataToJsonFile()
    {
        if(!File.Exists(endingSavePath))
        {
            Debug.LogWarning("EndingSaveFile이 없어서 새로 생성합니다");
            CurrentEndingSaveData = new EndingSaveData();
            return;
        }
        string json = File.ReadAllText(endingSavePath);
        CurrentEndingSaveData = JsonUtility.FromJson<EndingSaveData>(json);

        if (CurrentEndingSaveData == null) // 혹시 파싱 실패 대비
        {
            CurrentEndingSaveData = new EndingSaveData();
        }
    }
    public void UnlockEnding(string endingKey)
    {
        if(CurrentEndingSaveData == null)
        {
            CurrentEndingSaveData = new EndingSaveData();
        }
        CurrentEndingSaveData.AddEnding(endingKey);
        string json = JsonUtility.ToJson(CurrentEndingSaveData, true);
        File.WriteAllText(endingSavePath, json);
    }
    public bool HasSeenEnding(string endingKey)
    {
        if (CurrentEndingSaveData == null) return false;
        return CurrentEndingSaveData.FindEndingExist(endingKey);
    }
    #endregion
}
