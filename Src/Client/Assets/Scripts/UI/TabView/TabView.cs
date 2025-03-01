using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class TabView:MonoBehaviour
{
    public TabButton[] tabButton;
    public GameObject[] tabPages;

    public UnityAction<int> OnTabSelect;
    public int index = -1;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        for (int i = 0; i < tabButton.Length; i++)
        {
            tabButton[i].tabView = this;
            tabButton[i].tabIndex = i;
        }
        yield return new WaitForEndOfFrame();//等一帧
        SelectTab(0);//默认选择第一页
    }

    /// <summary>
    /// 选择第几页
    /// </summary>
    /// <param name="tabIndex"></param>
   public  void SelectTab(int tabIndex)
    {

        Debug.Log("你选择了第 " + tabIndex + " 页");
       if (this.index != tabIndex)
        {
            for (int i=0; i < tabButton.Length; i++)
            {
                tabButton[i].Select(i==tabIndex);
                if (i < tabPages.Length)
                {
                    tabPages[i].SetActive(i == tabIndex);
                }
                
            }
        }
        if (OnTabSelect!=null)
        {
            OnTabSelect(index);
        }
    }
}
