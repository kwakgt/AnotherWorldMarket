using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class CustomerPanel : MonoBehaviour
{
    Transform invenPanel;


    Image[] invenImage;
    void Awake()
    {
        invenPanel = transform.GetChild(0);

        invenImage = invenPanel.GetComponentsInChildren<Image>();   //InvenPanel의 Image까지 포함되어 Length = 13;
    }
    
    void Start()
    {
        gameObject.SetActive(false); //MouseController에서 GameObject.Find함수 사용을 위해 Start에서 비활성화, 비활성화된 오브젝트는 Find함수에 감지 안됨.
    }
    
    void Update()
    {
        Display();
    }

    void Display()
    {
        Unit unit = GameManager.instance.selectedUnit;
        if (unit == null) return;   //선택된 유닛이 없으면 종료

        for (int i = 1; i < unit.invenSizeAvailable + 1; i++)    //1부터 시작하므로 범위에 +1해준다, 0은 부모 Image여서 제외
        {
            if (unit.inventory[i - 1] != null)                  //unit 인벤트로니는 0부터
            {
                invenImage[i].sprite = unit.inventory[i - 1].sprite;
                invenImage[i].GetComponentInChildren<TextMeshProUGUI>().text = unit.inventory[i - 1].amount.ToString();
            }
        }
        
    }
}
