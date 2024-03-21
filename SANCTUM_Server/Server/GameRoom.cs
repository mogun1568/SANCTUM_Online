using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    // 클라만의 전용 공간은 아님. 몬스터나 스킬 등도 있을 수 있음
    class GameRoom :IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        // List나 Dictionary 등 대부분의 자료구조들은 멀티쓰레드 환경에서 돌아간다고 보정이되지 않음
        // 그렇기 때문에 lock으로 감싸줘야 함 -> Queue로 해결(Queue에서 lock으로 판별하게 수정)
        JobQueue _jopQueue = new JobQueue();
        // 패킷 모아 보내기
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job)
        {
            _jopQueue.Push(job);
        }

        public void Flush()
        {
            // JobQueue가 있기 때문에 lock을 안걸어도 됨

            // 시간복잡도 O(n^2)
            foreach (ClientSession s in _sessions)
            {
                s.Send(_pendingList);
            }
            //Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        // 이 부분이 어디까지 보내야 할지 알아야 하기 때문에 어려움
        public void Broadcast(ArraySegment<byte> segment)
        {
            // 다른 스레드와 공유하고 있는지 아닌지 계속 생각해봐야함
            // 다른 스레드와 공유하는 데이터
            _pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            // 플레이어 추가하고
            _sessions.Add(session);
            session.Room = this;

            // 신입생한테 모든 플레이어 목록 전송
            S_PlayerList players = new S_PlayerList();
            foreach (ClientSession s in _sessions)
            {
                players.players.Add(new S_PlayerList.Player()
                {
                    isSelf = (s == session),
                    playerId = s.SessionId,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ,
                });
            }
            session.Send(players.Write());

            // 신입생 입장을 모두에게 알린다
            S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
            enter.playerId = session.SessionId;
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;
            Broadcast(enter.Write());
        }

        public void Leave(ClientSession session)
        {
            // 플레이어 제거하고
            _sessions.Remove(session);

            // 모두에게 알린다
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.SessionId;
            Broadcast(leave.Write());
        }

        public void Move(ClientSession session, C_Move packet)
        {
            // 좌표 바꿔주고
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;

            // 모두에게 알린다
            S_BroadcastMove move = new S_BroadcastMove();
            move.playerId = session.SessionId;
            move.posX = packet.posX;
            move.posY = packet.posY;
            move.posZ = packet.posZ;
            Broadcast(move.Write());
        }

        public void Map(ClientSession session, C_Map packet)
        {
            // 좌표 저장하고
            foreach (C_Map.Map m in packet.maps)
            {
                session.Map[m.r, m.c] = m.nodeType;
            }

            // 모두에게 알린다
            S_BroadcastMap map = new S_BroadcastMap();
            map.playerId = session.SessionId;
            // packet.maps를 map.maps로 복사하기 전에 변환 수행
            map.maps = packet.maps.ConvertAll(m => new S_BroadcastMap.Map { r = m.r, c = m.c, nodeType = m.nodeType, isStart = m.isStart, isEnd = m.isEnd });
            Broadcast(map.Write());
        }
    }
}
