using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    [SerializeField]
    public int Id { get; set; }

    StatInfo _stat = new StatInfo();
    public StatInfo Stat
    {
        get { return _stat; }
        set
        {
            if (_stat.Equals(value))
            {
                return;
            }

            _stat = value;
        }
    }

    public float Speed
    {
        get { return Stat.Speed; }
        set { Stat.Speed = value; }
    }

    public virtual int Hp
    {
        get { return Stat.Hp; }
        set { Stat.Hp = value; }
    }

    public bool IsFPM
    {
        get { return Stat.IsFPM; }
        set { Stat.IsFPM = value; }
    }

    PositionInfo _positionInfo = new PositionInfo();
    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
            {
                return;
            }

            Pos = new Vector3(value.PosX, value.PosY, value.PosZ);
            Dir = new Vector3(value.DirX, value.DirY, value.DirZ);
        }
    }

    public void SyncPos(Vector3 addPos = default)
    {
        Vector3 destPos = Pos + addPos;
        transform.position = destPos;
    }

    public Vector3 Pos
    {
        get
        {
            return new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
        }

        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y && PosInfo.PosZ == value.z)
            {
                return;
            }

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            PosInfo.PosZ = value.z;
        }
    }

    public Vector3 Dir
    {
        get
        {
            return new Vector3(PosInfo.DirX, PosInfo.DirY, PosInfo.DirZ);
        }

        set
        {
            if (PosInfo.DirX == value.x && PosInfo.DirY == value.y && PosInfo.DirZ == value.z)
            {
                return;
            }

            PosInfo.DirX = value.x;
            PosInfo.DirY = value.y;
            PosInfo.DirZ = value.z;
        }
    }

    public virtual CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
        }
    }

    void OnEnable()
    {
        Init();
    }
    public virtual void Update()
    {

    }

    protected virtual void Init()
    {
        
    }

    // 스르륵 이동하는 것을 처리
    protected virtual void UpdateMoving()
    {
        Vector3 destPos = Pos;
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < Speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * Speed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir.normalized), 10 * Time.deltaTime);
            State = CreatureState.Moving;
        }
    }

    protected virtual void MoveToNextPos()
    {

    }

    [HideInInspector]
    public float _turnSpeed = 10f;
    [HideInInspector]
    public PositionInfo _targetPos = new PositionInfo();

    public virtual void LockOnTarget()
    {
        
    }

    public virtual void OnDamaged()
    {

    }

    public virtual void OnDead()
    {
        State = CreatureState.Die;

        // TODO : Effect
    }
}
