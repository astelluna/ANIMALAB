using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ImageSlot : MonoBehaviour
{
    public Image CurImage { get; private set; }
    public Image EffectImage { get; private set; }

    //저장용 데이터
    public CharacterSprite characterSpriteData { get; private set; }

    public void SetCharacterData(CharacterSprite data)
    {
        characterSpriteData = data;
    }
    private void Awake()
    {
        CurImage = GetComponent<Image>();
        EffectImage = GetComponentsInChildren<Image>(true).FirstOrDefault(img => img.gameObject != gameObject);
    }
}
