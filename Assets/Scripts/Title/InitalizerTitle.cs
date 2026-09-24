using UnityEngine;

public class InitalizerTitle : MonoBehaviour
{
    [SerializeField] SoundData titleSound;
    [SerializeField] SettingPanel settingPanel;
    private void Start()
    {
        SoundManager.Instance.Play(titleSound, SoundCategory.BGM);
        settingPanel.InitializeLanguage();
    }


}
