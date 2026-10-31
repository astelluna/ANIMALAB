using UnityEngine;
using UnityEngine.UI;

public class CreditPanel : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] Button privacyPolicyButton;


    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        privacyPolicyButton.onClick.AddListener(OnPrivacyPolicyButtonClick);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        privacyPolicyButton.onClick.RemoveAllListeners();
    }
    
    private void OnCloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }
    private void OnPrivacyPolicyButtonClick()
    {
        Application.OpenURL(PathManager.PrivacyPolicyWebUrl);
    }
    public void OpenPanel()
    {
        this.gameObject.SetActive(true);
    }
}
