using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public static class CSVToJsonWindow
{
    static Dictionary<int, DialogData> nodeMap = new Dictionary<int, DialogData>();
    static Dictionary<int, List<DialogChoice>> choiceMap = new Dictionary<int, List<DialogChoice>>();
    static Dictionary<int, List<DialogReward>> rewardMap = new Dictionary<int, List<DialogReward>>();
    static Dictionary<int, List<DialogCG>> cgMap = new Dictionary<int, List<DialogCG>>();
    static Dictionary<int, List<CharacterSprite>> characterSpriteMap = new Dictionary<int, List<CharacterSprite>>();
    static Dictionary<int, List<DialogSound>> dialogSoundMap = new Dictionary<int, List<DialogSound>>();

    private static void Init()
    {
        nodeMap.Clear();
        choiceMap.Clear();
        rewardMap.Clear();
        cgMap.Clear();
        characterSpriteMap.Clear();
        dialogSoundMap.Clear();
    }
    private static void MakeStroyDataJson()
    {
        Init();
        //가장 먼저 맨 최상위 노드부터 만듬
        //string[] nodeLines = File.ReadAllLines(PathManager.nodeFileName);
        List<string[]> nodeLines = ParseCsvFile(PathManager.nodeFileName);
        for (int i = 1; i < nodeLines.Count; i++)
        {
            string[] cols = nodeLines[i];
            DialogData node = new DialogData();
            node.id = int.Parse(cols[0]);
            node.speaker = cols[1];
            node.text = cols[2];
            node.nextnodeId = (cols[4] == "") ? 0 : int.Parse(cols[4]);
            node.OnEvent = cols[5];
            nodeMap.Add(node.id, node);
        }
        //2. choice
        List<string[]> choiceLines = ParseCsvFile(PathManager.choiceFileName);
        for (int i = 1; i < choiceLines.Count; i++)
        {
            string[] cols = choiceLines[i];
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
        List<string[]> conditionLines = ParseCsvFile(PathManager.choiceConditionFileName);
        for (int i = 1; i < conditionLines.Count; i++)
        {
            string[] cols = conditionLines[i];
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
        List<string[]> rewardLines = ParseCsvFile(PathManager.rewardFileName);
        for (int i = 1; i < rewardLines.Count; i++)
        {
            string[] cols = rewardLines[i];
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
        List<string[]> cgLines = ParseCsvFile(PathManager.cgFileName);
        for (int i = 1; i < cgLines.Count; i++)
        {
            string[] cols = cgLines[i];
            int nodeId = int.Parse(cols[0]);
            DialogCG cg = new DialogCG();
            cg.nodeId = nodeId;
            cg.type = cols[1];
            cg.key = cols[2];
            cg.category = cols[3];
            if (!cgMap.ContainsKey(nodeId))
            {
                cgMap[nodeId] = new List<DialogCG>();
            }
            cgMap[nodeId].Add(cg);
        }
        foreach (var pair in cgMap)
        {
            nodeMap[pair.Key].cg = pair.Value.ToArray();
        }
        //6. CharacterSprite
        List<string[]> characterSpriteLines = ParseCsvFile(PathManager.characterSpriteFileName);
        for (int i = 1; i < characterSpriteLines.Count; i++)
        {
            string[] cols = characterSpriteLines[i];

            if (cols[0] == "") continue;
            int nodeId = int.Parse(cols[0]);
            CharacterSprite characterSprite = new CharacterSprite();
            characterSprite.nodeId = nodeId;
            characterSprite.command = cols[1];
            characterSprite.characterId = cols[2];
            characterSprite.expression = cols[3];
            characterSprite.position = cols[4];
            characterSprite.effect = cols[5];
            characterSprite.form = cols[6];
            if (!characterSpriteMap.ContainsKey(nodeId))
            {
                characterSpriteMap[nodeId] = new List<CharacterSprite>();
            }
            characterSpriteMap[nodeId].Add(characterSprite);
        }
        foreach (var pair in characterSpriteMap)
        {
            nodeMap[pair.Key].spriteDatas = pair.Value.ToArray();
        }
        //7. DialogSound
        List<string[]> dialogSounds = ParseCsvFile(PathManager.dialogSoundFileName);
        for (int i = 1; i < dialogSounds.Count; i++)
        {
            string[] cols = dialogSounds[i];

            if (cols[0] == "") continue;
            int nodeId = int.Parse(cols[0]);
            DialogSound dialogSound = new DialogSound();
            dialogSound.nodeId = nodeId;
            dialogSound.key = cols[1];

            if (!dialogSoundMap.ContainsKey(nodeId))
            {
                dialogSoundMap[nodeId] = new List<DialogSound>();
            }
            dialogSoundMap[nodeId].Add(dialogSound);
        }
        foreach (var pair in dialogSoundMap)
        {
            nodeMap[pair.Key].dialogSounds = pair.Value.ToArray();
        }
        //8. ChapterData
        List<string[]> chapterDatas = ParseCsvFile(PathManager.chapterDataFileName);
        for (int i = 1; i < chapterDatas.Count; i++)
        {
            string[] cols = chapterDatas[i];

            if (cols[0] == "") continue;
            int chapterId = int.Parse(cols[2]);
            ChapterData chapterData = new ChapterData();
            chapterData.chapterId = int.Parse(cols[0]);
            chapterData.title = cols[1];
            chapterData.startNodeId = int.Parse(cols[2]);
            chapterData.thumbnailKey = cols[3];
            //클래스 넣어줌
            nodeMap[chapterId].chapterData = chapterData;
        }
        //마지막 JSON 저장
        DialogDataArray data = new DialogDataArray();
        data.nodes = nodeMap.Values.OrderBy(x => x.id).ToArray();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(PathManager.storyJsonFilePath, json);
    }
    private static void MakeItemDataJson()
    {
        //아이템 정보만 따로 json파일로 저장한다 (스토리 노드와 관련이 없어서 따로 저장)
        List<string[]> itemDataList = ParseCsvFile(PathManager.itemDataFileName);
        List<ItemClassJson> itemClassList = new List<ItemClassJson>();
        for (int i = 1; i < itemDataList.Count; i++)
        {
            string[] cols = itemDataList[i];

            if (cols[0] == "") continue;
            ItemClassJson itemClass = new ItemClassJson();
            itemClass.key = cols[0];
            itemClass.explain = cols[1];
            itemClassList.Add(itemClass);
        }

        ItemDataJson data = new ItemDataJson();
        data.itemClassList = new List<ItemClassJson>(itemClassList);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(PathManager.itemJsonFilePath, json);
    }

    public static void ReadCSV()
    {
        MakeStroyDataJson();
        MakeItemDataJson();
    }
    // ── RFC4180 규격 CSV 파서 ──────────────────────────────
    // 큰따옴표로 감싼 필드 안의 콤마, 줄바꿈, 이스케이프된 큰따옴표("")를
    // 전부 올바르게 처리합니다. File.ReadAllLines + Split(',') 조합은
    // 필드 안에 줄바꿈이 있으면 행이 깨지므로 반드시 이 방식을 써야 합니다.
    private static List<string[]> ParseCsvFile(string path)
    {
        string content = File.ReadAllText(path, Encoding.UTF8);
        List<string[]> rows = new List<string[]>();
        List<string> currentRow = new List<string>();
        StringBuilder field = new StringBuilder();
        bool inQuotes = false;

        if (content.Length > 0 && content[0] == '\uFEFF')
            content = content.Substring(1);

        for (int i = 0; i < content.Length; i++)
        {
            char c = content[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < content.Length && content[i + 1] == '"')
                    {
                        field.Append('"'); // 이스케이프된 "" -> "
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    field.Append(c); // 따옴표 안의 줄바꿈/콤마도 그냥 필드에 포함
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == ',')
                {
                    currentRow.Add(field.ToString());
                    field.Clear();
                }
                else if (c == '\r')
                {
                    // 무시 (\n에서 처리)
                }
                else if (c == '\n')
                {
                    currentRow.Add(field.ToString());
                    field.Clear();
                    rows.Add(currentRow.ToArray());
                    currentRow = new List<string>();
                }
                else
                {
                    field.Append(c);
                }
            }
        }

        if (field.Length > 0 || currentRow.Count > 0)
        {
            currentRow.Add(field.ToString());
            rows.Add(currentRow.ToArray());
        }

        return rows;
    }
}
