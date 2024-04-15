using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Google.Protobuf.Protocol;

public class WaveSpawner : MonoBehaviour
{
    //public static int EnemiesAlive;

    //public GameObject[] monsters;
    //public Wave[] waves;
    int waveCount = 5;

    //public Transform spawnPoint;    // 스폰할 위치

    public float timeBetweenWaves = 20f;
    private float countdown = 4f;
    //private bool isFirstWave = true;
    int expandCount = 0;
    int bossNum = 1;

    //public TextMeshProUGUI waveCountdownText;

    //private int waveIndex = 0;

    MyMapController map;

    //int monsterType;

    float bossTime = 60f;

    GameObject[] monsters;

    void Start()
    {
        //EnemiesAlive = 0;
        //otherScriptInstance = GameObject.Find("GameMaster").GetComponent<Map>();
        //otherScriptInstance = Managers.Game.map;
        //monsterType = GameManager.instance.pool.monsterPools.Length;
        //Managers.Scene.sceneFader.isFading = false;
        map = GetComponent<MyMapController>();
        Managers.Sound.Play("Bgms/old-story-from-scotland-147143", Define.Sound.Bgm);
        monsters = Resources.LoadAll<GameObject>("Prefabs/Monster");
    }

    void Update()
    {
        if (!Managers.Game.isLive)
        {
            return;
        }

        //if (EnemiesAlive > 0)
        //{
        //    return;
        //}

        if (!map.startSpawnEnemy)
        {
            return;
        }

        if (countdown <= 0f)
        {
            //if (isFirstWave)
            //{
            //    isFirstWave = false;
            //}
            //else
            //{
            if (expandCount > 1)
            {
                expandCount = 0;
                EnemyStat.AddHp += 20;
                waveCount = Mathf.RoundToInt(waveCount * 1.1f);
                Mathf.Clamp(waveCount, 0, 10);
                map.ExpendMap();
            }
            expandCount++;
            
            //}
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
            return;
        }

        countdown -= Time.deltaTime;

        countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);

        // 1분에 한번 출현
        if (Managers.Game.gameTime >= bossTime)
        {
            Debug.Log("spawnBoss");
            StartCoroutine(BossSpawnWave());
            bossTime *= 2f;
        }
    }

    IEnumerator SpawnWave()
    {
        Managers.Game.Rounds++;

        yield return new WaitForSeconds(6f);
        for (int i = 0; i < waveCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }

        //Wave wave = waves[waveIndex];

        //for (int i = 0; i < wave.count; i++)
        //{
        //    //SpawnEnemy(wave.enemy);
        //    //SpawnEnemy(monsters[Random.Range(0, monsters.Length)]);
        //    SpawnEnemy();
        //    yield return new WaitForSeconds(1f / wave.rate);
        //}

        //waveIndex++;

        //if (waveIndex == waves.Length)
        //{
        //    Debug.Log("LEVEL WON!");
        //    this.enabled = false;
        //}
    }

    IEnumerator BossSpawnWave()
    {
        for (int i = 0; i < bossNum; i++)
        {
            SpawnBossEnemy();
            yield return new WaitForSeconds(0.5f);
        }
        if (bossNum < 2)
        {
            bossNum++;
        }
        else
        {
            EnemyStat.BossAddHp += 100;
        }
    }

    void SpawnEnemy()
    {
        // 몬스터 원점이 발임
        int idx = Random.Range(0, monsters.Length);
        while (monsters[idx].name == "SalarymanDefault")
        {
            idx = Random.Range(0, monsters.Length);
        }
        //Debug.Log(monsters[Random.Range(0, monsters.Length)].name);
        GameObject monster = Managers.Resource.Instantiate($"Monster/{monsters[idx].name}", map.startObj.transform.position, map.startObj.transform.rotation);
        UpdateRoads(monster);
        //EnemiesAlive++;

        CheckUpdatedSpawnEnemy(monsters[idx].name);
    }

    void SpawnBossEnemy()
    {
        //Debug.Log("spawnBoss");
        Managers.Sound.Play("Bgms/battle-of-the-dragons-8037", Define.Sound.Bgm);
        GameObject monster = Managers.Resource.Instantiate("Monster/SalarymanDefault", map.startObj.transform.position, map.startObj.transform.rotation);
        UpdateRoads(monster);

        CheckUpdatedSpawnEnemy("SalarymanDefault");
    }

    void UpdateRoads(GameObject go)
    {
        go.GetComponent<EnemyMovement>().nextRoad = Managers.Object.MyMap.roads.First.Next;
        go.GetComponent<EnemyMovement>().mapId = Managers.Object.MyMap.Id;
    }

    void CheckUpdatedSpawnEnemy(string enemyName)
    {
        C_SpawnEnemy spawnEnemyPacket = new C_SpawnEnemy();
        spawnEnemyPacket.EnemyName = enemyName;
        Managers.Network.Send(spawnEnemyPacket);
    }
}

