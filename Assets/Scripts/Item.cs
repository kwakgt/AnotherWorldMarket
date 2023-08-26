using System;
using UnityEngine;


public class Item : IEquatable<Item>
{
    public string name { get; private set; }            //아이템명
    public int uniqueKey { get; private set; }          //아이템 고유번호
    public int price { get; private set; }              //아이템 가격
    public int amountOfShelf { get; private set; }      //선반 한칸에 올릴 수 있는 아이템 최대개수
    public int amountOfWarehouse { get; private set; }  //창고 한칸에 넣을 수 있는 아이템 최대개수


    public int amount { get; private set; }             //현재 아이템의 개수
    
    
    public Sprite sprite { get; private set; }          //아이템 이미지, Resources/Sprite에서 가져옴
    
    //아이템 처음 생성 시 개수는 0이다. PlusAmount함수로 개수를 추가해야된다.
    public Item(string _name, int _uniqueKey, int _price, int _amountOfShelf, int _amountOfWarehouse, Sprite _sprite)  
    {
        this.name = _name;
        this.uniqueKey = _uniqueKey;
        this.price = _price;
        this.amountOfShelf = _amountOfShelf;
        this.amountOfWarehouse = _amountOfWarehouse;
        this.sprite = _sprite;
    }

    public Item(Item item)
    {
        this.name = item.name;
        this.uniqueKey = item.uniqueKey;
        this.price = item.price;
        this.amountOfShelf = item.amountOfShelf;
        this.amountOfWarehouse = item.amountOfWarehouse;
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
        if (name.Equals(item.name) && uniqueKey.Equals(item.uniqueKey))
            return true;
        else
            return false;
    }
}
