using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JsonDataManager : Singleton<JsonDataManager>
{
    //다이얼로그 데이터들
    DialogData[] allNodes;
    static DialogData[] cashedNode;
    //아이템 데이터들
    ItemDataJson itemDataJson;
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        //다음 원하는 코드 작성
        JsonInit();
    }

    public static DialogData[] GetJsonData()
    {
        if (cashedNode != null) return cashedNode;

        string path = PathManager.storyJsonFilePath;
        string resourcePath = path
    .Replace("Assets/Resources/", "")
    .Replace(".json", "");
        TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);

        if (jsonFile == null)
        {
            Debug.LogError("StoryDialog2.json을 Resources 폴더에서 못 찾음. 경로 확인 필요.");
            return null;
        }
        string json = jsonFile.text;
        Debug.Log(json);
        cashedNode  = JsonUtility.FromJson<DialogDataArray>(json).nodes;
        return cashedNode;
    }

    private void GetItemDataToJson()
    {
        string path = PathManager.itemJsonFilePath;
        string resourcePath = path
    .Replace("Assets/Resources/", "")
    .Replace(".json", "");
        TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);

        if (jsonFile == null)
        {
            Debug.LogError("ItemData_1.json을 Resources 폴더에서 못 찾음. 경로 확인 필요.");
            return;
        }
        string json = jsonFile.text;
        Debug.Log(json);
        itemDataJson = JsonUtility.FromJson<ItemDataJson>(json);

    }

    void JsonInit()
    {
        allNodes = GetJsonData();
        GetItemDataToJson();
    }
    public DialogData[] GetDialogData()
    {
        return allNodes;
    }
    public ItemDataJson GetItemDataJson()
    {
        return itemDataJson;
    }
    public static List<ChapterData> GetChapterDataClass()
    {
        DialogData[] dataArray = GetJsonData();
        if (dataArray == null || dataArray.Length == 0)
        {
            Debug.LogError("노드가 이상해서 챕터 데이터 못만듬");
            return null;
        }
        List<ChapterData> chapterDataList = new List<ChapterData>();
        Dictionary<int, ChapterData> chapterDataDict = new Dictionary<int, ChapterData>();
        foreach(var item in dataArray)
        {
            if (item.chapterData.chapterId == 0) continue;
            if(chapterDataDict.ContainsKey(item.chapterData.chapterId))
            {
                Debug.LogError("챕터 아이디가 중복입니다 CSV테이블 데이터 확인해주세요");
                continue;
            }
            chapterDataDict.Add(item.chapterData.chapterId, item.chapterData);
        }
        chapterDataList = chapterDataDict.Values.OrderBy(x => x.chapterId).ToList();
        return chapterDataList;
    }
}
