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
    public void ShowWarningPanel(Action action)
    {
        OnYesButtonClick = action;
        this.gameObject.SetActive(true);
    }

    private void YesButtonClick()
    {
        OnYesButtonClick?.Invoke(); 
        OnYesButtonClick = null;
        this.gameObject.SetActive(false);
        
    }
    private void NoButtonClick()
    {
        OnYesButtonClick = null;
        this.gameObject.SetActive(false);

    }
}
