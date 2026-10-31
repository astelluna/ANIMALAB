using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUISlotManager : MonoBehaviour
{
    public static CharacterUISlotManager Instance { get; private set; }

    [SerializeField] private ImageSlot slotLeft;
    [SerializeField] private ImageSlot slotCenterLeft;
    [SerializeField] private ImageSlot slotCenterRight;
    [SerializeField] private ImageSlot slotRight;

    Dictionary<string, (eSpritePosition, ImageSlot)> characterIdToPosDict = new Dictionary<string, (eSpritePosition, ImageSlot)>();

    ImageCharacterEffect imageCharacterEffect;

    float brightAlpha = 1f;
    float darkAlpha = 0.5f;


    public List<CharacterSprite> GetCharacterShowData()
    {
        List<CharacterSprite> characterSpriteList = new List<CharacterSprite>();
        foreach(var item in characterIdToPosDict.Values)
        {
            ImageSlot imageSlot = item.Item2;
            characterSpriteList.Add(imageSlot.characterSpriteData);
        }
        return characterSpriteList;
    }
    public void SetCharacterShowData(List<CharacterSprite> spriteDataList)
    {
        if (spriteDataList == null) return;
        foreach(CharacterSprite item in spriteDataList)
        {
            CharacterDataSO charData = ResourceManager.Instance.GetCharacterData(item.characterId);
            Sprite curSprite = charData.GetExpression(item.Form, item.Expression);
            ShowImage(item.characterId, curSprite, charData.BodyHeight, item);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        imageCharacterEffect = GetComponent<ImageCharacterEffect>();


    }
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private Image GetImageToCharacterId(string id)
    {
        Image img = null;
        if(characterIdToPosDict.ContainsKey(id))
        {
            img = characterIdToPosDict[id].Item2.CurImage;
        }
        return img;
    }

    public bool HideImage(string id)
    {
        Image img = GetImageToCharacterId(id);
        img.gameObject.SetActive(false);
        if(img == null)
        {
            return false;
        }
        characterIdToPosDict.Remove(id);
        return true;
    }
    public void HideImageAll()
    {
        foreach(var k in characterIdToPosDict)
        {
            k.Value.Item2.gameObject.SetActive(false);
        }
        characterIdToPosDict.Clear();
    }
    private void SetImageAlpha(Image targetImage, float alpha)
    {
        Color c = targetImage.color;
        c.a = alpha;
        targetImage.color = c;
    }
    public void UpdateAllSpeakerAlpha(string characterId)
    {
        //현재 존재하는 모든 캐릭터들을 확인하면서, 존재하면 밝게, 아니면 어둡게 처리해야한다
        foreach(var item in characterIdToPosDict.Keys)
        {
            Image image = characterIdToPosDict[item].Item2.CurImage;
            if (item == characterId)
            {
                SetImageAlpha(image,brightAlpha);
            }
            else
            {
                SetImageAlpha(image, darkAlpha);
            }
        }
    }
    public bool ShowImage(string id, Sprite spriteImg, int height, CharacterSprite characterData)
    {
        ImageSlot image = null;
        bool isLeft = false;
        eSpritePosition position = characterData.Position;
        switch(position)
        {
            case eSpritePosition.Left:
                image = slotLeft;
                isLeft = true;
                break;
            case eSpritePosition.CenterLeft:
                image = slotCenterLeft;
                isLeft = true;
                break;
            case eSpritePosition.CenterRight:
                image = slotCenterRight;
                break;
            case eSpritePosition.Right:
                image = slotRight;
                break;
        }
        if (image == null) { 
            Debug.LogError("image가 null로 반환중");
            return false;
        }
        //이미지 컴포넌트의 height를 수정
        image.gameObject.SetActive(true);
        image.CurImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        //반환하기 전에, 만약 Left 계열로 호출되었다면 반전, 아니라면 그대로 돌려주자
        FlipImage(image.CurImage, isLeft);
        image.CurImage.sprite = spriteImg;

        if(characterIdToPosDict.ContainsKey(id))
        {
            characterIdToPosDict[id] = (position, image);
        }
        else
        {
            characterIdToPosDict.Add(id, (position, image));
        }
        //사용한 스프라이트 데이터 넣기
        image.SetCharacterData(characterData);

        return true;
    }
    public void ShowEffect(string id, eSpriteEffect spriteEffect)
    {
        if (!characterIdToPosDict.ContainsKey(id)) { return; }

        (eSpritePosition, ImageSlot) tuple = characterIdToPosDict[id];
        Image curImage = tuple.Item2.EffectImage;

        switch(spriteEffect)
        {
            case eSpriteEffect.Exclamation:
                imageCharacterEffect.ShowExclamationEffect(curImage);
                break;
            case eSpriteEffect.Question:
                imageCharacterEffect.ShowQuestionEffect(curImage);
                break;
            case eSpriteEffect.Cheer:
                imageCharacterEffect.ShowCheerEffect(curImage);
                break;
        }


    }
    private void FlipImage(Image img, bool flipLeft)
    {
        Vector3 scale = img.rectTransform.localScale;
        scale.x = flipLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        img.rectTransform.localScale = scale;
    }



}
