using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GalleryCgPanel : MonoBehaviour
{
    [SerializeField] Button cgButton;
    [SerializeField] Button endingButton;
    [SerializeField] Button characterButton;
    [SerializeField] Button closeButton;

    //CG 패널(cg 및 엔딩cg 전용)
    //이미지 프리팹을 하나 만들어서. 클릭하면 전체 화면을 보이고 다시 클릭하면 패널 비활성하도록, 이미지 비활성화와 활성화 어떻게 표시하는지 정의되어있어야함
    [SerializeField] GalleryCgListPanel cgListPanel;
    [SerializeField] GalleryShowImagePanel imageShowPanel; //이미지를 클릭하면 다른 패널 하나를 열어서 클릭한 이미지가 전체 화면형식으로 보이게끔 열거임
    
    //캐릭터 패널
    [SerializeField] GalleryCharacterPanel characterListPanel;
    private void Start()
    {
        characterPanelInit();
    }
    private void OnEnable()
    {
        cgListPanel.OnImageShow += ShowCgImage;
        cgButton.onClick.AddListener(CgButtonClick);
        endingButton.onClick.AddListener(EndingButtonClick);
        characterButton.onClick.AddListener(CharacterButtonClick);
        closeButton.onClick.AddListener(() => this.gameObject.SetActive(false));
    }
    private void OnDisable()
    {
        cgListPanel.OnImageShow -= ShowCgImage;
        cgListPanel.gameObject.SetActive(false);
        characterListPanel.gameObject.SetActive(false);

        cgButton.onClick.RemoveAllListeners();
        endingButton.onClick.RemoveAllListeners();
        characterButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }

    private void ShowCgImage(Sprite sprite)
    {
        imageShowPanel.SetImageAndShow(sprite);
    }
    void characterPanelInit()
    {
        CharacterDataSO[] data = ResourceManager.Instance.CharacterDataSOArray;
        characterListPanel.SetCharacterData(data);
    }

    void CgButtonClick()
    {
        //보여줄 패널 변경 UI
        cgListPanel.gameObject.SetActive(true);
        characterListPanel.gameObject.SetActive(false);
        cgListPanel.SetCGScrollViewData(eCgCategory.cg);
    }
    void EndingButtonClick()
    {//보여줄 패널 변경 UI
        cgListPanel.gameObject.SetActive(true);
        characterListPanel.gameObject.SetActive(false);

        cgListPanel.SetCGScrollViewData(eCgCategory.Ending);
    }
    void CharacterButtonClick()
    {
        //보여줄 패널 변경 UI
        cgListPanel.gameObject.SetActive(false);
        characterListPanel.gameObject.SetActive(true);
    }


}
