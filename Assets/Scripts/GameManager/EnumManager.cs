using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumManager
{
    /// <summary>
    /// 건물 입구 방향
    /// </summary>
    public enum Direction { Left, Down, Right, Up }
    
    /// <summary>
    /// 판매대,아이템 크기, 소형,중형,대형
    /// </summary>
    public enum ShelfType { SmallShelf, MediumShelf, LargeShelf }

    /// <summary>
    /// 유닛의 쇼핑백(사용가능한 인벤 수), 두손,비닐봉지,바구니,쇼핑카트
    /// </summary>
    public enum consumables { TwoHands = 2, PlasticBag = 4, Basket = 8, Cart = 12 }
    
    /// <summary>
    /// 차원명
    /// </summary>
    public enum Dimention { Astaria, Animaia, Manujhar, Navarore, Hyloth, Voltroth, Genierth, Dreatera, Devlearn, Holysacria }
    
    /// <summary>
    /// 직원이 하는 일
    /// </summary>
    public enum WorkType { Checking, Finding, Carrying, Emptying, Teleporting }
}
