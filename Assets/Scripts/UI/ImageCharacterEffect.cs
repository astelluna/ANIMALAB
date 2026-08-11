using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ImageCharacterEffect : MonoBehaviour
{
    //두트윈으로 특정 이미지가 들어오면, 해당 이미지를 특정 방식으로 표시를 해준다.
    [SerializeField] Sprite exclamationSprite;
    [SerializeField] Sprite questionSprite;
    [SerializeField] Sprite cheerSprite;

    public void ShowExclamationEffect(Image image)
    {
        image.gameObject.SetActive(true);
        image.sprite = exclamationSprite;
        RectTransform rect = image.rectTransform;

        rect.localScale = Vector3.zero;
        rect.localRotation = Quaternion.identity;
        //image.color = Color.white;

        Sequence seq = DOTween.Sequence();

        // 등장
        seq.Append(rect.DOScale(1f, 0.3f).SetEase(Ease.OutBack));

        // 왼쪽으로 살짝 기울기
        seq.Append(rect.DORotate(new Vector3(0, 0, 20f), 0.15f));

        // 1초 유지
        seq.AppendInterval(1f);

        // 사라짐
        seq.Append(image.DOFade(0f, 0.25f));

        seq.OnComplete(() =>
        {
            image.gameObject.SetActive(false);
        });
    }
    public void ShowQuestionEffect(Image image)
    {
        image.gameObject.SetActive(true);
        image.sprite = questionSprite;
        // 초기 상태
        image.transform.localScale = Vector3.zero;
        image.color = new Color(1, 1, 1, 1);

        Sequence seq = DOTween.Sequence();

        // 0 -> 1 크기로 커짐
        seq.Append(image.transform
            .DOScale(1f, 0.3f)
            .SetEase(Ease.OutBack));

        // 1초 유지
        seq.AppendInterval(1f);

        // 사라짐
        seq.Append(image.DOFade(0f, 0.3f));

        // 끝나면 초기화
        seq.OnComplete(() =>
        {
            image.gameObject.SetActive(false);
            image.color = Color.white;
        });
    }
    public void ShowCheerEffect(Image image)
    {
        image.gameObject.SetActive(true);
        image.sprite = cheerSprite;
        image.transform.localScale = Vector3.zero;
        Sequence seq = DOTween.Sequence();

        // 등장 + 확대
        seq.Append(image.DOFade(1f, 0.2f));
        seq.Join(image.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));

        // 잠시 유지
        seq.AppendInterval(0.3f);

        // 한번 사라짐
        seq.Append(image.DOFade(0f, 0.15f));

        // 다시 나타남
        seq.Append(image.DOFade(1f, 0.15f));

        // 잠시 유지
        seq.AppendInterval(0.3f);

        // 마지막 사라짐
        seq.Append(image.DOFade(0f, 0.3f));

        // 필요하면 끝난 후 비활성화
        seq.OnComplete(() =>
        {
            image.gameObject.SetActive(false);
        });
    }
}
