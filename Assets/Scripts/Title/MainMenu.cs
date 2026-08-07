using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //버튼을 누르면 맨 처음부터 시작한다는 경고가 뜨고, 확인을 누르면 세이브 파일이 초기화될것이다.
    SaveData data;
    [SerializeField] Button newGameButton;
    [SerializeField] Button continueButton;
    [SerializeField] WarningPanel warningPanel;
    [SerializeField] ChapterSelectPanel chapterSelectPanel;


    private void Awake()
    {
        warningPanel.OnYesButtonClick += CreateNewSaveData;
        newGameButton.onClick.AddListener(ShowConfirmPanel);
        continueButton.onClick.AddListener(ShowChapterSelectPanel);
    }
    
    private void OnDestroy()
    {
        warningPanel.OnYesButtonClick -= CreateNewSaveData;
        newGameButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
    }
    private void ShowConfirmPanel()
    {
        warningPanel.gameObject.SetActive(true);
    }
    private void ShowChapterSelectPanel()
    {
        chapterSelectPanel.gameObject.SetActive(true);
    }
    private void CreateNewSaveData()
    {
        SaveManager.Instance.CreateNewSaveFile();
    }

}
