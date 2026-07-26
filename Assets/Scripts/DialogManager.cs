using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public TMP_Text characterText;
    public TMP_Text dialogText;
    public Button[] choiceButtons;

    DialogData[] allNodes;
    int curId = 0;

    private void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("StoryData/StoryDialog2");

        if (jsonFile == null)
        {
            Debug.LogError("StoryDialog2.json을 Resources 폴더에서 못 찾음. 경로 확인 필요.");
            return;
        }
        string json = jsonFile.text;
        Debug.Log(json);
        allNodes = JsonUtility.FromJson<DialogDataArray>(json).nodes;
        ShowData(1);
    }
    void ShowData(int id)
    {
        curId = id;
        DialogData node = System.Array.Find(allNodes, n => n.id == id);
        dialogText.text = node.text;
        characterText.text = node.speaker;
        //버튼 표시
        foreach (var btn in choiceButtons)
        {
            btn.gameObject.SetActive(false);
        }
        //선택지 표시
        for (int i = 0; i < node.choices.Length; i++)
        {
            int goTo = node.choices[i].goTo;
            choiceButtons[i].gameObject.GetComponentInChildren<TMP_Text>().text = node.choices[i].choiceText;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => ShowData(goTo));
            choiceButtons[i].gameObject.SetActive(true);
        }
    }

}
