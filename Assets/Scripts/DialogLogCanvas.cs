using UnityEngine;
using UnityEngine.UI;

public class DialogLogCanvas : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] ScrollRect scrollRect;
    [field: SerializeField] public Transform DialogSpawnPositon { get; private set; }

    public bool IsCanvasShow => this.gameObject.activeInHierarchy;

    private void Awake()
    {
        closeButton.onClick.AddListener(CloseButtonClick);
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }
    private void CloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        ScrollToBottom();
    }
    void ScrollToBottom()
    {
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    public void AddLog(string speaker , string text)
    {
        CharacterDataSO imageData = ResourceManager.Instance.GetCharacterData(speaker);
        Sprite portrait = imageData != null ? imageData.Portrait : null;

        DialogLogUI log = DialogLogSpawner.Instance.GetPrefabByPool();
        log.SetLogImage(portrait, speaker, text);
        log.transform.SetParent(DialogSpawnPositon, false);
    }
}
