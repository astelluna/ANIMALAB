using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterSelectUI : MonoBehaviour
{
    [SerializeField] TMP_Text chapterTitleText;
    [SerializeField] Button leftArrowButton;
    [SerializeField] Button rightArrowButton;
    [SerializeField] Button selectButton;

    [SerializeField] Image curChapterPreview;
    [SerializeField] Image prevChapterPreview;
    [SerializeField] Image nextChapterPreview;

    List<ChapterData> chapters;
    int currentIndex = 0;

    private void Start()
    {
        chapters = NodeManager.GetChapterDataClass();
        leftArrowButton.onClick.AddListener(() => Move(-1));
        rightArrowButton.onClick.AddListener(() => Move(1));
        selectButton.onClick.AddListener(OnSelected);
    }
    void OnSelected()
    {
        //이제 최신 챕터를 선택했는지/ 이전 챕터를 선택했는지에 따라 다르게 동작해야함
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
        //
        bool unlocked = IsChapterUnlocked(chapter.chapterId);
        selectButton.interactable = unlocked;

        leftArrowButton.interactable = currentIndex > 0;
        rightArrowButton.interactable = currentIndex < chapters.Count - 1;

        //보여주기
        prevChapterPreview.gameObject.SetActive(currentIndex > 0);
        prevChapterPreview.gameObject.SetActive(currentIndex < chapters.Count - 1);

        if(currentIndex > 0)
        {
            //이전 챕터 썸내일 이미지 표시
        }
        if(currentIndex < chapters.Count -1)
        {
            //다음 챕터 썸네일 이미지 표시
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
