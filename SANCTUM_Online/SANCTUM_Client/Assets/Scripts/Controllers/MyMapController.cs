using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class MyMapController : NewMap
{
    // 우, 하, 좌, 상
    int[] dr = new int[4] { 0, 1, 0, -1 };
    int[] dc = new int[4] { 1, 0, -1, 0 };

    int[,] map = new int[mapDefaultLength, mapDefaultLength];
    bool[,] visit = new bool[mapDefaultLength, mapDefaultLength];

    public List<NodeInfo> nodes = new List<NodeInfo>();

    protected override void Init()
    {
        base.Init();

        Camera.main.transform.position = Pos + new Vector3(178, 40, 178);

        CreateDefaultMap();
        //CreateDefaultMap1();
        //CreateDefaultMap2();

        //for (int i = mapLength - 2; i < 6; i++)
        //{
        //    ExpendMap();
        //}

        //foreach (NodeInfo node in roads)
        //{
        //    Debug.Log($"{node.R}, {node.C}");
        //}
    }

    // 임시방편, 수정 필요
    public void GameStart()
    {
        Invoke("test", 1f);
    }

    void test()
    {
        for (int i = mapLength - 2; i < 6; i++)
        {
            ExpendMap();
        }

        CheckUpdatedMap();
        CheckUpdatedStartAndEndPoint();
    }

    // 1x1 맵 (이걸로 할 듯)
    protected void CreateDefaultMap()
    {
        int mid = mapDefaultLength / 2;

        map[mid, mid] = 1;
        CreateNode("RoadE", mid * NodeSize, mid * NodeSize);
        visit[mid, mid] = true;

        startPoint.SetValues(mid, mid);
        endPoint.SetValues(mid, mid);

        mapLength = 3;
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

    public override void ExpendMap()
    {
        CreateStartPath(startPoint);
        CreateEndPath(endPoint);

        //CheckUpdatedFlag();

        // 이거 렉 오짐
        //Print2DArray();

        //foreach (var node in roads)
        //{
        //    Debug.Log($"Node Values: R={node.R}, C={node.C}");
        //}
    }

    public override void CreateNode(string type, int r, int c, bool haveEnvironment = false)
    {
        if (type == "Ground")
        {
            if (Random.Range(0, 10) == 1)
            {
                haveEnvironment = true;
            }
        }

        base.CreateNode(type, r, c, haveEnvironment);

        nodes.Add(new NodeInfo
        {
            NodeType = type,
            PosInfo = new PositionInfo { PosX = r, PosZ = c },
            HaveEnvironment = haveEnvironment
        });
    }

    // 길이 만들어지는 경우가 너무 적으면
    // 길을 만들 개수를 랜덤으로 정하고 출발지와 도착지의 거리를 구해서 해야 할듯


    // 맵의 마지막 길 노드 다음 방향 구하기 
    int FirstPathDirection(LocationInfo node)
    {
        bool[] dir = new bool[4];

        for (int i = 0; i < 4; i++)
        {
            if (!visit[node.R + dr[i], node.C + dc[i]])
            {
                if (map[node.R + dr[i], node.C + dc[i]] == 1)
                {
                    continue;
                }

                int nextDirection = (i + 1) % 4;
                if (map[node.R + dr[i] + dr[nextDirection], node.C + dc[i] + dc[nextDirection]] == 1)
                {
                    continue;
                }
                nextDirection = (i + 3) % 4;
                if (map[node.R + dr[i] + dr[nextDirection], node.C + dc[i] + dc[nextDirection]] == 1)
                {
                    continue;
                }

                dir[i] = true;
            }
        }

        int result = Random.Range(0, 4);
        while (!dir[result])
        {
            result = Random.Range(0, 4);
        }

        return result;
    }

    // 첫 노드 다음 노드들 방향 정하기
    int NextPathDirection(LocationInfo node, int firstNodeDirection)
    {
        bool[] dir = new bool[4];

        int newPathDirection = (firstNodeDirection + 1) % 4;
        if (map[node.R + dr[newPathDirection], node.C + dc[newPathDirection]] != 1)
        {
            dir[newPathDirection] = true;
        }
        newPathDirection = (firstNodeDirection + 3) % 4;
        if (map[node.R + dr[newPathDirection], node.C + dc[newPathDirection]] != 1)
        {
            dir[newPathDirection] = true;
        }

        int result = Random.Range(0, 4);
        while (!dir[result])
        {
            result = Random.Range(0, 4);
        }

        return result;
    }

    void CreateStartPath(LocationInfo startNode)
    {
        // 밑에 SetValues 함수를 실행하면 startNode 값이 참조되어 바뀌므로 새로 NodeInfo 객체를 만들어서 할당해줘야 함
        LocationInfo node = new LocationInfo(startNode.R, startNode.C, startNode.Direction);

        // 첫 노드 방향, 현재 길 방향, 다음 길 방향, 회전 방향
        int firstNodeDirection, currentPathDirection, nextPathDirection, clockwise = 1;
        int nextR, nextC;
        bool isCorner;  // 코너 확인

        firstNodeDirection = FirstPathDirection(node);
        nextR = node.R + dr[firstNodeDirection];
        nextC = node.C + dc[firstNodeDirection];

        // map[nextR, nextC]에 길 노드 생성
        map[nextR, nextC] = 1;
        CreateNode("RoadS", nextR * NodeSize, nextC * NodeSize);
        startPoint.SetValues(nextR, nextC, (firstNodeDirection + 2) % 4);

        // 길을 더 만들지 말지 결정
        if (Random.Range(0, 2) == 0)
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
            CreateNode("RoadS", currentR * NodeSize, currentC * NodeSize);
            if (!isCorner)
            {
                startPoint.SetValues(currentR, currentC, (currentPathDirection + 2 * clockwise) % 4);
            }
            else
            {
                startPoint.SetValues(currentR, currentC, (currentPathDirection + clockwise) % 4);
            }

            if (Random.Range(0, 2) == 0)
            {
                return;
            }

            currentCoor[0] = currentR + dr[currentPathDirection];
            currentCoor[1] = currentC + dc[currentPathDirection];

            MaxNodeCount--;
        }
    }
    void CreateEndPath(LocationInfo endNode)
    {
        // 밑에 SetValues 함수를 실행하면 endNode 값이 참조되어 바뀌므로 새로 NodeInfo 객체를 만들어서 할당해줘야 함
        LocationInfo node = new LocationInfo(endNode.R, endNode.C, endNode.Direction);

        // 첫 노드 방향, 현재 길 방향, 다음 길 방향, 회전 방향, 길인지 땅인지 확인 변수
        int firstNodeDirection, currentPathDirection, nextPathDirection, clockwise = 1, createNode = 1;
        int nextR, nextC;
        bool isCorner; // 코너 확인

        firstNodeDirection = FirstPathDirection(node);
        nextR = node.R + dr[firstNodeDirection];
        nextC = node.C + dc[firstNodeDirection];

        // map[currentR, currentC]에 길 노드 생성
        map[nextR, nextC] = createNode;
        CreateNode("RoadE", nextR * NodeSize, nextC * NodeSize);
        visit[nextR, nextC] = true;
        endPoint.SetValues(nextR, nextC, (firstNodeDirection + 2) % 4);

        createNode = Random.Range(1, 3);

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
                    CreateNode("RoadE", currentR * NodeSize, currentC * NodeSize);
                    if (!isCorner)
                    {
                        endPoint.SetValues(currentR, currentC, (currentPathDirection + 2 * clockwise) % 4);
                    }
                    else
                    {
                        endPoint.SetValues(currentR, currentC, (currentPathDirection + clockwise) % 4);
                    }
                }
                else
                {
                    CreateNode("Ground", currentR * NodeSize, currentC * NodeSize);
                }
            }
            visit[currentR, currentC] = true;

            if (createNode == 1)
            {
                createNode = Random.Range(1, 3);
            }

            currentCoor[0] = currentR + dr[currentPathDirection];
            currentCoor[1] = currentC + dc[currentPathDirection];

            MaxNodeCount--;
        }

        UpdatePosition();

        mapLength += 2;
    }

    void CheckUpdatedMap()
    {
        C_CreateMap createPacket = new C_CreateMap();
        foreach (NodeInfo nodeInfo in nodes)
        {
            createPacket.NodeInfo.Add(nodeInfo);
        }
        Managers.Network.Send(createPacket);
        nodes.Clear();
    }

    void CheckUpdatedStartAndEndPoint()
    {
        C_Move movePacket = new C_Move();

        movePacket.IsStart = true;
        movePacket.PosInfo = new PositionInfo() { PosX = startPoint.R, PosZ = startPoint.C, Dir = startPoint.Direction};
        Managers.Network.Send(movePacket);

        movePacket.IsStart = false;
        movePacket.PosInfo = new PositionInfo() { PosX = endPoint.R, PosZ = endPoint.C, Dir = endPoint.Direction };
        Managers.Network.Send(movePacket);
    }
}
