using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
	public MyMapController MyMap { get; set; }
	Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

	public void Add(PlayerInfo info, bool myMap = false)
	{
		if (myMap)
		{
			GameObject go = Managers.Resource.Instantiate("Map/MyMap");
			go.name = info.Name;
			_objects.Add(info.PlayerId, go);

			MyMap = go.GetComponent<MyMapController>();
			MyMap.Id = info.PlayerId;
			MyMap.Pos = new Vector3(info.PosX, 1, info.PosY);
		}
		else
		{
			GameObject go = Managers.Resource.Instantiate("Map/Map");
			go.name = info.Name;
			_objects.Add(info.PlayerId, go);

			NewMap mc = go.GetComponent<NewMap>();
            mc.Id = info.PlayerId;
            mc.Pos = new Vector3(info.PosX, 1, info.PosY);
        }
    }

	public void Add(int id, GameObject go)
	{
		_objects.Add(id, go);
	}

	public void Remove(int id)
	{
		_objects.Remove(id);
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
		_objects.Clear();
	}
}
