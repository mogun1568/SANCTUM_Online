using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyControl))]
public class EnemyMovement : MonoBehaviour
{
    /*private Transform target;
    private int wavepointIndex = 0;

    private Enemy enemy;

    void Start()
    {
        enemy = GetComponent<Enemy>();

        target = Waypoints.points[0];
    }

    void Update()
    {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * enemy.speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) <= 0.4f)
        {
            GetNextWatpoint();
        }

        enemy.speed = enemy.monster.startSpeed;
    }

    void GetNextWatpoint()
    {
        if (wavepointIndex >= Waypoints.points.Length - 1)
        {
            EndPath();
            return;
        }

        wavepointIndex++;
        target = Waypoints.points[wavepointIndex];
    }

    void EndPath()
    {
        PlayerStats.Lives--;
        WaveSpawner.EnemiesAlive--;
        Destroy(gameObject);
    }*/


    public LinkedListNode<LocationInfo> nextRoad;

    //private Vector3 target;
    //private LinkedListNode<Vector3> Mnode;
    //private Enemy enemy;
    EnemyControl enemyControl;

    Animator anim;

    // EnemyStat로 옮길 예정
    public int mapId;

    public enum MonsterState
    {
        Moving,
        Attacking,
        Die,
    }

    MonsterState _state;

    Vector3 targetPosition;
    Vector3 direction;

    void UpdateMoving()
    {
        if (enemyControl._stat.HP <= 0)
        {
            _state = MonsterState.Die;
        }

        if (enemyControl.isAttack)
        {
            _state = MonsterState.Attacking;
        }

        //targetPosition = new Vector3(nextRoad.Value.R, 1, nextRoad.Value.C);
        direction = (targetPosition - transform.position).normalized;

        // Translate 메서드를 사용하여 오브젝트를 이동
        transform.Translate(enemyControl._stat.Speed * Time.deltaTime * direction, Space.World);

        // 현재 타겟 위치에 도달하면 다음 타겟으로 변경
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // 다음 타겟 설정
            GetNextWatpoint();
        }

        if (nextRoad == null)
        {
            return;
        }

        targetPosition = new Vector3(nextRoad.Value.R, 1, nextRoad.Value.C);
        direction = (targetPosition - transform.position).normalized;

        //if (Mnode == null)
        //{
        //    return;
        //}
        //Vector3 dir = target - transform.position;
        //transform.Translate(enemyControl._stat.Speed * Time.deltaTime * dir.normalized, Space.World);

        //if (Vector3.Distance(transform.position, target) <= 0.4f)
        //{
        //    GetNextWatpoint();
        //    dir = target - transform.position;
        //}

        // 부드럽게 변경
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10 * Time.deltaTime);

        // 애니메이션
        anim.SetBool("isAttack", enemyControl.isAttack);
        anim.SetFloat("health", enemyControl._stat.HP);

        //enemy.speed = enemy.monster.startSpeed;
    }

    void UpdateAttacking()
    {
        if (enemyControl._stat.HP <= 0)
        {
            _state = MonsterState.Die;
        }

        enemyControl.LockOnTarget();
        anim.SetBool("isAttack", true);
    }

    void UpdateDie()
    {
        anim.SetFloat("health", 0);
    }

    void OnEnable() // pool 때문에 Start에서 OnEnable로 바꿈
    {
        //nextRoad = map.roads.First.Next;

        //Mnode = Map.points.First;
        //target = Mnode.Value;
        enemyControl = GetComponent<EnemyControl>();

        _state = MonsterState.Moving;
        anim = GetComponent<Animator>();
        anim.SetBool("isAttack", false);
        anim.SetFloat("health", 100);
    }

    void Update()
    {
        if (!Managers.Game.isLive)
        {
            return;
        }

        if (enemyControl == null)
        {
            Debug.Log("null");
        }

        if (nextRoad == null)
        {
            return;
        }

        if (enemyControl._stat.HP > 0 && !enemyControl.isAttack)
        {
            _state = MonsterState.Moving;
        }

        switch (_state)
        {
            case MonsterState.Moving:
                UpdateMoving();
                break;
            case MonsterState.Attacking:
                UpdateAttacking();
                break;
            case MonsterState.Die:
                UpdateDie();
                break;
        }
    }

    void GetNextWatpoint()
    {
        nextRoad = nextRoad.Next;
        if (nextRoad == null)
        {
            EndPath();
            return;
        }
        targetPosition = new Vector3(nextRoad.Value.R, 1, nextRoad.Value.C);

        //if (Mnode.Next == null)
        //{
        //    EndPath();
        //    return;
        //}
        //Mnode = Mnode.Next;
        //target = Mnode.Value;
    }

    void EndPath()
    {
        if (mapId == Managers.Object.MyMap.Id)
        {
            Managers.Sound.Play("Effects/Hit3", Define.Sound.Effect);
            int lifeLost = (int)gameObject.GetComponent<EnemyStat>().LifeLost;
            Managers.Game.Lives -= lifeLost;
            //WaveSpawner.EnemiesAlive--;
        }
        Managers.Resource.Destroy(gameObject);
    }
}