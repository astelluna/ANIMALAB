using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

public class CSVDownloader : MonoBehaviour
{
    const string nodeUrl = "https://docs.google.com/spreadsheets/d/1WUSMSFL5T_RXyi9YMNJrnxuez8Bjg5jkzr6KtXq8xQc/export?format=csv&gid=0#gid=0";
    const string choiceUrl = "https://docs.google.com/spreadsheets/d/1WUSMSFL5T_RXyi9YMNJrnxuez8Bjg5jkzr6KtXq8xQc/export?format=csv&gid=905363447";
    const string choiceConditionUrl = "https://docs.google.com/spreadsheets/d/1WUSMSFL5T_RXyi9YMNJrnxuez8Bjg5jkzr6KtXq8xQc/export?format=csv&gid=831253886";
    const string rewardUrl = "https://docs.google.com/spreadsheets/d/1WUSMSFL5T_RXyi9YMNJrnxuez8Bjg5jkzr6KtXq8xQc/export?format=csv&gid=681914727";
    const string cgUrl = "https://docs.google.com/spreadsheets/d/1WUSMSFL5T_RXyi9YMNJrnxuez8Bjg5jkzr6KtXq8xQc/export?format=csv&gid=453507803";
    const string characterSpriteUrl = "https://docs.google.com/spreadsheets/d/1WUSMSFL5T_RXyi9YMNJrnxuez8Bjg5jkzr6KtXq8xQc/export?format=csv&gid=503407958";

    [MenuItem("Tools/Download CSV File & Make JSON")]
    public static void DownloadCSVAndMakeJSON()
    {
        WebClient client = new WebClient();

        client.DownloadFile(nodeUrl, PathManager.nodeFileName);
        client.DownloadFile(choiceUrl, PathManager.choiceFileName);
        client.DownloadFile(choiceConditionUrl,  PathManager.choiceConditionFileName);
        client.DownloadFile(rewardUrl, PathManager.rewardFileName);
        client.DownloadFile(cgUrl, PathManager.cgFileName);
        client.DownloadFile(characterSpriteUrl, PathManager.characterSpriteFileName);
        AssetDatabase.Refresh();
        Debug.Log("CSV 다운로드 완료. " + PathManager.csvPath);

        //이후 JSON으로 변환도 자동으로 해버리자
        CSVToJsonWindow.ReadCSV();
        AssetDatabase.Refresh();
        Debug.Log("CSV -> JSON 변환 완료. " + PathManager.jsonFilePath);
        //RewardSO도 자동화
        AutoRegisterRewardSO();
    }
    private static void AutoRegisterRewardSO()
    {
        DialogData[] jsonData = DialogManager.GetJsonData();

        List<(string, eDialogItemType)> allKeyUsed = new List<(string, eDialogItemType)>();
        foreach (DialogData item in jsonData)
        {
            foreach (DialogReward rewardData in item.reward)
            {
                string key = rewardData.key;
                if (!allKeyUsed.Any(x => x.Item1 == rewardData.key))
                {
                    allKeyUsed.Add((rewardData.key, rewardData.Type));
                }
            }
        }
        RewardIconSO table = AssetDatabase.LoadAssetAtPath<RewardIconSO>("Assets/RewardIconSO.asset");
        if(table == null)
        {
            Debug.LogError("RewardIconSO가 null입니다. 경로를 확인해주세요");
        }
        foreach ((string, eDialogItemType) tuple in allKeyUsed)
        {
            bool exists = table.rewardDataList.Exists(e => e.key == tuple.Item1);
            if (!exists)
            {
                table.rewardDataList.Add(new RewardIconEntry()
                {
                    key = tuple.Item1,
                    type = tuple.Item2,
                    icon = null
                });
                Debug.Log($"누락되 키와 타입 추가= {tuple.Item1}, {tuple.Item2}");
            }
        }
        EditorUtility.SetDirty(table);
        AssetDatabase.SaveAssets();
        Debug.Log("RewardIconSo 동기화 완료");
    }


}
