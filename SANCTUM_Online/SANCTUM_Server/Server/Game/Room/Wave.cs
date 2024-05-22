using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator *(Vector3 a, float d)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator /(Vector3 a, float d)
        {
            return new Vector3(a.x / d, a.y / d, a.z / d);
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            float num = a.x - b.x;
            float num2 = a.y - b.y;
            float num3 = a.z - b.z;
            return (float)Math.Sqrt(num * num + num2 * num2 + num3 * num3);
        }

        public static Vector3 Normalize(Vector3 value)
        {
            float num = Magnitude(value);
            if (num > 1E-05f)
            {
                return new Vector3(value.x / num, value.y / num, value.z / num);
            }
            return new Vector3(0, 0, 0); // zero vector
        }

        public static float Magnitude(Vector3 vector)
        {
            return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        public Vector3 normalized
        {
            get
            {
                return Vector3.Normalize(this);
            }
        }
    }

    public class Wave
    {
        public GameRoom Room { get; set; }

        int _waveCount = 5;
        int _expandCount = 0;
        int _BossTime = -1;
        int _BossCount = 1;

        long _nextWaveTick = 0;
        public void Update()
        {
            if (Room == null)
                return;

            if (_nextWaveTick > Environment.TickCount64)
                return;
            int moveTick = 20000;
            _nextWaveTick = Environment.TickCount64 + moveTick;

            if (_expandCount > 1)
            {
                _expandCount = 0;
                foreach (Player player in Room._players.Values)
                {
                    player.Map.ExpendMap();
                    foreach (Node node in player.nodeInfos())
                    {
                        node.Owner = player;
                        Room.EnterGame(node);
                    }
                    S_CreateMap rescreateMapPacket = new S_CreateMap();
                    rescreateMapPacket.PlayerId = player.Id;
                    Room.Broadcast(rescreateMapPacket);

                    // TODO : Enemy Hp 소량 증가
                    _waveCount = (int)Math.Round(_waveCount * 1.1);
                    _waveCount = Math.Min(_waveCount, 10);
                }
            }
            _expandCount++;
            _BossTime++;

            List<Task> tasks = new List<Task>();

            foreach (Player player in Room._players.Values)
            {
                tasks.Add(SpawnWave(player));

                if (_BossTime > 2)
                {
                    _BossTime = 0;
                    tasks.Add(SpawnBoss(player));
                }
            }
        }

        public async Task SpawnWave(Player player)
        {
            await Task.Delay(5000);
            for (int i = 0; i < _waveCount; i++)
            {
                SpawnEnemy(player);
                await Task.Delay(500);
            }
        }

        public async Task SpawnBoss(Player player)
        {
            for (int i = 0; i < _BossCount; i++)
            {
                SpawnEnemy(player, "SalarymanDefault");
                await Task.Delay(500);
            }

            if (_BossCount < 2)
            {
                _BossCount++;
            }
        }

        public void SpawnEnemy(Player player, string name = default)
        {
            Enemy enemy = ObjectManager.Instance.Add<Enemy>();

            string enemyName = name;
            if (enemyName == default)
            {
                enemyName = player.EnemyName();
            }
            
            StatInfo EnemyInfo = null;
            if (DataManager.EnemyDict.TryGetValue(enemyName, out EnemyInfo) == false)
            {
                return;
            }

            Map map = player.Map;

            enemy.Info.Name = enemyName;
            enemy.PosInfo.PosX = map.startPoint.PosX * map.GetNodeSize() + map.startR;
            enemy.PosInfo.PosZ = map.startPoint.PosZ * map.GetNodeSize() + map.startC;
            enemy.PosInfo.DirY = map.startPoint.DirY * 90;
            enemy.Stat.MergeFrom(EnemyInfo);
            enemy.Owner = player;
            enemy.nextRoad = player.Map.roads.First;

            Room.EnterGame(enemy);
        }
    }
}
