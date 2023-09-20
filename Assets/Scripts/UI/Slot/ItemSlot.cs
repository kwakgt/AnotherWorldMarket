using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemSlot : MonoBehaviour
{
    //아이템 이미지(RawImage), 아이템명 텍스트 각각 1개로 구성
    Item item;

    RawImage itemImage;
    TextMeshProUGUI itemName;

    bool isUsable = true;

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
        //SetItem 함수를 쓰면 무조건 사용가능 슬롯
        isUsable = true;
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
            gameObject.name = item.name;
            itemImage.color = Color.white;
            itemImage.texture = item.sprite.texture;
            itemName.text = item.amount.ToString();
        }
        else if(item == null && !isUsable)
        {
            //사용불가슬롯이라면 X 표시
            gameObject.name = "Unusable";
            itemImage.color = Color.white;
            itemImage.texture = SpriteManager.instance.unusableSlot.texture;
            itemName.text = null;
        }
        else
        {
            gameObject.name = "ItemSlot";
            itemImage.color = Color.clear; //투명하게 안하면 흰색배경이미지가 보임
            itemImage.texture = null;
            itemName.text = null;
        }
    }
}
