using UnityEngine;

public class InitalizerTitle : MonoBehaviour
{
    [SerializeField] SoundData titleSound;
    private void Start()
    {
        SoundManager.Instance.Play(titleSound, SoundCategory.BGM);
    }
}
