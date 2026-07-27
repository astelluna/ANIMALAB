using UnityEngine;

[CreateAssetMenu(menuName = "Item/Item Data", fileName = "ItemData")]
public class ItemDataSO : ScriptableObject
{
    [field: SerializeField] public string ItemId { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }
}
