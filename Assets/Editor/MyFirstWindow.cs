using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MyFirstWindow : EditorWindow
{
    private string _myText = "";
    private List<string> _items = new List<string>();
    [MenuItem("Tools/My First Window")]
    public static void Open()
    {
        //유니티에서 Tools/My First Window를 클릭하면 Open 함수가 실행된다는 의미이다.
        GetWindow<MyFirstWindow>("연습창");
    }
    private void OnGUI()
    {
        GUILayout.Label("안녕하세요!");

        _myText = EditorGUILayout.TextField("이름", _myText); //텍스트 입력란(TextField)가 뜬다
        GUILayout.Label("입력한값: " + _myText); //실시간으로 입력한 값이 뜬다.
        if(GUILayout.Button("클릭!"))
        {
            Debug.Log("버튼 눌림: " + _myText); //유니티 디버깅 창에 뜸
        }
        //배열 다루기
        //처음에는 0개라서 아래 for문이 실행되지않음. 버튼을 누르면? 1개씩 생성되고 텍스트 필드가 활성화 되게 된다.
        for(int i=0; i< _items.Count; i++)
        {
            //EditorGUILayout.TextField($"항목:{i}", _items[i]); -> _items[i]를 넣은 이유는 처음에는 _items[i] 값을 보여달라 이 의미임. 
            //사용자 입력으로 인해 값이 바뀌면? .TextField가 입력값을 반환해서 _items[i]가 바뀌고, 다음에 그릴때 사용자 입력이 계속 바뀜.. 대충 이런식
            _items[i] = EditorGUILayout.TextField($"항목:{i}", _items[i]);
        }
        if(GUILayout.Button("+추가"))
        {
            _items.Add("새 항목");
        }
        //삭제 버튼
        for(int i = 0; i< _items.Count; i++)
        {
            EditorGUILayout.BeginHorizontal(); // EndHorizontal이 나오기 전까지 모든 UI를 가로로 두겠다는 의미
            _items[i] = EditorGUILayout.TextField(_items[i]);

            if(GUILayout.Button("삭제"))
            {
                //이벤트 형식으로? 이 버튼이 눌렸었는지 체크해서 실행을 한다고 한다.
                _items.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                return; //인덱스가 그리다가 도중에 안맞게 되었으므로 return으로 처리(다음에 다시 그려서 정상적으로 처리하게 함)
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
