
using Entities;
using Managers;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;



class PlayerInputController : MonoBehaviour
{
    public Rigidbody rigidbody;

    SkillBridge.Message.CharacterState state;

    public Character character;

    public float rotateSpeed = 2.0f;

    public float turnAngle = 10;

    public int speed;

    public EntityController entityController;

    public bool onAir = false;


    public NavMeshAgent navMeshAgent;
    private bool autoNav=false;
    // Use this for initialization
    void Start()
    {
        state = SkillBridge.Message.CharacterState.Idle;
        rigidbody=this.GetComponent<Rigidbody>();
        //if (this.character == null)
        //{
        //    Debug.Log("Yes create character");
        //    DataManager.Instance.Load();
        //    NCharacterInfo cinfo = new NCharacterInfo();

        //    cinfo.Id = 1;
        //    cinfo.Name = "dscsd";
        //    cinfo.ConfigId = 1;
        //    cinfo.Entity = new NEntity();
        //    cinfo.Entity.Position = new NVector3();
        //    cinfo.Entity.Direction = new NVector3();
        //    cinfo.Entity.Direction.X = 0;
        //    cinfo.Entity.Direction.Y = 100;
        //    cinfo.Entity.Direction.Z = 0;
        //    cinfo.attributeDynamic = new NAttributeDynamic();
        //    this.character = new Character(cinfo);


        //    //cinfo.Entity.Position = new NVector3();
        //    //cinfo.Entity.Direction = new NVector3();
        //    //cinfo.Entity.Direction.X = 0;
        //    //cinfo.Entity.Direction.Y = 100;
        //    //cinfo.Entity.Direction.Z = 0;
        //    //this.character = new Character(cinfo);

        //    if (entityController != null) entityController.entity = this.character;
        
        //    ///导航代理
           
        //}
        if (navMeshAgent == null)
        {
            navMeshAgent = this.gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.stoppingDistance = 1.2f;//设置这个原因，不是跟目标位置重合停下，距离0.3mi的时候停下   
        }
    }

    public void StartNav(Vector3 target)
    {

        StartCoroutine(BegineNav(target));
    }

    IEnumerator BegineNav(Vector3 target)
    {
        navMeshAgent.SetDestination(target);//设置目的地，代理后台自动运算路径
        yield return null;///等一帧

        autoNav = true;

        ///角色状态要设为移动状态
        if (state != SkillBridge.Message.CharacterState.Move)
        {
            state=SkillBridge.Message.CharacterState.Move;
            this.character.MoveForward();
            this.SendEntityEvent(EntityEvent.MoveFwd);
            navMeshAgent.speed=this.character.speed/100f;
        }
    }

    public void StopNav()
    {
        autoNav = false;
        navMeshAgent.ResetPath();

        if (state != SkillBridge.Message.CharacterState.Idle)
        {
            state = SkillBridge.Message.CharacterState.Idle;
           this.rigidbody.velocity = Vector3.zero;  

            this.character.Stop();
            this.SendEntityEvent(EntityEvent.Idle);
        }

        NavPathRender.Instance.SetPath(null, Vector3.zero);
    }

    public void NavMove()
    {
        if (navMeshAgent.pathPending) return;
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)///寻路有没有完成
        {
            StopNav();
            return;
        }

        if(navMeshAgent.pathStatus!=NavMeshPathStatus.PathComplete) return;///寻路状态有没有完成

        ///移动方向键想自己走，寻路停止
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1)
        {
            StopNav() ;
            return;
        }

        NavPathRender.Instance.SetPath(navMeshAgent.path, navMeshAgent.destination);
       
        if (navMeshAgent.isStopped || navMeshAgent.remainingDistance < 1.2f)//有没有到达，状态停止
        {
            StopNav();
            return;
        }
        
    }

    private void FixedUpdate()
    {
        if (character == null)
        {
            return;
        }


        if (autoNav)
        {
            NavMove();
            return;
        }
        if (InputManager.Instance != null && InputManager.Instance.IsInputMode)
            return;

        ////输入模式不接受角色控制
       // if (InputManager.Instance.IsInputMode) return;
        //if (InputManager.Instance != null && InputManager.Instance.IsInputMode) return;

        float v = Input.GetAxis("Vertical");
        if (v > 0.01f)//向前
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveFwd);
            }
            this.rigidbody.velocity = this.rigidbody.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;

        }
        else if (v < -0.01f)//向后
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveBack();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            this.rigidbody.velocity = this.rigidbody.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else
        {
            if (state != SkillBridge.Message.CharacterState.Idle)
            {
                state = SkillBridge.Message.CharacterState.Idle;
                this.rigidbody.velocity = Vector3.zero;
                this.character.Stop();
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.Jump);
        }

        float h = Input.GetAxis("Horizontal");
        if (h < -0.1 || h > 0.1)//旋转
        {
            this.transform.Rotate(0, h * rotateSpeed, 0);
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion quaternion = new Quaternion();
            quaternion.SetFromToRotation(dir, this.transform.forward);

            if (quaternion.eulerAngles.y > this.turnAngle && quaternion.eulerAngles.y < (360 - this.turnAngle))

            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                rigidbody.transform.forward = this.transform.forward;
                this.SendEntityEvent(EntityEvent.None);
            }

        }
        //Debug.LogFormat("velocity {0}", this.rb.velocity.magnitude);
    }
    Vector3 lastPos;
    float laseSyne = 0;

    private void LateUpdate()
    {
        if (this.character == null) return;

        Vector3 offset = this.rigidbody.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        this.lastPos = this.rigidbody.transform.position;

        ///新加的两句
        Vector3Int goLoginPos = GameObjectTool.WorldToLogic(this.rigidbody.transform.position);
        float logicOffset = (goLoginPos - this.character.position).magnitude;

        //if ((GameObjectTool.WorldToLogic(this.rigidbody.transform.position) - this.character.position).magnitude > 50)
        if (logicOffset > 100)
        {
            {
                this.character.SetPosition(GameObjectTool.WorldToLogic(this.rigidbody.transform.position));
                this.SendEntityEvent(EntityEvent.None);
            }
            this.transform.position = this.rigidbody.transform.position;//位置同步


            ///寻路加的
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);

            if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                this.SendEntityEvent(EntityEvent.None);
            }


        }
    }



    public void SendEntityEvent(EntityEvent entityEvent, int param=0)
    {
        if (entityController != null)
            entityController.OnEntityEvent(entityEvent,param);
        MapService.Instance.SendMapEntitySyne(entityEvent, this.character.EntityData,param);
    }
}

