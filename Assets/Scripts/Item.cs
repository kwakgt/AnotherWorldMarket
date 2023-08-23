using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.U2D;

public class Item
{
    public string name { get; private set; }
    public int uniqueKey { get; private set; }
    public int price { get; private set; }

    public int amount { get; private set; }

    public Sprite sprite { get; private set; }
    public Item(string _name, int _uniqueKey, int _price, Sprite _sprite)  //아이템 처음 생성 시 개수는 1이다.
    {
        this.name = _name;
        this.uniqueKey = _uniqueKey;
        this.price = _price;
        this.sprite = _sprite;
    }

    public Item(Item item)
    {
        this.name = item.name;
        this.uniqueKey = item.uniqueKey;
        this.price = item.price;
        this.sprite = item.sprite;
    }

    public bool MinusAmount(int _amount)
    {
        amount -= _amount;
        if (amount < 0)
        {
            amount += _amount; 
            return false;    //빼고 난 후 amount 양이 음수 이면 실패이고 원상복구
        }
        else return true;
    }

    public bool PlusAmount(int _amount)
    {
        if(amount < 0) return false;
        amount += _amount;
        return true;
    }


    public bool Equals(Item item)
    {
        if (this.name == item.name && this.uniqueKey == item.uniqueKey)
            return true;
        else
            return false;
    }
}
