using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] Button openAffinityButton;
    [SerializeField] Button openItemSlotButton;
    [SerializeField] AffinityPanel affinityPanel;
    [SerializeField] ItemSlotPanel itemSlotPanel;

    public bool IsPanelShow => gameObject.activeInHierarchy;
    private void Awake()
    {
        closeButton.onClick.AddListener(CloseButtonClick);
        openAffinityButton.onClick.AddListener(OpenAffinityButtonClick);
        openItemSlotButton.onClick.AddListener(OpenItemSlotButtonClick);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        openAffinityButton.onClick.RemoveAllListeners();
        openItemSlotButton.onClick.RemoveAllListeners();
    }
    private void OpenAffinityButtonClick()
    {
        affinityPanel.gameObject.SetActive(true);
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
