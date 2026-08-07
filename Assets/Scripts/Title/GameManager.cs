using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int startNodeId { get; private set; } = 1;
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        //다음 원하는 코드 작성
    }
    private void SetStartNodeID(int id)
    {
        startNodeId = id;
    }
    public void GameStart(SaveData data)
    {
        if(data == null)
        {
            Debug.LogError("세이브 데이터가 null입니다.");
            return;
        }
        SetStartNodeID(data.nodeId);
        SceneChangeManager.Instance.LoadSceneWithFade(PathManager.InGameSceneName);
    }
}
