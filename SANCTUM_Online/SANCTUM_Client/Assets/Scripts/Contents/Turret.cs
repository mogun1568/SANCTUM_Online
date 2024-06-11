using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : BaseController
{
    Transform _partToRotate;

    void OnEnable()
    {
        _partToRotate = Util.FindChild(gameObject, "PartToRotate", true)?.transform;
        Util.FindChild(gameObject, "Camera", true)?.SetActive(false);
    }

    public override void Update()
    {
        base.Update();

        if (IsFPM)
        {
            return;
        }

        if (State != CreatureState.Attacking)
        {
            return;
        }

        LockOnTarget();
    }

    public override void LockOnTarget()
    {
        // 타겟 시점 고정
        Vector3 pos = new Vector3(_targetPos.PosX, _targetPos.PosY, _targetPos.PosZ);
        Vector3 dir = pos - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(_partToRotate.rotation, lookRotation, Time.deltaTime * _turnSpeed).eulerAngles;
        _partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }
}
