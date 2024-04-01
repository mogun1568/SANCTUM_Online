using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.UIElements;

public class Turret : MonoBehaviour
{
    /*private Transform target;
    //private Enemy targetEnemy;

    [HideInInspector] public Data.Item itemData;
    //[HideInInspector] public ItemData data;

    [Header("General")]
    [HideInInspector]
    public float range;
    [HideInInspector]
    public float health;

    [Header("Use Bullets (default)")]
    //public GameObject bulletPrefab;     // �Ѿ� ������
    string bulletType;
    //[HideInInspector] public int bulletIndex;
    [HideInInspector]
    public float fireRate;              // �߻� ����
    private float fireCountdown = 0f;   // �߻� �ð� ���� ����
    [HideInInspector]
    public float bulletSpeed;
    [HideInInspector]
    public float bulletDamage;

    //[Header("Use Laser (default)")]
    //public bool useLaser = false;

    //public int damageOverTime = 30;
    //public float slowAmount = .5f;

    //public LineRenderer lineRenderer;   // ������
    //public ParticleSystem impactEffect; // ������ ����Ʈ
    ////public Light impactLight;           // ������ ����Ʈ ����

    [Header("Unity Setup Fields")]

    public string enemyTag = "Enemy";
    public string towerTag = "Tower";

    public Transform partToRotate;
    public float turnSpeed = 10f;

    public Transform firePoint;

    // 1��Ī��� ����
    public bool isFPM;

    public bool isHealTower;



    void OnEnable()
    {
        itemData = Managers.Select.getItemData();

        if (itemData.itemName == "StandardTower")
        {
            range = 15f;
            fireRate = 1f;
            bulletSpeed = 50f;
            bulletDamage = 50f;
            health = 100f;
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
            InvokeRepeating("UpdateTarget", 0f, 0.5f);  // 0�� �Ŀ� 0.5�� ���� ����
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

        if (nearestEnemy != null && shortestDistance <= range)
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
        if (!GameManager.instance.isLive)
        {
            return;
        }

        if (isFPM)
        {
            if (Input.GetMouseButtonDown(0))  // ���콺 ��Ŭ���� ����
            {
                //FireBullet();
            }

            return;
        }

        // Ÿ�� ���� ���̰� �ϴ� �뵵(������ �ִ� ���� ����)
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
        if (fireCountdown <= 0f)
        {
            if (!isHealTower)
            {
                Shoot();
            }
            else
            {
                FindTowersInRadius();
            }
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
        //}
    }

    void ShowTowerInfo()
    {
        // ��ܿ� public���� UI �������
        // ���Ҵ� data.itemIcon���� �������� �ɵ�

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
        // Ÿ�� ���� ����
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        //Debug.Log(health);

        if (health <= 0)
        {
            // �Լ��� ���� ����
            Managers.Sound.Play("Effects/Explosion", Define.Sound.Effect);
            //GameManager.instance.soundManager.Play("Effects/Explosion", SoundManager.Sound.Effect);
            GameObject effect = Managers.Resource.Instantiate("Tower/Prefab/Void Explosion");
            effect.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            //GameObject effect = (GameObject)Instantiate(BuildManager.instance.destroyEffect, transform.position, Quaternion.identity);

            if (isFPM)
            {
                GameManager.instance.MainCamera.SetActive(true);
                GameManager.instance.isFPM = false;
                Cursor.lockState = CursorLockMode.None;
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
        Managers.Sound.Play("Effects/Arrow", Define.Sound.Effect);
        //GameManager.instance.soundManager.Play("Effects/Arrow", SoundManager.Sound.Effect);
        GameObject bulletGO = Managers.Resource.Instantiate($"Tower/Prefab/Bullet/{bulletType}");
        bulletGO.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
        //GameObject bulletGO = GameManager.instance.pool.GetBullet(bulletIndex, firePoint.position, firePoint.rotation);
        bulletGO.transform.GetChild(0).gameObject.SetActive(true);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        bullet.isFPM = false;
        bullet.speed = bulletSpeed;
        bullet.damage = bulletDamage;

        if (bullet != null)
        {
            bullet.Seek(target);
        }
    }

    void FireBullet()
    {
        Managers.Sound.Play("Effects/Arrow", Define.Sound.Effect);
        //GameManager.instance.soundManager.Play("Effects/Arrow", SoundManager.Sound.Effect);
        GameObject bulletGO = Managers.Resource.Instantiate("Tower/Prefab/Bullet/StandardBullet");
        bulletGO.transform.SetPositionAndRotation(transform.position, transform.rotation);
        //GameObject bulletGO = GameManager.instance.pool.GetBullet(turretData.bulletIndex, transform.position, transform.rotation);  // �Ѿ� ����
        bulletGO.transform.GetChild(0).gameObject.SetActive(false);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        bullet.isFPM = true;
        bullet.damage = bulletDamage * 1.5f;
        float bulletForce = bulletSpeed * 1.5f;

        bullet.firePoint = transform.position;
        bullet.range = range;

        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        // �Ѿ˿� ���� ���� �߻�
        bulletRigidbody.velocity = transform.forward * bulletForce;
    }

    void FindTowersInRadius()
    {
        //Debug.Log("1");
        GameObject[] towers = GameObject.FindGameObjectsWithTag(towerTag);
        foreach (GameObject tower in towers)
        {
            //Debug.Log("2");
            float distanceToTower = Vector3.Distance(transform.position, tower.transform.position);
            if (distanceToTower <= range && distanceToTower > 1)
            {
                //Debug.Log("2");
                Heal(tower, bulletDamage * 0.2f);
            }
        }
    }

    void Heal(GameObject _tower, float healAmount)
    {
        //Debug.Log("3");
        float towerHealth = _tower.GetComponent<Turret>().health;
        if (towerHealth + healAmount > 100f)
        {
            // towerHealth ������ ���� �������� �۾��̹Ƿ� ���� Turret ������Ʈ�� health ������ ������ ���� ����
            //towerHealth = 100f;
            _tower.GetComponent<Turret>().health = 100f;
        }
        else
        {
            //towerHealth += healAmount;
            _tower.GetComponent<Turret>().health += healAmount;
        }
        Vector3 t = _tower.GetComponent<Turret>().transform.position;
        //Debug.Log((t, towerHealth));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }*/
}
