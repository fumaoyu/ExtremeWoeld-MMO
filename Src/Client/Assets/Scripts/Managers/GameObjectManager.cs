
using Entities;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



public class GameObjectManager : MonoSingleton<GameObjectManager>
{
    /// <summary>
    /// j角色字典管理所有地图角色
    /// </summary>
    Dictionary<int,GameObject> Character=new Dictionary<int,GameObject>();

    /// <summary>
    /// mono单例要重写Onstart()
    /// </summary>
    protected override void OnStart()
    {
        //base.OnStart();
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
    }

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
    }


    private void Update()
    {
        
    }

    private void OnCharacterEnter(Character arg0)
    {
        CreateCharacterObject(arg0);
    }

    private void OnCharacterLeave(Character arg0)
    {
        if (!Character.ContainsKey(arg0.entityId))//判空
            return;
        if (Character[arg0.entityId] != null)//包含
        {
            Destroy(Character[arg0.entityId]);
            this.Character.Remove(arg0.entityId);
        }
    }

    IEnumerator InitGameObjects()
    {
       foreach (var character in CharacterManager.Instance.Characters.Values)//创建每一个对象
        {
            CreateCharacterObject (character);
            yield return null;
        }
    }

    /// <summary>
    /// 创建游戏对象物体
    /// </summary>
    /// <param name="character"></param>
    private void CreateCharacterObject(Character cha)
    {
        if (!Character.ContainsKey(cha.entityId) || Character[cha.entityId] == null)//判断存不存在
        {
            ////---------------------------------------------------------
            UnityEngine.Object obj = Resloader.Load<UnityEngine.Object>(cha.Define.Resource);//资源加载
            if (obj == null)
            {
                Debug.LogErrorFormat("Character [{0}]  Resource [{1}] not exited", cha.Define.TID, cha.Define.Resource);
                return;
            }
            ////创建的所有游戏对象都在管理类对象子物体上
            GameObject gos = (GameObject)Instantiate(obj,this.transform);
            gos.name = "Character_" + cha.Info.Id + "_" + cha.Info.Name;
            Character[cha.entityId] = gos;
            //创建各个游戏对象头顶上名字等级UI显示
            UIWorldElementManager.Instance.AddCharacterNameBar(gos.transform, cha);
        }
        //放括号外面有原因的角色进入离开或者切换场景角色信息不对
        this.InitGameObject(Character[cha.entityId], cha);
    }

    private void InitGameObject(GameObject go, Character character)
    {
        go.transform.position = GameObjectTool.LogicToWorld(character.position);//坐标转换
        go.transform.forward = GameObjectTool.LogicToWorld(character.direction);
        this.Character[character.entityId] = go;
        EntityController entityController = go.GetComponent<EntityController>();
        if (entityController != null)
        {
            entityController.entity = character;
            entityController.isPlayer = character.IsPlayer;
        }
        PlayerInputController playerInputController = go.GetComponent<PlayerInputController>();
        if (playerInputController != null)
        {

            // if (character.Info.Id == Models.User.Instance.CurrentCharacter.Id)//当前角色
            if (character.IsCurrentPlayer)
            {
                User.Instance.CurrentCharacterObject = go;//当前游戏对象

                MainPlayerCamera.Instance.player = go;
                playerInputController.enabled = true;
                playerInputController.character = character;
                playerInputController.entityController = entityController;
            }
            else
            {
                playerInputController.enabled = false;
            }
        }
    }
}

