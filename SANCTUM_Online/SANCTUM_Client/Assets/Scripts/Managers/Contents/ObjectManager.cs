using Data;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class ObjectManager
{
	public MyMapController MyMap { get; set; }
	public Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public void Add(ObjectInfo info, bool myMap = false)
	{
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);
        if (objectType == GameObjectType.Player)
		{
            if (myMap)
            {
                GameObject go = Managers.Resource.Instantiate("Map/MyMap");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

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

                NewMap mc = go.GetComponent<NewMap>();
                mc.Id = info.ObjectId;
                mc.PosInfo = info.PosInfo;
                mc.Stat = info.StatInfo;
            }
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
