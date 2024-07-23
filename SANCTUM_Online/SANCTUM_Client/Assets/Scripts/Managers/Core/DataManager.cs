using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<string, ItemInfo> ItemDict { get; private set; } = new Dictionary<string, ItemInfo>();
    public Dictionary<string, StatInfo> ProjectileDict { get; private set; } = new Dictionary<string, StatInfo>();

    public void Init()
    {
        ItemDict = LoadJson<Data.ItemData, string, ItemInfo>("ItemData").MakeDict();
        ProjectileDict = LoadJson<Data.ProjectileData, string, StatInfo>("ProjectileData").MakeDict();
    }

    // 이 부분 잘 모르겠음
    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        //string text = File.ReadAllText($"Assets/Resources/Data/{path}.json");
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
    