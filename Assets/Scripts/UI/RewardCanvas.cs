using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardCanvas : MonoBehaviour
{
    //어떤 보상을 띄워야 한다고 알림이 온다. 그러면? 아이콘을 띄운다. 
    [SerializeField] RewardIconSO rewardIconSO;
    RewardPanel rewardPanel;

    public bool IsRewardPanelShow { get; private set; }

    private void Awake()
    {
        rewardPanel = GetComponentInChildren<RewardPanel>(true);
        rewardPanel.OnPanelClosed += Close;
    }
    private void OnDestroy()
    {
        rewardPanel.OnPanelClosed -= Close;
    }
    public void Show(DialogReward[] dialogRewards)
    {
        bool isHaveItem = false;
        foreach(DialogReward item in dialogRewards)
        {
            if (item.Type == eDialogItemType.Item)
            {
                isHaveItem = true;
                Sprite itemSprite = ResourceManager.Instance.GetItemData(item.key).Icon;
                string itemDesc = ResourceManager.Instance.GetLocalizationText(item.key);
                rewardPanel.AddDataArray(itemSprite, itemDesc);
            }
        }
        if(isHaveItem)
        {
            rewardPanel.gameObject.SetActive(true);
            rewardPanel.ShowPanel();
            IsRewardPanelShow = true;
        }
    }
    public void Close()
    {
        IsRewardPanelShow = false;
        rewardPanel.gameObject.SetActive(false);
    }
}
