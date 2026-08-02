using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUISlotManager : MonoBehaviour
{
    [SerializeField] private Image slotLeft;
    [SerializeField] private Image slotCenterLeft;
    [SerializeField] private Image slotCenterRight;
    [SerializeField] private Image slotRight;

    Dictionary<string, (eSpritePosition,Image)> characterIdToPosDict = new Dictionary<string, (eSpritePosition, Image)>();

    private Image GetImageToCharacterId(string id)
    {
        Image img = null;
        if(characterIdToPosDict.ContainsKey(id))
        {
            img = characterIdToPosDict[id].Item2;
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
    public bool ShowImage(string id, Sprite spriteImg, eSpritePosition position, float alpha, int height)
    {
        Image image = null;
        bool isLeft = false;
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
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        //반환하기 전에, 만약 Left 계열로 호출되었다면 반전, 아니라면 그대로 돌려주자
        FlipImage(image, isLeft);
        SetImageAlpha(image, alpha);
        image.sprite = spriteImg;
        image.gameObject.SetActive(true);
        characterIdToPosDict.Add(id, (position, image));

        return true;
    }
    private void FlipImage(Image img, bool flipLeft)
    {
        Vector3 scale = img.rectTransform.localScale;
        scale.x = flipLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        img.rectTransform.localScale = scale;
    }


}
