using System;
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

    [SerializeField] CharacterUISlotManager characterPosManager;
    [SerializeField] RewardCanvas rewardCanvas;

    [SerializeField] ChapterShowPanel chapterShowPanel;

    //배경
    [SerializeField] BackgroundScript backgroundScript;
    [SerializeField] BackgroundDatabaseSO backgroundDataSO;

    private void Start()
    {
        //입력과 json파일 초기화
        InputInit();
        JsonInit();

        curId = 1;
        ShowData(curId);
    }

    private void HandleNext()
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

        foreach(var result in results)
        {
            if(result.gameObject.GetComponent<Button>() != null)
            {
                return true;
            }
        }
        return false;
    }

    void MouseLButtonClick(CallbackContext ctx)
    {
        // 마우스가 UI(버튼 등) 위에 있으면 전역 클릭 무시
        if (IsPointerOverUI()) { Debug.Log("버튼위에ㅓㅅ 클릭"); return; }
        if (HasChoices()) { return; }
        if (rewardCanvas.IsRewardPanelShow) { return; }
        if (chapterShowPanel.IsChapterPanelShow) { return; }
        HandleNext();
    }

    public static DialogData[] GetJsonData()
    {

        string path = PathManager.jsonFilePath;
        string resourcePath = path
    .Replace("Assets/Resources/", "")
    .Replace(".json", "");
        TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);

        if (jsonFile == null)
        {
            Debug.LogError("StoryDialog2.json을 Resources 폴더에서 못 찾음. 경로 확인 필요.");
            return null;
        }
        string json = jsonFile.text;
        Debug.Log(json);
        return JsonUtility.FromJson<DialogDataArray>(json).nodes;
        //allNodes = JsonUtility.FromJson<DialogDataArray>(json).nodes;
    }

    void JsonInit()
    {
        allNodes = GetJsonData();
    }

    void ShowData(int id)
    {
        curId = id;
        curNode = System.Array.Find(allNodes, n => n.id == id);
        //node의 string 정보를 캐릭터 정보로 바꿔야함.
        //CharacterDataSO characterData = GetPeopleSOData(curNode.speaker, curNode.Expression);

        
        //만약 챕터 정보가 있다면? 챕터 패널부터 실행을 시켜준다.
        if(curNode.chapterDatas != null && curNode.chapterDatas.chapterId > 0)
        {
            //콜백방식으로 패널 보여주기가 끝나면 대화 정보를 뿌려준다.
            ChapterData chapterData = curNode.chapterDatas;
            chapterShowPanel.SetChapterInfo(chapterData.chapterId, chapterData.title, () => 
            {
                ShowNodeContent();
            });
        }
        else
        {
            ShowNodeContent();
        }
        
        

        //이벤트가 있다면 실행시키자
    }
    void ShowNodeContent()
    {
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
        if (printSlowTextCor != null)
        {
            StopCoroutine(printSlowTextCor);
        }
        printSlowTextCor = StartCoroutine(PrintSlowText(curNode.text));

        //캐릭터 이미지 띄우기
        if (curNode.spriteDatas.Length > 0)
        {
            foreach (var spriteData in curNode.spriteDatas)
            {
                CharacterDataSO charData = ResourceManager.Instance.GetCharacterData(spriteData.characterId);
                switch (spriteData.Command)
                {
                    case eCommandType.Show:
                        //이제 해당 이미지에 표시를 해버리자.
                        Sprite curSprite = charData.GetExpression(spriteData.Expression);
                        if (curSprite == null) continue;
                        //말하고 있는 사람이면 투명도 1, 아니면 -1로 한다.
                        float alpha = 1;
                        if (spriteData.characterId != curNode.speaker)
                        {
                            alpha = 0.5f;
                        }
                        characterPosManager.ShowImage(charData.CharacterId, curSprite, spriteData.Position, alpha, charData.BodyHeight);
                        break;
                    case eCommandType.Hide:
                        if (!characterPosManager.HideImage(spriteData.characterId))
                        {
                            Debug.LogError("Sprite Hide 실패. 아이디가 없었습니다");
                        }
                        break;
                    case eCommandType.ClearAll:
                        //현재 존재하는 화면의 스프라이트 들을 전부 지우자
                        characterPosManager.HideImageAll();
                        break;
                    case eCommandType.Effect:
                        characterPosManager.ShowEffect(charData.CharacterId, spriteData.Effect);
                        break;
                    default:
                        Debug.LogError("스프라이트 처리할때 이상한 커맨드가 들어옴. 확인 필요" + spriteData.nodeId);
                        break;
                }
            }
        }
        //보상이 있다면? 보상 이미지 띄우기
        DialogReward[] dialogRewards = curNode.reward;
        if (dialogRewards.Length > 0)
        {
            rewardCanvas.Show(dialogRewards);
        }
        //소리가 있다면 재생시킨다.
        foreach (DialogSound sound in curNode.dialogSounds)
        {
            if (sound.Key == eSoundID.none)
            {
                Debug.LogError($"SoundID가 none입니다. CSV 데이터를 확인해주세요 nodeId = {curNode.id}");
            }
            SoundManager.Instance.Play(sound.Key);
        }
        //배경이 있다면? 표시시킨다
        if(curNode.cg.Length > 0)
        {
            foreach(DialogCG cgData in curNode.cg)
            {
                Sprite sprite = backgroundDataSO.GetSprite(cgData.key);
                backgroundScript.SetBackgroundImage(sprite);
            }
        }
    }

}
