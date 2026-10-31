using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    //오디오 소스
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource ambientSource;

    //오디오믹서
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup uiGroup;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup ambientGroup;
    //사운드 데이터베이스
    [SerializeField] private SoundDatabase soundDatabase;
    //오디오믹서
    [SerializeField] private AudioMixer audioMixer;
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        //다음 원하는 코드 작성
    }
    private void Start()
    {
        LoadAndApplyAllVolumes();
    }

    private AudioMixerGroup GetMixerGroup(SoundCategory category)
    {
        switch(category)
        {
            case SoundCategory.SFX:
                return sfxGroup;
            case SoundCategory.UI:
                return uiGroup;
            case SoundCategory.BGM:
                return bgmGroup;
            case SoundCategory.Ambient:
                return ambientGroup;
            default:
                return null;
        }
    }
    private AudioSource GetSource(SoundCategory category)
    {
        switch (category)
        {
            case SoundCategory.SFX: return sfxSource;
            case SoundCategory.UI: return uiSource;
            case SoundCategory.BGM: return bgmSource;
            case SoundCategory.Ambient: return ambientSource;
            default: return null;
        }
    }
    public void Play(eSoundID id)
    {
        //런타임 도중 ID값으로 소리를 재생시킬떄만 사용
        SoundData soundData = soundDatabase.Get(id);
        
        if (soundData == null)
        {
            Debug.LogError("SoundData가 null");
            return;
        }
        //세이브 매니저에 현재 BGM 저장
        if(soundData.category == SoundCategory.BGM)
        {
            SaveManager.Instance.SetBackgroundMusicId(id);
        }

        AudioSource source = GetSource(soundData.category);
        source.outputAudioMixerGroup = GetMixerGroup(soundData.category);
        source.clip = soundData.clips[Random.Range(0, soundData.clips.Length)];
        source.volume = soundData.volume;
        source.pitch = Random.Range(soundData.pitchRange.x, soundData.pitchRange.y);
        source.loop = soundData.loop;
        if(soundData.loop)
        {
            source.Play();
        }
        else
        {
            source.PlayOneShot(source.clip);
        }
    }
    public void Play(SoundData soundData, SoundCategory category)
    {
        //게임내에서 특정 BGM이나 소리를 재생시킬때 사용
        AudioSource source = GetSource(category);
        source.outputAudioMixerGroup = GetMixerGroup(soundData.category);
        source.clip = soundData.clips[Random.Range(0, soundData.clips.Length)];
        source.volume = soundData.volume;
        source.pitch = Random.Range(soundData.pitchRange.x, soundData.pitchRange.y);
        source.loop = soundData.loop;
        if (soundData.loop)
        {
            source.Play();
        }
        else
        {
            source.PlayOneShot(source.clip);
        }
    }

    //볼륨 설정
    public void SetBGMVolume(float value) => SetVolume(PathManager.BGM_KEY, PathManager.BGM_PARAM, value);
    public void SetSFXVolume(float value) => SetVolume(PathManager.SFX_KEY, PathManager.SFX_PARAM, value);
    public void SetAmbientVolume(float value) => SetVolume(PathManager.AMBIENT_KEY, PathManager.AMBIENT_PARAM, value);
    public void SetUIVolume(float value) => SetVolume(PathManager.UI_KEY, PathManager.UI_PARAM, value);

    public float GetBGMVolume() => PlayerPrefs.GetFloat(PathManager.BGM_KEY, 1f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat(PathManager.SFX_KEY, 1f);
    public float GetAmbientVolume() => PlayerPrefs.GetFloat(PathManager.AMBIENT_KEY, 1f);
    public float GetUIVolume() => PlayerPrefs.GetFloat(PathManager.UI_KEY, 1f);

    private void SetVolume(string prefKey, string mixerParam, float value)
    {
        value = Mathf.Clamp01(value);
        float dB = value > 0.0001f ? Mathf.Log10(value) * 20f : -80f;
        audioMixer.SetFloat(mixerParam, dB);
        PlayerPrefs.SetFloat(prefKey, value);
    }

    private void LoadAndApplyAllVolumes()
    {
        SetVolume(PathManager.BGM_KEY, PathManager.BGM_PARAM, GetBGMVolume());
        SetVolume(PathManager.SFX_KEY, PathManager.SFX_PARAM, GetSFXVolume());
        SetVolume(PathManager.AMBIENT_KEY, PathManager.AMBIENT_PARAM, GetAmbientVolume());
        SetVolume(PathManager.UI_KEY, PathManager.UI_PARAM, GetUIVolume());
    }
}
