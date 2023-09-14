using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemSlot : MonoBehaviour
{
    //������ �̹���(RawImage), �����۸� �ؽ�Ʈ ���� 1���� ����
    Item item;

    RawImage itemImage;
    TextMeshProUGUI itemName;

    bool isUsable;

    void Awake()
    {
        itemImage = GetComponentInChildren<RawImage>();
        itemName = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        Display();
    }

    public void SetItem(Item _item)
    {
        //SetItem �Լ��� ���� ������ ��밡�� ����
        isUsable = true;

        if (_item == null)
            item = null;
        else
            item = _item;
    }

    public void SetUnusableItemSlot()
    {
        item = null;
        isUsable = false;
    }

    void Display()
    {
        if(item != null && isUsable)
        {
            itemImage.color = Color.white;
            itemImage.texture = item.sprite.texture;
            itemName.text = item.amount.ToString();
        }
        else if(item == null && !isUsable)
        {
            //���Ұ������̶�� X ǥ��
            itemImage.color = Color.white;
            itemImage.texture = SpriteManager.instance.unusableSlot.texture;
            itemName.text = null;
        }
        else
        {
            itemImage.color = Color.clear; //�����ϰ� ���ϸ� �������̹����� ����
            itemImage.texture = null;
            itemName.text = null;
        }
    }
}