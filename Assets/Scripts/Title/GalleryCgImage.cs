using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GalleryCgImage : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] Image lockImage;


    public event Action<Sprite> OnImageClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        //이 이미지를 클릭했다면?

        if (lockImage.gameObject.activeInHierarchy) return;
        //이벤트를 위에 보내면 됨.
        OnImageClick?.Invoke(image.sprite);
    }

    public void ActiveAndSet(bool isRead, CGData data )
    {
        this.gameObject.SetActive(true);
        ShowLockImage(!isRead);
        //데이터 설정
        image.sprite = data.image;
    }

    private void ShowLockImage(bool isLock)
    {
        lockImage.gameObject.SetActive(isLock);
    }
}
