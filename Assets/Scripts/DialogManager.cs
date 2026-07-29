using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class DialogManager : MonoBehaviour
{
    public TMP_Text characterText;
    public TMP_Text dialogText;
    public Button[] choiceButtons;

    DialogData[] allNodes;
    int curId = 0;

    bool impossibleMoveNextDialog = false;
    const string ellipsisToken = "\uE000"; // 텍스트에 안 쓰이는 특수 문자 하나
    //어떤 문자열이 주어졌다고 가정하자. "안녕하세요. 오늘 날씨가 좋아요. 나가실건가요?";
    //이런 문자열이 마우스 클릭을 하면?. 하나씩 실행된다고 가정하자. 일단 string으로 하자. SB쓰지말고 하기.
    //추후에는 글자가 확 나타나는게 아닌 서서히 나타나도록. 클릭하면 바로 딱 출력되도록.
    Queue<string> stringQue = new Queue<string>();
    public void ShowStr(string str)
    {
        dialogText.text = "";
        string temp = Regex.Replace(str, @"\.{2,}", ellipsisToken);
        // 2. 마침표/느낌표/물음표 뒤에 공백이 올 때만 문장을 나눔
        string[] raw = Regex.Split(temp, @"(?<=[.!?])\s+");
        // 3. 치환했던 말줄임표 복원 + 빈 문자열 제거
        string[] sentences = raw
            .Select(s => s.Replace(ellipsisToken, "..."))
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();

        foreach(var item in sentences)
        {
            stringQue.Enqueue(item);
        }
        float time = 0;
        foreach(var item in stringQue)
        {
            Invoke(nameof(Func), time);
            time++;
        }
    }
    private void Func()
    {
        if (stringQue.Count == 0) return;

        string buf = stringQue.Dequeue();
        dialogText.text = dialogText.text + " " + buf;
    }



    private void Start()
    {
        string path = PathManager.jsonFilePath;
        string resourcePath = path
    .Replace("Assets/Resources/", "")
    .Replace(".json", "");
        TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);

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
        //node의 string 정보를 캐릭터 정보로 바꿔야함.
        CharacterDataSO characterData = GetPeopleSOData(node.speaker, node.Expression);


        ShowStr(node.text); //테스트용
        //dialogText.text = node.text;
        characterText.text = characterData.DisplayName;
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
        //TODO
        //캐릭터의 초상화, Sprite 이미지 등 띄워야 한다
    }
    CharacterDataSO GetPeopleSOData(string name, eEmotion emotionType)
    {
        //TODO
        CharacterDataSO characterData = ResourceManager.Instance.GetCharacterData(name);
        if(characterData == null)
        {
            Debug.LogError($"{name}의 캐릭터SO 데이터가 없습니다. 에러");
            return null;
        }
        return characterData;
    }

}
