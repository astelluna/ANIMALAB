using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character Data", fileName = "CharacterData")]
public class CharacterDataSO : ScriptableObject
{
    [field: SerializeField] public string CharacterId { get; private set; }
    //[field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public Sprite Portrait { get; private set; }
    [field: SerializeField] public ExpressionEntry[] Expressions { get; private set; } //몸 전체 보이는 스프라이트
    [field: SerializeField] public Color NameColor { get; private set; }
    [field: SerializeField] public int InitialAffinity { get; private set; } = 0;
    [field: SerializeField] public int MaxAffinity { get; private set; } = 100;
    [field:Header("캐릭터 키")]
    [field: SerializeField] public int BodyHeight { get; private set; } = 800;

    public Sprite GetExpression(eEmotion emotion)
    {
        foreach(var e in Expressions)
        {
            if(e.emotion == emotion)
            {
                return e.sprite;
            }
        }
        //못찾았다면? null 보내자
        Debug.LogError("스프라이트 못찾음");
        return null;
    }
}

[System.Serializable]
public class ExpressionEntry
{
    public eEmotion emotion;
    public Sprite sprite;
}
