using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : MonoBehaviour
{

    public Text money;

    /// <summary>
    /// 钻石预留自己加
    /// </summary>
    public Text diamond;

    public GameObject bagitem;

    public Transform[] pages;
    List<Image> slots;

    public GameObject UnlockedP;
    // Start is called before the first frame update
    void Start()
    {
        if (slots == null)
        {
            slots = new List<Image>();
            for (int i = 0; i < this.pages.Length; i++)
            {
                slots.AddRange(this.pages[i].GetComponentsInChildren<Image>(true));
            }
        }

        StartCoroutine(InitBags());
    }

    IEnumerator InitBags()
    {

        for (int i = 0; i < BagManager.Instance.Items.Length; i++)//已经解锁的
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemId > 0)
            {
                GameObject go = Instantiate(bagitem, slots[i].transform);
                var ui = go.GetComponent<UIIconItem>();
                var der = ItemManager.Instance.Items[item.ItemId].Define;
                ui.SetMainIcon(der.Icon, item.Count.ToString());//设置图标数量
            }
        }
        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++)//没有解锁的
        {
            //slots[i].color = Color.gray;
            GameObject go = Instantiate(UnlockedP, slots[i].transform);
            //var ui = go.GetComponent<UIIconItem>();
            //var der = ItemManager.Instance.Items[item.ItemId].Define;
        }
        yield return null;
    }

    public void SetTitle(string title)
    {
        this.money.text = User.Instance.CurrentCharacterInfo.Id.ToString();
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();

        this.Clear();
        StartCoroutine(InitBags());
    }

    public void OnclickClose()
    {
        UIManager.Instance.Close(this.GetType());
    }

    void Clear()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
