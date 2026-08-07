using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ChapterSelectPanel : MonoBehaviour
{
    [SerializeField] TMP_Text chapterTitleText;
    [SerializeField] Button leftArrowButton;
    [SerializeField] Button rightArrowButton;
    [SerializeField] Button selectButton;

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

    private void Start()
    {
        chapters = NodeManager.GetChapterDataClass();
        leftArrowButton.onClick.AddListener(() => Move(-1));
        rightArrowButton.onClick.AddListener(() => Move(1));
        selectButton.onClick.AddListener(OnSelected);

        warningPanel.OnYesButtonClick += StartFromNewChapter;
    }
    private void OnDestroy()
    {
        if(warningPanel != null) warningPanel.OnYesButtonClick -= StartFromNewChapter;
    }
    void OnSelected()
    {
        //이제 최신 챕터를 선택했는지/ 이전 챕터를 선택했는지에 따라 다르게 동작해야함
        //1. 최신 챕터를 선택했다면? 최근 세이브 데이터의 노드를 참조하여 거기부터 시작하도록 해야함.
        //게임매니저로 실행해야하나 이제? 흠..
        SaveData data = SaveManager.Instance.LoadSaveFile();
        int selChapterIndex = currentIndex + 1;

        if (selChapterIndex == data.curChapterId)
        {
            if (data != null)
            {
                GameManager.Instance.SetStartNodeID(data.nodeId);
            }
        }
        else if(selChapterIndex < data.curChapterId)
        {
            //2. 만약 이전 노드를 선택했다면?
            //TODO. 확인하는 패널을 하나 띄움. 그리고 거기에서도 yes 버튼을 누른다면? 아래 코드를 실행한다
            if(true)
            {
                warningPanel.gameObject.SetActive(true);
            }
        }
    }

    private void StartFromNewChapter()
    {
        //세이브데이터를 챕터 n정보로 설정하고, 
        ChapterData chapterData = chapters[currentIndex];
        //SaveData newSaveData = new SaveData();
        //newSaveData.curChapterId = chapterData.chapterId;
        //newSaveData.nodeId = chapterData.startNodeId;
        SaveManager.Instance.CreateSaveData(chapterData.chapterId, chapterData.startNodeId);
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
        ChapterData chapter = chapters[currentIndex];
        chapterTitleText.text = $"챕터 {chapter.chapterId}, {chapter.title}";
        curChapterPreview.SetImage(ResourceManager.Instance.GetChapterThumnail(chapter.thumbnailKey).thumbnail);
        bool unlocked = IsChapterUnlocked(chapter.chapterId);
        selectButton.interactable = unlocked;
        if(unlocked)
        {
            curChapterPreview.ShowLockImage(!unlocked);
        }

        leftArrowButton.interactable = currentIndex > 0;
        rightArrowButton.interactable = currentIndex < chapters.Count - 1;

        //보여주기
        prevChapterPreview.gameObject.SetActive(currentIndex > 0);
        prevChapterPreview.gameObject.SetActive(currentIndex < chapters.Count - 1);

        if(currentIndex > 0)
        {
            //이전 챕터 썸네일 표시
            ChapterData prevChapter = chapters[currentIndex - 1];
            bool prevUnlocked = IsChapterUnlocked(prevChapter.chapterId);
            prevChapterPreview.SetImage(ResourceManager.Instance.GetChapterThumnail(prevChapter.thumbnailKey).thumbnail);
            if (prevUnlocked)
            {
                prevChapterPreview.ShowLockImage(!prevUnlocked);
            }
        }
        if(currentIndex < chapters.Count -1)
        {
            //다음 챕터 썸네일 이미지 표시
            ChapterData nextChapter = chapters[currentIndex + 1];
            bool nextUnlocked = IsChapterUnlocked(nextChapter.chapterId);
            nextChapterPreview.SetImage(ResourceManager.Instance.GetChapterThumnail(nextChapter.thumbnailKey).thumbnail);
            if (nextUnlocked)
            {
                nextChapterPreview.ShowLockImage(!nextUnlocked);
            }
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
