using UnityEngine;
using UnityEngine.UI;

public class CharacterUISlotManager : MonoBehaviour
{
    [SerializeField] private Image slotLeft;
    [SerializeField] private Image slotCenterLeft;
    [SerializeField] private Image slotCenterRight;
    [SerializeField] private Image slotRight;

    public Image GetImageToIndex(eSpritePosition position)
    {
        Image image = null;
        switch(position)
        {
            case eSpritePosition.Left:
                image = slotLeft;
                break;
            case eSpritePosition.CenterLeft:
                image = slotCenterLeft;
                break;
            case eSpritePosition.CenterRight:
                image = slotCenterRight;
                break;
            case eSpritePosition.Right:
                image = slotRight;
                break;

        }
        if (image == null) { Debug.LogError("image가 null로 반환중"); }
        return image;
    }


}
