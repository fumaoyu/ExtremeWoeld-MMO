using Common.Data;
using Managers;
using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    public Text title;
    public Text gold;
    public Text diamond;

    public GameObject shopItem;
    ShopDefine shop;//商店类别id
    public Transform[] itemRoot; 
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitItem());
    }

    IEnumerator InitItem()
    {
        int count = 0;
        int page = 0;
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)//小于等于0道具不销售
            {
                GameObject gp = Instantiate(shopItem, itemRoot[page]);////
                UIShopItem uI = gp.GetComponent<UIShopItem>();
                uI.SetShopItem(shop.ID , kv.Key, kv.Value, this);
                count++;
                if (count >= 10)//商店分页
                {
                    count = 0;
                    page++;
                    itemRoot[page].gameObject.SetActive(true);
                }
            }
        }
        yield return null;
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.gold.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
        this.diamond.text=User.Instance.CurrentCharacterInfo.Diamond.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private UIShopItem selectedItem;//被选中的商品
    internal void SelectShopItem(UIShopItem uIShopItem)
    {
      if (selectedItem != null)
        {
            selectedItem.Selected = false;
        }
        selectedItem = uIShopItem;//切换另一个选择的商品
    }

    public void OnClickBuy()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要购买的道具", "购买提示");
            return;
        }
        if (!ShopManager.Instance.BuyItem(this.shop.ID, this.selectedItem.ShopItemID, PayType.Gold,1))//这里后面支付方式要改一下
        {
            //ItemService.Instance.SendBuyItem()
            
        }

    }

    public void OnClickClose()
    {
        UIManager.Instance.Close(this.GetType());
    }
}
