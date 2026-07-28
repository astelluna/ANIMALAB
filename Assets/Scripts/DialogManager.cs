using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public TMP_Text characterText;
    public TMP_Text dialogText;
    public Button[] choiceButtons;

    DialogData[] allNodes;
    int curId = 0;

    private void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("StoryData/StoryData_1");

        if (jsonFile == null)
        {
            Debug.LogError("StoryDialog2.json을 Resources 폴더에서 못 찾음. 경로 확인 필요.");
            return;
        }
        string json = jsonFile.text;
        Debug.Log(json);
        allNodes = JsonUtility.FromJson<DialogDataArray>(json).nodes;
        ShowData(1);
    }
    void ShowData(int id)
    {
        curId = id;
        DialogData node = System.Array.Find(allNodes, n => n.id == id);
        //node의 string 정보를 캐릭터 정보로 바꿔야함.
        CharacterDataSO characterData = GetPeopleSOData(node.speaker, node.Expression);


        dialogText.text = node.text;
        characterText.text = characterData.DisplayName;
        //버튼 표시
        foreach (var btn in choiceButtons)
        {
            btn.gameObject.SetActive(false);
        }
        //선택지 표시
        for (int i = 0; i < node.choices.Length; i++)
        {
            int goTo = node.choices[i].goTo;
            choiceButtons[i].gameObject.GetComponentInChildren<TMP_Text>().text = node.choices[i].choiceText;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => ShowData(goTo));
            choiceButtons[i].gameObject.SetActive(true);
        }
        //TODO
        //캐릭터의 초상화, Sprite 이미지 등 띄워야 한다
    }
    CharacterDataSO GetPeopleSOData(string name, eEmotion emotionType)
    {
        //TODO
        CharacterDataSO characterData = ResourceManager.Instance.GetCharacterData(name);
        if(characterData == null)
        {
            Debug.LogError($"{name}의 캐릭터SO 데이터가 없습니다. 에러");
            return null;
        }
        return characterData;
    }

}
