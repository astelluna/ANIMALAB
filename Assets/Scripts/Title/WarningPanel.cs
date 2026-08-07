using System;
using UnityEngine;
using UnityEngine.UI;

public class WarningPanel : MonoBehaviour
{
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    public event Action OnYesButtonClick;

    private void Awake()
    {
        yesButton.onClick.AddListener(YesButtonClick);
        noButton.onClick.AddListener(NoButtonClick);
    }
    private void OnDestroy()
    {
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
    }

    private void YesButtonClick()
    {
        OnYesButtonClick?.Invoke();
        this.gameObject.SetActive(false);
    }
    private void NoButtonClick()
    {
        this.gameObject.SetActive(false);
    }
}
