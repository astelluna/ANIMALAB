using UnityEngine;
using UnityEngine.UI;

public class SaveCompletePanel : MonoBehaviour
{
    [SerializeField] Button closeButton;

    public bool IsPanelShow => gameObject.activeInHierarchy;
    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }

    private void OnCloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }
    public void OnPanelShow()
    {
        this.gameObject.SetActive(true);
    }
}
