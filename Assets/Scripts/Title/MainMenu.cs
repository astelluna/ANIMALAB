using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //버튼을 누르면 맨 처음부터 시작한다는 경고가 뜨고, 확인을 누르면 세이브 파일이 초기화될것이다.
    [SerializeField] Button newGameButton;
    [SerializeField] Button continueButton;
    [SerializeField] Button galleryButton;
    [SerializeField] WarningPanel warningPanel;
    [SerializeField] ChapterSelectPanel chapterSelectPanel;
    [SerializeField] GalleryCgPanel galleryCgPanel;


    private void Awake()
    {
        newGameButton.onClick.AddListener(ShowConfirmPanel);
        continueButton.onClick.AddListener(ShowChapterSelectPanel);
        galleryButton.onClick.AddListener(ShowGalleryPanel);
    }
    
    private void OnDestroy()
    {
        newGameButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
    }
    private void ShowConfirmPanel()
    {
        warningPanel.ShowWarningPanel(CreateNewSaveData);
    }
    private void ShowChapterSelectPanel()
    {
        chapterSelectPanel.gameObject.SetActive(true);
    }
    private void ShowGalleryPanel()
    {
        galleryCgPanel.gameObject.SetActive(true);
    }
    private void CreateNewSaveData()
    {
        SaveData saveData = SaveManager.Instance.CreateNewSaveFile();
        GameManager.Instance.GameStart(saveData);
    }

}
