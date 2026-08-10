using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    [SerializeField] public CharacterDataSO[] CharacterDataSOArray { get; private set; }
    [SerializeField] CharacterDataSO basicCharacterData;

    [SerializeField] ChapterThumbnailDatabaseSO chapterThumbnailDatabaseSO;

    private Dictionary<string, CharacterDataSO> characterDataDict = new Dictionary<string, CharacterDataSO>();
    private Dictionary<string, ChapterThumbnail> chapterThumbailDict = new Dictionary<string, ChapterThumbnail>();

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
}
