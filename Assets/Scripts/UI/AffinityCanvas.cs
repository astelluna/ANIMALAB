using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AffinityCanvas : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] List<AffinityImage> affinityImageList = new List<AffinityImage>();
    

    private void Awake()
    {
        closeButton.onClick.AddListener(() => this.gameObject.SetActive(false));
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }
    private void OnEnable()
    {
        SetAffinityImageList();
    }

    public void SetAffinityImageList()
    {
        IReadOnlyDictionary<string, AffinityData> affinityDict = SaveManager.Instance.CurrentSaveData.AffinityDict;
        if (affinityImageList.Count != affinityDict.Count)
        {
            //이미지 갯수가 맞지 않으면 지금은 오류임
            Debug.LogError("이미지 갯수가 다릅니다. 확인해주세요");
        }
        int index = 0;
        foreach(AffinityData item in affinityDict.Values)
        {
            if (index < affinityImageList.Count)
            {
                CharacterDataSO characterData = ResourceManager.Instance.GetCharacterData(item.id);
                Sprite characterSprite = characterData.GetExpressionByAffinity(item.CharacterForm, item.amount);
                affinityImageList[index].SetAffinityImage(characterSprite, item.amount, item.maxAmount);
                index++;
            }
        }
    }

}
