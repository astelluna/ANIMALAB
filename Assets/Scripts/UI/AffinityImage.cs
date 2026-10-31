using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AffinityImage : MonoBehaviour
{
    [SerializeField] Image characterImage;
    [SerializeField] Image affinityBarImage;
    [SerializeField] TextMeshProUGUI affinityText;

    public void SetAffinityImage(Sprite sprite, int amount, int max,int height)
    {
        string text = ResourceManager.Instance.GetLocalizationText(PathManager.affinityTextName);
        characterImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        characterImage.sprite = sprite;
        affinityText.text = text + ":" + amount.ToString();
        affinityBarImage.fillAmount = (float)amount / max;
    }

}
