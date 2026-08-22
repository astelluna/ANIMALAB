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
public enum eExpression
{
    //기본, 행복, 분노, 슬픔, 놀람, 두려움, 혐오
    none, normal , happy, angry, sad, surprize, fear, aversion, care
}
public enum eCommandType
{
    none, Show, Hide, ClearAll, Effect
}
public enum eSpriteEffect
{
    none, Exclamation, Question, Cheer
}
public enum eSpritePosition
{
    none, Left, Right, CenterLeft, CenterRight
}
public enum eBackgroundType
{
    none,Background,CG
}
public enum eCgCategory
{
    none, cg, Ending
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
    public string textKey; //텍스트 키값 (로컬라이제이션과 매칭) 
    public string EndingKey;
    public DialogCG[] cg;
    public DialogChoice[] choices;
    public DialogReward[] reward;
    public int nextnodeId;
    public string OnEvent; //어떤 이펙트 터져야 한다. 혹은 코드상으로 특별한 처리가 필요하다면 이거 사용
    public CharacterSprite[] spriteDatas;
    public DialogSound[] dialogSounds;
    public ChapterData chapterData;
}

[Serializable]
public class ChapterData
{
    public int chapterId;
    public string title;
    public int startNodeId;
    public string thumbnailKey;

    public bool haveData()
    {
        if(chapterId == 0)
        {
            return false;
        }
        return true;
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
    public string effect;
    public string form;

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
    public eExpression Expression
    {
        get
        {
            if (Enum.TryParse<eExpression>(expression, true, out var result))
            {
                return result;
            }
            return eExpression.none;
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
    public eSpriteEffect Effect
    {
        get
        {
            if (Enum.TryParse<eSpriteEffect>(effect, true, out var result))
            {
                return result;
            }
            return eSpriteEffect.none;
        }
    }
    public eCharacterForm Form
    {
        get
        {
            if (Enum.TryParse<eCharacterForm>(form, true, out var result))
            {
                return result;
            }
            return eCharacterForm.none;
        }
    }
}

[Serializable]
public class DialogCG
{
    public int nodeId;
    public string type;
    public string key; //CG 리소스 키. JSON에 고유하게 넣음
    public string category;
    public eBackgroundType Type
    {
        get
        {
            if (Enum.TryParse<eBackgroundType>(type, true, out var result))
            {
                return result;
            }
            return eBackgroundType.none;
        }
    }
    public eCgCategory Category
    {
        get
        {
            if (Enum.TryParse<eCgCategory>(category, true, out var result))
            {
                return result;
            }
            return eCgCategory.none;
        }
    }
}
[Serializable]
public class DialogChoice
{
    public int nodeId;
    public int choiceIndex;

    public string choiceTextId;
    public int goTo;
    public int goToFail;
    public DialogCondition[] condition; //버튼 클릭 후 안에서 따로 성공/실패 여부를 처리할때 사용
    public DialogCondition[] interActableCondition;  //버튼 클릭 가능 여부 판정
}
[Serializable]
public class DialogCondition
{
    //선택지에서 조건 체크가 필요하다면 이 항목이 있다.
    //public int nodeId;
    //public int choiceIndex;

    public string logic;
    public string type; //어떤 타입인지
    public string key; //
    public int min; 
    public int max;
    //public bool IsInteractable; //DialogChoice에 interActableCondition로 따로 들어가기 때문에 필요없음
    public eLogicType Logic
    {
        get
        {
            if (Enum.TryParse<eLogicType>(logic, true, out var result))
            {
                return result;
            }
            return eLogicType.None;
        }
    }
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
//[Serializable]
//public class ConditionItem
//{
//    //선택지에서 조건 체크가 필요하다면 이 항목이 있다.
//    public string logic;
//    public string type; //어떤 타입인지
//    public string key; //
//    public int min; 
//    public int max;
//    public eLogicType Logic
//    {
//        get
//        {
//            if (Enum.TryParse<eLogicType>(logic, true, out var result))
//            {
//                return result;
//            }
//            return eLogicType.None;
//        }
//    }
//    public eDialogItemType ConditionType
//    {
//        get
//        {
//            if (Enum.TryParse<eDialogItemType>(type, true,out var result))
//            {
//                return result;
//            }
//            return eDialogItemType.None;
//        }
//    }
//
//}
[Serializable]
public class DialogReward
{
    public string type;
    public string key;
    public int amount;

    public eDialogItemType Type
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
[Serializable]
public class DialogSound
{
    public int nodeId;
    public string key;

    public eSoundID Key
    {
        get
        {
            if (Enum.TryParse<eSoundID>(key, true, out var result))
            {
                return result;
            }
            return eSoundID.none;
        }
    }
}

