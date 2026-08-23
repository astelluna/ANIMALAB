using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdsManager : Singleton<AdsManager>
{
    private InterstitialAd interstitialAd;

    // 처음에는 테스트 광고 ID 사용
    private string adUnitId =
        "ca-app-pub-3940256099942544/1033173712";
    protected override void Awake()
    {
        base.Awake();

        MobileAds.Initialize(initStatus =>
        {
            LoadInterstitial();
        });
    }

    //광고 미리 로드

    public void LoadInterstitial()
    {
        AdRequest request = new AdRequest();

        InterstitialAd.Load(
            adUnitId,
            request,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError(
                        $"광고 로드 실패: {error}"
                    );

                    return;
                }

                interstitialAd = ad;

                Debug.Log("전면 광고 로드 완료");
            });
    }

    //광고 띄우기
    public void ShowInterstitial(System.Action onClosed)
    {
        //광고제거 패키지 구매했으면 광고 안띄움
        if (IAPManager.Instance.IsAdRemoved)
        {
            onClosed?.Invoke();
            return;
        }

        //광고 띄우기
        if (interstitialAd != null &&
        interstitialAd.CanShowAd())
        {
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                LoadInterstitial();

                onClosed?.Invoke();
            };

            interstitialAd.Show();
        }
        else
        {
            // 광고가 없으면 그냥 진행
            onClosed?.Invoke();

            LoadInterstitial();
        }
    }
}
