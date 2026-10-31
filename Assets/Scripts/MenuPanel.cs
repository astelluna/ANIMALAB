using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] Button openAffinityButton;
    [SerializeField] Button openItemSlotButton;
    [SerializeField] Button settingButton;
    [SerializeField] Button gotoTitleButton;
    [SerializeField] AffinityPanel affinityPanel;
    [SerializeField] ItemSlotPanel itemSlotPanel;
    [SerializeField] SettingPanel settingPanel;
    
    public bool IsPanelShow => gameObject.activeInHierarchy;
    private void Awake()
    {
        closeButton.onClick.AddListener(CloseButtonClick);
        openAffinityButton.onClick.AddListener(OpenAffinityButtonClick);
        openItemSlotButton.onClick.AddListener(OpenItemSlotButtonClick);
        settingButton.onClick.AddListener(SettingButtonClick);
        gotoTitleButton.onClick.AddListener(GoToTitleButtonClick);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        openAffinityButton.onClick.RemoveAllListeners();
        openItemSlotButton.onClick.RemoveAllListeners();
        gotoTitleButton.onClick.RemoveAllListeners();
    }
    private void OpenAffinityButtonClick()
    {
        affinityPanel.gameObject.SetActive(true);
    }
    private void SettingButtonClick()
    {
        settingPanel.OpenPanel();
    }
    private void GoToTitleButtonClick()
    {
        SceneChangeManager.Instance.LoadSceneWithFade(PathManager.TitleSceneName);
    }
    private void OpenItemSlotButtonClick()
    {
        itemSlotPanel.ActiveItemSlotCanvas(SaveManager.Instance.GetSaveItemList());
    }
    private void CloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }
    public void OpenPanel()
    {
        this.gameObject.SetActive(true);
    }
}
