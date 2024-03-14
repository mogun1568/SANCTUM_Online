using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NodeInfo
{
    public int R { get; set; }
    public int C { get; set; }
    public int Direction { get; set; }

    public NodeInfo(int r = 0, int c = 0, int direction = 0)
    {
        R = r;
        C = c;
        Direction = direction;
    }

    public void SetValues(int newR, int newC, int newDirection = 0)
    {
        R = newR;
        C = newC;
        Direction = newDirection;
    }
}

public class NewMap : MonoBehaviour
{
    GameObject roadPrefab;
    GameObject groundPrefab;
    GameObject EnvironmentPrefab;
    //[SerializeField] GameObject parentPrefab;
    [SerializeField] GameObject roadParent;
    [SerializeField] GameObject GroundParent;
    //[SerializeField] GameObject EnvironmentParentPrefab;
    [SerializeField] GameObject EnvironmentParent;

    //GameObject startPrefab;
    //GameObject endPrefab;
    [HideInInspector] public GameObject startObj;
    GameObject endObj;

    GameObject enemyPrefab;

    const int mapDefaultLength = 101;

    int[,] map = new int[mapDefaultLength, mapDefaultLength];
    bool[,] visit = new bool[mapDefaultLength, mapDefaultLength];
    public static LinkedList<NodeInfo> roads = new LinkedList<NodeInfo>();

    // ��, ��, ��, ��
    int[] dr = new int[4] { 0, 1, 0, -1 };
    int[] dc = new int[4] { 1, 0, -1, 0 };

    NodeInfo startPoint = new NodeInfo();
    NodeInfo endPoint = new NodeInfo();

    int mapLength = 3;
    int NodeSize = 4;

    void Start()
    {
        Init();
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        if (mapLength > mapDefaultLength / 2 - 1)
    //        {
    //            return;
    //        }

    //        ExpendMap();

    //        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //    }

    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        Instantiate(enemyPrefab);
    //    }
    //}

    void Print2DArray()
    {
        string s = "(" + startPoint.R + ", " + startPoint.C + "), (" + endPoint.R + ", " + endPoint.C + ")";
        Debug.Log(s);

        string patternString = "";

        for (int i = 0; i < mapDefaultLength; i++)
        {
            for (int j = 0; j < mapDefaultLength; j++)
            {
                patternString += map[i, j].ToString();
            }
            patternString += "\n"; // �� �� ��� �� �� �ٲ�
        }

        Debug.Log(patternString);

        /*string boolString = "";

        for (int i = 0; i < mapDefaultLength; i++)
        {
            for (int j = 0; j < mapDefaultLength; j++)
            {
                if (visit[i, j])
                {
                    boolString += 1;
                } else
                {
                    boolString += 0;
                }
            }
            boolString += "\n"; // �� �� ��� �� �� �ٲ�
        }

        Debug.Log(boolString);*/
    }

    void Init()
    {
        //parent = Instantiate(parentPrefab);
        //EnvironmentParent = Instantiate(EnvironmentParentPrefab);
        startObj = Managers.Resource.Instantiate("Start");
        endObj = Managers.Resource.Instantiate("End");

        CreateDefaultMap();
        //CreateDefaultMap1();
        //CreateDefaultMap2();

        for (int i = mapLength - 2; i < 6; i++)
        {
            ExpendMap();
        }

        //foreach (NodeInfo node in roads)
        //{
        //    Debug.Log($"{node.R}, {node.C}");
        //}
    }

    // 1x1 �� (�̰ɷ� �� ��)
    void CreateDefaultMap()
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
    // 3x3 ��
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

    // 2x2 �� (�� Ȯ�忡�� ������ �ִ� �� �ȵ�)
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
        CreateStartPath(startPoint);
        CreateEndPath(endPoint);

        // �̰� �� ����
        //Print2DArray();

        //foreach (var node in roads)
        //{
        //    Debug.Log($"Node Values: R={node.R}, C={node.C}");
        //}
    }


    // ���� ��������� ��찡 �ʹ� ������
    // ���� ���� ������ �������� ���ϰ� ������� �������� �Ÿ��� ���ؼ� �ؾ� �ҵ�


    // ���� ������ �� ��� ���� ���� ���ϱ� 
    int FirstPathDirection(NodeInfo node)
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
            
    // ù ��� ���� ���� ���� ���ϱ�
    int NextPathDirection(NodeInfo node, int firstNodeDirection)
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

    void UpdatePosition()
    {
        startObj.transform.position = new Vector3(startPoint.R * NodeSize, 1, startPoint.C * NodeSize);
        startObj.transform.rotation = Quaternion.Euler(0, startPoint.Direction * 90, 0);
        StartCoroutine(SetScaleCoroutine(startObj.transform, 1));
        endObj.transform.position = new Vector3(endPoint.R * NodeSize, 1, endPoint.C * NodeSize);
        endObj.transform.rotation = Quaternion.Euler(0, endPoint.Direction * 90 + 180, 0);
        StartCoroutine(SetScaleCoroutine(endObj.transform, 0.003f));
    }

    void CreateNode(string type, int r, int c)
    {
        GameObject node;

        if (type == "RoadS")
        {
            node = Managers.Resource.Instantiate($"Environment/ForestGroundDirt", new Vector3(r, 0, c), Quaternion.identity, roadParent.transform);
            //Instantiate(roadPrefab, new Vector3(r, 0, c), Quaternion.identity, parent.transform);
            roads.AddFirst(new NodeInfo(r, c));
        } else if (type == "RoadE") 
        {
            node = Managers.Resource.Instantiate($"Environment/ForestGroundDirt", new Vector3(r, 0, c), Quaternion.identity, roadParent.transform);
            //Instantiate(roadPrefab, new Vector3(r, 0, c), Quaternion.identity, parent.transform);
            roads.AddLast(new NodeInfo(r, c));
        } else
        {
            float y = Random.Range(-0.25f, 0.25f);
            node = Managers.Resource.Instantiate($"Environment/ForestGround01", new Vector3(r, y, c), Quaternion.identity, GroundParent.transform);
            //Instantiate(groundPrefab, new Vector3(r, y, c), Quaternion.identity, parent.transform);
            SetChildCount(node.transform);
            if (Random.Range(0, 10) == 1)
            {
                node.GetComponent<Node>().enviroment = true;
                CreateEnvironment(r, y, c);
            }
        }

        StartCoroutine(MoveObjectCoroutine(node.transform));
        SetTransparency(node);
    }

    void CreateEnvironment(int r, float y, int c)
    {
        GameObject EnvironmentObj = Managers.Resource.Instantiate($"Environment/TFF_Birch_Tree_01A", new Vector3(r, y + 1, c), Quaternion.identity, EnvironmentParent.transform);
        //Instantiate(EnvironmentPrefab, new Vector3(r, y + NodeSize - 0.5f, c), Quaternion.identity, EnvironmentParent.transform);
        StartCoroutine(SetScaleCoroutine(EnvironmentObj.transform, 0.8f));
    }

    void SetChildCount(Transform go)
    {
        // �ڽ� ������Ʈ ���� ��Ȱ��ȭ (Node ��ũ��Ʈ�� �ű�� ���� ��)
        foreach (Transform child in go)
        {
            child.gameObject.SetActive(false);
        }

        int activeObjectCount = Random.Range(0, 4);

        while (activeObjectCount > 0)
        {
            int randomIndex = Random.Range(1, go.childCount);

            while (go.GetChild(randomIndex).gameObject.activeSelf)
            {
                randomIndex = Random.Range(1, go.childCount);
            }

            go.GetChild(randomIndex).gameObject.SetActive(true);

            // �ڷ�ƾ���� ũ�� ���� �Լ� �߰� ����
            StartCoroutine(SetScaleCoroutine(go.GetChild(randomIndex).gameObject.transform, 0.8f));

            activeObjectCount--;
        }
    }

    void SetTransparency(GameObject go)
    {
        // ������Ʈ�� Material�� Transparency Level�� shader ��ũ��Ʈ������ _Tweak_transparency ��
        int transparencyPropertyID = Shader.PropertyToID("_Tweak_transparency");

        go.GetComponent<Renderer>().material.SetFloat(transparencyPropertyID, -1);

        StartCoroutine(FadeTransparencyCoroutine(go.GetComponent<Renderer>().material, transparencyPropertyID));
    }

    IEnumerator MoveObjectCoroutine(Transform go)
    {
        // �̵��� �Ÿ��� �ð� ����
        float distanceToMove = -2f;
        float duration = 2f;

        // ���� ��ġ�� ���� ��ġ ���
        Vector3 endPos = go.position;
        Vector3 startPos = endPos + new Vector3(0, distanceToMove, 0);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // ���� �ð��� ���� ������ ��ġ ���
            go.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);

            // ��� �ð� ������Ʈ
            elapsedTime += Time.deltaTime;

            // �� ������ ���
            yield return null;
        }

        go.position = endPos;
    }

    IEnumerator FadeTransparencyCoroutine(Material transparentMaterial, int transparencyPropertyID)
    {
        float currentTransparency = transparentMaterial.GetFloat(transparencyPropertyID);
        float targetTransparency = 0.0f;
        float duration = 2f; // ���濡 �ɸ��� �ð�

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // _Tweak_transparency ���� ������ ���� (Lerp �Լ� ���)
            float newTransparency = Mathf.Lerp(currentTransparency, targetTransparency, elapsedTime / duration);

            // Material�� ����� _Tweak_transparency �� ����
            transparentMaterial.SetFloat(transparencyPropertyID, newTransparency);

            // ��� �ð� ������Ʈ
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // ������ ���� (�÷�Ʈ�� ��Ȯ���� ����)
        transparentMaterial.SetFloat(transparencyPropertyID, targetTransparency);
    }

    IEnumerator SetScaleCoroutine(Transform go, float Scale)
    {
        float duration = 2f;
        float elapsedTime = 0f;

        Vector3 initialScale = new Vector3(0.001f, 0.001f, 0.001f);
        Vector3 targetScale = new Vector3(Scale, Scale, Scale);

        while (elapsedTime < duration)
        {
            // ���� �ð��� ���� ������ ũ�� ���
            go.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);

            // ��� �ð� ������Ʈ
            elapsedTime += Time.deltaTime;

            // �� ������ ���
            yield return null;
        }

        go.localScale = targetScale;
    }

    void CreateStartPath(NodeInfo startNode)
    {
        // �ؿ� SetValues �Լ��� �����ϸ� startNode ���� �����Ǿ� �ٲ�Ƿ� ���� NodeInfo ��ü�� ���� �Ҵ������ ��
        NodeInfo node = new NodeInfo(startNode.R, startNode.C, startNode.Direction);

        // ù ��� ����, ���� �� ����, ���� �� ����, ȸ�� ����
        int firstNodeDirection, currentPathDirection, nextPathDirection, clockwise = 1;
        int nextR, nextC;
        bool isCorner;  // �ڳ� Ȯ��

        firstNodeDirection = FirstPathDirection(node);
        nextR = node.R + dr[firstNodeDirection];
        nextC = node.C + dc[firstNodeDirection];

        // map[nextR, nextC]�� �� ��� ����
        map[nextR, nextC] = 1;
        CreateNode("RoadS", nextR * NodeSize, nextC * NodeSize);
        startPoint.SetValues(nextR, nextC, (firstNodeDirection + 2) % 4);

        // ���� �� ������ ���� ����
        if (Random.Range(0, 2) == 0)
        {
            return;
        }

        currentPathDirection = NextPathDirection(node, firstNodeDirection);
        // �ݽð� ������ ���
        if (firstNodeDirection - currentPathDirection == -3 || firstNodeDirection - currentPathDirection == 1)
        {
            clockwise = 3;
        }
        // ���� ����
        nextPathDirection = (currentPathDirection + clockwise) % 4;

        int[] currentCoor = new int[2] { nextR + dr[currentPathDirection], nextC + dc[currentPathDirection] };
        int MaxNodeCount = mapLength * 4 - 4 - 1;

        while (MaxNodeCount > 0)
        {
            int currentR = currentCoor[0];
            int currentC = currentCoor[1];
            isCorner = false;

            // �ڳ��� ���
            if (!visit[currentR + dr[nextPathDirection], currentC + dc[nextPathDirection]])
            {
                currentPathDirection = nextPathDirection;
                nextPathDirection = (currentPathDirection + clockwise) % 4;

                isCorner = true;
            }

            if (!isCorner)
            {
                // ���� �ִ� ���
                if (map[currentR + dr[nextPathDirection], currentC + dc[nextPathDirection]] == 1)
                {
                    return;
                }
                // �밢���� �ִ� ���
                else if (map[currentR + dr[currentPathDirection] + dr[nextPathDirection], currentC + dc[currentPathDirection] + dc[nextPathDirection]] == 1)
                {
                    return;
                }
            }

            // map[currentR, currentC]�� �� ��� ����
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
    void CreateEndPath(NodeInfo endNode)
    {
        // �ؿ� SetValues �Լ��� �����ϸ� endNode ���� �����Ǿ� �ٲ�Ƿ� ���� NodeInfo ��ü�� ���� �Ҵ������ ��
        NodeInfo node = new NodeInfo(endNode.R, endNode.C, endNode.Direction);

        // ù ��� ����, ���� �� ����, ���� �� ����, ȸ�� ����, ������ ������ Ȯ�� ����
        int firstNodeDirection, currentPathDirection, nextPathDirection, clockwise = 1, createNode = 1;
        int nextR, nextC;
        bool isCorner; // �ڳ� Ȯ��

        firstNodeDirection = FirstPathDirection(node);
        nextR = node.R + dr[firstNodeDirection];
        nextC = node.C + dc[firstNodeDirection];

        // map[currentR, currentC]�� �� ��� ����
        map[nextR, nextC] = createNode;
        CreateNode("RoadE", nextR * NodeSize, nextC * NodeSize);
        visit[nextR, nextC] = true;
        endPoint.SetValues(nextR, nextC, (firstNodeDirection + 2) % 4);

        createNode = Random.Range(1, 3);

        currentPathDirection = NextPathDirection(node, firstNodeDirection);
        // �ݽð� ������ ���
        if (firstNodeDirection - currentPathDirection == -3 || firstNodeDirection - currentPathDirection == 1)
        {
            clockwise = 3;
        }
        // ���� ����
        nextPathDirection = (currentPathDirection + clockwise) % 4;

        int[] currentCoor = new int[2] { nextR + dr[currentPathDirection], nextC + dc[currentPathDirection] };
        int MaxNodeCount = mapLength * 4 - 4 - 1;

        while (MaxNodeCount > 0)
        {
            int currentR = currentCoor[0];
            int currentC = currentCoor[1];
            isCorner = false;

            // �ڳ��� ���
            if (!visit[currentR + dr[nextPathDirection], currentC + dc[nextPathDirection]])
            {
                currentPathDirection = nextPathDirection;
                nextPathDirection = (currentPathDirection + clockwise) % 4;

                isCorner = true;
            }

            if (createNode == 1 && !isCorner)
            {
                // �տ� �ִ� ���
                if (map[currentR + dr[currentPathDirection], currentC + dc[currentPathDirection]] == 1)
                {
                    createNode = 2;
                }
                // ���� �ִ� ���
                else if (map[currentR + dr[nextPathDirection], currentC + dc[nextPathDirection]] == 1)
                {
                    createNode = 2;
                }
                // �밢���� �ִ� ���
                else if (map[currentR + dr[currentPathDirection] + dr[nextPathDirection], currentC + dc[currentPathDirection] + dc[nextPathDirection]] == 1)
                {
                    createNode = 2;
                }
            }

            // map[currentR, currentC]�� �� ��� ����
            if (map[currentR, currentC] == 0)
            {
                map[currentR, currentC] = createNode;
                if (createNode == 1)
                {
                    CreateNode("RoadE", currentR * NodeSize, currentC * NodeSize);
                    if (!isCorner)
                    {
                        endPoint.SetValues(currentR, currentC, (currentPathDirection + 2 * clockwise) % 4);
                    } else
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

    public static void clear()
    {
        roads.Clear();
    }
}
