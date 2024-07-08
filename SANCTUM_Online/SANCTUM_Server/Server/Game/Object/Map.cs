using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Map
    {
        // 우, 하, 좌, 상
        int[] dr = new int[4] { 0, 1, 0, -1 };
        int[] dc = new int[4] { 1, 0, -1, 0 };

        const int mapDefaultLength = 31;
        int mapLength = 3;
        const int NodeSize = 4;

        int[,] map = new int[mapDefaultLength, mapDefaultLength];
        bool[,] visit = new bool[mapDefaultLength, mapDefaultLength];

        public PositionInfo startPoint = new PositionInfo();
        PositionInfo endPoint = new PositionInfo();

        public List<Node> nodes = new List<Node>();
        public LinkedList<Node> roads = new LinkedList<Node>();

        public int startR;
        public int startC;

        bool[,] _collision = new bool[mapDefaultLength, mapDefaultLength];
        Node[,] _objects = new Node[mapDefaultLength, mapDefaultLength];

        Random random = new Random();

        public Player _owner;

        public void Init(Player owner)
        {
            _owner = owner;

            CreateDefaultMap();
            for (int i = mapLength - 2; i < 6; i++)
            {
                CreateStartPath(startPoint);
                CreateEndPath(endPoint);
            }
            GenerateMap();
        }

        #region MAP_EDITOR

        void GenerateMap()
        {
            GenerateByPath("../../../../../Common/MapData");
            GenerateByPath("../../../../../SANCTUM_Client/Assets/Resources/Map");
        }

        void GenerateByPath(string pathPrefix)
        {
            using (var writer = File.CreateText($"{pathPrefix}/Map_{_owner.Id}.txt"))
            {
                writer.WriteLine(startPoint.PosX);
                writer.WriteLine(startPoint.PosZ);
                writer.WriteLine(startPoint.DirY);
                writer.WriteLine(endPoint.PosX);
                writer.WriteLine(endPoint.PosZ);
                writer.WriteLine(endPoint.DirY);

                writer.WriteLine(mapDefaultLength);
                writer.WriteLine(NodeSize);

                for (int i = 0; i < mapDefaultLength; i++)
                {
                    for (int j = 0; j < mapDefaultLength; j++)
                    {
                        writer.Write(map[i, j]);
                    }
                    writer.WriteLine();
                }
            }
        }

        #endregion

        // 1x1 맵 (이걸로 할 듯)
        protected void CreateDefaultMap()
        {
            int mid = mapDefaultLength / 2;

            map[mid, mid] = 1;
            CreateNode("RoadE", mid, mid);
            visit[mid, mid] = true;

            SetValues(startPoint, mid, mid);
            SetValues(endPoint, mid, mid);

            mapLength = 3;
        }

        public void SetValues(PositionInfo pos, int posX, int posZ, int dir = 0)
        {
            pos.PosX = posX;
            pos.PosZ = posZ;
            pos.DirY = dir;
        }

        public int GetNodeSize()
        {
            return NodeSize;
        }

        /*
        // 3x3 맵
        void CreateDefaultMap1()
        {
            int mid = mapDefaultLength / 2;

            startPoint[0] = mid - 1;
            startPoint[1] = mid;
            endPoint[0] = mid + 1;
            endPoint[1] = mid;

            for (int i = mid - 1; i < mid + 2; i++)
            {
                for (int j = mid - 1; j < mid + 2; j++)
                {
                    if (j == mid)
                    {
                        map[i, j] = 1;
                        Instantiate(roadPrefab, new Vector3(i, 0, j), Quaternion.identity, parent.transform);
                    } else
                    {
                        map[i, j] = 2;
                        Instantiate(groundPrefab, new Vector3(i, 0, j), Quaternion.identity, parent.transform);
                    }
                    visit[i, j] = true;
                }
            }

            mapLength = 5;
        }

        // 2x2 맵 (맵 확장에서 문제가 있는 듯 안됨)
        void CreateDefaultMap2()
        {
            int mid = mapDefaultLength / 2;
            int[] nodeR = new int[4] { mid - 1, mid, mid - 1, mid };
            int[] nodeC = new int[4] { mid - 1, mid - 1, mid, mid };

            int startNode = Random.Range(0, 4);
            int endNode = Random.Range(0, 4);
            while (Mathf.Abs(nodeR[startNode] - nodeR[endNode]) + Mathf.Abs(nodeC[startNode] - nodeC[endNode]) != 1)
            {
                endNode = Random.Range(0, 4);
            }

            startPoint[0] = nodeR[startNode];
            startPoint[1] = nodeC[startNode];
            endPoint[0] = nodeR[endNode];
            endPoint[1] = nodeC[endNode];

            for (int i = 0; i < 4; i++)
            {
                if (i == startNode || i == endNode)
                {
                    map[nodeR[i], nodeC[i]] = 1;
                    Instantiate(roadPrefab, new Vector3(nodeR[i], 0, nodeC[i]), Quaternion.identity, parent.transform);
                } else
                {
                    map[nodeR[i], nodeC[i]] = 2;
                    Instantiate(groundPrefab, new Vector3(nodeR[i], 0, nodeC[i]), Quaternion.identity, parent.transform);
                }

                visit[nodeR[i], nodeC[i]] = true;
            }
        }
        */

        public void ExpendMap()
        {
            nodes.Clear();

            CreateStartPath(startPoint);
            CreateEndPath(endPoint);

            GenerateMap();
        }

        Vector3 MapToArr(PositionInfo posInfo)
        {
            int x = (int)posInfo.PosX;
            int z = (int)posInfo.PosZ;

            x = (x - startR) / NodeSize;
            z = (z - startC) / NodeSize;

            Vector3 pos = new Vector3(x, 0, z);
            return pos;
        }

        public bool ApplyBuild(PositionInfo posInfo)
        {
            Vector3 arrPos = MapToArr(posInfo);
            int x = (int)arrPos.x;
            int z = (int)arrPos.z;

            if (_collision[x, z])
            {
                return false;
            }

            _collision[x, z] = true;
            return true;
        }

        //Vector3 ArrToMap(int x, int z)
        //{
        //    x = x * NodeSize + startR;
        //    z = z * NodeSize + startC;

        //    Vector3 pos = new Vector3(x, 0, z);
        //    return pos;
        //}

        void CreateNode(string type, int r, int c, bool haveEnvironment = false)
        {
            Node node = ObjectManager.Instance.Add<Node>();
            if (node == null)
            {
                return;
            }

            {
                _objects[r, c] = node;
                if (type[0] == 'R' || haveEnvironment)
                {
                    _collision[r, c] = true;
                }
            }

            node.Info.Name = type;

            StatInfo statInfo = null;
            if (haveEnvironment)
            {
                node._haveEnvironment = true;
                type = "GroundE";
            }
            if (DataManager.StatDict.TryGetValue(type, out statInfo) == false)
            {
                return;
            }

            node.PosInfo.PosX = r;
            node.PosInfo.PosY = 0;
            node.PosInfo.PosZ = c;
            node.Stat.MergeFrom(statInfo);
            nodes.Add(node);

            Node road = new Node();
            road.PosInfo.PosX = node.PosInfo.PosX * NodeSize + startR;
            road.PosInfo.PosY = 1;
            road.PosInfo.PosZ = node.PosInfo.PosZ * NodeSize + startC;

            if (type == "RoadS")
            {
                roads.AddFirst(road);
            }
            else if (type == "RoadE")
            {
                roads.AddLast(road);
            }
        }

        // 길이 만들어지는 경우가 너무 적으면
        // 길을 만들 개수를 랜덤으로 정하고 출발지와 도착지의 거리를 구해서 해야 할듯


        // 맵의 마지막 길 노드 다음 방향 구하기 
        int FirstPathDirection(PositionInfo node)
        {
            bool[] dir = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                if (!visit[(int)node.PosX + dr[i], (int)node.PosZ + dc[i]])
                {
                    if (map[(int)node.PosX + dr[i], (int)node.PosZ + dc[i]] == 1)
                    {
                        continue;
                    }

                    int nextDirection = (i + 1) % 4;
                    if (map[(int)node.PosX + dr[i] + dr[nextDirection], (int)node.PosZ + dc[i] + dc[nextDirection]] == 1)
                    {
                        continue;
                    }
                    nextDirection = (i + 3) % 4;
                    if (map[(int)node.PosX + dr[i] + dr[nextDirection], (int)node.PosZ + dc[i] + dc[nextDirection]] == 1)
                    {
                        continue;
                    }

                    dir[i] = true;
                }
            }

            int result = random.Next(0, 4);

            while (!dir[result])
            {
                result = random.Next(0, 4);
            }

            return result;
        }

        // 첫 노드 다음 노드들 방향 정하기
        int NextPathDirection(PositionInfo node, int firstNodeDirection)
        {
            bool[] dir = new bool[4];

            int newPathDirection = (firstNodeDirection + 1) % 4;
            if (map[(int)node.PosX + dr[newPathDirection], (int)node.PosZ + dc[newPathDirection]] != 1)
            {
                dir[newPathDirection] = true;
            }
            newPathDirection = (firstNodeDirection + 3) % 4;
            if (map[(int)node.PosX + dr[newPathDirection], (int)node.PosZ + dc[newPathDirection]] != 1)
            {
                dir[newPathDirection] = true;
            }

            int result = random.Next(0, 4);
            while (!dir[result])
            {
                result = random.Next(0, 4);
            }

            return result;
        }

        void CreateStartPath(PositionInfo startNode)
        {
            // 밑에 SetValues 함수를 실행하면 startNode 값이 참조되어 바뀌므로 새로 NodeInfo 객체를 만들어서 할당해줘야 함
            PositionInfo node = new PositionInfo()
            {
                PosX = startNode.PosX,
                PosZ = startNode.PosZ,
                DirY = startNode.DirY,
            };

            // 첫 노드 방향, 현재 길 방향, 다음 길 방향, 회전 방향
            int firstNodeDirection, currentPathDirection, nextPathDirection, clockwise = 1;
            int nextR, nextC;
            bool isCorner;  // 코너 확인

            firstNodeDirection = FirstPathDirection(node);
            nextR = (int)node.PosX + dr[firstNodeDirection];
            nextC = (int)node.PosZ + dc[firstNodeDirection];

            // map[nextR, nextC]에 길 노드 생성
            map[nextR, nextC] = 1;
            CreateNode("RoadS", nextR, nextC);
            SetValues(startPoint, nextR, nextC, (firstNodeDirection + 2) % 4);

            // 길을 더 만들지 말지 결정
            if (random.Next(0, 2) == 0)
            {
                return;
            }

            currentPathDirection = NextPathDirection(node, firstNodeDirection);
            // 반시계 방향인 경우
            if (firstNodeDirection - currentPathDirection == -3 || firstNodeDirection - currentPathDirection == 1)
            {
                clockwise = 3;
            }
            // 다음 방향
            nextPathDirection = (currentPathDirection + clockwise) % 4;

            int[] currentCoor = new int[2] { nextR + dr[currentPathDirection], nextC + dc[currentPathDirection] };
            int MaxNodeCount = mapLength * 4 - 4 - 1;

            while (MaxNodeCount > 0)
            {
                int currentR = currentCoor[0];
                int currentC = currentCoor[1];
                isCorner = false;

                // 코너인 경우
                if (!visit[currentR + dr[nextPathDirection], currentC + dc[nextPathDirection]])
                {
                    currentPathDirection = nextPathDirection;
                    nextPathDirection = (currentPathDirection + clockwise) % 4;

                    isCorner = true;
                }

                if (!isCorner)
                {
                    // 옆에 있는 경우
                    if (map[currentR + dr[nextPathDirection], currentC + dc[nextPathDirection]] == 1)
                    {
                        return;
                    }
                    // 대각선에 있는 경우
                    else if (map[currentR + dr[currentPathDirection] + dr[nextPathDirection], currentC + dc[currentPathDirection] + dc[nextPathDirection]] == 1)
                    {
                        return;
                    }
                }

                // map[currentR, currentC]에 길 노드 생성
                map[currentR, currentC] = 1;
                CreateNode("RoadS", currentR, currentC);
                if (!isCorner)
                {
                    SetValues(startPoint, currentR, currentC, (currentPathDirection + 2 * clockwise) % 4);
                }
                else
                {
                    SetValues(startPoint, currentR, currentC, (currentPathDirection + clockwise) % 4);
                }

                if (random.Next(0, 2) == 0)
                {
                    return;
                }

                currentCoor[0] = currentR + dr[currentPathDirection];
                currentCoor[1] = currentC + dc[currentPathDirection];

                MaxNodeCount--;
            }
        }
        void CreateEndPath(PositionInfo endNode)
        {
            // 밑에 SetValues 함수를 실행하면 endNode 값이 참조되어 바뀌므로 새로 NodeInfo 객체를 만들어서 할당해줘야 함
            PositionInfo node = new PositionInfo()
            {
                PosX = endNode.PosX,
                PosZ = endNode.PosZ,
                DirY = endNode.DirY,
            };

            // 첫 노드 방향, 현재 길 방향, 다음 길 방향, 회전 방향, 길인지 땅인지 확인 변수
            int firstNodeDirection, currentPathDirection, nextPathDirection, clockwise = 1, createNode = 1;
            int nextR, nextC;
            bool isCorner; // 코너 확인

            firstNodeDirection = FirstPathDirection(node);
            nextR = (int)node.PosX + dr[firstNodeDirection];
            nextC = (int)node.PosZ + dc[firstNodeDirection];

            // map[currentR, currentC]에 길 노드 생성
            map[nextR, nextC] = createNode;
            CreateNode("RoadE", nextR , nextC);
            visit[nextR, nextC] = true;
            SetValues(endPoint, nextR, nextC, (firstNodeDirection + 2) % 4);

            createNode = random.Next(1, 3);

            currentPathDirection = NextPathDirection(node, firstNodeDirection);
            // 반시계 방향인 경우
            if (firstNodeDirection - currentPathDirection == -3 || firstNodeDirection - currentPathDirection == 1)
            {
                clockwise = 3;
            }
            // 다음 방향
            nextPathDirection = (currentPathDirection + clockwise) % 4;

            int[] currentCoor = new int[2] { nextR + dr[currentPathDirection], nextC + dc[currentPathDirection] };
            int MaxNodeCount = mapLength * 4 - 4 - 1;

            while (MaxNodeCount > 0)
            {
                int currentR = currentCoor[0];
                int currentC = currentCoor[1];
                isCorner = false;

                // 코너인 경우
                if (!visit[currentR + dr[nextPathDirection], currentC + dc[nextPathDirection]])
                {
                    currentPathDirection = nextPathDirection;
                    nextPathDirection = (currentPathDirection + clockwise) % 4;

                    isCorner = true;
                }

                if (createNode == 1 && !isCorner)
                {
                    // 앞에 있는 경우
                    if (map[currentR + dr[currentPathDirection], currentC + dc[currentPathDirection]] == 1)
                    {
                        createNode = 2;
                    }
                    // 옆에 있는 경우
                    else if (map[currentR + dr[nextPathDirection], currentC + dc[nextPathDirection]] == 1)
                    {
                        createNode = 2;
                    }
                    // 대각선에 있는 경우
                    else if (map[currentR + dr[currentPathDirection] + dr[nextPathDirection], currentC + dc[currentPathDirection] + dc[nextPathDirection]] == 1)
                    {
                        createNode = 2;
                    }
                }

                // map[currentR, currentC]에 길 노드 생성
                if (map[currentR, currentC] == 0)
                {
                    map[currentR, currentC] = createNode;
                    if (createNode == 1)
                    {
                        CreateNode("RoadE", currentR, currentC);
                        if (!isCorner)
                        {
                            SetValues(endPoint, currentR, currentC, (currentPathDirection + 2 * clockwise) % 4);
                        }
                        else
                        {
                            SetValues(endPoint, currentR, currentC, (currentPathDirection + clockwise) % 4);
                        }
                    }
                    else
                    {
                        bool haveEnvironment = false;

                        if (random.Next(0, 10) == 1)
                        {
                            haveEnvironment = true;
                            map[currentR, currentC] = 3;
                        }

                        CreateNode("Ground", currentR, currentC, haveEnvironment);
                    }
                }
                visit[currentR, currentC] = true;

                if (createNode == 1)
                {
                    createNode = random.Next(1, 3);
                }

                currentCoor[0] = currentR + dr[currentPathDirection];
                currentCoor[1] = currentC + dc[currentPathDirection];

                MaxNodeCount--;
            }

            mapLength += 2;
        }
    }
}
