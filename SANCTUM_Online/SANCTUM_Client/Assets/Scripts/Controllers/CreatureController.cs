using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyMovement;

public class CreatureController : BaseController
{
    protected Animator _animator;

    public override CreatureState State
    {
        get { return base.State; }
        set { base.State = value; UpdateAnimation(); }
    }

    public override void Update()
    {
        UpdateController();
    }

    protected override void Init()
    {
        base.Init();

        _animator = GetComponent<Animator>();
        //transform.position += new Vector3(0, 0, 0);

        UpdateAnimation();
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Attacking:
                UpdateAttacking();
                break;
            case CreatureState.Die:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateAnimation()
    {
        if (_animator == null)
            return;

        switch (State)
        {
            case CreatureState.Moving:
                _animator.Play("Run");
                break;
            case CreatureState.Attacking:
                _animator.Play("Attack");
                break;
            case CreatureState.Die:
                _animator.Play("Die");
                break;
        }
    }

    protected virtual void UpdateIdle()
    {

    }

    protected override void UpdateMoving()
    {
        base.UpdateMoving();
    }

    protected virtual void UpdateAttacking()
    {

    }

    protected virtual void UpdateDead()
    {
        Debug.Log($"Die {Id}");
    }
}
