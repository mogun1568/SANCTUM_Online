using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [HideInInspector] public Data.Enemy enemyData;

    [HideInInspector] public EnemyStat _stat;

    string towerTag = "Tower";

    bool isDie;

    [HideInInspector] public bool isAttack;

    private Transform target;

    private Coroutine dotCoroutine;
    private Coroutine slowCoroutine;

    void OnEnable() // pool 때문에 Start에서 OnEnable로 바꿈
    {
        int index = gameObject.name.IndexOf("(Clone)");
        string enemyName;
        if (index > 0)
        {
            enemyName = gameObject.name.Substring(0, index);
        } else
        {
            enemyName = gameObject.name;
        }
        enemyData = Managers.Data.EnemyDict[enemyName];

        gameObject.GetOrAddComponent<EnemyMovement>();
        gameObject.GetOrAddComponent<Poolable>();
        _stat = gameObject.GetOrAddComponent<EnemyStat>();
        if (enemyData.enemyType == "Attack")
        {
            _stat.IsAttack();
        }
        if (enemyData.enemyType == "Boss")
        {
            _stat.IsBoss();
        }

        isDie = false;
        target = null;
        isAttack = false;

        if (enemyData.enemyType != "General")
        {
            InvokeRepeating("UpdateTarget", 0f, 0.5f);  // 0초 후에 0.5초 마다 실행
        }
    }

    void UpdateTarget()
    {
        if (!Managers.Game.isLive)
        {
            return;
        }

        GameObject[] towers = GameObject.FindGameObjectsWithTag(towerTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestTower = null;
        foreach (GameObject tower in towers)
        {
            float distanceToTower = Vector3.Distance(transform.position, tower.transform.position);
            if (distanceToTower < shortestDistance)
            {
                shortestDistance = distanceToTower;
                nearestTower = tower;
            }
        }

        if (nearestTower != null && shortestDistance <= _stat.Range)
        {
            isAttack = true;
            target = nearestTower.transform;
            // EnemyMovement에서 isAttack 불러와서 작동
        }
        else
        {
            target = null;
            isAttack = false;
            // EnemyMovement에서 isAttack 불러와서 작동
        }
    }

    public void LockOnTarget()
    {
        // 타겟 시점 고정
        Vector3 dir = target.position - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);

        //Quaternion lookRotation = Quaternion.LookRotation(dir);
        //Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f).eulerAngles;
        //transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Update()
    {
        if (enemyData.enemyType == "General")
        {
            return;
        }

        if (!Managers.Game.isLive)
        {
            return;
        }

        if (target == null)
        {
            return;
        }

        if (_stat.FireCountdown <= 0f)
        {
            AttackTower();
            _stat.FireCountdown = 1f / _stat.FireRate;
        }

        _stat.FireCountdown -= Time.deltaTime;
    }

    public void AttackTower()
    {
        Debug.Log("me");
        target.GetComponent<TowerControl>().TakeDamage(_stat.BulletDamage);
        ParticleSystem gunParticle = GetComponentInChildren<ParticleSystem>();
        gunParticle.Play();
    }

    public void TakeDamage(float amount)
    {
        _stat.HP -= amount;

        if (!isDie && _stat.HP <= 0)
        {
            Die();
            isDie = true;
        }
    }

    public void Slow(float amount, float pct)
    {
        TakeDamage(amount);
        _stat.Speed = _stat.StartSpeed * (1f - pct);
        // 버그 해결(코루틴 시작도 안했는데 죽으면 오류남, 코루틴 도중에 죽는건 오류X)
        if (_stat.HP > 0)
        {
            // 코루틴 실행 중에 또 실행되려하면 재실행
            if (slowCoroutine != null)
            {
                StopCoroutine(slowCoroutine);
            }
            slowCoroutine = StartCoroutine(ResetMonsterSpeed(3f));
        }
    }

    IEnumerator ResetMonsterSpeed(float sec)
    {
        yield return new WaitForSeconds(sec);
        _stat.Speed = _stat.StartSpeed;
    }

    // 가능한 버그 이미 죽었음에도 함수가 돌아갈 수 있음(죽음 이펙트가 죽어도 반복될 수도)
    public void DotDamage(float amount)
    {
        TakeDamage(amount);
        // 버그 해결(코루틴 시작도 안했는데 죽으면 오류남, 코루틴 도중에 죽는건 오류X)
        if (_stat.HP > 0)
        {
            if (dotCoroutine != null)
            {
                StopCoroutine(dotCoroutine);
            }
            dotCoroutine = StartCoroutine(ApplyDamageOverTime(amount * 0.1f));
        }

    }

    IEnumerator ApplyDamageOverTime(float damageAmount)
    {
        // 코루틴이 실행중인지 확인하는 bool변수 만들고 endtime 변수를 만들어서 실행중일 때는 endtime 늘리는 방식으로 수정할거임
        // slow 함수도 이런식이면 될듯
        float elapsedTime = 0f;

        while (elapsedTime < 5)
        {
            // 0.5초 대기
            yield return new WaitForSeconds(0.5f);
            TakeDamage(damageAmount);
            elapsedTime += 1f;
        }
    }

    void Die()
    {
        StopAllCoroutines();

        Managers.Sound.Play("Effects/Monster_Die", Define.Sound.Effect);

        if (enemyData.enemyType == "Boss")
        {
            Managers.Sound.Play("Bgms/old-story-from-scotland-147143", Define.Sound.Bgm);
        }

        Debug.Log("Die");

        Managers.Game.GetExp(_stat.Exp);

        Invoke("InvokeDeath", 1f);
    }

    void InvokeDeath()
    {
        GameObject deathEffect = Managers.Resource.Instantiate("DeathEffect", transform.position, Quaternion.identity);
        StartCoroutine(DestroyEffect(deathEffect));

        Managers.Resource.Destroy(gameObject);
    }

    IEnumerator DestroyEffect(GameObject effect)
    {
        yield return new WaitForSeconds(5f);

        Managers.Resource.Destroy(effect);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 5);
    }
}
