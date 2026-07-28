using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class CSVToJsonWindow
{
    static Dictionary<int, DialogData> nodeMap = new Dictionary<int, DialogData>();
    static Dictionary<int, List<DialogChoice>> choiceMap = new Dictionary<int, List<DialogChoice>>();
    static Dictionary<int, List<DialogReward>> rewardMap = new Dictionary<int, List<DialogReward>>();
    static Dictionary<int, List<DialogCG>> cgMap = new Dictionary<int, List<DialogCG>>();

    private static void Init()
    {
        nodeMap.Clear();
        choiceMap.Clear();
        rewardMap.Clear();
        cgMap.Clear();
    }
    public static void ReadCSV()
    {
        Init();

        //가장 먼저 맨 최상위 노드부터 만듬
        string[] nodeLines = File.ReadAllLines(PathManager.nodeFileName);
        for (int i = 1; i < nodeLines.Length; i++)
        {
            string[] cols = nodeLines[i].Split(',');
            DialogData node = new DialogData();
            node.id = int.Parse(cols[0]);
            node.speaker = cols[1];
            node.text = cols[2];
            node.expression = cols[3];
            node.OnEvent = cols[4];
            nodeMap.Add(node.id, node);
        }
        //2. choice
        string[] choiceLines = File.ReadAllLines(PathManager.choiceFileName);
        for (int i = 1; i < choiceLines.Length; i++)
        {
            string[] cols = choiceLines[i].Split(',');
            int nodeId = int.Parse(cols[0]);

            DialogChoice choice = new DialogChoice();
            choice.choiceText = cols[2];
            choice.goTo = int.Parse(cols[3]);
            if (!choiceMap.ContainsKey(nodeId))
            {
                choiceMap[nodeId] = new List<DialogChoice>();
            }
            choiceMap[nodeId].Add(choice);
        }
        foreach (var pair in choiceMap)
        {
            nodeMap[pair.Key].choices = pair.Value.ToArray();
        }
        //3. condition
        string[] conditionLines = File.ReadAllLines(PathManager.choiceConditionFileName);
        for (int i = 1; i < conditionLines.Length; i++)
        {
            string[] cols = conditionLines[i].Split(',');
            int nodeId = int.Parse(cols[0]);
            int choiceIndex = int.Parse(cols[1]);

            DialogChoice choice = nodeMap[nodeId].choices[choiceIndex];

            if (choice.condition == null)
            {
                choice.condition = new DialogCondition();
                choice.condition.logic = cols[2];
                choice.condition.conditions = new ConditionItem[0];
            }
            ConditionItem item = new ConditionItem();
            item.type = cols[3];
            item.key = cols[4];
            item.min = int.Parse(cols[5]);
            item.max = int.Parse(cols[6]);

            // 배열이라 Add가 안되므로 List 사용
            List<ConditionItem> list = choice.condition.conditions.ToList();
            list.Add(item);
            choice.condition.conditions = list.ToArray();
        }
        //4. Reward
        string[] rewardLines = File.ReadAllLines(PathManager.rewardFileName);
        for (int i = 1; i < rewardLines.Length; i++)
        {
            string[] cols = rewardLines[i].Split(',');
            int nodeId = int.Parse(cols[0]);
            DialogReward reward = new DialogReward();
            reward.type = cols[1];
            reward.key = cols[2];
            reward.amount = int.Parse(cols[3]);
            if (!rewardMap.ContainsKey(nodeId))
            {
                rewardMap[nodeId] = new List<DialogReward>();
            }
            rewardMap[nodeId].Add(reward);
        }
        //넣기
        foreach (var pair in rewardMap)
        {
            nodeMap[pair.Key].reward = pair.Value.ToArray();
        }
        //5. CG
        string[] cgLines = File.ReadAllLines(PathManager.cgFileName);
        for (int i = 1; i < cgLines.Length; i++)
        {
            string[] cols = cgLines[i].Split(',');
            int nodeId = int.Parse(cols[0]);
            DialogCG cg = new DialogCG();
            cg.key = cols[1];
            cg.effect = cols[2];
            if(!cgMap.ContainsKey(nodeId))
            {
                cgMap[nodeId] = new List<DialogCG>();
            }
            cgMap[nodeId].Add(cg);
        }
        foreach(var pair in cgMap)
        {
            nodeMap[pair.Key].cg = pair.Value.ToArray();
        }
        //마지막 JSON 저장
        DialogDataArray data = new DialogDataArray();
        data.nodes = nodeMap.Values.OrderBy(x => x.id).ToArray();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(PathManager.jsonFilePath, json);
    }
}
