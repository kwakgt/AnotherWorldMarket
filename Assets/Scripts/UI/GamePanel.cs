using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    Transform timePanel;
    Slider fillTime; //시간 이미지
    int maxTime = 20;
    
    Transform menuPanel;
    Image[] menuImage;
    
    Transform infoPanel;
    TextMeshProUGUI[] infoText;


    void Awake()
    {
        timePanel = transform.GetChild(0);
        fillTime = timePanel.GetChild(0).GetComponent<Slider>();

        menuPanel = transform.GetChild(2);
        menuImage = menuPanel.GetComponentsInChildren<Image>();

        infoPanel = transform.GetChild(3);
        infoText = infoPanel.GetComponentsInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        Clock();
        ChangeColorOfGameMode();
        DisplayInfo();
    }

    void Clock()    //UI 시계
    {
        fillTime.maxValue = maxTime;
        fillTime.value += Time.deltaTime;
        if (fillTime.value >= maxTime) fillTime.value = 0;
    }

    void ChangeColorOfGameMode() //게임모드에 따른 아이콘 색 변경
    {
        if (GameManager.instance.CompareTo(GameManager.GameMode.Seller))
        {
            menuImage[1].color = Color.green;
            menuImage[2].color = Color.white;
        }
        else if (GameManager.instance.CompareTo(GameManager.GameMode.Builder))
        {
            menuImage[1].color = Color.white;
            menuImage[2].color = Color.green;
        }
        else
        {
            menuImage[1].color = Color.white;
            menuImage[2].color = Color.white;
        }
    }

    void DisplayInfo()
    {
        infoText[0].text = MarketManager.instance.customerCount.ToString();
        infoText[1].text = MarketManager.instance.customerCycleTime.ToString();
        infoText[2].text = MarketManager.instance.totalMoney.ToString();
        infoText[3].text = MarketManager.instance.deadCustomer.ToString();
    }
}
