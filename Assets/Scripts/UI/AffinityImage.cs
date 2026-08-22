using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AffinityImage : MonoBehaviour
{
    [SerializeField] Image characterImage;
    [SerializeField] Image affinityBarImage;
    [SerializeField] TextMeshProUGUI affinityText;

    public void SetAffinityImage(Sprite sprite, int amount, int max)
    {
        string text = ResourceManager.Instance.GetLocalizationText(PathManager.affinityTextName);

        characterImage.sprite = sprite;
        affinityText.text = text + ":" + amount.ToString();
        affinityBarImage.fillAmount = (float)amount / max;
    }

}
