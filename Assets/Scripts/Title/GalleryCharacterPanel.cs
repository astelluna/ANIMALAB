using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GalleryCharacterPanel : MonoBehaviour
{
    [SerializeField] Transform buttonMakePos;

    //캐릭터 갯수만큼 생성되는 프리팹
    [SerializeField] GalleryCharacterButton galleryCharacterButtonPrefab;

    List<GalleryCharacterButton> galleryCharacterButtonList = new List<GalleryCharacterButton>();

    //현재 선택한 캐릭터의 데이터를 가지고있게된다.
    CharacterDataSO curCharacterData;
    //오른쪽에 표시되는 이미지 및 버튼으로 이미지 변경해가야함
    int curIndex = 0;
    [SerializeField] Image showImage;
    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;

    private static readonly eCharacterForm[] forms = Enum.GetValues(typeof(eCharacterForm))
    .Cast<eCharacterForm>()
    .Where(v => v != eCharacterForm.none)
    .OrderBy(v => (int)v)
    .ToArray();
    private void Awake()
    {
        prevButton.onClick.AddListener(() => MoveDataToImage(-1));
        nextButton.onClick.AddListener(() => MoveDataToImage(1));
    }
    private void OnDisable()
    {
        curCharacterData = null;
    }
    private void OnDestroy()
    {
        prevButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();
        foreach(var item in galleryCharacterButtonList)
        {
            item.OnButtonClick -= ShowCharacterImage;
        }
        
    }
    
    private void MoveDataToImage(int amount)
    {
        curIndex += amount;
        curIndex = Mathf.Clamp(curIndex, 0, forms.Length - 1);
        UpdateCurrentFormDisplay();
    }
    

    private void ShowCharacterImage(CharacterDataSO data)
    {
        curCharacterData = data;
        //기본 이미지를 처음에는 표시해주자.
        curIndex = Array.IndexOf(forms, eCharacterForm.beast1);
        UpdateCurrentFormDisplay();
    }
    private void UpdateCurrentFormDisplay()
    {
        prevButton.interactable = curIndex > 0;
        nextButton.interactable = curIndex < forms.Length - 1;

        eCharacterForm form = forms[curIndex];
        showImage.sprite = curCharacterData.GetExpression(form, eExpression.normal);
    }


    public void SetCharacterData(CharacterDataSO[] characterDataArray)
    {
        int count = characterDataArray.Length;

        while(count > galleryCharacterButtonList.Count)
        {
            GalleryCharacterButton characterButton = Instantiate(galleryCharacterButtonPrefab, buttonMakePos);
            characterButton.OnButtonClick += ShowCharacterImage;
            galleryCharacterButtonList.Add(characterButton);
        }
        //일단 모두 비활성화해야함
        foreach(var item in galleryCharacterButtonList)
        {
            item.gameObject.SetActive(false);
        }
        //활성화해주면서 데이터 설정해준다
        for(int i=0; i< count; i++)
        {
            CharacterDataSO characterData = characterDataArray[i];
            GalleryCharacterButton buttonData = galleryCharacterButtonList[i];
            buttonData.ActiveAndSetData(characterData);
        }

    }
}
