using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    Transform timePanel;
    Transform menuPanel;


    Slider fillTime; //시간 이미지

    Image[] menuImage;

    int maxTime = 20;
    void Awake()
    {
        timePanel = transform.GetChild(0);
        fillTime = timePanel.GetChild(0).GetComponent<Slider>();


        menuPanel = transform.GetChild(2);
        menuImage = menuPanel.GetComponentsInChildren<Image>();
    }

    private void Update()
    {
        Clock();
        ChangeColorOfGameMode();
    }

    void Clock()    //UI 시계
    {
        fillTime.maxValue = maxTime;
        fillTime.value += Time.deltaTime;
        if (fillTime.value >= maxTime) fillTime.value = 0;
    }

    void ChangeColorOfGameMode() //게임모드에 따른 아이콘 색 변경
    {
        if (GameManager.instance.gameMode == GameManager.GameMode.Seller) 
        {
            menuImage[1].color = Color.green;
            menuImage[2].color = Color.white;
        }
        else if (GameManager.instance.gameMode == GameManager.GameMode.Builder) 
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

}
