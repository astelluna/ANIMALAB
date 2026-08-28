using UnityEngine;
using UnityEngine.UI;

public class CreditPanel : MonoBehaviour
{
    [SerializeField] Button closeButton;
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
    public void OpenPanel()
    {
        this.gameObject.SetActive(true);
    }
}
