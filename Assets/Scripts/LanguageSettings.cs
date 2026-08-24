using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSettings : MonoBehaviour
{
    public void SetLanguageKorean()
    {
        SetLocale("ko");
    }

    public void SetLanguageEnglish()
    {
        SetLocale("en");
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
