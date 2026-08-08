using System.Collections.Generic;
using System;

[Serializable]
public class SaveAffinity
{
    public string key;
    public int value;
}
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
public class SaveData
{
    public int curChapterId;
    public int nodeId;

    public List<SaveAffinity> saveAffinities = new List<SaveAffinity>();
    public List<SaveItem> saveItems = new List<SaveItem>();
    public List<SaveData> saveDatas = new List<SaveData>();

    //스킵기능을 위해 읽었던 노드id 기록
    public List<int> readNodeIds;

    //Dictionary<string, int> affinityDict = new Dictionary<string, int>(); //캐릭터id, 호감도수치
    //Dictionary<string, int> itemDict = new Dictionary<string, int>(); //아이템id, 갯수
    //Dictionary<string, int> flagDict = new Dictionary<string, int>(); //뭔가 특별한 이벤트가 진행해야할때? 아직안쓸듯

}
