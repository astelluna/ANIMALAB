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
    public void SetStartNodeID(int id)
    {
        startNodeId = id;
    }
}
