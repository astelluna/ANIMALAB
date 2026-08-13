using System;
using System.Collections.Generic;
using UnityEngine;

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
        HashSet<string> readCgIdHash = SaveManager.Instance.LoadSaveFile().GetReadCGData();
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
    }
}
