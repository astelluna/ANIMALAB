using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.Experimental.GraphView;
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

    const string ellipsisToken = "\uE000"; // 텍스트에 안 쓰이는 특수 문자 하나
    //어떤 문자열이 주어졌다고 가정하자. "안녕하세요. 오늘 날씨가 좋아요. 나가실건가요?";
    //이런 문자열이 마우스 클릭을 하면?. 하나씩 실행된다고 가정하자. 일단 string으로 하자. SB쓰지말고 하기.
    //추후에는 글자가 확 나타나는게 아닌 서서히 나타나도록. 클릭하면 바로 딱 출력되도록.
    Queue<string> stringQue = new Queue<string>();

    PlayerInput playerInput; //컴포넌트를 가져오기 위해
    InputActionMap mainActionMap; //플레이어인풋 컴포넌트의 수많은 기능중 인풋액션맵
    InputAction clickAction;     //액션 하나하나
    public void ShowStr(string str)
    {
        dialogText.text = "";
        string temp = Regex.Replace(str, @"\.{2,}", ellipsisToken);
        // 2. 마침표/느낌표/물음표 뒤에 공백이 올 때만 문장을 나눔
        string[] raw = Regex.Split(temp, @"(?<=[.!?])\s+");
        // 3. 치환했던 말줄임표 복원 + 빈 문자열 제거
        string[] sentences = raw
            .Select(s => s.Replace(ellipsisToken, "..."))
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();

        foreach(var item in sentences)
        {
            stringQue.Enqueue(item);
        }
    }
    private void Func()
    {
        if (stringQue.Count == 0)
        {
            //여기서 체크. 만약에 nextNode가 0이라면?++, 아니라면 해당 노드로 이동
            if(curNode.nextnodeId == 0)
            {
                curId++;
            }
            else
            {
                curId = curNode.nextnodeId;
            }
            ShowData(curId);
        }

        string buf = stringQue.Dequeue();
        dialogText.text = dialogText.text + " " + buf;
    }
    public bool CanMoveNextDialog()
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
        if (CanMoveNextDialog()) { return; }
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


        ShowStr(curNode.text); //테스트용
        //dialogText.text = node.text;
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
