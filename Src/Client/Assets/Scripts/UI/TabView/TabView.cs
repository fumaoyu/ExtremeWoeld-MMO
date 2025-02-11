using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



public class TabView:MonoBehaviour
{
    public TabButton[] tabButton;
    public GameObject[] tabPages;

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
        index = tabIndex;
        //if (OnTabSelect!=null)
        //{
        //    OnTabSelect(index);
        //}
    }
}
