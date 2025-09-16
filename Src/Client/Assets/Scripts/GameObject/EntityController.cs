using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Managers;
using Assets.Scripts.Entities;
/// <summary>
/// 根据服务器发过来的数据驱动操作逻辑行为，其他的玩家对象
/// </summary>
public class EntityController : MonoBehaviour,IEntityNotify,IEntityController
{ 

    public Animator anim;
    public Rigidbody rb;
    private AnimatorStateInfo currentBaseState;

    public Entity entity;

    public UnityEngine.Vector3 position;
    public UnityEngine.Vector3 direction;
    Quaternion rotation;

    public UnityEngine.Vector3 lastPosition;
    Quaternion lastRotation;

    public float speed;
    public float animSpeed = 1.5f;
    public float jumpPower = 3.0f;

    public bool isPlayer = false;//是不是玩家

    public RideController rideController;

    private int currentRide = 0;

    /// <summary>
    /// 坐骑骨骼位置点
    /// </summary>
    public Transform rideBone;

    // Use this for initialization
    void Start () {
        if (entity != null)
        {
            EntityManager.Instance.RegisterEntityChangeNotify(entity.entityId, this);
            this.UpdateTransform();
        }

        if (!this.isPlayer)
            rb.useGravity = false;//不是玩家把刚体使用重力关掉，
    }

    void UpdateTransform()
    {
        this.position = GameObjectTool.LogicToWorld(entity.position);//逻辑坐标转世界
        this.direction = GameObjectTool.LogicToWorld(entity.direction);

        this.rb.MovePosition(this.position);
        this.transform.forward = this.direction;
        this.lastPosition = this.position;
        this.lastRotation = this.rotation;
    }
	
    void OnDestroy()
    {
        if (entity != null)
            Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, entity.entityId, entity.position, entity.direction, entity.speed);

        if (UIWorldElementManager.Instance != null)//万一管理器已经被销毁了，
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.entity == null)
            return;

        this.entity.OnUpdate(Time.fixedDeltaTime);

        if (!this.isPlayer)
        {
            this.UpdateTransform();
        }
    }
	
	public void OnEntityChanged(Entity entity)
    {
        Debug.LogFormat("OnEntityChanged :ID:{0} POS:{1} DIR:{2} SPD:{3} ", entity.entityId, entity.position, entity.direction, entity.speed);
    }

     
    public void OnEntityRemoved()
    {
        if (UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        Destroy(this.gameObject);
    }

    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch(entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
            case EntityEvent.Ride:
                {
                    this.Ride(param);
                }
                break;
        }
        ///我移动坐骑也移动
        ///
        if (this.rideController != null) this.rideController.OnEntityEvent(entityEvent, param);
    }

    public void Ride(int rideId)
    {
        if (currentRide == rideId) return;
        currentRide = rideId;
        ///上坐骑
        if (rideId > 0)
        {
            this.rideController = GameObjectManager.Instance.LoadRide(rideId, this.transform);///设置坐骑和坐上去的位置
        }

        else
        {
            Destroy(this.rideController.gameObject);

            this.rideController = null;
        }

        //下坐骑
        if (this.rideController == null)
        {
            this.anim.transform.localPosition = Vector3.zero;
            this.anim.SetLayerWeight(1, 0);/////设置权重
        }
        else
        {
            this.rideController.SetRider(this);////设置骑坐骑的人 着
            this.anim.SetLayerWeight(1, 1);
        }
    }

    /// <summary>
    /// 坐骑蹦蹦跳跳的时候保证人物和坐骑始终在一起
    /// </summary>
    /// <param name="position"></param>
    public void SetRidePosition(Vector3 position)
    {
        this.anim.transform.position = position + (this.anim.transform.position - this.rideBone.position);
    }

    void OnMouseDown()
    {
        Creature target=this.entity as Creature;///检查
        if (target.IsCurrentPlayer)
        {
            return;
        }
        BattleManager.Instance.CurrentTarget = this.entity as Creature; 
    }

    public void PlayAnim(string name)
    {
        this.anim.SetTrigger(name);
    }

    public void SetStandby(bool standby)
    {
        this.anim.SetBool("Standby", standby);
    }
}
