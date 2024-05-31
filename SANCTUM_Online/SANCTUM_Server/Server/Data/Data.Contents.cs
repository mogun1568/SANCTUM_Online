using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    #region Stat
    [Serializable]
    public class StatData : ILoader<string, StatInfo>
    {
        public List<StatInfo> stats = new List<StatInfo>();

        public Dictionary<string, StatInfo> MakeDict()
        {
            Dictionary<string, StatInfo> dict = new Dictionary<string, StatInfo>();
            foreach (StatInfo stat in stats)
            {
                dict.Add(stat.Name, stat);
            }
            return dict;
        }
    }
    #endregion

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

    #region Enemy
    [Serializable]
    public class EnemyData : ILoader<string, StatInfo>
    {
        public List<StatInfo> enemys = new List<StatInfo>();

        public Dictionary<string, StatInfo> MakeDict()
        {
            Dictionary<string, StatInfo> dict = new Dictionary<string, StatInfo>();
            foreach (StatInfo enemy in enemys)
            {
                dict.Add(enemy.Name, enemy);
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
