using System;
using UnityEngine;
using UnityEngine.UI;

public class InitConfirmPanel : MonoBehaviour
{
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    public event Action yesButtonClicked;
    private void Awake()
    {
        yesButton.onClick.AddListener(ClickYesButton);
        noButton.onClick.AddListener(ClickNoButton);
    }
    private void OnDestroy()
    {
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
    }

    private void ClickYesButton()
    {
        yesButtonClicked?.Invoke();
        this.gameObject.SetActive(false);
    }
    private void ClickNoButton()
    {
        this.gameObject.SetActive(false);
    }
}
