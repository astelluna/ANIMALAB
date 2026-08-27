using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NodeConditionChecker
{
    public static int CheckChoiceCondition(DialogChoice dialogChoice)
    {
        if(IsSuccessCondition(dialogChoice.condition))
        {
            return dialogChoice.goTo;
        }
        else
        {
            return dialogChoice.goToFail;
        }
    }
    public static bool CheckButtonInteractable(DialogChoice dialogChoice)
    {
        return IsSuccessCondition(dialogChoice.interActableCondition);
    }
    private static bool IsSuccessCondition(DialogCondition[] dialogConditions)
    {
        if (dialogConditions == null || dialogConditions.Length == 0)
        {
            return true; // 조건 없으면 무조건 통과
        }
        List<DialogCondition> andItemList = dialogConditions.Where(w => w.Logic == eLogicType.AND).ToList();
        List<DialogCondition> orItemList = dialogConditions.Where(w => w.Logic == eLogicType.OR).ToList();

        //AND 그룹: 항목이 있고, 전부 만족해야 true. 항목 없으면 false(해당없음)
        bool andPassed = false;
        if(andItemList.Count > 0)
        {
            andPassed = true;
            foreach(var item in andItemList)
            {
                if(!CheckCondition(item))
                {
                    andPassed = false;
                    break;
                }
            }
        }

        //OR 그룹: 하나라도 만족하면 true;
        bool orPassed = false;
        if(orItemList.Count > 0)
        {
            foreach(var item in orItemList)
            {
                if(CheckCondition(item))
                {
                    orPassed = true;
                    break;
                }
            }
        }
        return andPassed || orPassed;
    }
    private static bool CheckCondition(DialogCondition conditionData)
    {
        SaveData saveData = SaveManager.Instance.LoadSaveFile();
        var log = conditionData.Logic;
        bool isTrue = true;

        eDialogItemType type = conditionData.ConditionType;
        switch (type)
        {
            case eDialogItemType.Affinity:
                IReadOnlyDictionary<string, AffinityData> affinityDict = saveData.AffinityDict;
                if (affinityDict.ContainsKey(conditionData.key))
                {
                    AffinityData affinityData = affinityDict[conditionData.key];
                    bool conditionCheck = (conditionData.min <= affinityData.amount && affinityData.amount <= conditionData.max);
                    isTrue &= conditionCheck;
                }
                else
                {
                    Debug.LogError("세이브에 호감도 데이터없음");
                }
                break;
            case eDialogItemType.Item:
                IReadOnlyDictionary<string, SaveItem> haveItemDict = saveData.SaveItemDict;
                if (haveItemDict.ContainsKey(conditionData.key))
                {
                    SaveItem saveItem = haveItemDict[conditionData.key];
                    bool conditionCheck = (conditionData.min <= saveItem.amount && saveItem.amount <= conditionData.max);
                    isTrue &= conditionCheck;
                }
                else
                {
                    //아이템 존재하지않는다면 조건 false임
                    Debug.Log("세이브에 아이템 데이터없음");
                    return false;
                }
                break;
            default:
                Debug.LogError("정의되지 않은 ConditionType: " + conditionData.type);
                isTrue = false;
                break;
        }
        return isTrue;
    }
}
