using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    //[SerializeField] private GameObject panelRoot;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider ambientSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Button closeButton;

    //언어 설정
    [SerializeField] private Button koreanButton;
    [SerializeField] private Button englishButton;


    public bool IsPanelShow => this.gameObject.activeInHierarchy;
    private void Awake()
    {
        closeButton.onClick.AddListener(ClosePanel);

        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
        ambientSlider.onValueChanged.AddListener(SoundManager.Instance.SetAmbientVolume);
        uiSlider.onValueChanged.AddListener(SoundManager.Instance.SetUIVolume);
        koreanButton.onClick.AddListener(SetLanguageKorean);
        englishButton.onClick.AddListener(SetLanguageEnglish);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        bgmSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        ambientSlider.onValueChanged.RemoveAllListeners();
        uiSlider.onValueChanged.RemoveAllListeners();
        koreanButton.onClick.RemoveAllListeners();
        englishButton.onClick.RemoveAllListeners();
    }
    public void OpenPanel()
    {
        bgmSlider.SetValueWithoutNotify(SoundManager.Instance.GetBGMVolume());
        sfxSlider.SetValueWithoutNotify(SoundManager.Instance.GetSFXVolume());
        ambientSlider.SetValueWithoutNotify(SoundManager.Instance.GetAmbientVolume());
        uiSlider.SetValueWithoutNotify(SoundManager.Instance.GetUIVolume());
        UpdateLanguageButtons();
        gameObject.SetActive(true);
    }
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    //언어
    private void UpdateLanguageButtons()
    {
        string currentLocale = LocalizationSettings.SelectedLocale.Identifier.Code;

        bool isKorean = currentLocale == "ko";
        koreanButton.interactable = !isKorean;
        englishButton.interactable = isKorean;
    }
    public void SetLanguageKorean()
    {
        SetLocale("ko");
        UpdateLanguageButtons();
    }

    public void SetLanguageEnglish()
    {
        SetLocale("en");
        UpdateLanguageButtons();
    }
    public void InitializeLanguage()
    {
        // 이미 언어를 선택한 적이 있다면 저장된 언어 사용
        if (PlayerPrefs.HasKey("SelectedLocale"))
        {
            SetLocale(PlayerPrefs.GetString("SelectedLocale"));
            return;
        }

        // 처음 실행한 경우 기기 언어 확인
        string localeCode =
            Application.systemLanguage == SystemLanguage.Korean
            ? "ko"
            : "en";

        SetLocale(localeCode);
    }

    private void SetLocale(string localeCode)
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        if (locale == null)
        {
            Debug.LogError($"{localeCode} 로케일을 찾을 수 없습니다. Localization Settings에 등록되어 있는지 확인하세요.");
            return;
        }

        LocalizationSettings.SelectedLocale = locale;
        PlayerPrefs.SetString("SelectedLocale", localeCode);
        PlayerPrefs.Save();
    }
}
