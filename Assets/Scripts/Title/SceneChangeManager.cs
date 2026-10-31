using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        //다음 원하는 코드 작성
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
        fadeImage.gameObject.SetActive(false);
    }

    [SerializeField] Image fadeImage;

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }
    IEnumerator FadeAndLoad(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        yield return fadeImage.DOFade(1f, 0.8f).WaitForCompletion();
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return fadeImage.DOFade(0f, 0.8f).WaitForCompletion();
        fadeImage.gameObject.SetActive(false);
    }


}
