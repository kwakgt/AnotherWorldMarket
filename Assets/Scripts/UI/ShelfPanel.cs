using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShelfPanel : MonoBehaviour
{
    Transform invenPanel;

    
    Image[] invenImage;
    void Awake()
    {
        invenPanel = transform.GetChild(0);
        invenImage = invenPanel.GetComponentsInChildren<Image>();   //InvenPanel�� Image���� ���Ե�
    }

    void Start()
    {
        gameObject.SetActive(false);   //MouseController���� GameObject.Find�Լ� ����� ���� Start���� ��Ȱ��ȭ, ��Ȱ��ȭ�� ������Ʈ�� Find�Լ��� ���� �ȵ�.
    }

    void Update()
    {
        Display();
    }

    void Display()
    {
        Shelf shelf = GameManager.instance.selectedShelf;
        if (shelf == null) return;   //���õ� ������ ������ ����

        for (int i = 1; i < shelf.ItemSlot.Length + 1; i++)    //1���� �����ϹǷ� ������ +1���ش�, 0�� �θ� Image���� ����
        {
            if (shelf.ItemSlot[i - 1] != null)                  //unit �κ�Ʈ�δϴ� 0����
            {
                invenImage[i].sprite = shelf.ItemSlot[i - 1].sprite;
                invenImage[i].GetComponentInChildren<TextMeshProUGUI>().text = shelf.ItemSlot[i - 1].amount.ToString();
            }
        }
    }
}