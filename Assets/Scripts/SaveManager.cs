using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    string path;

    public SaveData CurrentSaveData { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        //다음 원하는 코드 작성
        path = Application.persistentDataPath + PathManager.saveFileName;
    }

    public SaveData CreateNewSaveFile()
    {
        SaveData saveData = new SaveData();
        saveData.curChapterId = 1;
        saveData.nodeId = 1;
        //예외: 스킵을 위해 읽었던 노드목록은 초기화하면 안됨
        SaveData prevData = LoadSaveFile();
        saveData.readNodeIds = (prevData != null) ? prevData.readNodeIds : new List<int>();

        ////
        CheckAndFixAffinityData(saveData);
        //saveData.SetAffinityData(ResourceManager.Instance.CharacterDataSOArray);
        ////

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(path, json);
        CurrentSaveData = saveData;
        return saveData;
    }
    public void CheckAndFixAffinityData(SaveData saveData)
    {
        //Affinity가 null이면 정보를 넣어준다. 초기정보로
        if(saveData.AffinityDict.Count == 0)
        {
            saveData.SetAffinityData(ResourceManager.Instance.CharacterDataSOArray);
        }
    }

    //public void CreateSaveData(int chapterid, int nodeId)
    public void CreateSaveData(SaveData data)
    {
        //TODO,   호감도 및 아이템은 아직 모르겠다. 
        //return saveData;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        CurrentSaveData = data;
        CheckAndFixAffinityData(CurrentSaveData);
    }
    public SaveData LoadSaveFile()
    {
        if (!File.Exists(path))
        {
            Debug.LogError("SaveFile이 없어서 못불러왔습니다");
            return null;
        }
        string json = File.ReadAllText(path);
        CurrentSaveData = JsonUtility.FromJson<SaveData>(json);
        CheckAndFixAffinityData(CurrentSaveData);
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
}
