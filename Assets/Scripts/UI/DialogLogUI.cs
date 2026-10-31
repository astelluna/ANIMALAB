using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogLogUI : MonoBehaviour
{
    [SerializeField] Image speakerImage;
    [SerializeField] TextMeshProUGUI spekerText;
    [SerializeField] TextMeshProUGUI dialogText;

    public void SetLogImage(Sprite img, string speker, string dialog)
    {
        speakerImage.sprite = img;
        spekerText.text = speker;
        dialogText.text = dialog;
    }
}
