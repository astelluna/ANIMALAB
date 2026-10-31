using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryCgListPanel : MonoBehaviour
{
    [SerializeField] Transform spawnPos;
    [SerializeField] GalleryCgImage cgImagePrefab; 
    List<GalleryCgImage> galleryCgImageList = new List<GalleryCgImage>();

    public event Action<Sprite> OnImageShow;

    private void OnDestroy()
    {
        foreach(var item in galleryCgImageList)
        {
            if(item != null)
            {
                item.OnImageClick -= ImageShow;
            }
        }
    }
    private void ImageShow(Sprite sprite)
    {
        OnImageShow?.Invoke(sprite);
    }
    public void SetCGScrollViewData(eCgCategory type)
    {
        //데이터 설정
        List<CGData> cgDataList = ResourceManager.Instance.GetCGDataList(type);
        HashSet<string> readCgIdHash = null;
        if (type  == eCgCategory.cg)
        {
            readCgIdHash = SaveManager.Instance.LoadGallerySaveFile().GetReadCgData();
        }
        else if( type == eCgCategory.Ending)
        {
            readCgIdHash = SaveManager.Instance.LoadGallerySaveFile().GetReadEndingData();
        }

        //이미지 하나마다
        while (cgDataList.Count > galleryCgImageList.Count)
        {
            GalleryCgImage galleryCgImage = Instantiate(cgImagePrefab, spawnPos);
            galleryCgImage.OnImageClick += ImageShow;
            galleryCgImageList.Add(galleryCgImage);
        }
        //초기에는 모두 activeFalse 처리해준다.
        foreach (GalleryCgImage item in galleryCgImageList)
        {
            item.gameObject.SetActive(false);
        }
        //이제 cgId에 따라 하나씩 표시, sprite 적용, saveData로 비활성화 표시.
        for (int i = 0; i < cgDataList.Count; i++)
        {
            CGData data = cgDataList[i];
            bool isRead = readCgIdHash.Contains(data.key) ? true : false;
            galleryCgImageList[i].ActiveAndSet(isRead, data);
        }

        // 여기 추가: 레이아웃 강제 갱신 후 스크롤바 상태 재확인
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(spawnPos.GetComponent<RectTransform>());

        // ScrollRect가 이 Content를 참조하는 경우, 스크롤바 갱신을 한 번 더 트리거
        ScrollRect scrollRect = GetComponentInParent<ScrollRect>(); // 혹은 직접 참조 필드로 캐싱해두는게 좋음
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f; // 맨 위로 스크롤 초기화 (선택)
            Canvas.ForceUpdateCanvases();
        }
    }
}
