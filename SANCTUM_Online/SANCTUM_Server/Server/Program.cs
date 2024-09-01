﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Data;
using Server.Game;
using ServerCore;

namespace Server.Game
{
    class Program
    {
        static Listener _listener = new Listener();
        static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>();

        public static void TickWaitingRoom(WaitingRoom room, int tick = 100)
        {
            var timer = new System.Timers.Timer();
            timer.Interval = tick;
            timer.Elapsed += ((s, e) => { room.Update(); });
            timer.AutoReset = true;
            timer.Enabled = true;

            _timers.Add(timer);

            // 끄고 싶을 때
            //timer.Stop();
        }

        public static void TickRoom(GameRoom room, int tick = 100)
        {
            var timer = new System.Timers.Timer();
            timer.Interval = tick;
            timer.Elapsed += ((s, e) => { room.Update(); });
            timer.AutoReset = true;
            timer.Enabled = true;

            _timers.Add(timer);

            // 끄고 싶을 때
            //timer.Stop();
        }

        static void Main(string[] args)
        {
            ConfigManager.LoadConfig();
            DataManager.LoadData();

            WaitingRoom room = RoomManager.Instance.AddWaitingRoom();
            TickWaitingRoom(room, 50);

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            DeleteAllFiles();

            // TODO
            while (true)
            {
                Thread.Sleep(100);
            }
        }

        static void DeleteAllFiles()
        {
            DeleteAllFilesInFolder("../../../../../Common/MapData");
            DeleteAllFilesInFolder("../../../../../SANCTUM_Client/Assets/Resources/Map");
            DeleteAllFilesInFolder("../../../../../SANCTUM_Client/Assets/Resources/Inventory");
        }

        static void DeleteAllFilesInFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                // 폴더 내 모든 파일 경로 가져오기
                string[] files = Directory.GetFiles(folderPath);

                foreach (string file in files)
                {
                    try
                    {
                        // 각 파일을 삭제합니다.
                        File.Delete(file);
                        //Console.WriteLine($"파일 삭제됨: {file}");
                    }
                    catch (IOException ioEx)
                    {
                        // 파일 삭제 중 오류 발생 시 출력
                        Console.WriteLine($"파일 삭제 중 오류 발생: {file}, 오류: {ioEx.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"폴더를 찾을 수 없습니다: {folderPath}");
            }
        }
    }
}
