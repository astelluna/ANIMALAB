using TMPro;
using UnityEngine;

public class ItemSlotText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemExplain;

    public void SetItemSlotText(string name, string amount, string explain)
    {
        itemName.text = name + " (" + amount + ")";
        itemExplain.text = explain;
    }

    public void ClearText()
    {
        itemName.text = "";
        itemExplain.text = "";
    }

}
