using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumManager
{
    /// <summary>
    /// ����ü �Ա� ����, �Ա��������θ� ������ ���ٰ����ϴ�.
    /// </summary>
    public enum Direction { Left, Down, Right, Up }
    
    /// <summary>
    /// �ǸŴ�,������ ũ��(����,����,����)
    /// </summary>
    public enum ShelfType { SmallShelf, MediumShelf, LargeShelf }

    /// <summary>
    /// ������ ���ι�(��밡���� �κ� ��), �μ�,��Һ���,�ٱ���,����īƮ. �Ҹ�ǰ�̴�.
    /// </summary>
    public enum consumables { TwoHands = 2, PlasticBag = 4, Basket = 8, Cart = 12 }
    
    /// <summary>
    /// ������
    /// </summary>
    public enum Dimention { Astaria, Animaia, Manujhar, Navarore, Hyloth, Voltroth, Genierth, Dreatera, Devlearn, Holysacria }

    /// <summary>
    /// ������ �ൿ����
    /// </summary>
    public enum WorkType { Purchase, Checking, Finding, Carrying, Emptying, Teleporting, Felling, Mining, Collecting, Hunting, Fishing }

    /// <summary>
    /// ����,���� ���� ���п�
    /// </summary>
    public enum UnitType { Customer, Staff}

    /// <summary>
    /// ���� ����
    /// </summary>
    public enum Tribe { Human, Elf, Dwarf, Oce }

    /// <summary>
    /// ������ ���� ���� Max, Min��, ������ �ش� �� ������ ������ ���´�.
    /// UnitManager tribe�� Key��
    /// </summary>
    public enum StatBind { PurchaseMin, PurchaseMax, CarryingMin, CarryingMax, FellingMin, FellingMax, MiningMin, MiningMax, CollectingMin, CollectingMax, HuntingMin, HuntingMax, FishingMin, FishingMax }

}