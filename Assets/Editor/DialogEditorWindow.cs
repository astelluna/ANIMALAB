using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DialogEditorWindow : EditorWindow
{
    private DialogDataArray _data;
    private string _filePath;
    private Vector2 _listScroll;
    private Vector2 _detailScroll;
    private int _selectedIndex = -1;

    private CharacterDataSO[] _allCharacters;
    private ItemDataSO[] _allItems;

    [MenuItem("Tools/Dialog Editor")]
    public static void Open()
    {
        GetWindow<DialogEditorWindow>("Dialog Editor");
    }

    private void OnEnable()
    {
        _filePath = Path.Combine(Application.dataPath, "Resources/StoryData/StoryDialog2.json");
        if (File.Exists(_filePath))
            LoadFile();

        LoadAllCharacters();
        LoadAllItems();
    }

    private void OnGUI()
    {
        DrawToolbar();

        if (_data == null)
        {
            EditorGUILayout.HelpBox("JSON 파일을 먼저 로드하세요.", MessageType.Info);
            return;
        }

        EditorGUILayout.BeginHorizontal();
        DrawNodeList();
        DrawNodeDetail();
        EditorGUILayout.EndHorizontal();
    }

    // ---------- 상단 툴바 ----------
    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("파일 열기", EditorStyles.toolbarButton))
        {
            string path = EditorUtility.OpenFilePanel("Dialog JSON 열기", Application.dataPath, "json");
            if (!string.IsNullOrEmpty(path))
            {
                _filePath = path;
                LoadFile();
            }
        }

        if (GUILayout.Button("저장", EditorStyles.toolbarButton))
        {
            SaveFile();
        }

        if (GUILayout.Button("캐릭터/아이템 새로고침", EditorStyles.toolbarButton))
        {
            LoadAllCharacters();
            LoadAllItems();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField(_filePath, EditorStyles.miniLabel);

        EditorGUILayout.EndHorizontal();
    }

    // ---------- 좌측: 노드 리스트 ----------
    private void DrawNodeList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        EditorGUILayout.LabelField("노드 목록", EditorStyles.boldLabel);

        _listScroll = EditorGUILayout.BeginScrollView(_listScroll, GUILayout.ExpandHeight(true));

        for (int i = 0; i < _data.nodes.Length; i++)
        {
            var node = _data.nodes[i];
            string label = $"#{node.id} [{node.speaker}]";

            GUI.backgroundColor = (i == _selectedIndex) ? Color.cyan : Color.white;
            if (GUILayout.Button(label, GUILayout.Height(24)))
            {
                _selectedIndex = i;
                GUI.FocusControl(null);
            }
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        if (GUILayout.Button("+ 노드 추가"))
            AddNode();

        if (_selectedIndex >= 0 && GUILayout.Button("- 선택 노드 삭제"))
            DeleteNode(_selectedIndex);

        EditorGUILayout.EndVertical();
    }

    // ---------- 우측: 상세 편집 ----------
    private void DrawNodeDetail()
    {
        EditorGUILayout.BeginVertical();

        if (_selectedIndex < 0 || _selectedIndex >= _data.nodes.Length)
        {
            EditorGUILayout.LabelField("좌측에서 노드를 선택하세요.");
            EditorGUILayout.EndVertical();
            return;
        }

        _detailScroll = EditorGUILayout.BeginScrollView(_detailScroll);

        var node = _data.nodes[_selectedIndex];

        EditorGUILayout.LabelField($"노드 #{node.id} 편집", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        node.id = EditorGUILayout.IntField("ID", node.id);

        DrawSpeakerDropdown(node);

        EditorGUILayout.LabelField("대사");
        node.text = EditorGUILayout.TextArea(node.text, GUILayout.Height(50));

        node.expression = EditorGUILayout.EnumPopup("표정", node.Expression).ToString();
        node.OnEvent = EditorGUILayout.TextField("OnEvent", node.OnEvent);

        EditorGUILayout.Space();
        DrawCG(node);

        EditorGUILayout.Space();
        DrawChoices(node);

        EditorGUILayout.Space();
        DrawRewards(node);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    // ---------- 화자 드롭다운 ----------
    private void DrawSpeakerDropdown(DialogData node)
    {
        if (_allCharacters == null || _allCharacters.Length == 0)
        {
            EditorGUILayout.HelpBox("CharacterDataSO가 프로젝트에 없습니다.", MessageType.Warning);
            node.speaker = EditorGUILayout.TextField("화자 (speaker)", node.speaker);
            return;
        }

        string[] ids = _allCharacters.Select(c => c.CharacterId).ToArray();
        int currentIndex = System.Array.IndexOf(ids, node.speaker);
        int newIndex = EditorGUILayout.Popup("화자 (speaker)", currentIndex, ids);

        if (newIndex >= 0 && newIndex < ids.Length)
            node.speaker = ids[newIndex];
    }

    // ---------- CG 편집 ----------
    private void DrawCG(DialogData node)
    {
        EditorGUILayout.LabelField("CG (클릭 시 순서대로 표시)", EditorStyles.boldLabel);

        node.cg ??= new DialogCG[0];

        for (int i = 0; i < node.cg.Length; i++)
        {
            var cg = node.cg[i];
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField($"#{i + 1}번째 CG (클릭하면 다음으로)");
            cg.key = EditorGUILayout.TextField("Key", cg.key);
            cg.effect = EditorGUILayout.EnumPopup("Effect", cg.Expression).ToString();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = i > 0;
            if (GUILayout.Button("위로 ↑"))
            {
                (node.cg[i], node.cg[i - 1]) = (node.cg[i - 1], node.cg[i]);
            }
            GUI.enabled = i < node.cg.Length - 1;
            if (GUILayout.Button("아래로 ↓"))
            {
                (node.cg[i], node.cg[i + 1]) = (node.cg[i + 1], node.cg[i]);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("이 CG 삭제"))
            {
                node.cg = RemoveAt(node.cg, i);
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("+ CG 추가"))
        {
            node.cg = Append(node.cg, new DialogCG { key = "", effect = "fadeIn" });
        }
    }

    // ---------- Choices 편집 ----------
    private void DrawChoices(DialogData node)
    {
        EditorGUILayout.LabelField("선택지 (Choices)", EditorStyles.boldLabel);

        node.choices ??= new DialogChoice[0];

        for (int i = 0; i < node.choices.Length; i++)
        {
            var choice = node.choices[i];
            EditorGUILayout.BeginVertical("box");

            choice.choiceText = EditorGUILayout.TextField("선택지 텍스트", choice.choiceText);
            choice.goTo = EditorGUILayout.IntField("이동할 노드 ID (goTo)", choice.goTo);

            DrawCondition(choice);

            if (GUILayout.Button("이 선택지 삭제"))
            {
                node.choices = RemoveAt(node.choices, i);
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("+ 선택지 추가"))
        {
            node.choices = Append(node.choices, new DialogChoice { choiceText = "새 선택지", goTo = node.id });
        }
    }

    // ---------- Condition 편집 ----------
    private void DrawCondition(DialogChoice choice)
    {
        bool hasCondition = choice.condition != null;   // 수정: Length 체크 제거
        bool newHasCondition = EditorGUILayout.Toggle("조건 사용", hasCondition);

        if (newHasCondition && choice.condition == null)
            choice.condition = new DialogCondition { logic = "AND", conditions = new ConditionItem[0] };
        else if (!newHasCondition && choice.condition != null)
            choice.condition = null;

        if (choice.condition == null) return;

        EditorGUI.indentLevel++;
        choice.condition.logic = EditorGUILayout.EnumPopup("Logic", choice.condition.Logic).ToString();

        choice.condition.conditions ??= new ConditionItem[0];

        for (int i = 0; i < choice.condition.conditions.Length; i++)
        {
            var cond = choice.condition.conditions[i];
            EditorGUILayout.BeginVertical("helpBox");

            cond.type = EditorGUILayout.EnumPopup("Type", cond.ConditionType).ToString();
            DrawKeyField(cond.ConditionType, cond);
            cond.min = EditorGUILayout.IntField("Min", cond.min);
            cond.max = EditorGUILayout.IntField("Max", cond.max);

            if (GUILayout.Button("조건 삭제"))
            {
                choice.condition.conditions = RemoveAt(choice.condition.conditions, i);
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
                return;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("+ 조건 추가"))
        {
            choice.condition.conditions = Append(choice.condition.conditions, new ConditionItem { type = "affinity" });
        }

        EditorGUI.indentLevel--;
    }

    // ---------- Reward 편집 ----------
    private void DrawRewards(DialogData node)
    {
        EditorGUILayout.LabelField("보상 (Reward)", EditorStyles.boldLabel);

        node.reward ??= new DialogReward[0];

        for (int i = 0; i < node.reward.Length; i++)
        {
            var r = node.reward[i];
            EditorGUILayout.BeginVertical("box");

            r.type = EditorGUILayout.EnumPopup("Type", r.Type).ToString();
            DrawKeyField(r.Type, r);
            r.amount = EditorGUILayout.IntField("Amount", r.amount);

            if (GUILayout.Button("보상 삭제"))
            {
                node.reward = RemoveAt(node.reward, i);
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("+ 보상 추가"))
        {
            node.reward = Append(node.reward, new DialogReward { type = "item" });
        }
    }

    // ---------- Key 필드 공용 (캐릭터/아이템 드롭다운) ----------
    // ConditionItem과 DialogReward 둘 다 "key" 필드를 가지므로 object로 받아 처리
    private void DrawKeyField(eDialogItemType type, ConditionItem cond)
    {
        cond.key = DrawKeyFieldInternal(type, cond.key);
    }

    private void DrawKeyField(eDialogItemType type, DialogReward reward)
    {
        reward.key = DrawKeyFieldInternal(type, reward.key);
    }

    private string DrawKeyFieldInternal(eDialogItemType type, string currentKey)
    {
        if (type == eDialogItemType.Affinity)
        {
            if (_allCharacters == null || _allCharacters.Length == 0)
                return EditorGUILayout.TextField("Key (캐릭터)", currentKey);

            string[] ids = _allCharacters.Select(c => c.CharacterId).ToArray();
            int idx = System.Array.IndexOf(ids, currentKey);
            int newIdx = EditorGUILayout.Popup("Key (캐릭터)", idx, ids);
            return (newIdx >= 0 && newIdx < ids.Length) ? ids[newIdx] : currentKey;
        }
        else if (type == eDialogItemType.Item)
        {
            if (_allItems == null || _allItems.Length == 0)
                return EditorGUILayout.TextField("Key (아이템)", currentKey);

            string[] ids = _allItems.Select(it => it.ItemId).ToArray();
            int idx = System.Array.IndexOf(ids, currentKey);
            int newIdx = EditorGUILayout.Popup("Key (아이템)", idx, ids);
            return (newIdx >= 0 && newIdx < ids.Length) ? ids[newIdx] : currentKey;
        }

        return EditorGUILayout.TextField("Key", currentKey);
    }

    // ---------- 캐릭터/아이템 SO 스캔 ----------
    private void LoadAllCharacters()
    {
        string[] guids = AssetDatabase.FindAssets("t:CharacterDataSO");
        _allCharacters = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<CharacterDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToArray();
    }

    private void LoadAllItems()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemDataSO");
        _allItems = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<ItemDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToArray();
    }

    // ---------- 노드 추가/삭제 ----------
    private void AddNode()
    {
        int newId = 1;
        foreach (var n in _data.nodes)
            newId = Mathf.Max(newId, n.id + 1);

        var newNode = new DialogData
        {
            id = newId,
            speaker = "",
            text = "",
            expression = "normal",
            cg = new DialogCG[0],           // 추가
            choices = new DialogChoice[0],
            reward = new DialogReward[0],
            OnEvent = ""
        };

        _data.nodes = Append(_data.nodes, newNode);
        _selectedIndex = _data.nodes.Length - 1;
    }

    private void DeleteNode(int index)
    {
        _data.nodes = RemoveAt(_data.nodes, index);
        _selectedIndex = -1;
    }

    // ---------- 배열 유틸 ----------
    private static T[] Append<T>(T[] array, T item)
    {
        var list = new List<T>(array) { item };
        return list.ToArray();
    }

    private static T[] RemoveAt<T>(T[] array, int index)
    {
        var list = new List<T>(array);
        list.RemoveAt(index);
        return list.ToArray();
    }

    // ---------- 파일 입출력 ----------
    private void LoadFile()
    {
        string json = File.ReadAllText(_filePath);
        _data = JsonUtility.FromJson<DialogDataArray>(json);
        _selectedIndex = -1;
    }

    private void SaveFile()
    {
        string json = JsonUtility.ToJson(_data, true);
        File.WriteAllText(_filePath, json);
        AssetDatabase.Refresh();
        Debug.Log("저장 완료: " + _filePath);
    }
}