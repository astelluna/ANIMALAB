using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ImageSlot : MonoBehaviour
{
    public Image CurImage { get; private set; }
    public Image EffectImage { get; private set; }

    private void Awake()
    {
        CurImage = GetComponent<Image>();
        EffectImage = GetComponentsInChildren<Image>(true).FirstOrDefault(img => img.gameObject != gameObject);
    }
}
