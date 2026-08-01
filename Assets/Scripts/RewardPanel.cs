using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItem
{
    public Sprite SpriteImage { get; private set; }
    public string Text { get; private set; }
    public RewardItem(Sprite image, string msg)
    {
        SpriteImage = image;
        Text = msg;
    }
}

public class RewardPanel : MonoBehaviour
{
    public event Action OnPanelClosed;
    Button closeButton;
    [SerializeField] Image rewardImage;
    [SerializeField] TextMeshProUGUI textUI;

    Queue<RewardItem> itemQue = new Queue<RewardItem>();

    private void Awake()
    {
        closeButton = GetComponentInChildren<Button>();
        closeButton.onClick.AddListener(CloseButtonActive);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }

    public void AddDataArray(Sprite image, string text)
    {
        RewardItem rewardItem = new RewardItem(image, text);
        itemQue.Enqueue(rewardItem);
    }

    public void ShowPanel()
    {
        if(itemQue.Count > 0)
        {
            RewardItem rewardItem = itemQue.Dequeue();
            rewardImage.sprite = rewardItem.SpriteImage;
            textUI.text = rewardItem.Text;
        }
    }


    private void CloseButtonActive()
    {
        if(itemQue.Count > 0)
        {
            ShowPanel();
            return;
        }
        
        OnPanelClosed?.Invoke();
    }

}
