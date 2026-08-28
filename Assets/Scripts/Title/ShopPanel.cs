using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] Button buyAdRemoveButton;
    [SerializeField] TMP_Text buyAdRemoveButtonText;

    [Header("Localization")]
    [SerializeField] LocalizedString removeAdsDefaultText;   // shop.remove_ads.default
    [SerializeField] LocalizedString removeAdsWithPriceText; // shop.remove_ads.with_price
    [SerializeField] LocalizedString removeAdsPurchasedText; // shop.remove_ads.purchased

    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        buyAdRemoveButton.onClick.AddListener(OnBuyAdRemoveButtonClick);

        IAPManager.Instance.OnRemoveAdsProductReady += UpdateButtonState;
        IAPManager.Instance.OnRemoveAdsPurchased += UpdateButtonState;

        // 언어가 바뀌면 (설정에서 변경 등) 버튼 텍스트도 갱신
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        buyAdRemoveButton.onClick.RemoveAllListeners();

        if (IAPManager.Instance != null)
        {
            IAPManager.Instance.OnRemoveAdsProductReady -= UpdateButtonState;
            IAPManager.Instance.OnRemoveAdsPurchased -= UpdateButtonState;
        }

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    public void OpenPanel()
    {
        this.gameObject.SetActive(true);
        UpdateButtonState();
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        UpdateButtonState();
    }

    private async void UpdateButtonState()
    {
        if (IAPManager.Instance.IsAdRemoved)
        {
            buyAdRemoveButtonText.text = await removeAdsPurchasedText.GetLocalizedStringAsync().Task;
            buyAdRemoveButton.interactable = false;
            return;
        }

        var price = IAPManager.Instance.RemoveAdsPriceString;

        if (string.IsNullOrEmpty(price))
        {
            buyAdRemoveButtonText.text = await removeAdsDefaultText.GetLocalizedStringAsync().Task;
        }
        else
        {
            // Smart Format 인자로 가격 전달
            removeAdsWithPriceText.Arguments = new object[] { price };
            buyAdRemoveButtonText.text = await removeAdsWithPriceText.GetLocalizedStringAsync().Task;
        }

        buyAdRemoveButton.interactable = true;
    }

    private void OnCloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }

    private void OnBuyAdRemoveButtonClick()
    {
        IAPManager.Instance.BuyRemoveAds();
    }
}