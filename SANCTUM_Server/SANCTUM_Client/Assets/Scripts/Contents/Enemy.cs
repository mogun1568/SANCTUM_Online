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

    private float fireRate;              // 발사 간격
    private float fireCountdown = 0f;   // 발사 시간 조정 변수

    private Coroutine dotCoroutine;
    private Coroutine slowCoroutine;

    public GameObject deathEffect;

    [SerializeField] ParticleSystem gunParticle;

    void OnEnable() // pool 때문에 Start에서 OnEnable로 바꿈
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
            InvokeRepeating("UpdateTarget", 0f, 0.5f);  // 0초 후에 0.5초 마다 실행
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
        // 버그 해결(코루틴 시작도 안했는데 죽으면 오류남, 코루틴 도중에 죽는건 오류X)
        if (health > 0)
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
        speed = monster.startSpeed;
    }

    // 가능한 버그 이미 죽었음에도 함수가 돌아갈 수 있음(죽음 이펙트가 죽어도 반복될 수도)
    public void DotDamage(float amount)
    {
        TakeDamage(amount);
        // 버그 해결(코루틴 시작도 안했는데 죽으면 오류남, 코루틴 도중에 죽는건 오류X)
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
        // 코루틴이 실행중인지 확인하는 bool변수 만들고 endtime 변수를 만들어서 실행중일 때는 endtime 늘리는 방식으로 수정할거임
        // slow 함수도 이런식이면 될듯
        float elapsedTime = 0f;

        while (elapsedTime < 5)
        {
            // 0.5초 대기
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