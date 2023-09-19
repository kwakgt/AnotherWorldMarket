using System;
using UnityEngine;
using EnumManager;

public class Item : IEquatable<Item>
{
    //상수
    public string name { get; }            //아이템명
    public int uniqueKey { get;}          //아이템 고유번호
    public Dimension dimension { get; }    //얻을 수 있는 차원
    public StaffWork workType { get; }      //얻을 수 있는 작업
    public int price { get; }              //아이템 가격
    public int amountOfShelf { get; }      //판매대 한칸에 올릴 수 있는 아이템 최대개수
    public int amountOfWarehouse { get; }  //창고 한칸에 넣을 수 있는 아이템 최대개수
    public Sprite sprite { get; }          //아이템 이미지, Resources/Sprite에서 가져옴
    
    
    //변수
    //아이템 처음 생성 시 개수는 0이다. PlusAmount함수로 개수를 추가해야된다.
    public int amount { get; private set; }             //현재 아이템의 개수

    public Item(string _name, int _uniqueKey, Dimension _dimension, StaffWork _workType, int _price, int _amountOfShelf, int _amountOfWarehouse, Sprite _sprite)
    {
        name = _name;
        uniqueKey = _uniqueKey;
        dimension = _dimension;
        workType = _workType;
        price = _price;
        amountOfShelf = _amountOfShelf;
        amountOfWarehouse = _amountOfWarehouse;
        sprite = _sprite;
    }

    public Item(Item item, int _amount = 0)
    {
        name = item.name;
        uniqueKey = item.uniqueKey;
        dimension= item.dimension;
        workType = item.workType;
        price = item.price;
        amountOfShelf = item.amountOfShelf;
        amountOfWarehouse = item.amountOfWarehouse;
        sprite = item.sprite;
        
        amount = _amount;
    }

    public bool MinusAmount(int _amount) //마이너스 성공여부 후 플러스
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
        if (item == null)
            return false;

        if (name.Equals(item.name) && uniqueKey.Equals(item.uniqueKey))
            return true;
        else
            return false;
    }

    /*
    public static bool operator ==(Item item1, Item item2)
    {                                                       //연산자 오버로딩 함수는 public, static으로 선언되어야 한다. 반환타입은 아무거나 상관없다.
        if (ReferenceEquals(item1, null))                   //==연산자 오버로딩 시 ==으로 NULL체크를 할 수 없어서 새로 정의해야함
            return ReferenceEquals(item2, null);            //ReferenceEquals(a,b) 함수는 두 값의 참조값을 비교한다.
        else if (ReferenceEquals(item2, null))
            return false;

        return item1.Equals(item2);
    }

    public static bool operator !=(Item item1, Item item2)
    {
        if (ReferenceEquals(item1, null))
            return !ReferenceEquals(item2, null);
        else if (ReferenceEquals(item2, null))
            return true;

        return !item1.Equals(item2);
    }
    */
}
