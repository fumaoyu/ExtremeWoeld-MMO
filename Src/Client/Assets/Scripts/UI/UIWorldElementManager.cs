using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager>
{

    public GameObject nameBarPrefab;

    private Dictionary<Transform, UIWorldElement> elements= new Dictionary<Transform, UIWorldElement>();    
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
        gameObject .GetComponent<UINameBar>().character = character;//血条需要知道角色，谁的血条

        gameObject.SetActive(true);

        this.elements[owner] = gameObject.GetComponent<UIWorldElement>();
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.elements.ContainsKey(owner))
        {
            Destroy(this.elements[owner]);//销毁
            this.elements.Remove(owner);//字典移除
        }
    }
}
