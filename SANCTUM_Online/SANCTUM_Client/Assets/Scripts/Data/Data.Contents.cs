using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Item
    [Serializable]
    public class ItemData : ILoader<string, ItemInfo>
    {
        public List<ItemInfo> items = new List<ItemInfo>();

        public Dictionary<string, ItemInfo> MakeDict()
        {
            Dictionary<string, ItemInfo> dict = new Dictionary<string, ItemInfo>();
            foreach (ItemInfo item in items)
            {
                dict.Add(item.ItemName, item);
            }
            return dict;
        }
    }
    #endregion

    #region Projectile
    [Serializable]
    public class ProjectileData : ILoader<string, StatInfo>
    {
        public List<StatInfo> projectiles = new List<StatInfo>();

        public Dictionary<string, StatInfo> MakeDict()
        {
            Dictionary<string, StatInfo> dict = new Dictionary<string, StatInfo>();
            foreach (StatInfo projectile in projectiles)
            {
                dict.Add(projectile.Name, projectile);
            }
            return dict;
        }
    }
    #endregion
}