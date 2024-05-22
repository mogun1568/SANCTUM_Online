using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseController
{
    public override void Update()
    {
        UpdateMoving();
    }

    protected override void UpdateMoving()
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
            State = CreatureState.Moving;
        }
    }
}
