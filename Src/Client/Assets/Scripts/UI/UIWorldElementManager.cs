using Entities;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager>
{

    public GameObject nameBarPrefab;
    public GameObject npcStatusPrefab;

    private Dictionary<Transform, GameObject> elementNames = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, GameObject> elementNpcStatus = new Dictionary<Transform, GameObject>();
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCharacterNameBar(Transform owner, Character character)
    {
        GameObject gameObject = Instantiate(nameBarPrefab, this.transform);
        gameObject.name = "NamBar" + character.entityId;
        gameObject.GetComponent<UIWorldElement>().owner = owner;
        gameObject.GetComponent<UINameBar>().character = character;//血条需要知道角色，谁的血条

        gameObject.SetActive(true);
          
		  
		  
        this.elementNames[owner] = gameObject;
       // this.elementNames[owner] = gameObject.GetComponent<UIWorldElement>();
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.elementNames.ContainsKey(owner))
        {
            Destroy(this.elementNames[owner]);//销毁
            this.elementNames.Remove(owner);//字典移除
        }
    }

    /// <summary>
    /// 增加npc状态
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="status"></param>
    public void AddNpcQuestStatus(Transform owner, NpcQuestStatus status)
    {
        if (this.elementNpcStatus.ContainsKey(owner))//先判断有没有包含
        {
            elementNpcStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        }
        else
        {
            GameObject go = Instantiate(npcStatusPrefab, this.transform);
            go.name = "NpcQuestStatus" + owner.name;
            go.GetComponent<UIWorldElement>().owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            this.elementNpcStatus[owner] = go;
        
        }
    }

    public void RemoveNpcQuestStatus(Transform owner)
    {
        if (this.elementNpcStatus.ContainsKey(owner))
        {
            Destroy(this.elementNpcStatus[owner]);/////////感觉这里应该是
            this.elementNpcStatus.Remove(owner);
        }
    }
}
