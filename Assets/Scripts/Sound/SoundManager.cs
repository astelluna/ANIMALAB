using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    //오디오 소스
    [SerializeField] private AudioSource sfxSource;
    //[SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource bgmSource;
    //[SerializeField] private AudioSource ambientSource;

    //오디오믹서
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup uiGroup;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup ambientGroup;
    //사운드 데이터베이스
    [SerializeField] private SoundDatabase soundDatabase;
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
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
    public void Play(eSoundID id)
    {
        SoundData soundData = soundDatabase.Get(id);
        if (soundData == null) return;

        AudioSource source = soundData.category == SoundCategory.BGM ? bgmSource : sfxGroup;
        source.outputAudioMixerGroup = GetMixerGroup(soundData.category);

        source.clip = soundData.clips[Random.Range(0, soundData.clips.Length)];
        source.volume = soundData.volume;
        source.pitch = Random.Range(soundData.pitchRange.x, soundData.pitchRange.y);
        source.loop = soundData.loop;

        source.Play();
    }
}
