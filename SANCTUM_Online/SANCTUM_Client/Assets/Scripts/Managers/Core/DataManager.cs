using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<string, Data.Item> ItemDict { get; private set; } = new Dictionary<string, Data.Item>();
    public Dictionary<string, StatInfo> EnemyDict { get; private set; } = new Dictionary<string, StatInfo>();

    public void Init()
    {
        ItemDict = LoadJson<Data.ItemData, string, Data.Item>("ItemData").MakeDict();
        EnemyDict = LoadJson<Data.EnemyData, string, StatInfo>("EnemyData").MakeDict();
    }

    // �� �κ� �� �𸣰���
    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        //Debug.Log(textAsset.text);
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
