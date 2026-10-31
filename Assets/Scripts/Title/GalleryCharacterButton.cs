using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GalleryCharacterButton : MonoBehaviour
{
    //여기는 생성되기전에 캐릭터 정보를 가지고있고, 버튼 크릭되면 그 캐릭터 정보를 반환해주자;;
    private CharacterDataSO characterDataSO;
    public event Action<CharacterDataSO> OnButtonClick;

    [SerializeField] Button characterButton;
    [SerializeField] TextMeshProUGUI buttonText;

    private void Awake()
    {
        characterButton.onClick.AddListener(ButtonClicked);
    }
    private void OnDestroy()
    {
        characterButton.onClick.RemoveAllListeners();
    }

    private void ButtonClicked()
    {
        OnButtonClick?.Invoke(characterDataSO);
    }

    public void ActiveAndSetData(CharacterDataSO data)
    {
        this.gameObject.SetActive(true);
        characterDataSO = data;
        buttonText.text = characterDataSO.CharacterId;
    }
}
