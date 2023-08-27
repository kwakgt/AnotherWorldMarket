using System.Collections;
using UnityEngine;

public class Customer : Unit
{
    int amountOfPurchase = 3;
    int maxWaitTime = 3;        //���ð�

    protected override void Awake()
    {
        base.Awake();
        type = Type.Customer;
    }

    protected override void Start()
    {
        base.Start();
    }

    IEnumerator CustomerRoutine()
    {
        if (shelf.FindItemInSlot(target) != null)           //�Ŵ뿡 ������ ������
            yield return StartCoroutine("BuyingItem");      //�Ŵ뿡 ���� ����

        if (money <= 0 || buyCount <= 0 || IsInventoryFull())   //��, ����Ƚ���� ���ų� �κ��丮�� ���� á����
            GoHome();                                           //���� ����.
        else
            GoMarket();                                         //�ƴϸ� �ٸ� �Ŵ� ã��

        if ((Vector2)transform.position == respawn)          //������ġ�� �������̶�� ������Ʈ �ı�
            Destroy(this.gameObject);
        yield return new WaitForSeconds(0.25f);             //������ ������ ���ð�
    }

    IEnumerator BuyingItem()
    {
        if ((Vector2)transform.position == target)
        {
            Item shelfItem = shelf.FindItemInSlot(target);      //�Ŵ������ ����
            int shelfIndex = shelf.FindItemSlotIndex(target);   //�Ŵ������ ���� �ε���

            yield return StartCoroutine("Waiting", maxWaitTime);
            if (shelfItem == null) yield break;     //���ð����� ������ �� �ȸ����� �����Ƿ� �ѹ� �� �˻�
            if (!FindKeyByValue(shelfItem).Equals(-1))                 //�� �κ��丮�� �������� �����Ѵٸ�, FindKeyByValue: �������� �κ����� ã�� �� ������ �ε��� -1�� ��ȯ��
            {
                int key = FindKeyByValue(shelfItem);                    //�κ��丮 Ű ã��
                int amountToBuy = PurchaseForFitPrice(shelfItem);    //�������� ������ ���ŷ� ���ϱ�
                BuyItem(inventory[key], shelfItem, shelfIndex, amountToBuy);//���ŷ���ŭ ������ �缭 �κ��丮 �����ۿ� ���ϱ�
            }
            else                                                            //�� �κ��丮�� �������� ���ٸ�
            {
                Item myItem = new Item(shelfItem);                          //������ ���� ����
                int amountToBuy = PurchaseForFitPrice(shelfItem);        //�������� ������ ���ŷ� ���ϱ�
                if (BuyItem(myItem, shelfItem, shelfIndex, amountToBuy))     //���ŷ���ŭ ������ ���
                    inventory[invenIndex.RemoveFirst().value] = myItem;   //�κ��丮�� ������ ������ �ֱ�
            }
            --buyCount;     //����Ƚ�� ����
            yield return new WaitForSeconds(0.25f);             //������ ������ ���ð�
        }
    }


    bool BuyItem(Item myItem, Item shelfItem, int shelfIndex, int amountToBuy)
    {
        //�� �����ۿ� ���ŷ���ŭ ���ϰ� �Ŵ�����ۿ� ���ŷ���ŭ ����
        money -= shelfItem.price * amountToBuy;                 //���� ���
        if (money < 0)                                           //���� �����ϸ�
        {
            money += shelfItem.price * amountToBuy;             //���󺹱�
            return false;
        }
        EnablePriceText("+" + (shelfItem.price * amountToBuy).ToString());
        myItem.PlusAmount(amountToBuy);                         //�κ��丮�� �����ۿ� ���ŷ���ŭ �߰�
        shelfItem.MinusAmount(amountToBuy);                     //�Ŵ�����ۿ� ���ŷ���ŭ ����
        if (shelfItem.amount <= 0)                              //�Ŵ������ ���� 0 �����̸�
            shelf.EmptyItemSlot(shelfIndex);                    //�Ŵ������ ����

        return true;
    }

    void EnablePriceText(string text)
    {
        priceText.text = text;
        priceText.gameObject.SetActive(true);
    }

    int PurchaseForFitPrice(Item shelfItem)  //���� �����ݿ� �°� ������ ����, ���ż��� ���ϴ� �Լ�
    {
        int amount = Random.Range(1, amountOfPurchase + 1);
        return Mathf.Clamp(amount, 0, money / shelfItem.price);
    }
}