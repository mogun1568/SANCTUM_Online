using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocationInfo
{
    public int R { get; set; }
    public int C { get; set; }
    public int Direction { get; set; }

    public LocationInfo(int r = 0, int c = 0, int direction = 0)
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

public class NewMap : BaseController
{
    GameObject roadParent;
    GameObject GroundParent;
    GameObject EnvironmentParent;

    [HideInInspector] public GameObject startObj;
    GameObject endObj;

    public LinkedList<LocationInfo> roads = new LinkedList<LocationInfo>();

    int startR, startC;

    public int StartPointR { get; set; }
    public int StartPointC { get; set; }
    public int StartPointDir { get; set; }
    public int EndPointR { get; set; }
    public int EndPointC { get; set; }
    public int EndPointDir { get; set; }

    public int MapDefaultSize { get; set; }
    public int NodeSize { get; set; }

    //int[,] _map;

    public void LoadMap(int mapId, string pathPrefix = "Assets/Resources/Map")
    {
        string mapName = "Map_" + mapId.ToString();

        // Collision 관련 파일
        string text = File.ReadAllText($"{pathPrefix}/{mapName}.txt");
        StringReader reader = new StringReader(text);

        StartPointR = int.Parse(reader.ReadLine());
        StartPointC = int.Parse(reader.ReadLine());
        StartPointDir = int.Parse(reader.ReadLine());
        EndPointR = int.Parse(reader.ReadLine());
        EndPointC = int.Parse(reader.ReadLine());
        EndPointDir = int.Parse(reader.ReadLine());

        MapDefaultSize = int.Parse(reader.ReadLine());
        NodeSize = int.Parse(reader.ReadLine());

        //_map = new int[MapDefaultSize, MapDefaultSize];

        //for (int i = 0; i < MapDefaultSize; i++)
        //{
        //    string line = reader.ReadLine();
        //    for (int j = 0; j < MapDefaultSize; j++)
        //    {
        //        _map[i, j] = line[j] - '0';
        //    }
        //}
    }

    protected override void Init()
    {
        transform.position = Pos;
        startR = (int)Pos.x;
        startC = (int)Pos.z;

        roadParent = Managers.Resource.Instantiate("Map/Parent", default, default, transform);
        roadParent.name = "" + "Roads";
        GroundParent = Managers.Resource.Instantiate("Map/Parent", default, default, transform);
        GroundParent.name = "" + "Nodes";
        EnvironmentParent = Managers.Resource.Instantiate("Map/Parent", default, default, transform);
        EnvironmentParent.name = "" + "Environments";

        startObj = Managers.Resource.Instantiate("Map/Start", default, default, transform);
        endObj = Managers.Resource.Instantiate("Map/End", default, default, transform);

        LoadMap(Id);
    }

    public void UpdatePosition()
    {
        startObj.transform.position = new Vector3(StartPointR * NodeSize + startR, 1, StartPointC * NodeSize + startC);
        startObj.transform.rotation = Quaternion.Euler(0, StartPointDir * 90, 0);
        StartCoroutine(SetScaleCoroutine(startObj.transform, 1));
        endObj.transform.position = new Vector3(EndPointR * NodeSize + startR, 1, EndPointC * NodeSize + startC);
        endObj.transform.rotation = Quaternion.Euler(0, EndPointDir * 90 + 180, 0);
        StartCoroutine(SetScaleCoroutine(endObj.transform, 0.003f));
    }

    public virtual void CreateNode(ObjectInfo objInfo)
    {
        int r = (int)objInfo.PosInfo.PosX * NodeSize + startR;
        int c = (int)objInfo.PosInfo.PosZ * NodeSize + startC;

        GameObject go;

        if (roadParent == null)
        {
            Debug.Log("null");
        }

        if (objInfo.Name == "RoadS")
        {
            go = Managers.Resource.Instantiate($"Map/ForestGroundDirt", new Vector3(r, 0, c), Quaternion.identity, roadParent.transform);
            roads.AddFirst(new LocationInfo(r, c));
        } else if (objInfo.Name == "RoadE") 
        {
            go = Managers.Resource.Instantiate($"Map/ForestGroundDirt", new Vector3(r, 0, c), Quaternion.identity, roadParent.transform);
            roads.AddLast(new LocationInfo(r, c));
        } else
        {
            float y = Random.Range(-0.25f, 0.25f);
            go = Managers.Resource.Instantiate($"Map/ForestGround01", new Vector3(r, y, c), Quaternion.identity, GroundParent.transform);
            
            Node node = go.GetComponent<Node>();
            node.Id = objInfo.ObjectId;
            node.MasterId = objInfo.MasterId;
            node.PosInfo = objInfo.PosInfo;
            node.Stat = objInfo.StatInfo;
            
            SetChildCount(go.transform);
            if (objInfo.StatInfo.HaveEnvironment)
            {
                go.GetComponent<Node>().environment = true;
                CreateEnvironment(r, y, c);
            }
        }

        //go.name = objInfo.Name;
        Managers.Object._objects.Add(objInfo.ObjectId, go);

        StartCoroutine(MoveObjectCoroutine(go.transform));
        SetTransparency(go);
    }

    protected void CreateEnvironment(int r, float y, int c)
    {
        GameObject EnvironmentObj = Managers.Resource.Instantiate($"Map/Environment/TFF_Birch_Tree_01A", new Vector3(r, y + 1, c), Quaternion.identity, EnvironmentParent.transform);
        StartCoroutine(SetScaleCoroutine(EnvironmentObj.transform, 0.8f));
    }

    void SetChildCount(Transform go)
    {
        // 자식 오브젝트 전부 비활성화 (Node 스크립트로 옮길까 생각 중)
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

            StartCoroutine(SetScaleCoroutine(go.GetChild(randomIndex).gameObject.transform, 0.8f));

            activeObjectCount--;
        }
    }

    void SetTransparency(GameObject go)
    {
        // 오브젝트의 Material의 Transparency Level이 shader 스크립트에서는 _Tweak_transparency 임
        int transparencyPropertyID = Shader.PropertyToID("_Tweak_transparency");

        go.GetComponent<Renderer>().material.SetFloat(transparencyPropertyID, -1);

        StartCoroutine(FadeTransparencyCoroutine(go.GetComponent<Renderer>().material, transparencyPropertyID));
    }

    IEnumerator MoveObjectCoroutine(Transform go)
    {
        // 이동할 거리와 시간 설정
        float distanceToMove = -2f;
        float duration = 2f;

        // 시작 위치와 종료 위치 계산
        Vector3 endPos = go.position;
        Vector3 startPos = endPos + new Vector3(0, distanceToMove, 0);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 현재 시간에 따라 보간된 위치 계산
            go.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);

            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;

            // 한 프레임 대기
            yield return null;
        }

        go.position = endPos;
    }

    IEnumerator FadeTransparencyCoroutine(Material transparentMaterial, int transparencyPropertyID)
    {
        float currentTransparency = transparentMaterial.GetFloat(transparencyPropertyID);
        float targetTransparency = 0.0f;
        float duration = 2f; // 변경에 걸리는 시간

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // _Tweak_transparency 값을 서서히 변경 (Lerp 함수 사용)
            float newTransparency = Mathf.Lerp(currentTransparency, targetTransparency, elapsedTime / duration);

            // Material에 변경된 _Tweak_transparency 값 설정
            transparentMaterial.SetFloat(transparencyPropertyID, newTransparency);

            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 최종값 설정 (플로트의 정확성을 위해)
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
            // 현재 시간에 따라 보간된 크기 계산
            go.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);

            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;

            // 한 프레임 대기
            yield return null;
        }

        go.localScale = targetScale;
    }

    public void clear()
    {
        roads.Clear();
    }
}
