using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }

    public class DataManager
    {
        public static Dictionary<string, StatInfo> StatDict { get; private set; } = new Dictionary<string, StatInfo>();
        public static Dictionary<string, Data.Item> ItemDict { get; private set; } = new Dictionary<string, Data.Item>();
        public static Dictionary<string, StatInfo> EnemyDict { get; private set; } = new Dictionary<string, StatInfo>();
        public static Dictionary<string, StatInfo> ProjectileDict { get; private set; } = new Dictionary<string, StatInfo>();

        public static List<string> EnemyList { get; private set; }

        public void MakeList()
        {

        }

        public static void LoadData()
        {
            StatDict = LoadJson<Data.StatData, string, StatInfo>("StatData").MakeDict();
            ItemDict = LoadJson<Data.ItemData, string, Data.Item>("ItemData").MakeDict();
            EnemyDict = LoadJson<Data.EnemyData, string, StatInfo>("EnemyData").MakeDict();
            ProjectileDict = LoadJson<Data.ProjectileData, string, StatInfo>("ProjectileData").MakeDict();

            EnemyList = new List<string>(EnemyDict.Keys);
        }

        static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
        {
            string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text);
        }
    }
}
