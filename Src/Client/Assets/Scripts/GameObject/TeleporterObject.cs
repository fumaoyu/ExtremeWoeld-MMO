using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using Services;
using UnityEngine;



public class TeleporterObject : MonoBehaviour
{
    /// <summary>
    /// 传送点id
    /// </summary>
    public int TelePorID;

    public Mesh mesh = null;

    private void Start()
    {
        this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
        Debug.Log("传送点脚本初始化");
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("碰撞函数触发");
    }
    private void Update()
    {
    //   Debug.Log("传送点跟新");
    
    }


     void OnTriggerEnter(Collider other)
    {
        Debug.Log("到达传送点");
        PlayerInputController playerInputController = other.GetComponent<PlayerInputController>();
        if (playerInputController != null&&playerInputController.isActiveAndEnabled)
        {
            TeleporterDefine td = DataManager.Instance.Teleporters[this.TelePorID];
            if (td == null)
            {
                Debug.LogErrorFormat("  TeleporterObject: Character {0}  Enter Teleporter [{1}] ,But TeleporterDefine not existed", playerInputController.character.Info.Name, this.TelePorID);
                return;
            }
            Debug.LogFormat("TeleporterObject: Charatcter [{0}] Enter Teleporter [{1}:{2}]", playerInputController.character.Info.Name, td.ID, td.Name);
            if (td.LinkTo > 0)
            {
                if (DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
                {
                    MapService.Instance.SendMapTeleport(this.TelePorID);
                }
                else
                {
                    Debug.LogErrorFormat("Teleporter: ID {0} LinkID {1} error", td.ID, td.LinkTo);
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("stay函数调用");
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit函数调用");
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (this.mesh!=null)
        {
            Gizmos.DrawWireMesh(this.mesh,this.transform.position+Vector3.up*this.transform.localScale.y*0.5f,this.transform.rotation,this.transform.localScale);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0,this.transform.position,this.transform.rotation,1f,EventType.Repaint);
    }
  #endif
    
   
}
