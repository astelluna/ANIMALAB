using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class ResourceManager : Singleton<ResourceManager>
{
    [field:SerializeField] public CharacterDataSO[] CharacterDataSOArray { get; private set; }
    [SerializeField] CharacterDataSO basicCharacterData;

    [SerializeField] ChapterThumbnailDatabaseSO chapterThumbnailDatabaseSO;

    [SerializeField] ItemDataSO itemDataSO;

    [SerializeField] CGDatabaseSO cgDatabaseSO;

    [SerializeField] EndingDatabaseSO endingDatabaseSO;


    private Dictionary<string, CharacterDataSO> characterDataDict = new Dictionary<string, CharacterDataSO>();
    private Dictionary<string, ChapterThumbnail> chapterThumbailDict = new Dictionary<string, ChapterThumbnail>();

    //로컬라이제이션
    private StringTable dialogTable;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        //다음 원하는 코드 작성
        Init();
        
    }

    private void Init()
    {
        for(int i=0; i< CharacterDataSOArray.Length; i++)
        {
            string id = CharacterDataSOArray[i].CharacterId;
            if (!characterDataDict.ContainsKey(id))
            {
                characterDataDict.Add(id, CharacterDataSOArray[i]);
            }
            else
            {
                Debug.LogError($"캐릭터 ID가 중복됩니다. 확인해주세요 {i+1}번째");
            }
        }
        //챕터 데이터
        List<ChapterThumbnail> thumbnailList = chapterThumbnailDatabaseSO.chapterThumbnails;
        for(int i=0; i< thumbnailList.Count; i++)
        {
            string key = thumbnailList[i].key;
            if(!chapterThumbailDict.ContainsKey(key))
            {
                chapterThumbailDict.Add(key, thumbnailList[i]);
            }
            else
            {
                Debug.LogError($"섬네일 KEY가 중복됩니다. 확인해주세요 {i + 1}번째");
            }
        }
        //로컬라이제이션
        dialogTable = LocalizationSettings.StringDatabase.GetTableAsync(PathManager.dialogTableName).WaitForCompletion();
    }
    public string GetLocalizationText(string key)
    {
        string trimKey = key.Trim();
        if(trimKey != key)
        {
            Debug.LogWarning($"{key}key값에 공백이 있어서 자동 삭제했습니다 SO확인만 해주세요");
        }
        var entry = dialogTable.GetEntry(trimKey);

        if(entry == null)
        {
            Debug.LogError($"{key}값 로컬라이제이션 못불러옴");
        }

        return entry != null ? entry.GetLocalizedString() : key;
    }
    public CharacterDataSO GetCharacterData(string id)
    {
        CharacterDataSO so = null;
        if(characterDataDict.ContainsKey(id))
        {
            so = characterDataDict[id];
        }
        else
        {
            so = basicCharacterData;
        }
        return so;
    }
    public ChapterThumbnail GetChapterThumnail(string key)
    {
        if(!chapterThumbailDict.ContainsKey(key))
        {
            Debug.LogError("썸네일 키를 못찾음. CSV와 SO를 확인해주세요");
            return null;
        }
        return chapterThumbailDict[key];
    }

    public ItemDataClass GetItemData(string id)
    {
        return itemDataSO.GetItemData(id);
    }
    public CGData GetCGData(string id)
    {
        return cgDatabaseSO.GetCGData(id);
    }
    public EndingData GetEndingData(string key)
    {
        return endingDatabaseSO.GetEndingData(key);
    }
    public List<CGData> GetCGDataList(eCgCategory type)
    {
        return cgDatabaseSO.GetCGDataByCategory(type);
    }
}
