using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterSelectPanel : MonoBehaviour
{
    [SerializeField] TMP_Text chapterTitleText;
    [SerializeField] Button leftArrowButton;
    [SerializeField] Button rightArrowButton;
    [SerializeField] Button selectButton;
    [SerializeField] Button initButton;
    [SerializeField] Button closeButton;

    [SerializeField] ChapterSelectImage curChapterPreview;
    [SerializeField] ChapterSelectImage prevChapterPreview;
    [SerializeField] ChapterSelectImage nextChapterPreview;

    List<ChapterData> chapters;
    int currentIndex = 0;

    WarningPanel warningPanel;

    private void Awake()
    {
        warningPanel = GetComponentInChildren<WarningPanel>(true);
    }
    private void OnEnable()
    {
        //chapters = NodeManager.GetChapterDataClass();
        SaveData saveData = SaveManager.Instance.LoadSaveFile();
        chapters = JsonDataManager.GetChapterDataClass();

        currentIndex = saveData.curChapterId - 1; //인덱스 0부터 시작이므로 -1
        Move(0);
    }
    private void Start()
    {
        //chapters = NodeManager.GetChapterDataClass();
        leftArrowButton.onClick.AddListener(() => Move(-1));
        rightArrowButton.onClick.AddListener(() => Move(1));
        selectButton.onClick.AddListener(OnSelected);
        initButton.onClick.AddListener(InitCurrentChapterData);
        closeButton.onClick.AddListener(CloseButtonClick);
    }
    private void OnDestroy()
    {
        leftArrowButton.onClick.RemoveAllListeners();
        rightArrowButton.onClick.RemoveAllListeners();
        selectButton.onClick.RemoveAllListeners();
        initButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }
    private void CloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }

    private void InitCurrentChapterData()
    {
        //현재 챕터의 데이터는 초기화할수있는 방법이 없어서 추가
        warningPanel.ShowWarningPanel(StartFromNewChapter);
    }
    void OnSelected()
    {
        //1. 최신 챕터를 선택했다면? 최근 세이브 데이터의 노드를 참조하여 거기부터 시작하도록 해야함.
        SaveData data = SaveManager.Instance.LoadSaveFile();
        int selChapterIndex = currentIndex + 1;

        if (selChapterIndex == data.curChapterId)
        {
            if (data != null)
            {
                GameManager.Instance.GameStart(data);
            }
        }
        else if(selChapterIndex < data.curChapterId)
        {
            //2. 만약 이전 노드를 선택했다면? 확인하는 패널을 하나 띄움. 그리고 거기에서도 yes 버튼을 누른다면? 아래 코드를 실행한다
            if(true)
            {
                warningPanel.ShowWarningPanel(StartFromNewChapter);
                //warningPanel.gameObject.SetActive(true);
            }
        }
    }

    private void StartFromNewChapter()
    {
        //세이브데이터를 챕터 n정보로 설정하고, 
        ChapterData chapterData = chapters[currentIndex];
        SaveData saveData = SaveManager.Instance.LoadSaveFile() ?? new SaveData();
        saveData.curChapterId = chapterData.chapterId;
        saveData.nodeId = chapterData.startNodeId;
        SaveManager.Instance.CreateSaveData(saveData);
        //그리고 게임 시작
        OnSelected();
    }


    void Move(int direction)
    {
        int newIndex = currentIndex + direction;
        if (newIndex < 0 || newIndex >= chapters.Count) return;
        currentIndex = newIndex;
        UpdateDisplay();
    }
    void UpdateDisplay()
    {
        SaveData data = SaveManager.Instance.LoadSaveFile();
        ChapterData chapter = chapters[currentIndex];
        chapterTitleText.text = $"챕터 {chapter.chapterId}, {chapter.title}";
        curChapterPreview.SetImage(ResourceManager.Instance.GetChapterThumnail(chapter.thumbnailKey).thumbnail);
        bool unlocked = IsChapterUnlocked(chapter.chapterId);
        selectButton.interactable = unlocked;
        curChapterPreview.ShowLockImage(!unlocked);
        
        leftArrowButton.interactable = currentIndex > 0;
        rightArrowButton.interactable = currentIndex < chapters.Count - 1;

        if(currentIndex+1 == data.curChapterId)
        {
            initButton.gameObject.SetActive(true);
        }
        else
        {
            initButton.gameObject.SetActive(false);
        }
        //보여주기
        prevChapterPreview.gameObject.SetActive(currentIndex > 0);
        nextChapterPreview.gameObject.SetActive(currentIndex < chapters.Count - 1);

        if(currentIndex > 0)
        {
            //이전 챕터 썸네일 표시
            ChapterData prevChapter = chapters[currentIndex - 1];
            bool prevUnlocked = IsChapterUnlocked(prevChapter.chapterId);
            prevChapterPreview.SetImage(ResourceManager.Instance.GetChapterThumnail(prevChapter.thumbnailKey).thumbnail);
            prevChapterPreview.ShowLockImage(!prevUnlocked);
            
        }
        if(currentIndex < chapters.Count -1)
        {
            //다음 챕터 썸네일 이미지 표시
            ChapterData nextChapter = chapters[currentIndex + 1];
            bool nextUnlocked = IsChapterUnlocked(nextChapter.chapterId);
            nextChapterPreview.SetImage(ResourceManager.Instance.GetChapterThumnail(nextChapter.thumbnailKey).thumbnail);
            nextChapterPreview.ShowLockImage(!nextUnlocked);
        }
    }
    bool IsChapterUnlocked(int chapterId)
    {
        //세이브 데이터를 불러와야할듯? 흠
        int clearedChapter = 0;
        SaveData data = SaveManager.Instance.LoadSaveFile();
        if(data != null)
        {
            clearedChapter = data.curChapterId;
        }
        return chapterId <= clearedChapter;
    }
}
