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
        characterImage.sprite = sprite;
        affinityText.text = "호감도: " + amount.ToString();
        affinityBarImage.fillAmount = (float)max / amount;
    }

}
