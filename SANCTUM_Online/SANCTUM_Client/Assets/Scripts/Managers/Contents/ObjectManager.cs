using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
	public MyMapController MyMap { get; set; }
	public Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

	public void Add(PlayerInfo info, bool myMap = false)
	{
        if (myMap)
		{
			GameObject go = Managers.Resource.Instantiate("Map/MyMap");
			go.name = info.Name;
			_objects.Add(info.PlayerId, go);

			MyMap = go.GetComponent<MyMapController>();
			MyMap.Id = info.PlayerId;
			MyMap.PosInfo = info.PosInfo;
		}
		else
		{
			GameObject go = Managers.Resource.Instantiate("Map/Map");
			go.name = info.Name;
			_objects.Add(info.PlayerId, go);

			NewMap mc = go.GetComponent<NewMap>();
            mc.Id = info.PlayerId;
			mc.PosInfo = info.PosInfo;
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

	public void RemoveMyPlayer()
	{
		if (MyMap == null)
		{
			return;
		}

		Remove(MyMap.Id);
		MyMap = null;
	}

    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public GameObject Find(Vector3 Pos)
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
    }
}
