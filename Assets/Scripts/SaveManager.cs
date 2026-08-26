using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    string savePath;
    string endingSavePath;

    public SaveData CurrentSaveData { get; private set; }
    public GallerySaveData CurrentGallerySaveData { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        //다음 원하는 코드 작성
        savePath = Application.persistentDataPath + PathManager.saveFileName;
        endingSavePath = Application.persistentDataPath + PathManager.endingSaveFileName;
    }
    private void Start()
    {
        LoadSaveDataToJsonFile();
        LoadGallerySaveDataToJsonFile();
    }
    #region saveData전용
    public SaveData CreateNewSaveFile()
    {
        SaveData saveData = new SaveData();
        saveData.curChapterId = 1;
        saveData.nodeId = 1;
        //예외: 스킵을 위해 읽었던 노드목록은 초기화하면 안됨
        //SaveData prevData = LoadSaveFile(); //기존 세이브파일 읽어서 반환해야함.
        saveData.readNodeIds = (CurrentSaveData != null) ? CurrentSaveData.readNodeIds : new List<int>();

        CheckAndFixAffinityData(saveData);

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        CurrentSaveData = saveData;
        return saveData;
    }
    public void CheckAndFixAffinityData(SaveData saveData)
    {
        //Affinity가 save파일에 정보가 있으면 딕셔너리를 채워줌. 없다면 리소스 매니저에서 리스트와 딕셔너리 채운다
        saveData.Init();
        if(saveData.AffinityDict.Count == 0)
        {
            saveData.SetAffinityData(ResourceManager.Instance.CharacterDataSOArray);
        }
    }

    public void CreateSaveData(SaveData data, bool isChapterEnd = false)
    {
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
        //현재 화면에 캐릭터 누가 표시되고있는지도 넣기
        if(CharacterUISlotManager.Instance != null)
        {
            List<CharacterSprite> list =  CharacterUISlotManager.Instance.GetCharacterShowData();
            data.SetCharacterSpriteData(new List<CharacterSprite>(list));
        }
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        CurrentSaveData = data;
        CheckAndFixAffinityData(CurrentSaveData);
    }

    public void LoadSaveDataToJsonFile()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("SaveFile이 없어서 새로 생성합니다");
            CreateNewSaveFile();
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
            Debug.LogWarning("CurrentSaveData가 null이라 새로 생성합니다");
            CreateNewSaveFile();
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
    public void SetCurBackgroundKey(string key)
    {
        CurrentSaveData.SetCurBackgroundKey(key);
    }
    public Sprite GetCurBackgroundSprite()
    {
        return CurrentSaveData.GetBackground();
    }
    public List<CharacterSprite> GetSpriteDataList()
    {
        return CurrentSaveData.GetCharacterSpriteData();
    }
    public bool TryAddReward(int node)
    {
        return CurrentSaveData.TryAddRewardNode(node);
    }
    #endregion
    #region GallerySave전용
    public void LoadGallerySaveDataToJsonFile()
    {
        if(!File.Exists(endingSavePath))
        {
            Debug.LogWarning("EndingSaveFile이 없어서 새로 생성합니다");
            CurrentGallerySaveData = new GallerySaveData();
            return;
        }
        string json = File.ReadAllText(endingSavePath);
        CurrentGallerySaveData = JsonUtility.FromJson<GallerySaveData>(json);

        if (CurrentGallerySaveData == null) // 혹시 파싱 실패 대비
        {
            CurrentGallerySaveData = new GallerySaveData();
        }
    }
    public GallerySaveData LoadGallerySaveFile()
    {
        if (CurrentGallerySaveData == null)
        {
            Debug.LogError("GallerySaveData null 반환될거임 처리 필요합니다");
        }

        return CurrentGallerySaveData;
    }
    public void UnlockGalleryData(string key, eCgCategory category)
    {
        if(CurrentGallerySaveData == null)
        {
            CurrentGallerySaveData = new GallerySaveData();
        }

        if(category == eCgCategory.cg)
        {
            CurrentGallerySaveData.AddCg(key);
        }
        else if(category == eCgCategory.Ending)
        {
            CurrentGallerySaveData.AddEnding(key);
        }
        string json = JsonUtility.ToJson(CurrentGallerySaveData, true);
        File.WriteAllText(endingSavePath, json);
    }
    public bool HasSeenEnding(string key, eCgCategory category)
    {
        if (CurrentGallerySaveData == null) return false;
        bool hasSeen = false;

        if(category == eCgCategory.cg)
        {
            hasSeen= CurrentGallerySaveData.FindCgExist(key);
        }
        else if(category == eCgCategory.Ending)
        {
            hasSeen = CurrentGallerySaveData.FindEndingExist(key);
        }

        return hasSeen;
    }
    #endregion
}
