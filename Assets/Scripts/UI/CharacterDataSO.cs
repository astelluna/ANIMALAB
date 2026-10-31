using System;
using System.Collections.Generic;
using UnityEngine;

//단계마다 Sprite와 ExpressionEntry를 가지고 있게 하자
public enum eCharacterForm
{
    none, normal, beast1, beast2, beastFinal
}
[Serializable]
public class ExpressionEntry
{
    public eExpression emotion;
    public Sprite sprite;
}

[Serializable]
public class CharacterFormSprite
{
    [field: SerializeField] public eCharacterForm Form { get; private set; }
    //[field: SerializeField] public Sprite Portrait { get; private set; } //만약 표정별로 대화로그  초상화를 바꾸고싶다면 쓰자
    [field: SerializeField] public ExpressionEntry[] FullBodyExpressions { get; private set; } //몸 전체 보이는 스프라이트 (표정별로)
}



[CreateAssetMenu(menuName = "Character/Character Data", fileName = "CharacterData")]
public class CharacterDataSO : ScriptableObject
{
    [field: SerializeField] public string CharacterId { get; private set; }

    [field: SerializeField] public List<CharacterFormSprite> characterFormSpriteList { get; private set; } = new List<CharacterFormSprite>();
    [field: SerializeField] public Color NameColor { get; private set; }
    [field: SerializeField] public int InitialAffinity { get; private set; } = 0;
    [field: SerializeField] public int MaxAffinity { get; private set; } = 100;
    [field:Header("캐릭터 키")]
    [field: SerializeField] public int BodyHeight { get; private set; } = 800;
    [field: SerializeField] public Sprite Portrait { get; private set; } //기본 초상화 표정  - 기본 지문에 사용하기 위해 추가

    //검색용 딕셔너리
    private Dictionary<eCharacterForm, CharacterFormSprite> characterFormDict;

    //지문에서 사용하는 지문에 따른 표정을 그대로 표시하는 메서드
    public Sprite GetExpression(eCharacterForm characterForm, eExpression emotion)
    {
        InitDict();
        if (characterFormDict.TryGetValue(characterForm, out CharacterFormSprite characterformData))
        {
            foreach(var e in characterformData.FullBodyExpressions)
            {
                if(e.emotion == emotion)
                {
                    return e.sprite;
                }
            }
        }
        //못찾았다면? null 보내자
        Debug.LogError("스프라이트 못찾음");
        return null;
    }
    private void InitDict()
    {
        if (characterFormDict == null)
        {
            characterFormDict = new Dictionary<eCharacterForm, CharacterFormSprite>();
            foreach (CharacterFormSprite characterformsprite in characterFormSpriteList)
            {
                if (characterFormDict.ContainsKey(characterformsprite.Form))
                {
                    Debug.LogError($"중복된 타입의 폼 데이터가 있습니다");
                    continue;
                }
                characterFormDict.Add(characterformsprite.Form, characterformsprite);

            }
        }
    }
    public Sprite GetPortrait()
    {
        return Portrait;
    }
}
