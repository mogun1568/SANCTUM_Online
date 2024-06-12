using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Server.Game
{
    public class Projectile : GameObject
    {
        public Projectile()
        {
            ObjectType = GameObjectType.Projectile;
        }

        public virtual void Update()
        {

        }

        public void HitTarget(Enemy target)
        {
            switch (Stat.Type)
            {
                case "General":
                    Damage(target);
                    break;
                case "Explode":
                    ExplodeDamage(target);
                    break;
                case "Dot":
                    DotDamage(target);
                    break;
                case "Slow":
                    SlowSpeed(target);
                    break;
                case "Massive":
                    MassiveDamage(target);
                    break;
            }
        }

        // 데미지
        void Damage(Enemy target)
        {
            target.OnDamaged(Master, (int)(Attack * Owner.Attack));
        }

        // 광범위 타격
        void ExplodeDamage(Enemy target)
        {
            List<Enemy> targets = Room.FindEnemies(e =>
            {
                float dist = Vector3.Distance(Pos, e.Pos);
                return dist <= 8f;
            });

            foreach (Enemy e in targets)
            {
                Damage(e);
            }
        }

        // 도트 데이지 코드
        async void DotDamage(Enemy target)
        {
            await target.OnDotDamaged(Master, (int)(Attack * Owner.Attack));
        }

        async void SlowSpeed(Enemy target)
        {
            await target.OnSlow(Master, (int)(Attack * Owner.Attack));
        }

        void MassiveDamage(Enemy target)
        {
            target.OnDamaged(Master, (int)(Attack * Owner.Attack) * 2);
        }
    }
}
