using UnityEngine;
using UnityEngine.UI;

public class ChapterSelectImage : MonoBehaviour
{
    //자물쇠 이미지 접근
    [SerializeField] Image lockImage;

    private Image _curImage;
    private Image curImage
    {
        get
        {
            if (_curImage == null) _curImage = GetComponent<Image>();
            return _curImage;
        }
    }

    public void ShowLockImage(bool isShow)
    {
        if(isShow)
        {
            lockImage.gameObject.SetActive(true);
        }
        else
        {
            lockImage.gameObject.SetActive(false);
        }
    }
    public void SetImage(Sprite sprite)
    {
        curImage.sprite = sprite;
    }
}
