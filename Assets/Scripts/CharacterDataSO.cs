using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character Data", fileName = "CharacterData")]
public class CharacterDataSO : ScriptableObject
{
    [field: SerializeField] public string CharacterId { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public Sprite Portrait { get; private set; }
    [field: SerializeField] public Sprite[] Expressions { get; private set; }
    [field: SerializeField] public Color NameColor { get; private set; }
    [field: SerializeField] public int InitialAffinity { get; private set; } = 0;
    [field: SerializeField] public int MaxAffinity { get; private set; } = 100;
}
