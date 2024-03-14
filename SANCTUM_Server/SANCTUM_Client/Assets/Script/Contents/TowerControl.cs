using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TowerControl : MonoBehaviour
{

    [HideInInspector] public TowerStat _stat;

    private Transform target;

    [HideInInspector] public Data.Item itemData;

    [Header("General")]

    [Header("Use Bullets (default)")]
    string bulletType;  // 총알 타입

    //[Header("Use Laser (default)")]
    //public bool useLaser = false;

    //public int damageOverTime = 30;
    //public float slowAmount = .5f;

    //public LineRenderer lineRenderer;   // 레이저
    //public ParticleSystem impactEffect; // 레이저 이펙트
    ////public Light impactLight;           // 레이저 이펙트 조명

    [Header("Unity Setup Fields")]

    public string enemyTag = "Enemy";
    public string towerTag = "Tower";

    [SerializeField] Transform partToRotate;
    public float turnSpeed = 10f;

    [SerializeField] Transform firePoint;

    // 1인칭모드 변수
    public bool isFPM;

    public bool isHealTower;

    void OnEnable()
    {
        partToRotate = Util.FindChild(gameObject, "PartToRotate", true)?.transform;
        firePoint = Util.FindChild(gameObject, "FirePoint", true)?.transform;
        if (firePoint == null)
        {
            Debug.Log("null");
            firePoint = partToRotate;
        }

        itemData = Managers.Select.getItemData();

        gameObject.GetOrAddComponent<Poolable>();
        _stat = gameObject.GetOrAddComponent<TowerStat>();

        if (_stat.TowerType == "StandardTower")
        {
            _stat.IsStandard(); 
        }

        target = null;
        isHealTower = false;
        if (itemData.itemName == "Water")
        {
            isHealTower = true;
        }
        isFPM = false;
        bulletType = itemData.bulletType;
        transform.GetComponentsInChildren<Camera>(true)[0].gameObject.SetActive(false);

        if (!isHealTower)
        {
            InvokeRepeating("UpdateTarget", 0f, 0.5f);  // 0초 후에 0.5초 마다 실행
        }
    }

    void UpdateTarget()
    {
        if (isFPM)
        {
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            //if (enemy.GetComponent<Enemy>().health <= 0)
            //{
            //    continue;
            //}

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= _stat.Range)
        {
            target = nearestEnemy.transform;
            //targetEnemy = nearestEnemy.GetComponent<Enemy>();          
        }
        else
        {
            target = null;
        }
    }

    void Update()
    {
        if (!Managers.Game.isLive)
        {
            return;
        }

        if (isFPM)
        {
            return;
        }

        // 타워 정보 보이게 하는 용도(눌려져 있는 동안 실행)
        //if (Input.GetKey(KeyCode.C))
        //{

        //}

        if (target == null && !isHealTower)
        {
            //if (useLaser)
            //{
            //    if (lineRenderer.enabled)
            //    {
            //        lineRenderer.enabled = false;
            //        impactEffect.Stop();
            //        //impactLight.enabled = false;
            //    }
            //}

            return;
        }

        LockOnTarget();

        //if (useLaser)
        //{
        //    Laser();
        //} else
        //{
        if (_stat.FireCountdown <= 0f)
        {
            if (!isHealTower)
            {
                Shoot();
            }
            else
            {
                FindTowersInRadius();
            }
            _stat.FireCountdown = 1f / _stat.FireRate;
        }

        _stat.FireCountdown -= Time.deltaTime;
        //}
    }

    void ShowTowerInfo()
    {
        // 상단에 public으로 UI 대려오고
        // 원소는 data.itemIcon으로 데려오렴 될듯

        //range
        //fireRate
        //bulletSpeed
        //bulletDamage
        //health
    }

    void LockOnTarget()
    {
        if (partToRotate == null)
        {
            return;
        }
        // 타겟 시점 고정
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    public void TakeDamage(float amount)
    {
        _stat.HP -= amount;
        //Debug.Log(health);

        if (_stat.HP <= 0)
        {
            // 함수로 구현 예정
            Managers.Sound.Play("Effects/Explosion", Define.Sound.Effect);
            //GameManager.instance.soundManager.Play("Effects/Explosion", SoundManager.Sound.Effect);
            GameObject effect = Managers.Resource.Instantiate("Tower/Prefab/Void Explosion", transform.position, Quaternion.identity);
            StartCoroutine(DestroyEffect(effect));
            //effect.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            //GameObject effect = (GameObject)Instantiate(BuildManager.instance.destroyEffect, transform.position, Quaternion.identity);

            if (isFPM)
            {
                GetComponentInChildren<FirstPersonCamera>().ExitFirstPersonMode();
                //Camera.main.gameObject.SetActive(true);
                //Managers.Game.isFPM = false;
                //Cursor.lockState = CursorLockMode.None;
            }

            Managers.Resource.Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }

    //void Laser()
    //{
    //    targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
    //    targetEnemy.Slow(slowAmount);

    //    if (!lineRenderer.enabled)
    //    {
    //        lineRenderer.enabled = true;
    //        impactEffect.Play();
    //        //impactLight.enabled = true;
    //    }

    //    lineRenderer.SetPosition(0, firePoint.position);
    //    lineRenderer.SetPosition(1, target.position);

    //    Vector3 dir = firePoint.position - target.position;

    //    impactEffect.transform.position = target.position + dir.normalized;

    //    impactEffect.transform.rotation = Quaternion.LookRotation(dir);
    //}

    void Shoot()
    {
        Debug.Log("shoot");
        Managers.Sound.Play("Effects/Arrow", Define.Sound.Effect);
        //GameManager.instance.soundManager.Play("Effects/Arrow", SoundManager.Sound.Effect);
        GameObject bulletGO = Managers.Resource.Instantiate($"Tower/Prefab/Bullet/{bulletType}", firePoint.position, firePoint.rotation);
        //bulletGO.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
        //GameObject bulletGO = GameManager.instance.pool.GetBullet(bulletIndex, firePoint.position, firePoint.rotation);
        //bulletGO.transform.GetChild(0).gameObject.SetActive(true);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        bullet.isFPM = false;
        bullet.speed = _stat.BulletSpeed;
        bullet.damage = _stat.BulletDamage;

        if (bullet != null)
        {
            bullet.Seek(target);
        }
    }

    void FindTowersInRadius()
    {
        //Debug.Log("1");
        GameObject[] towers = GameObject.FindGameObjectsWithTag(towerTag);
        foreach (GameObject tower in towers)
        {
            //Debug.Log("2");
            float distanceToTower = Vector3.Distance(transform.position, tower.transform.position);
            if (distanceToTower <= _stat.Range && distanceToTower > 1)
            {
                //Debug.Log("2");
                Heal(tower, _stat.BulletDamage * 0.2f);
            }
        }
    }

    void Heal(GameObject _tower, float healAmount)
    {
        //Debug.Log("3");
        TowerStat towerStat = _tower.GetComponent<TowerControl>()._stat;
        if (towerStat.HP + healAmount > towerStat.MaxHp)
        {
            // towerHealth 변수에 대한 지역적인 작업이므로 실제 Turret 컴포넌트의 health 값에는 영향을 주지 않음
            //towerHealth = 100f;
            _tower.GetComponent<TowerControl>()._stat.HP = towerStat.MaxHp;
        }
        else
        {
            //towerHealth += healAmount;
            _tower.GetComponent<TowerControl>()._stat.HP += healAmount;
        }
        //Vector3 t = _tower.GetComponent<Turret>().transform.position;
        //Debug.Log((t, towerHealth));
    }

    IEnumerator DestroyEffect(GameObject effect)
    {
        yield return new WaitForSeconds(5f);

        Managers.Resource.Destroy(effect);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _stat.Range);
    }
}
