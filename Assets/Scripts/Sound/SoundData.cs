using UnityEngine;

public enum SoundCategory { SFX, UI, BGM, Ambient }

[CreateAssetMenu(menuName = "Sound/SoundData")]
public class SoundData : ScriptableObject
{
    public AudioClip[] clips;
    public float volume = 1f;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    public bool loop = false;
    public SoundCategory category = SoundCategory.SFX;
}
