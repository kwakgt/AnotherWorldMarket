using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitPanel : MonoBehaviour
{
    Transform invenPanel;
    Image[] invenImage;

    Transform infoPanel;
    TextMeshProUGUI infoText;
    void Awake()
    {
        invenPanel = transform.GetChild(0);
        invenImage = invenPanel.GetComponentsInChildren<Image>();   //InvenPanel�� Image���� ���ԵǾ� Length = 13;

        infoPanel = transform.GetChild(1);
        infoText = infoPanel.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        gameObject.SetActive(false); //MouseController���� GameObject.Find�Լ� ����� ���� Start���� ��Ȱ��ȭ, ��Ȱ��ȭ�� ������Ʈ�� Find�Լ��� ���� �ȵ�.
    }

    void Update()
    {
        Display();
    }

    void Display()
    {
        Unit unit = GameManager.instance.selectedUnit;
        if (unit == null) return;   //���õ� ������ ������ ����

        infoText.text = "buyCount: " + unit.buyCount + "\nmoney : " + unit.money;     //test

        //�κ��丮 �г�
        for (int i = 1; i < unit.invenSizeAvailable + 1; i++)    //1���� �����ϹǷ� ������ +1���ش�, 0�� �θ� Image���� ����
        {
            if (unit.inventory[i - 1] != null)                  //unit �κ�Ʈ�δϴ� 0����
            {
                invenImage[i].sprite = unit.inventory[i - 1].sprite;
                invenImage[i].GetComponentInChildren<TextMeshProUGUI>().text = unit.inventory[i - 1].amount.ToString();
            }
            else
            {
                invenImage[i].sprite = null;
                invenImage[i].GetComponentInChildren<TextMeshProUGUI>().text = null;
            }
        }
    }
}