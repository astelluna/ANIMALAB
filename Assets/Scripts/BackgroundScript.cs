using UnityEngine;
using UnityEngine.UI;

public class BackgroundScript : MonoBehaviour
{
    Image backgroundImage;

    private void Awake()
    {
        if (backgroundImage == null) { backgroundImage = GetComponent<Image>(); }
    }

    public void SetBackgroundImage(Sprite sprite)
    {
        if(backgroundImage == null) { backgroundImage = GetComponent<Image>(); }
        backgroundImage.sprite = sprite;
    }

}
