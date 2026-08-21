using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Button exitButton;

    private void Awake()
    {
        exitButton.onClick.AddListener(ExitButtonClick);
    }
    private void OnDestroy()
    {
        exitButton.onClick.RemoveAllListeners();
    }

    //public void SetAndShowEndingPanel(string titleKey, string descKey)
    public void SetAndShowEndingPanel(EndingData data)
    {
        titleText.text = ResourceManager.Instance.GetLocalizationText(data.title);
        descriptionText.text = ResourceManager.Instance.GetLocalizationText(data.description);
        this.gameObject.SetActive(true);
    }
    private void ExitButtonClick()
    {
        SceneChangeManager.Instance.LoadSceneWithFade(PathManager.TitleSceneName);
        this.gameObject.SetActive(false);
    }


}
