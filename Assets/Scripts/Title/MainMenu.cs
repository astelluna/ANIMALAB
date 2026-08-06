using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //버튼을 누르면 맨 처음부터 시작한다는 경고가 뜨고, 확인을 누르면 세이브 파일이 초기화될것이다.
    SaveData data;
    [SerializeField] Button newGameButton;
    [SerializeField] InitConfirmPanel confirmPanel;

    private void Awake()
    {
        confirmPanel.yesButtonClicked += CreateNewSaveData;
        newGameButton.onClick.AddListener(ShowConfirmPanel);
    }
    
    private void OnDestroy()
    {
        confirmPanel.yesButtonClicked -= CreateNewSaveData;
        newGameButton.onClick.RemoveAllListeners();
    }
    private void ShowConfirmPanel()
    {
        confirmPanel.gameObject.SetActive(true);
    }
    private void CreateNewSaveData()
    {
        SaveManager.Instance.CreateNewSaveFile();
    }

}
