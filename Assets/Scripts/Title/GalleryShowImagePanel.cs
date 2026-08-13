using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GalleryShowImagePanel : MonoBehaviour, IPointerClickHandler
{
    //[field: SerializeField] public Image PanelImage { get; private set; }
    private Image panelImage;

    public void OnPointerClick(PointerEventData eventData)
    {
        this.gameObject.SetActive(false);
    }
    public void SetImageAndShow(Sprite sprite)
    {
        panelImage.sprite = sprite;
        this.gameObject.SetActive(true);
    }
}
