using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CreatureController
{
    public override void Update()
    {
        base.Update();

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
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * _turnSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }
}