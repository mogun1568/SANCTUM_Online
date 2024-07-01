using Data;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Windows;

public class ObjectManager
{
    public RoomSelectUI RoomList {  get; set; }
    public RoomUI MyRoom { get; set; }

    public MyMapController MyMap { get; set; }
    public Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
    public List<NewMap> _players = new List<NewMap>();

    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public void roomAdd(ObjectInfo info, bool myMap = false)
    {
        if (myMap)
        {
            RoomList = Managers.UI.ShowSceneUI<RoomSelectUI>("RoomSelectUI");
        }
    }

    public void Add(ObjectInfo info, bool myMap = false)
	{
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);
        if (objectType == GameObjectType.Player)
		{
            if (myMap)
            {
                MyRoom = Managers.UI.ShowPopupUI<RoomUI>("RoomUI");

                GameObject go = Managers.Resource.Instantiate("Map/MyMap");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);
                _players.Add(go.GetComponent<NewMap>());

                MyMap = go.GetComponent<MyMapController>();
                MyMap.Id = info.ObjectId;
                MyMap.PosInfo = info.PosInfo;
                MyMap.Stat = info.StatInfo;
            }
            else
            {
                GameObject go = Managers.Resource.Instantiate("Map/Map");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);
                _players.Add(go.GetComponent<NewMap>());

                NewMap mc = go.GetComponent<NewMap>();
                mc.Id = info.ObjectId;
                mc.PosInfo = info.PosInfo;
                mc.Stat = info.StatInfo;
            }

            MyRoom.Join(info.Name, myMap);
        }
		else if (objectType == GameObjectType.Node)
		{
            GameObject go = Managers.Object.FindById(info.OwnerId);
            if (go == null)
            {
                return;
            }

            NewMap mc = go.GetComponent<NewMap>();
            if (mc == null)
            {
                return;
            }

            mc.CreateNode(info);
        }
        else if (objectType == GameObjectType.Turret)
        {
            Managers.Sound.Play("Effects/Build", Define.Sound.Effect);
            string towerName = info.Name.Substring(0, info.Name.Length - 5);
            Vector3 pos = new Vector3(info.PosInfo.PosX, info.PosInfo.PosY, info.PosInfo.PosZ);
            GameObject go = Managers.Resource.Instantiate($"Tower/Prefab/{towerName}/{info.Name}", pos, Quaternion.identity);

            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            Turret tc = go.GetComponent<Turret>();
            tc.Id = info.ObjectId;
            tc.PosInfo = info.PosInfo;
            tc.Stat = info.StatInfo;

            if (info.StatInfo.Name == "Water")
            {
                Transform healEffect = go.transform.GetChild(go.transform.childCount - 1);
                healEffect.localScale = new Vector3(tc.Stat.Range * 2, healEffect.localScale.y, tc.Stat.Range * 2);
            }

            Debug.Log("Turret build!");
        }
        else if (objectType == GameObjectType.Enemy)
        {
            Vector3 pos = new Vector3(info.PosInfo.PosX, 1, info.PosInfo.PosZ);
            Vector3 dir = new Vector3(0, info.PosInfo.DirY, 0);
            GameObject go = Managers.Resource.Instantiate($"Monster/{info.Name}", pos, Quaternion.Euler(dir));
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            Enemy ec = go.GetComponent<Enemy>();
            ec.Id = info.ObjectId;
            ec.PosInfo = info.PosInfo;
            ec.Stat = info.StatInfo;

            if (ec.Stat.Type == "Boss")
                Managers.Sound.Play("Bgms/battle-of-the-dragons-8037", Define.Sound.Bgm);

            //ec.SyncPos(new Vector3(0, 1, 0));

            /*GameObject monster = Managers.Resource.Instantiate($"Monster/{info.Name}", 
                new Vector3(info.PosInfo.PosX, 1, info.PosInfo.PosZ), 
                Quaternion.Euler(0, info.PosInfo.Dir, 0));

            EnemyControl ec = monster.GetComponent<EnemyControl>();
            ec.Stat = info.StatInfo;

            // 맵 생성 전에 몬스터가 생기는 버그가 있음 (확인해봐야 함)
            GameObject owner = Managers.Object.FindById(info.OwnerId);
            if (owner == null)
            {
                Debug.Log("No owner");
            }
            NewMap map = Managers.Object.FindById(info.OwnerId).GetComponent<NewMap>();
            if (map == null)
            {
                Debug.Log("No map");
            }
            monster.GetComponent<EnemyMovement>().nextRoad = map.roads.First.Next;
            monster.GetComponent<EnemyMovement>().mapId = map.Id;*/
        }
        else if (objectType == GameObjectType.Projectile)
        {
            Managers.Sound.Play("Effects/Arrow", Define.Sound.Effect);
            Vector3 pos = new Vector3(info.PosInfo.PosX, info.PosInfo.PosY, info.PosInfo.PosZ);
            Vector3 dir = new Vector3(info.PosInfo.DirX, info.PosInfo.DirY, info.PosInfo.DirZ);
            GameObject go = Managers.Resource.Instantiate($"Tower/Prefab/Bullet/{info.Name}", pos, Quaternion.LookRotation(dir));

            if (info.StatInfo.Level == 2)
                go.transform.GetChild(0).gameObject.SetActive(false);
            else
                go.transform.GetChild(0).gameObject.SetActive(true);

            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            Projectile pc = go.GetComponent<Projectile>();
            pc.Id = info.ObjectId;
            pc.PosInfo = info.PosInfo;
            pc.Stat = info.StatInfo;
        }
	}

    public void Remove(int id)
	{
        GameObject go = FindById(id);
        if (go == null)
        {
            return;
        }

        // 정리 필요
        GameObjectType got = GetObjectTypeById(id);
        switch (got)
        {
            case GameObjectType.Player:
                if (MyRoom != null)
                    MyRoom.exit(id);
                break;
            case GameObjectType.Turret:
                //Managers.Sound.Play("Effects/Explosion", Define.Sound.Effect);
                //Managers.Resource.Instantiate("Tower/Prefab/Void Explosion", go.transform.position, Quaternion.identity);
                break;
            case GameObjectType.Enemy:
                Managers.Sound.Play("Effects/Monster_Die", Define.Sound.Effect);
                Managers.Resource.Instantiate("DeathEffect", go.transform.position, Quaternion.identity);

                //if (go.name == "SalarymanDefault")
                //    Managers.Sound.Play("Bgms/old-story-from-scotland-147143", Define.Sound.Bgm);
                break;
        }

        _objects.Remove(id);
        Managers.Resource.Destroy(go);
    }

    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public GameObject FindCreature(Vector3 Pos)
	{
		foreach (GameObject obj in _objects.Values)
		{
			CreatureController cc = obj.GetComponent<CreatureController>();
			if (cc == null)
				continue;

			if (cc.Pos == Pos)
				return obj;
		}

		return null;
	}

	public GameObject Find(Func<GameObject, bool> condition)
	{
		foreach (GameObject obj in _objects.Values)
		{
			if (condition.Invoke(obj))
				return obj;
		}

		return null;
	}

	public void Clear()
	{
        foreach (GameObject obj in _objects.Values)
        {
            Managers.Resource.Destroy(obj);
        }
        _objects.Clear();
        MyMap = null;
    }
}
