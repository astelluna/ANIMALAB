using System.IO;
using UnityEngine;
using UnityEngine.XR;

public class SaveManager : Singleton<SaveManager>
{
    string path;
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        //다음 원하는 코드 작성
        path = Application.persistentDataPath + PathManager.saveFileName;
    }

    public void CreateNewSaveFile()
    {
        SaveData saveData = new SaveData();
        saveData.curChapterId = 1;
        saveData.nodeId = 1;
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(path, json);
    }

    public void CreateSaveData(int chapterid, int nodeId)
    {
        SaveData saveData = new SaveData();
        saveData.curChapterId = chapterid;
        saveData.nodeId = nodeId;
        //TODO,   호감도 및 아이템은 아직 모르겠다. 
        //return saveData;
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(path, json);
    }
    public SaveData LoadSaveFile()
    {
        if (!File.Exists(path))
        {
            Debug.LogError("SaveFile이 없어서 못불러왔습니다");
            return null;
        }
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }
}
