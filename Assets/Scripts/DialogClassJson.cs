using System;
using System.Threading;
using UnityEngine;
public enum eLogicType
{
    None, AND, OR
}
public enum eDialogItemType
{
    None, Affinity, Item
}
public enum eEmotion
{
    //기본, 행복, 분노, 슬픔, 놀람, 두려움, 혐오
    none, normal , happy, angry, sad, surprize, fear, aversion
}
public enum eCGEffect
{
    fadeIn,fadeOut,Instant
}

[Serializable]
public class DialogDataArray
{
    public DialogData[] nodes; //nodes 로 해야함. JSON 최상위가 nodes라는 이름
}
[Serializable]
public class DialogData
{
    public int id; //대사 id. 점점 증가할거임.
    public string speaker; //추후에 SO로 바꿀거임. 누가? 말을 하는지
    public string text; //텍스트 대사
    public string expression; //표정
    public DialogCG[] cg;
    public DialogChoice[] choices;
    public DialogReward[] reward;
    public int nextnodeId;
    public string OnEvent; //어떤 이펙트 터져야 한다. 혹은 코드상으로 특별한 처리가 필요하다면 이거 사용
    public CharacterSprite[] spriteDatas;

    public eEmotion Expression
    {
        get
        {
            if (Enum.TryParse<eEmotion>(expression, true, out var result))
            {
                return result;
            }
            return eEmotion.none;
        }
    }
}
[Serializable]
public class CharacterSprite
{
    public int nodeId;
    public string command;
    public string characterId;
    public string expression;
    public string position;

    public eCommandType Command
    {
        get
        {
            if (Enum.TryParse<eCommandType>(command, true, out var result))
            {
                return result;
            }
            return eCommandType.none;
        }
    }
    public eEmotion Expression
    {
        get
        {
            if (Enum.TryParse<eEmotion>(expression, true, out var result))
            {
                return result;
            }
            return eEmotion.none;
        }
    }

    public eSpritePosition Position
    {
        get
        {
            if (Enum.TryParse<eSpritePosition>(position, true, out var result))
            {
                return result;
            }
            return eSpritePosition.none;
        }
    }
}

public enum eCommandType
{
    none, Show, Hide, ClearAll
}
public enum eSpritePosition
{
    none, Left, Right, CenterLeft,CenterRight
}


[Serializable]
public class DialogCG
{
    public string key; //CG 리소스 키. JSON에 고유하게 넣음
    public string effect; //"fadeIn, fadeOut, instant" 등 연출 (서서히 나타남, 서서히 사라짐, 즉시 바뀜)
    public eCGEffect Expression
    {
        get
        {
            if (Enum.TryParse<eCGEffect>(effect, true, out var result))
            {
                return result;
            }
            return eCGEffect.fadeIn;
        }
    }
}
[Serializable]
public class DialogChoice
{
    public string choiceText;
    public int goTo;
    public DialogCondition condition;
}
[Serializable]
public class DialogCondition
{
    public string logic;
    public ConditionItem[] conditions;

    public eLogicType Logic
    {
        get
        {
            if(Enum.TryParse<eLogicType>(logic,true,out var result))
            {
                return result;
            }
            return eLogicType.None;
        }
    }

}
[Serializable]
public class ConditionItem
{
    //선택지에서 조건 체크가 필요하다면 이 항목이 있다.
    public string type; //어떤 타입인지
    public string key; //
    public int min; 
    public int max;

    public eDialogItemType ConditionType
    {
        get
        {
            if (Enum.TryParse<eDialogItemType>(type, true,out var result))
            {
                return result;
            }
            return eDialogItemType.None;
        }
    }

}
[Serializable]
public class DialogReward
{
    public string type;
    public string key;
    public int amount;

    public eDialogItemType RewardType
    {
        get
        {
            if (Enum.TryParse<eDialogItemType>(type, true, out var result))
            {
                return result;
            }
            return eDialogItemType.None;
        }
    }
}

