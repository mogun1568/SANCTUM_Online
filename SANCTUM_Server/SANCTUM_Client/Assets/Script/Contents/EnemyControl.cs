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

    void OnEnable() // pool ������ Start���� OnEnable�� �ٲ�
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
            InvokeRepeating("UpdateTarget", 0f, 0.5f);  // 0�� �Ŀ� 0.5�� ���� ����
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
            // EnemyMovement���� isAttack �ҷ��ͼ� �۵�
        }
        else
        {
            target = null;
            isAttack = false;
            // EnemyMovement���� isAttack �ҷ��ͼ� �۵�
        }
    }

    public void LockOnTarget()
    {
        // Ÿ�� ���� ����
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
        // ���� �ذ�(�ڷ�ƾ ���۵� ���ߴµ� ������ ������, �ڷ�ƾ ���߿� �״°� ����X)
        if (_stat.HP > 0)
        {
            // �ڷ�ƾ ���� �߿� �� ����Ƿ��ϸ� �����
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

    // ������ ���� �̹� �׾������� �Լ��� ���ư� �� ����(���� ����Ʈ�� �׾ �ݺ��� ����)
    public void DotDamage(float amount)
    {
        TakeDamage(amount);
        // ���� �ذ�(�ڷ�ƾ ���۵� ���ߴµ� ������ ������, �ڷ�ƾ ���߿� �״°� ����X)
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
        // �ڷ�ƾ�� ���������� Ȯ���ϴ� bool���� ����� endtime ������ ���� �������� ���� endtime �ø��� ������� �����Ұ���
        // slow �Լ��� �̷����̸� �ɵ�
        float elapsedTime = 0f;

        while (elapsedTime < 5)
        {
            // 0.5�� ���
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
