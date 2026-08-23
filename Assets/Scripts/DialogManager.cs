using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;


[RequireComponent(typeof(PlayerInput))]
public class DialogManager : MonoBehaviour
{
    [SerializeField] TMP_Text characterText;
    [SerializeField] TMP_Text dialogText;
    [SerializeField] Button[] choiceButtons;
    [SerializeField] Button saveButton;
    [SerializeField] Button skipButton;
    [SerializeField] Button openDialogLogButton;
    [SerializeField] Button openAffinityButton;
    [SerializeField] Button openItemSlotButton;

    private TMP_Text[] choiceButtonTextArray;

    DialogData[] allNodes;
    int curId = 0;
    DialogData curNode;
    ItemDataJson itemDataJson;

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
    //SerializeField] CGDatabaseSO backgroundDataSO; //리소스 매니저에서 가져와야한다.

    //챕터
    int curChapterNo = 0;

    //저장
    HashSet<int> readNodeHash = new HashSet<int>();

    //스킵
    float skipDelayTime = 0.2f;
    WaitForSeconds skipDelay;
    Coroutine skipCoroutine;

    //다이럴로그 로그창 전용
    [SerializeField] DialogLogPanel dialogLogCanvas;

    //호감도 창 전용
    [SerializeField] AffinityPanel affinityPanel;
    //아이템 창
    [SerializeField] ItemSlotPanel itemSlotPanel;

    //다국어(로컬라이제이션)
    private string curLocalizedText = ""; // 가져온 실제 텍스트를 저장
    private EndingData curEndingData = null; // 엔딩 데이터 존재하면 넣고, 게임 끝냄
    [SerializeField] EndingPanel endingPanel;
    private void Awake()
    {
        skipDelay = new WaitForSeconds(skipDelayTime);

        saveButton.onClick.AddListener(SaveButtonClick);
        skipButton.onClick.AddListener(SkipButtonClick);
        openDialogLogButton.onClick.AddListener(OpenDialogLogButtonClick);
        openAffinityButton.onClick.AddListener(OpenAffinityButtonClick);
        openItemSlotButton.onClick.AddListener(OpenItemSlotButtonClick);


        choiceButtonTextArray = new TMP_Text[choiceButtons.Length];
        for (int i=0; i< choiceButtons.Length; i++)
        {
            choiceButtonTextArray[i] = choiceButtons[i].GetComponentInChildren<TMP_Text>();
            if (choiceButtonTextArray[i] == null)
            {
                Debug.LogError($"choiceButtons[{i}]에 TMP_Text가 없습니다.");
            }
        }
    }
    private void OnDestroy()
    {
        saveButton.onClick.RemoveAllListeners();
        skipButton.onClick.RemoveAllListeners();
        openDialogLogButton.onClick.RemoveAllListeners();
        openAffinityButton.onClick.RemoveAllListeners();
        openItemSlotButton.onClick.RemoveAllListeners();
    }
    private void OpenItemSlotButtonClick()
    {
        itemSlotPanel.ActiveItemSlotCanvas(SaveManager.Instance.GetSaveItemList());
    }

    
    private void OnEnable()
    {
        LoadGame();
    }
    private void SkipButtonClick()
    {
        //스킵 행동. 쭉쭉 일단 while로 진행한다.
        if(skipCoroutine == null)
        {
            skipCoroutine = StartCoroutine(SkipCor());
        }
    }
    IEnumerator SkipCor()
    {
        while(true)
        {
            yield return skipDelay;
            if (!CanSkipAble())
            {
                skipCoroutine = null;
                yield break;
            }
            HandleNext();
        }
    }
    private bool CanSkipAble()
    {
        int nextNodeId = FindNextNodeId();
        DialogData nextNode = System.Array.Find(allNodes, n => n.id == nextNodeId);
        if (nextNode == null) return false;
        if (nextNode.choices.Length > 0 || nextNode.reward.Length > 0 || nextNode.chapterData.haveData() || !readNodeHash.Contains(nextNodeId))
        {
            return false;
        }
        return true;
    }
    private void StopSkipCor()
    {
        if (skipCoroutine != null)
        {
            StopCoroutine(skipCoroutine);
            skipCoroutine = null;
        }
    }
    private void OpenAffinityButtonClick()
    {
        affinityPanel.gameObject.SetActive(true);
    }
    private void OpenDialogLogButtonClick()
    {
        dialogLogCanvas.OpenPanel();
    }
    private void ClearDialogLog()
    {
        dialogLogCanvas.ClearLog();
    }
    private void SaveButtonClick()
    {
        CreateSaveData();
    }
    public SaveData LoadSaveFile()
    {
        return SaveManager.Instance.LoadSaveFile();
    }

    private void LoadGame()
    {
        SaveData saveData = LoadSaveFile();
        if(saveData == null)
        {
            readNodeHash = new HashSet<int>();
            return;
        }
        readNodeHash = new HashSet<int>(saveData.readNodeIds);
        //현재 배경 불러오기 (null이라면 처음 게임 시작할때이므로 아닐떄만 넣자)
        Sprite backgroundSprite = SaveManager.Instance.GetCurBackgroundSprite();
        if(backgroundSprite != null)
        {
            backgroundScript.SetBackgroundImage(SaveManager.Instance.GetCurBackgroundSprite());
        }
        
    }
    public void CreateSaveData(bool isChapterEnd = false)
    {
        SaveData saveData = SaveManager.Instance.LoadSaveFile() ?? new SaveData();
        saveData.curChapterId = curChapterNo;
        saveData.nodeId = curNode.id;
        saveData.readNodeIds = readNodeHash.ToList();

        //TODO,   호감도 및 아이템은 아직 모르겠다. 
        SaveManager.Instance.CreateSaveData(saveData, isChapterEnd);
    }

    private void Start()
    {
        //입력과 json파일 초기화
        Init();

        curId = GameManager.Instance.startNodeId;
        Debug.Log($"Ingame씬 -> 시작노드ID: {curId}");
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
            dialogText.text = curLocalizedText;
            return;
        }
        //예외처리2. 선택지가 있다면? 하면안됨
        if(curNode.choices.Length > 0)
        {
            return;
        }
        //만약 엔딩 정보가 존재한다면? 엔딩 패널을 표시하고. 끝낼 준비를 하자.
        if (curEndingData != null)
        {
            //엔딩 패널을 열어서 정보 표시할거임
            endingPanel.SetAndShowEndingPanel(curEndingData);
            SaveManager.Instance.UnlockGalleryData(curEndingData.key,eCgCategory.Ending);
            return;
        }

        curId = FindNextNodeId();
        ShowData(curId);
    }


    private int FindNextNodeId()
    {
        if (curNode.nextnodeId == 0) 
        {
            return curId + 1;
        }
        else 
        {
            return curNode.nextnodeId; 
        }
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

    IEnumerator LoadAndPrintText(string key)
    {
        curLocalizedText = ResourceManager.Instance.GetLocalizationText(key);
        yield return PrintSlowText(curLocalizedText);
    }
    public bool HasChoices()
    {
        if(curNode.choices.Length > 0)
        {
            return true;
        }
        return false;
    }
    void Init()
    {
        playerInput = GetComponent<PlayerInput>();
        mainActionMap = playerInput.actions.FindActionMap("Player");
        clickAction = mainActionMap.FindAction("Click");
        clickAction.performed += (ctx) => MouseLButtonClick(ctx);

        allNodes = JsonDataManager.Instance.GetDialogData();
        itemDataJson = JsonDataManager.Instance.GetItemDataJson();
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
        if (dialogLogCanvas.IsCanvasShow) { return; }
        if (affinityPanel.IsPanelShow) { return; }
        if (itemSlotPanel.IsPanelShow) { return; }
        if (endingPanel.IsPanelShow) { return; }
        StopSkipCor();
        HandleNext();
    }

    void ShowData(int id)
    {
        curId = id;
        curNode = System.Array.Find(allNodes, n => n.id == id);
        readNodeHash.Add(curId);

        //만약 엔딩 정보가 있다면? 엔딩 정보를 설정해주자.
        if(!string.IsNullOrEmpty(curNode.EndingKey))
        {
            curEndingData = ResourceManager.Instance.GetEndingData(curNode.EndingKey);
        }
        else
        {
            curEndingData = null;
        }
        //만약 챕터 정보가 있다면? 챕터 패널부터 실행을 시켜준다.
        if (curNode.chapterData != null && curNode.chapterData.chapterId > 0)
        {
            AdsManager.Instance.ShowInterstitial(StartNewChapter);
        }
        else
        {
            ShowNodeContent();
        }
    }
    void StartNewChapter()
    {
        curChapterNo = curNode.chapterData.chapterId;
        //콜백방식으로 패널 보여주기가 끝나면 대화 정보를 뿌려준다.
        ChapterData chapterData = curNode.chapterData;
        chapterShowPanel.SetChapterInfo(chapterData.chapterId, chapterData.title, () =>
        {
            ShowNodeContent();
        });

        //세이브를 시켜주자.
        CreateSaveData(true);
        ClearDialogLog();
    }

    void ShowNodeContent()
    {
        characterText.text = ResourceManager.Instance.GetLocalizationText(curNode.speaker);
        //버튼 표시
        foreach (var btn in choiceButtons)
        {
            btn.gameObject.SetActive(false);
        }
        //선택지 표시
        for (int i = 0; i < curNode.choices.Length; i++)
        {
            DialogChoice dialogChoice = curNode.choices[i];
            //choiceButtons[i].gameObject.GetComponentInChildren<TMP_Text>().text = dialogChoice.choiceText;
            //choiceButtonTextArray[i].text = dialogChoice.choiceText;

            //텍스트는 로컬라이제이션에서 불러와야함
            choiceButtonTextArray[i].text = ResourceManager.Instance.GetLocalizationText(dialogChoice.choiceTextId);

            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].gameObject.SetActive(true);
            //하기전에 먼저 버튼에 조건이 있어서 비활성화 해야하는지 체크
            if (!NodeConditionChecker.CheckButtonInteractable(dialogChoice))
            {
                choiceButtons[i].interactable = false;
                continue;
            }
            int goTo = NodeConditionChecker.CheckChoiceCondition(dialogChoice);
            choiceButtons[i].onClick.AddListener(() => ShowData(goTo));
            choiceButtons[i].interactable = true;
            
        }

        //텍스트를 느리게 재생시키는 코루틴 다시 실행
        if (printSlowTextCor != null)
        {
            StopCoroutine(printSlowTextCor);
        }
        printSlowTextCor = StartCoroutine(LoadAndPrintText(curNode.textKey));

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
                        Sprite curSprite = charData.GetExpression(spriteData.Form, spriteData.Expression);
                        if (curSprite == null) continue;
                        //말하고 있는 사람이면 투명도 1, 아니면 -1로 한다.
                        //float alpha = 1;
                        //if (spriteData.characterId != curNode.speaker)
                        //{
                        //    alpha = 0.5f;
                        //}
                        characterPosManager.ShowImage(charData.CharacterId, curSprite, spriteData.Position, charData.BodyHeight);
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
        //만약 말하고있는 사람의 이미지가 현재 화면에 존재한다면 투명도 조절 하기
        characterPosManager.UpdateAllSpeakerAlpha(curNode.speaker);

        //다이얼로그 로그창 띄우기
        string speakerText = ResourceManager.Instance.GetLocalizationText(curNode.speaker);
        string textKeyText = ResourceManager.Instance.GetLocalizationText(curNode.textKey);
        dialogLogCanvas.AddLog(speakerText, textKeyText);
        //보상이 있다면? 보상 이미지 띄우기
        DialogReward[] dialogRewards = curNode.reward;
        if (dialogRewards.Length > 0)
        {
            //호감도 외 보상
            //중복 지급을 막기위해 세이브데이터에서 안받았을때만 받도록 처리
            if (SaveManager.Instance.TryAddReward(curNode.id))
            {
                rewardCanvas.Show(dialogRewards);
                //호감도 보상
                foreach (DialogReward rewardData in dialogRewards)
                {
                    if (rewardData.Type == eDialogItemType.Affinity)
                    {
                        //호감도
                        SaveManager.Instance.AddAffinityData(rewardData.key, rewardData.amount);
                    }
                    else
                    {
                        ItemClassJson itemClass = itemDataJson.GetItemInfo(rewardData.key);
                        if (itemClass != null)
                        {
                            SaveItem saveItem = new SaveItem();
                            saveItem.key = rewardData.key;
                            saveItem.amount = rewardData.amount;
                            //saveItem.explain = itemClass.explain;
                            SaveManager.Instance.AddItemData(saveItem);
                        }
                    }
                }
            }
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
                Sprite sprite = ResourceManager.Instance.GetCGOrBackgroundData(cgData.key).image;
                backgroundScript.SetBackgroundImage(sprite);
                //런타임 세이브 데이터에 하나씩 추가를 한다
                SaveManager.Instance.SetCurBackgroundKey(cgData.key);
                SaveManager.Instance.UnlockGalleryData(cgData.key, eCgCategory.cg);
                //SaveManager.Instance.LoadSaveFile().SetReadCGData(cgData.key);

            }
        }
    }

}
