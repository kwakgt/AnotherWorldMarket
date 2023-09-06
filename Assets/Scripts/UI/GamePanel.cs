using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    Transform timePanel;
    TextMeshProUGUI numOfDays;
    Slider fillTime; //시간 이미지
    int oneDay = 720; //1Day, 12분
    int oneHour = 30;   //1시간, 30초

    Transform speedPanel;
    Image[] speedImage;

    Transform menuPanel;
    Image[] menuImage;
    
    Transform infoPanel;
    TextMeshProUGUI[] infoText;


    void Awake()
    {
        timePanel = transform.GetChild(0);
        numOfDays = timePanel.GetComponentInChildren<TextMeshProUGUI>();
        fillTime = timePanel.GetChild(0).GetComponent<Slider>();

        speedPanel = transform.GetChild(1);
        speedImage = speedPanel.GetComponentsInChildren<Image>();

        menuPanel = transform.GetChild(2);
        menuImage = menuPanel.GetComponentsInChildren<Image>();

        infoPanel = transform.GetChild(3);
        infoText = infoPanel.GetComponentsInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        Clock();
        ChangeIconColor();
        DisplayInfo();
    }

    void Clock()    //UI 시계
    {
        fillTime.maxValue = oneDay;
        fillTime.value += Time.deltaTime;

        if(fillTime.value % oneHour == 0)   //한시간마다 게임매니저에 시간저장
        {
            GameManager.instance.hour++;
        }

        if (fillTime.value >= oneDay)  //하루마다 시간초기화
        {
            fillTime.value = 0;
            GameManager.instance.day++;
            GameManager.instance.hour = 0;
        }

        numOfDays.text = "Day " + GameManager.instance.day.ToString();
    }

    void ChangeIconColor() //게임모드에 따른 아이콘 색 변경
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

        switch (Time.timeScale)
        {
            case 0:     ChangeSpeedIconColor(true, false, false, false); break;
            case 1:     ChangeSpeedIconColor(false, true, false, false); break;
            case 2:     ChangeSpeedIconColor(false, false, true, false); break;
            case 3:     ChangeSpeedIconColor(false, false, false, true); break;
            default:    ChangeSpeedIconColor(false, false, false, false); break;
        }
    }

    void ChangeSpeedIconColor(bool pause, bool play, bool forward, bool fastForward)
    {
        if(pause)   speedImage[1].color = Color.red;
        else speedImage[1].color = Color.white;
        if(play)    speedImage[2].color = Color.green;
        else speedImage[2].color= Color.white;
        if(forward) speedImage[3].color = Color.green;
        else speedImage[3].color = Color.white;
        if (fastForward) speedImage[4].color = Color.green;
        else speedImage[4].color = Color.white;
    }

    void DisplayInfo()
    {
        infoText[0].text = MarketManager.instance.customerCount.ToString();
        infoText[1].text = MarketManager.instance.customerCycleTime.ToString();
        infoText[2].text = MarketManager.instance.totalMoney.ToString();
        infoText[3].text = MarketManager.instance.deadCustomer.ToString();
    }
}
