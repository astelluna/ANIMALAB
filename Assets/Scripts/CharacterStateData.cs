using System.Collections.Generic;
using UnityEngine;

public class CharacterStateData
{
    //캐릭터들이 현재 가지고있는 형태를 기억하기 위해서 사용
    public string CharacterId { get; private set; }
    public eSpritePosition Position{ get; private set; }
    public eCharacterForm CharacterForm{ get; private set; }
    public eExpression Expression { get; private set; }


    //Dictionary<string, eCharacterForm> characterFormDict = new Dictionary<string, eCharacterForm>();

    public CharacterStateData(string id, eSpritePosition pos, eCharacterForm form, eExpression emotion)
    {
        CharacterId = id;
        Position = pos;
        CharacterForm = form;
        Expression = emotion;
    }
}
