using System;
using UnityEngine;

public class DialogLogSpawner : MonoBehaviour
{
    public static DialogLogSpawner Instance { get; private set; }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    //오브젝트풀 프리팹,사이즈 , 선언
    [SerializeField] DialogLogUI _dialogLogPrefab;
    [SerializeField] int poolSize = 10;
    ObjPool<DialogLogUI> _dialogLogPool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _dialogLogPool = new ObjPool<DialogLogUI>(_dialogLogPrefab, poolSize, gameObject.transform);
    }
    //생성 코드
    public DialogLogUI GetPrefabByPool()
    {
        DialogLogUI logBuf = _dialogLogPool.GetObject();
        //logBuf.transform.position = trf.position;
        return logBuf;
    }
    //오브젝트 반환
    public void ReturnPrefabToPool(DialogLogUI logPrefab)
    {
        _dialogLogPool.ReturnObject(logPrefab);
    }
}