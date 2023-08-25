using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    Transform timePanel;
    Transform speedPanel;
    Transform menuPanel;


    Slider fill; //시간 이미지

    Image[] menuImage;

    int maxTime = 20;
    void Awake()
    {
        timePanel = transform.GetChild(0);
        fill = timePanel.GetChild(0).GetComponent<Slider>();

        speedPanel = transform.GetChild(1);

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
        fill.maxValue = maxTime;
        fill.value += Time.deltaTime;
        if (fill.value >= maxTime) fill.value = 0;
    }

    void ChangeColorOfGameMode()
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
    public void ChangeGameSpeed(int controlKey = 3)   //스피드버튼 사용
    {
        if (GameManager.instance.gameMode == GameManager.GameMode.Builder) return;

        switch (controlKey)
        {
            case 1: { Time.timeScale = 0.5f; break; }
            case 2: { Time.timeScale = 0; break; }
            case 3: { Time.timeScale = 1f; break; }
            case 4: { Time.timeScale = 2f; break; }
            default: { Time.timeScale = 1f; break; }
        }
    }
}
