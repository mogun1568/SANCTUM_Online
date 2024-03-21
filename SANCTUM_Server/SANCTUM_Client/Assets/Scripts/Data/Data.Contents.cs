using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Item
    [Serializable]
    public class Item
    {
        public string itemType;
        public string itemName;
        public string itemIcon;
        public string bulletType;
        public float upgradeAmount;
        public int returnExp;
    }

    [Serializable]
    public class ItemData : ILoader<string, Item>
    {
        public List<Item> items = new List<Item>();

        public Dictionary<string, Item> MakeDict()
        {
            Dictionary<string, Item> dict = new Dictionary<string, Item>();
            foreach (Item item in items)
            {
                dict.Add(item.itemName, item);
            }
            return dict;
        }
    }
    #endregion

    #region Enemy
    [Serializable]
    public class Enemy
    {
        public string enemyType;
        public string enemyName;
    }

    [Serializable]
    public class EnemyData : ILoader<string, Enemy>
    {
        public List<Enemy> enemys = new List<Enemy>();

        public Dictionary<string, Enemy> MakeDict()
        {
            Dictionary<string, Enemy> dict = new Dictionary<string, Enemy>();
            foreach (Enemy enemy in enemys)
            {
                dict.Add(enemy.enemyName, enemy);
            }
            return dict;
        }
    }
    #endregion
}