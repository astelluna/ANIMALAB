using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;


[RequireComponent(typeof(PlayerInput))]
public class DialogManager : MonoBehaviour
{
    public TMP_Text characterText;
    public TMP_Text dialogText;
    public Button[] choiceButtons;

    DialogData[] allNodes;
    int curId = 0;
    DialogData curNode;

    PlayerInput playerInput; //컴포넌트를 가져오기 위해
    InputActionMap mainActionMap; //플레이어인풋 컴포넌트의 수많은 기능중 인풋액션맵
    InputAction clickAction;     //액션 하나하나

    bool isTyping = false;
    Coroutine printSlowTextCor;
    private void Func()
    {
        //예외처리. 만약 타이핑중이라면? 코루틴 멈추고 바로 표시되도록 하고 return 처리하자
        if(isTyping)
        {
            isTyping = false;
            StopCoroutine(printSlowTextCor);
            printSlowTextCor = null;
            dialogText.text = curNode.text;
            return;
        }
        //예외처리2. 선택지가 있다면? 하면안됨
        if(curNode.choices.Length > 0)
        {
            return;
        }

        if (curNode.nextnodeId == 0) { curId++; }
        else { curId = curNode.nextnodeId; }
        ShowData(curId);
    }

    //해야할것. 어떤 메세지가 존재한다. 이거를 이제 표시를 해보자
    IEnumerator PrintSlowText(string str)
    {
        dialogText.text = "";
        isTyping = true;
        int index = 0;
        while (index < str.Length)
        {
            dialogText.text = dialogText.text + str[index];
            index++;
            yield return new WaitForSeconds(0.1f);
        }
        isTyping = false;
    }
    public bool HasChoices()
    {
        if(curNode.choices.Length > 0)
        {
            return true;
        }
        return false;
    }

    void InputInit()
    {
        playerInput = GetComponent<PlayerInput>();
        mainActionMap = playerInput.actions.FindActionMap("Player");
        clickAction = mainActionMap.FindAction("Click");
        clickAction.performed += (ctx) => MouseLButtonClick(ctx);
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }




    void MouseLButtonClick(CallbackContext ctx)
    {
        // 마우스가 UI(버튼 등) 위에 있으면 전역 클릭 무시
        if (IsPointerOverUI()) { Debug.Log("버튼위에ㅓㅅ 클릭"); return; }
        if (HasChoices()) { return; }
        Func();
    }
    void JsonInit()
    {
        string path = PathManager.jsonFilePath;
        string resourcePath = path
    .Replace("Assets/Resources/", "")
    .Replace(".json", "");
        TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);

        if (jsonFile == null)
        {
            Debug.LogError("StoryDialog2.json을 Resources 폴더에서 못 찾음. 경로 확인 필요.");
            return;
        }
        string json = jsonFile.text;
        Debug.Log(json);
        allNodes = JsonUtility.FromJson<DialogDataArray>(json).nodes;
    }

    private void Start()
    {
        //입력과 json파일 초기화
        InputInit();
        JsonInit();

        curId = 1;
        ShowData(curId);
    }

    void ShowData(int id)
    {
        curId = id;
        curNode = System.Array.Find(allNodes, n => n.id == id);
        //node의 string 정보를 캐릭터 정보로 바꿔야함.
        CharacterDataSO characterData = GetPeopleSOData(curNode.speaker, curNode.Expression);

        characterText.text = curNode.speaker;
        //버튼 표시
        foreach (var btn in choiceButtons)
        {
            btn.gameObject.SetActive(false);
        }
        //선택지 표시
        for (int i = 0; i < curNode.choices.Length; i++)
        {
            int goTo = curNode.choices[i].goTo;
            choiceButtons[i].gameObject.GetComponentInChildren<TMP_Text>().text = curNode.choices[i].choiceText;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => ShowData(goTo));
            choiceButtons[i].gameObject.SetActive(true);
        }

        //코루틴 다시 실행
        if(printSlowTextCor != null)
        {
            StopCoroutine(printSlowTextCor);
        }
        printSlowTextCor = StartCoroutine(PrintSlowText(curNode.text));
        //TODO
        //캐릭터의 초상화, Sprite 이미지 등 띄워야 한다
    }
    CharacterDataSO GetPeopleSOData(string name, eEmotion emotionType)
    {
        CharacterDataSO characterData = ResourceManager.Instance.GetCharacterData(name);
        if(characterData == null)
        {
            Debug.LogError($"{name}의 캐릭터SO 데이터가 없습니다. 에러");
            return null;
        }
        return characterData;
    }

}
