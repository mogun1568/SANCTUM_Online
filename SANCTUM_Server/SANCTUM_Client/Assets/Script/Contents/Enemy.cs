using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /*public MonsterData monster;

    string towerTag = "Tower";

    bool isDie;

    [HideInInspector] public bool isAttack;

    //public float startSpeed = 10f;

    [HideInInspector] public static float addHealth = 0;
    [HideInInspector] public float health;


    [HideInInspector]
    public float speed;

    private float range = 8f;

    private Transform target;

    private float fireRate;              // �߻� ����
    private float fireCountdown = 0f;   // �߻� �ð� ���� ����

    private Coroutine dotCoroutine;
    private Coroutine slowCoroutine;

    public GameObject deathEffect;

    [SerializeField] ParticleSystem gunParticle;

    void OnEnable() // pool ������ Start���� OnEnable�� �ٲ�
    {
        //speed = startSpeed;
        isDie = false;
        health = monster.startHealth + addHealth;
        speed = monster.startSpeed;
        fireRate = monster.fireRate;
        target = null;
        isAttack = false;

        if (monster.monsterType != MonsterData.MonsterType.General)
        {
            InvokeRepeating("UpdateTarget", 0f, 0.5f);  // 0�� �Ŀ� 0.5�� ���� ����
        }
    }

    void UpdateTarget()
    {
        if (!GameManager.instance.isLive)
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

        if (nearestTower != null && shortestDistance <= range)
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
        if (monster.monsterType == MonsterData.MonsterType.General)
        {
            return;
        }

        if (!GameManager.instance.isLive)
        {
            return;
        }

        if (target == null)
        {
            return;
        }

        if (fireCountdown <= 0f)
        {
            AttackTower();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    public void AttackTower()
    {
        //target.GetComponent<Turret>().TakeDamage(monster.damage);
        gunParticle.Play();
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        //Debug.Log(health);

        if (!isDie && health <= 0)
        {
            Die();
            isDie = true;
        }
    }

    public void Slow(float amount, float pct)
    {
        TakeDamage(amount);
        speed = monster.startSpeed * (1f - pct);
        // ���� �ذ�(�ڷ�ƾ ���۵� ���ߴµ� ������ ������, �ڷ�ƾ ���߿� �״°� ����X)
        if (health > 0)
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
        speed = monster.startSpeed;
    }

    // ������ ���� �̹� �׾������� �Լ��� ���ư� �� ����(���� ����Ʈ�� �׾ �ݺ��� ����)
    public void DotDamage(float amount)
    {
        TakeDamage(amount);
        // ���� �ذ�(�ڷ�ƾ ���۵� ���ߴµ� ������ ������, �ڷ�ƾ ���߿� �״°� ����X)
        if (health > 0)
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
            //Debug.Log(health);
        }
    }

    //public AudioClip audioClip;

    void Die()
    {
        StopAllCoroutines();

        Managers.Sound.Play("Effects/Monster_Die", Define.Sound.Effect);
        //GameManager.instance.soundManager.Play("Effects/Monster_Die", SoundManager.Sound.Effect);

        if (monster.monsterType == MonsterData.MonsterType.Boss)
        {
            Managers.Sound.Play("Bgms/old-story-from-scotland-147143", Define.Sound.Bgm);
            //GameManager.instance.soundManager.Play("Bgms/old-story-from-scotland-147143", SoundManager.Sound.Bgm);
        }

        Debug.Log("Die");

        //WaveSpawner.EnemiesAlive--;

        GameManager.instance.GetExp(monster.exp);

        Invoke("InvokeDeath", 1f);
    }

    void InvokeDeath()
    {
        GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);

        Managers.Resource.Destroy(gameObject);
        //gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 5);
    }*/
}