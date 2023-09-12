using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumManager
{
    /// <summary>
    /// 구조체 입구 방향, 입구방향으로만 유닛이 접근가능하다.
    /// </summary>
    public enum Direction { Left, Down, Right, Up }
    
    /// <summary>
    /// 판매대,아이템 크기(소형,중형,대형)
    /// </summary>
    public enum ShelfType { SmallShelf, MediumShelf, LargeShelf }

    /// <summary>
    /// 유닛의 쇼핑백(사용가능한 인벤 수), 두손,비닐봉지,바구니,쇼핑카트. 소모품이다.
    /// </summary>
    public enum consumables { TwoHands = 2, PlasticBag = 4, Basket = 8, Cart = 12 }
    
    /// <summary>
    /// 차원명
    /// </summary>
    public enum Dimension { Astaria, Animaia, Manujhar, Navarore, Hyloth, Voltroth, Genierth, Dreatera, Devlearn, Holysacria }

    /// <summary>
    /// 직원의 행동명령
    /// </summary>
    public enum WorkType { Purchase, Checking, Finding, Carrying, Emptying, Teleporting, Felling, Mining, Collecting, Hunting, Fishing }

    /// <summary>
    /// 고객,직원 유닛 구분용
    /// </summary>
    public enum UnitType { Customer, Staff}

    /// <summary>
    /// 유닛 종족
    /// </summary>
    public enum Tribe { Human, Elf, Dwarf, Oce }

    /// <summary>
    /// 종족에 따른 스탯 Max, Min값, 유닛은 해당 값 사이의 스탯을 갖는다.
    /// UnitManager tribe의 Key값
    /// </summary>
    public enum StatBind { PurchaseMin, PurchaseMax, CarryingMin, CarryingMax, FellingMin, FellingMax, MiningMin, MiningMax, CollectingMin, CollectingMax, HuntingMin, HuntingMax, FishingMin, FishingMax }

    /// <summary>
    /// UI매니저 관리 패널명
    /// </summary>
    public enum PanelName { Off, DimensionPanel, StaffManagementPanel }

}
