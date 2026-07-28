using System.Net;
using UnityEditor;
using UnityEngine;

public static class PathManager
{
    //Json
    public const string jsonFilePath = "Assets/Resources/StoryData/StoryData_1.json";
    //CSV
    public const string csvPath = "Assets/Resources/StoryData/CSV/";
    public const string nodeFileName = csvPath + "Node.csv";
    public const string choiceFileName = csvPath + "Choice.csv";
    public const string choiceConditionFileName = csvPath + "Choicecondition.csv";
    public const string rewardFileName = csvPath + "Reward.csv";
    public const string cgFileName = csvPath + "Cg.csv";
}

public class CSVDownloader : MonoBehaviour
{
    const string nodeUrl = "https://docs.google.com/spreadsheets/d/1WUSMSFL5T_RXyi9YMNJrnxuez8Bjg5jkzr6KtXq8xQc/export?format=csv&gid=0#gid=0";
    const string choiceUrl = "https://docs.google.com/spreadsheets/d/1WUSMSFL5T_RXyi9YMNJrnxuez8Bjg5jkzr6KtXq8xQc/export?format=csv&gid=905363447";
    const string choiceConditionUrl = "https://docs.google.com/spreadsheets/d/1WUSMSFL5T_RXyi9YMNJrnxuez8Bjg5jkzr6KtXq8xQc/export?format=csv&gid=831253886";
    const string rewardUrl = "https://docs.google.com/spreadsheets/d/1WUSMSFL5T_RXyi9YMNJrnxuez8Bjg5jkzr6KtXq8xQc/export?format=csv&gid=681914727";
    const string cgUrl = "https://docs.google.com/spreadsheets/d/1WUSMSFL5T_RXyi9YMNJrnxuez8Bjg5jkzr6KtXq8xQc/export?format=csv&gid=453507803";

    [MenuItem("Tools/Download CSV File & Make JSON")]
    public static void DownloadCSVAndMakeJSON()
    {
        WebClient client = new WebClient();

        client.DownloadFile(nodeUrl, PathManager.nodeFileName);
        client.DownloadFile(choiceUrl, PathManager.choiceFileName);
        client.DownloadFile(choiceConditionUrl,  PathManager.choiceConditionFileName);
        client.DownloadFile(rewardUrl, PathManager.rewardFileName);
        client.DownloadFile(cgUrl, PathManager.cgFileName);
        AssetDatabase.Refresh();
        Debug.Log("CSV 다운로드 완료. " + PathManager.csvPath);

        //이후 JSON으로 변환도 자동으로 해버리자
        CSVToJsonWindow.ReadCSV();
        AssetDatabase.Refresh();
        Debug.Log("CSV -> JSON 변환 완료. " + PathManager.jsonFilePath);
    }


}
