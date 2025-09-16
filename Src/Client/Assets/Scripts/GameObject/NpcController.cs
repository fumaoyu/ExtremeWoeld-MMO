using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public int NpcId;

    SkinnedMeshRenderer renderer;
    public Animator ani;
    Color origincolor;//npc点击之后改变一下颜色，方便知道

    /// <summary>
    /// 防止连续点击
    /// </summary>
    private bool inInteractiv = false;


    /// <summary>
    /// 配置表中数据
    /// </summary>
    public NpcDefine npcDefine;


    NpcQuestStatus questStatus;
    // Start is called before the first frame update
    void Start()
    {
        renderer = this.GetComponentInChildren<SkinnedMeshRenderer>();

        ani = this.GetComponent<Animator>();
        origincolor = renderer.sharedMaterial.color; 
        npcDefine = NpcManager.Instance.GetNpcDefine(NpcId);

        NpcManager.Instance.UpdateNpcPosition(this.NpcId, this.transform.position);

        this.StartCoroutine(Actions());

        RefreshNpcStatus();//打开初始化
        QuestManager.Instance.onQuestStatusChanged += OnQuestStatusChanged;//任务改变通知npcs刷新小图标
    }

    void OnQuestStatusChanged(Quest quest)
    {
        this.RefreshNpcStatus();
    }

    /// <summary>
    /// 刷新npc状态
    /// </summary>
    private void RefreshNpcStatus()
    {
        questStatus = QuestManager.Instance.GetQuestStatusByNpc(this.NpcId);
        UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform, questStatus);
    }

    private void OnDestroy()///npc销毁时
    {
        QuestManager.Instance.onQuestStatusChanged -= OnQuestStatusChanged;
        if (UIWorldElementManager.Instance != null)
        {
            UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
        }
    }



    /// <summary>
    /// 这个协成随机更换动作使用的
    /// </summary>
    /// <returns></returns>
    IEnumerator Actions()
    {
        while (true)
        {
            if (inInteractiv)
                yield return new WaitForSeconds(2f);//等待两秒
            else
                yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
            this.Relax();
        }
    }

 
    // Update is called once per frame
    void Update()
    {
        
    }

    private void Relax()
    {
        ani.SetTrigger("Relax");
    }

    void Interactive()
    {
        if (!inInteractiv)
        {
            inInteractiv = true;
            StartCoroutine(DoInteractive());
        }

    }

    /// <summary>
    /// 左交互
    /// </summary>
    /// <returns></returns>
     IEnumerator DoInteractive()
    {
        yield return FaceToPlayer();///等待这个面向玩家函数完成
        if (NpcManager.Instance.Interactive(npcDefine))//交互成功
        {
            ani.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(3f);//三秒之内无法重复点击
        inInteractiv = false;
    }

    /// <summary>
    /// 转向面对玩家
    /// </summary>
    /// <returns></returns>
    IEnumerator FaceToPlayer()
    {
        Vector3 faceto = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceto)) > 3)//如果角度大于3npc才转向,小于就不转
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceto, Time.deltaTime * 5f);
            yield return null;
        }
    }
    /// <summary>
    /// 鼠标点击下触发交互
    /// </summary>
    void OnMouseDown()
    {
        ///鼠标点击npc,判断一下距离，再确定是不是寻路
        if (Vector3.Distance(this.transform.position, User.Instance.CurrentCharacterObject.transform.position) > 2f)
        {
            User.Instance.CurrentCharacterObject.StartNav(this.transform.position);
        }

        Interactive();
    }
    /// <summary>
    /// 在npc 上高亮
    /// </summary>
    void OnMouseOver()
    {
        Highlight(true);
    }


    void OnMouseEnter()
    {
        Highlight(true);
    }

    void OnMouseExit()
    {
        Highlight(false);
    }
 

 

   /// <summary>
   /// 点击Npc颜色高亮
   /// </summary>
   /// <param name="islight"></param>
    private void Highlight(bool islight)
    {
        if (islight)
        {
            if (renderer.sharedMaterial.color != Color.red)
            {
                renderer.sharedMaterial.color = Color.red;
            }

        }
        else
        {
            if (renderer.sharedMaterial.color != origincolor)
                renderer.sharedMaterial.color = origincolor;
        }
    }

}
