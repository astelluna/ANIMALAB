using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterShowPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI chapterText;
    [SerializeField] TextMeshProUGUI chapterNameText;

    Image panelImage;

    public event Action OnPanelEnd;

    //public bool IsChapterPanelShow { get; private set; }
    public bool IsChapterPanelShow => gameObject.activeInHierarchy;

    private void Awake()
    {
        panelImage = GetComponent<Image>();
        Color color = panelImage.color;
        color.a = 0;
        panelImage.color = color;
    }

    public void SetChapterInfo(int chapterNo, string nameText, Action OnPanelShowEnd)
    {
        this.gameObject.SetActive(true);
        chapterText.text = "Chapter " + chapterNo;
        chapterNameText.text = nameText;
        Show(OnPanelShowEnd);
    }

    private void Show(Action OnPanelShowEnd)
    {
        // 초기 상태
        float initAlpha = 0.5f;
        Color color = panelImage.color;
        color.a = initAlpha;
        panelImage.color = color;
        Color textColor = chapterText.color;
        textColor.a = 1f;
        chapterText.color = textColor;
        Color nameColor = chapterNameText.color;
        nameColor.a = 1f;
        chapterNameText.color = nameColor;

        chapterText.transform.localScale = Vector3.one * 0.7f;
        chapterNameText.transform.localScale = Vector3.one * 0.7f;

        Sequence seq = DOTween.Sequence();

        // 패널 페이드 인
        seq.Append(panelImage.DOFade(1f, 0.4f));

        // 텍스트 팝업
        seq.Join(chapterText.transform.DOScale(1f, 0.5f)
            .SetEase(Ease.OutBack));
        seq.Join(chapterNameText.transform.DOScale(1f, 0.5f)
            .SetEase(Ease.OutBack));

        // 잠깐 유지
        seq.AppendInterval(2f);
        // 미리 이벤트 호출 (끝나고 호출하면 너무 느림)
        seq.AppendCallback(() => OnPanelShowEnd?.Invoke());

        // 패널 페이드 아웃
        seq.Append(panelImage.DOFade(0f, 0.8f));
        seq.Join(chapterText.DOFade(0f, 0.8f));
        seq.Join(chapterNameText.DOFade(0f, 0.8f));

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
