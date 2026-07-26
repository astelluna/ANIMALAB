using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField] CharacterDataSO[] characterDataSOArray;

    private Dictionary<string, CharacterDataSO> characterDataDict = new Dictionary<string, CharacterDataSO>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Init();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    private void Init()
    {
        for(int i=0; i< characterDataSOArray.Length; i++)
        {
            string id = characterDataSOArray[i].CharacterId;
            if (!characterDataDict.ContainsKey(id))
            {
                characterDataDict.Add(id, characterDataSOArray[i]);
            }
            else
            {
                Debug.LogError($"캐릭터 ID가 중복됩니다. 확인해주세요 {i+1}번째");
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
        return so;
    }
}
