using Google.Protobuf.Protocol;
using Server.Data;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class LevelManager
    {
        public Player _player;
        public int _countLevelUp;
        bool _isPractice;
        public bool _isShow;

        Random _random = new Random();

        public void Init(Player player)
        {
            _player = player;
        }

        public void GetExp(int exp)
        {
            _player.Stat.Exp += exp;

            while (_player.Stat.Exp >= _player.Stat.TotalExp)
            {
                _player.Stat.Exp -= _player.Stat.TotalExp;
                _player.Stat.TotalExp = (int)(_player.Stat.TotalExp * 1.5f);
                _player.Stat.TotalExp = Math.Min(_player.Stat.TotalExp, 10);
                _countLevelUp++;
            }

            S_ChangeStat changeStatPacket = new S_ChangeStat();
            changeStatPacket.ObjectId = _player.Id;
            changeStatPacket.StatInfo = _player.Stat;
            _player.Room.Broadcast(changeStatPacket);

            if (!_isPractice && !_player.IsFPM && !_player.isPause && _countLevelUp > 0)
            {
                _isPractice = true;
                LevelUp();
            }   
        }

        public void RandomItem()
        {
            // ItemDict 중에서 랜덤 3개 아이템 활성화
            int[] ran = new int[3];
            while (true)
            {
                ran[0] = _random.Next(0, DataManager.ItemDict.Count);
                ran[1] = _random.Next(0, DataManager.ItemDict.Count);
                ran[2] = _random.Next(0, DataManager.ItemDict.Count);

                // 중복 검사
                if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
                {
                    break;
                }
            }

            S_LevelUp levelUpPacket = new S_LevelUp();
            levelUpPacket.PlayerId = _player.Id;
            foreach (int i in ran)
               levelUpPacket.ItemIdxs.Add(i);
            _player.Session.Send(levelUpPacket);
            Console.WriteLine($"{_isShow}, {_countLevelUp}");

            // 추후 벨런스 문제가 있다면 동일 아이템 개수 제한 기능 추가
        }

        public async void LevelUp()
        {
            await LevelUpUIAsync();
        }

        async Task LevelUpUIAsync()
        {
            while (_countLevelUp > 0)
            {
                // isShow가 false가 될 때까지 대기
                while (_isShow)
                {
                    await Task.Delay(100); // 100ms 간격으로 상태 확인 (필요에 따라 조정)
                }

                RandomItem();
                _countLevelUp--;
                _isShow = true;
            }

            _isPractice = false;
        }
    }
}
